using System.Text.Json;
using Qute.Directus.Http;
using Qute.Directus.Models;

namespace Qute.Directus.Services;

/// <summary>
/// Generic service for CRUD operations on Directus collection items.
/// Supports both strongly-typed and dynamic (JsonElement) operations.
/// </summary>
public sealed class ItemsService
{
    private readonly DirectusHttpClient _http;

    public ItemsService(DirectusHttpClient http) => _http = http;

    // ─── Read ──────────────────────────────────────────────────────────

    /// <summary>List all items in a collection.</summary>
    public Task<DirectusListResponse<T>> GetManyAsync<T>(string collection, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<T>($"items/{collection}", query, ct);

    /// <summary>List all items in a collection using a query builder.</summary>
    public Task<DirectusListResponse<T>> GetManyAsync<T>(string collection, Action<QueryParameters> configure, CancellationToken ct = default)
    {
        var q = new QueryParameters();
        configure(q);
        return GetManyAsync<T>(collection, q, ct);
    }

    /// <summary>Retrieve a single item by ID.</summary>
    public Task<T> GetByIdAsync<T>(string collection, string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<T>($"items/{collection}/{id}", query, ct);

    /// <summary>Retrieve a single item by ID using a query builder.</summary>
    public Task<T> GetByIdAsync<T>(string collection, string id, Action<QueryParameters> configure, CancellationToken ct = default)
    {
        var q = new QueryParameters();
        configure(q);
        return GetByIdAsync<T>(collection, id, q, ct);
    }

    /// <summary>Retrieve a singleton item.</summary>
    public Task<T> GetSingletonAsync<T>(string collection, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<T>($"items/{collection}/singleton", query, ct);

    // ─── Dynamic (JsonElement) Read ────────────────────────────────────

    /// <summary>List all items in a collection as dynamic JSON objects.</summary>
    public Task<DirectusListResponse<JsonElement>> GetManyAsync(string collection, QueryParameters? query = null, CancellationToken ct = default)
        => GetManyAsync<JsonElement>(collection, query, ct);

    /// <summary>Retrieve a single item by ID as a dynamic JSON object.</summary>
    public Task<JsonElement> GetByIdAsync(string collection, string id, QueryParameters? query = null, CancellationToken ct = default)
        => GetByIdAsync<JsonElement>(collection, id, query, ct);

    /// <summary>Retrieve a single item by ID as a dynamic JSON object using a query builder.</summary>
    public Task<JsonElement> GetByIdAsync(string collection, string id, Action<QueryParameters> configure, CancellationToken ct = default)
        => GetByIdAsync<JsonElement>(collection, id, configure, ct);

    // ─── Create ────────────────────────────────────────────────────────

    /// <summary>Create a single item in a collection.</summary>
    public Task<T> CreateAsync<T>(string collection, object item, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<T>($"items/{collection}", item, query, ct);

    /// <summary>Create multiple items in a collection.</summary>
    public Task<DirectusListResponse<T>> CreateManyAsync<T>(string collection, IEnumerable<object> items, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostListAsync<T>($"items/{collection}", items, query, ct);

    // ─── Update ────────────────────────────────────────────────────────

    /// <summary>Update a single item by ID.</summary>
    public Task<T> UpdateAsync<T>(string collection, string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<T>($"items/{collection}/{id}", data, query, ct);

    /// <summary>Update multiple items at once.</summary>
    public Task<DirectusListResponse<T>> UpdateManyAsync<T>(string collection, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchListAsync<T>($"items/{collection}", data, query, ct);

    /// <summary>Update a singleton item.</summary>
    public Task<T> UpdateSingletonAsync<T>(string collection, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<T>($"items/{collection}/singleton", data, query, ct);

    // ─── Delete ────────────────────────────────────────────────────────

    /// <summary>Delete a single item by ID.</summary>
    public Task DeleteAsync(string collection, string id, CancellationToken ct = default)
        => _http.DeleteAsync($"items/{collection}/{id}", ct);

    /// <summary>Delete multiple items by IDs or query.</summary>
    public Task DeleteManyAsync(string collection, object keysOrQuery, CancellationToken ct = default)
        => _http.DeleteAsync($"items/{collection}", keysOrQuery, ct);
}
