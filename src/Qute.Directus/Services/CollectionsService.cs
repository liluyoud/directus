using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Collections;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus collection operations.
/// </summary>
public sealed class CollectionsService
{
    private readonly DirectusHttpClient _http;

    public CollectionsService(DirectusHttpClient http) => _http = http;

    /// <summary>List all collections.</summary>
    public Task<DirectusListResponse<DirectusCollection>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusCollection>("collections", query, ct);

    /// <summary>Retrieve a single collection by name.</summary>
    public Task<DirectusCollection> GetByIdAsync(string collection, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusCollection>($"collections/{collection}", query, ct);

    /// <summary>Create a new collection.</summary>
    public Task<DirectusCollection> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusCollection>("collections", body, query, ct);

    /// <summary>Update a collection's metadata.</summary>
    public Task<DirectusCollection> UpdateAsync(string collection, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusCollection>($"collections/{collection}", data, query, ct);

    /// <summary>Delete a collection (and all its items).</summary>
    public Task DeleteAsync(string collection, CancellationToken ct = default)
        => _http.DeleteAsync($"collections/{collection}", ct);
}
