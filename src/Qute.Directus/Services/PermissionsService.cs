using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Permissions;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus permission operations.
/// </summary>
public sealed class PermissionsService
{
    private readonly DirectusHttpClient _http;

    public PermissionsService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusPermission>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusPermission>("permissions", query, ct);

    public Task<DirectusPermission> GetByIdAsync(int id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusPermission>($"permissions/{id}", query, ct);

    public Task<DirectusPermission> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusPermission>("permissions", body, query, ct);

    public Task<DirectusListResponse<DirectusPermission>> CreateManyAsync(IEnumerable<object> items, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostListAsync<DirectusPermission>("permissions", items, query, ct);

    public Task<DirectusPermission> UpdateAsync(int id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusPermission>($"permissions/{id}", data, query, ct);

    public Task<DirectusListResponse<DirectusPermission>> UpdateManyAsync(object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchListAsync<DirectusPermission>("permissions", data, query, ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => _http.DeleteAsync($"permissions/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<int> ids, CancellationToken ct = default)
        => _http.DeleteAsync("permissions", ids, ct);
}
