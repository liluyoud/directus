using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Qute.Directus.Models;

/// <summary>
/// Fluent builder for Directus query parameters (fields, filter, sort, limit, offset, search, meta, deep).
/// </summary>
public sealed class QueryParameters
{
    private string? _raw;
    private List<string>? _fields;
    private List<object>? _filters;
    private string[]? _sort;
    private int? _limit;
    private int? _offset;
    private string? _search;
    private string? _meta;
    private object? _deep;
    private string? _version;
    private bool? _noRelationalWildcard;
    private readonly Dictionary<string, string> _extra = new();

    /// <summary>Control what fields are returned in the response.</summary>
    public QueryParameters Fields(params string[] fields)
    {
        if (fields == null || fields.Length == 0)
            return this;

        _fields ??= new List<string>();
        _fields.AddRange(fields);
        return this;
    }

    /// <summary>
    /// Select items matching the given filter condition.
    /// Calling this (or <see cref="Where"/> and the other <c>Where*</c> operator helpers) more than once
    /// combines every condition with a logical AND — there is no need to hand-build an <c>_and</c> filter yourself.
    /// </summary>
    public QueryParameters Filter(object filter)
    {
        _filters ??= new List<object>();
        _filters.Add(filter);
        return this;
    }

    /// <summary>Builds a single-operator filter fragment (<c>{ field: { op: value } }</c>) and adds it via <see cref="Filter"/>.</summary>
    private QueryParameters Op(string field, string directusOperator, object? value)
        => Filter(new Dictionary<string, object?> { [field] = new Dictionary<string, object?> { [directusOperator] = value } });

    /// <summary>
    /// Shorthand for an equality filter on a single field (<c>{ field: { _eq: value } }</c>).
    /// Composes with other <see cref="Filter"/>/<see cref="Where"/> calls via AND.
    /// </summary>
    public QueryParameters Where(string field, object? value) => Op(field, "_eq", value);

    /// <summary>Filters where <paramref name="field"/> is not equal to <paramref name="value"/> (<c>_neq</c>).</summary>
    public QueryParameters WhereNot(string field, object? value) => Op(field, "_neq", value);

    /// <summary>Filters where <paramref name="field"/> is greater than <paramref name="value"/> (<c>_gt</c>).</summary>
    public QueryParameters WhereGreaterThan(string field, object value) => Op(field, "_gt", value);

    /// <summary>Filters where <paramref name="field"/> is greater than or equal to <paramref name="value"/> (<c>_gte</c>).</summary>
    public QueryParameters WhereGreaterThanOrEqual(string field, object value) => Op(field, "_gte", value);

    /// <summary>Filters where <paramref name="field"/> is less than <paramref name="value"/> (<c>_lt</c>).</summary>
    public QueryParameters WhereLessThan(string field, object value) => Op(field, "_lt", value);

    /// <summary>Filters where <paramref name="field"/> is less than or equal to <paramref name="value"/> (<c>_lte</c>).</summary>
    public QueryParameters WhereLessThanOrEqual(string field, object value) => Op(field, "_lte", value);

    /// <summary>Filters where <paramref name="field"/> is one of the given <paramref name="values"/> (<c>_in</c>).</summary>
    public QueryParameters WhereIn(string field, params object[] values) => Op(field, "_in", values);

    /// <summary>Filters where <paramref name="field"/> is none of the given <paramref name="values"/> (<c>_nin</c>).</summary>
    public QueryParameters WhereNotIn(string field, params object[] values) => Op(field, "_nin", values);

    /// <summary>Filters where <paramref name="field"/> contains <paramref name="value"/> as a substring, case-sensitive (<c>_contains</c>).</summary>
    public QueryParameters WhereContains(string field, string value) => Op(field, "_contains", value);

    /// <summary>Filters where <paramref name="field"/> contains <paramref name="value"/> as a substring, case-insensitive (<c>_icontains</c>).</summary>
    public QueryParameters WhereIContains(string field, string value) => Op(field, "_icontains", value);

    /// <summary>Filters where <paramref name="field"/> starts with <paramref name="value"/> (<c>_starts_with</c>).</summary>
    public QueryParameters WhereStartsWith(string field, string value) => Op(field, "_starts_with", value);

    /// <summary>Filters where <paramref name="field"/> ends with <paramref name="value"/> (<c>_ends_with</c>).</summary>
    public QueryParameters WhereEndsWith(string field, string value) => Op(field, "_ends_with", value);

    /// <summary>Filters where <paramref name="field"/> is <c>null</c> (<c>_null</c>).</summary>
    public QueryParameters WhereNull(string field) => Op(field, "_null", true);

    /// <summary>Filters where <paramref name="field"/> is not <c>null</c> (<c>_nnull</c>).</summary>
    public QueryParameters WhereNotNull(string field) => Op(field, "_nnull", true);

    /// <summary>Filters where <paramref name="field"/> is empty (empty string or empty array) (<c>_empty</c>).</summary>
    public QueryParameters WhereEmpty(string field) => Op(field, "_empty", true);

    /// <summary>Filters where <paramref name="field"/> is not empty (<c>_nempty</c>).</summary>
    public QueryParameters WhereNotEmpty(string field) => Op(field, "_nempty", true);

    /// <summary>Filters where <paramref name="field"/> is between <paramref name="from"/> and <paramref name="to"/>, inclusive (<c>_between</c>).</summary>
    public QueryParameters WhereBetween(string field, object from, object to) => Op(field, "_between", new[] { from, to });

    /// <summary>
    /// Escape hatch for any Directus filter operator not covered by a named <c>Where*</c> method
    /// (e.g. a future operator, or one specific to a custom field type).
    /// </summary>
    public QueryParameters WhereOperator(string field, string directusOperator, object? value) => Op(field, directusOperator, value);

    /// <summary>
    /// Combines every condition built inside <paramref name="configure"/> with a logical OR, then
    /// composes that group with the rest of the filter via AND — exactly like chaining another
    /// <see cref="Where"/> call. Nest calls to build arbitrarily deep <c>_and</c>/<c>_or</c> trees.
    /// </summary>
    /// <example>
    /// <code>
    /// q.Where("category", "news")
    ///  .Or(o => o.Where("featured", true).WhereGreaterThan("views", 1000));
    /// // => { "_and": [ {"category":{"_eq":"news"}},
    /// //                {"_or": [{"featured":{"_eq":true}}, {"views":{"_gt":1000}}]} ] }
    /// </code>
    /// </example>
    public QueryParameters Or(Action<QueryParameters> configure)
    {
        var sub = new QueryParameters();
        configure(sub);
        if (sub._filters is { Count: > 0 })
            Filter(new Dictionary<string, object> { ["_or"] = sub._filters });
        return this;
    }

    /// <summary>
    /// Excludes archived items, assuming the Directus 12+ convention of a boolean <c>archived</c> field
    /// (the default suggested for new collections in the Data Studio). Composes via AND.
    /// </summary>
    public QueryParameters NotArchived(string field = "archived") => Where(field, false);

    /// <summary>Selects only archived items. See <see cref="NotArchived"/>.</summary>
    public QueryParameters OnlyArchived(string field = "archived") => Where(field, true);

    /// <summary>
    /// Filters by a string <c>status</c> field — the convention used by collections created before Directus 12.
    /// For collections created on Directus 12+, prefer <see cref="NotArchived"/>/<see cref="OnlyArchived"/>.
    /// </summary>
    public QueryParameters WithStatus(string status, string field = "status") => Where(field, status);

    /// <summary>Sort the returned items. Prefix with <c>-</c> for descending order.</summary>
    public QueryParameters Sort(params string[] sort) { _sort = sort; return this; }

    /// <summary>Limit the number of returned items.</summary>
    public QueryParameters Limit(int limit) { _limit = limit; return this; }

    /// <summary>Skip a number of items when fetching data.</summary>
    public QueryParameters Offset(int offset) { _offset = offset; return this; }

    /// <summary>Full-text search across all searchable fields.</summary>
    public QueryParameters Search(string search) { _search = search; return this; }

    /// <summary>What metadata to return. Use <c>"*"</c>, <c>"total_count"</c>, or <c>"filter_count"</c>.</summary>
    public QueryParameters Meta(string meta) { _meta = meta; return this; }

    /// <summary>Deep filter/limit/sort on relational fields.</summary>
    public QueryParameters Deep(object deep) { _deep = deep; return this; }

    /// <summary>Retrieve item state from a specific Content Version key.</summary>
    public QueryParameters Version(string version) { _version = version; return this; }

    /// <summary>Exclude reverse relations when using wildcard fields.</summary>
    public QueryParameters NoRelationalWildcard(bool value = true) { _noRelationalWildcard = value; return this; }

    /// <summary>Add a custom query parameter.</summary>
    public QueryParameters Custom(string key, string value) { _extra[key] = value; return this; }

    /// <summary>
    /// Use a raw query string instead of the fluent builder.
    /// Accepts either "a=1" or "?a=1" — stored without the leading '?'.
    /// When defined, <see cref="ToQueryString"/> returns this value verbatim.
    /// </summary>
    public QueryParameters Raw(string raw)
    {
        _raw = string.IsNullOrEmpty(raw) ? null : raw.Trim();
        return this;
    }

    /// <summary>Creates a <see cref="QueryParameters"/> instance from a raw query string.</summary>
    public static QueryParameters FromRaw(string raw) => new QueryParameters().Raw(raw);

    /// <summary>
    /// Constructs the query string portion (without leading '?') from the configured parameters.
    /// </summary>
    public string ToQueryString()
    {
        if (!string.IsNullOrEmpty(_raw))
            return _raw;

        var parts = new List<string>();

        if (_fields != null && _fields.Count > 0)
            parts.Add($"fields={HttpUtility.UrlEncode(string.Join(",", _fields))}");

        if (_filters is { Count: > 0 })
        {
            object filterPayload = _filters.Count == 1
                ? _filters[0]
                : new Dictionary<string, object> { ["_and"] = _filters };

            var filterJson = JsonSerializer.Serialize(filterPayload, Serialization.DirectusJsonOptions.Default);
            parts.Add($"filter={HttpUtility.UrlEncode(filterJson)}");
        }

        if (_sort is { Length: > 0 })
            parts.Add($"sort={HttpUtility.UrlEncode(string.Join(",", _sort))}");

        if (_limit.HasValue)
            parts.Add($"limit={_limit.Value}");

        if (_offset.HasValue)
            parts.Add($"offset={_offset.Value}");

        if (!string.IsNullOrEmpty(_search))
            parts.Add($"search={HttpUtility.UrlEncode(_search)}");

        if (!string.IsNullOrEmpty(_meta))
            parts.Add($"meta={HttpUtility.UrlEncode(_meta)}");

        if (_deep is not null)
        {
            var deepJson = JsonSerializer.Serialize(_deep, Serialization.DirectusJsonOptions.Default);
            parts.Add($"deep={HttpUtility.UrlEncode(deepJson)}");
        }

        if (!string.IsNullOrEmpty(_version))
            parts.Add($"version={HttpUtility.UrlEncode(_version)}");

        if (_noRelationalWildcard == true)
            parts.Add("alias[]=no_relational_wildcard");

        foreach (var (key, value) in _extra)
            parts.Add($"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}");
            
        return string.Join("&", parts);
    }

    /// <summary>Returns an empty <see cref="QueryParameters"/> instance.</summary>
    public static QueryParameters Empty => new();
}
