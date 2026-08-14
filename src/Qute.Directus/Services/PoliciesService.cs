using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Policies;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus access policy operations.
/// </summary>
public sealed class PoliciesService
{
    private readonly DirectusHttpClient _http;

    public PoliciesService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusPolicy>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusPolicy>("policies", query, ct);

    public Task<DirectusPolicy> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusPolicy>($"policies/{id}", query, ct);

    public Task<DirectusPolicy> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusPolicy>("policies", body, query, ct);

    public Task<DirectusPolicy> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusPolicy>($"policies/{id}", data, query, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"policies/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("policies", ids, ct);
}
