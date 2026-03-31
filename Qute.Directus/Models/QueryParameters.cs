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
    private string[]? _fields;
    private object? _filter;
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
    public QueryParameters Fields(params string[] fields) { _fields = fields; return this; }

    /// <summary>Select items matching the given filter conditions.</summary>
    public QueryParameters Filter(object filter) { _filter = filter; return this; }

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

        if (_fields is { Length: > 0 })
            parts.Add($"fields={HttpUtility.UrlEncode(string.Join(",", _fields))}");

        if (_filter is not null)
        {
            var filterJson = JsonSerializer.Serialize(_filter, Serialization.DirectusJsonOptions.Default);
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
