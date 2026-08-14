using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Relations;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus relation operations.
/// </summary>
public sealed class RelationsService
{
    private readonly DirectusHttpClient _http;

    public RelationsService(DirectusHttpClient http) => _http = http;

    /// <summary>List all relations.</summary>
    public Task<DirectusListResponse<DirectusRelation>> GetAllAsync(CancellationToken ct = default)
        => _http.GetListAsync<DirectusRelation>("relations", ct: ct);

    /// <summary>List relations for a specific collection.</summary>
    public Task<DirectusListResponse<DirectusRelation>> GetByCollectionAsync(string collection, CancellationToken ct = default)
        => _http.GetListAsync<DirectusRelation>($"relations/{collection}", ct: ct);

    /// <summary>Retrieve a specific relation.</summary>
    public Task<DirectusRelation> GetByIdAsync(string collection, string field, CancellationToken ct = default)
        => _http.GetAsync<DirectusRelation>($"relations/{collection}/{field}", ct: ct);

    /// <summary>Create a new relation.</summary>
    public Task<DirectusRelation> CreateAsync(object body, CancellationToken ct = default)
        => _http.PostAsync<DirectusRelation>("relations", body, ct: ct);

    /// <summary>Update a relation.</summary>
    public Task<DirectusRelation> UpdateAsync(string collection, string field, object data, CancellationToken ct = default)
        => _http.PatchAsync<DirectusRelation>($"relations/{collection}/{field}", data, ct: ct);

    /// <summary>Delete a relation.</summary>
    public Task DeleteAsync(string collection, string field, CancellationToken ct = default)
        => _http.DeleteAsync($"relations/{collection}/{field}", ct);
}
