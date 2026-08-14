using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Roles;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus role operations.
/// </summary>
public sealed class RolesService
{
    private readonly DirectusHttpClient _http;

    public RolesService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusRole>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusRole>("roles", query, ct);

    public Task<DirectusRole> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusRole>($"roles/{id}", query, ct);

    public Task<DirectusRole> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusRole>("roles", body, query, ct);

    public Task<DirectusListResponse<DirectusRole>> CreateManyAsync(IEnumerable<object> items, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostListAsync<DirectusRole>("roles", items, query, ct);

    public Task<DirectusRole> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusRole>($"roles/{id}", data, query, ct);

    public Task<DirectusListResponse<DirectusRole>> UpdateManyAsync(object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchListAsync<DirectusRole>("roles", data, query, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"roles/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("roles", ids, ct);
}
