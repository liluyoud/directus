using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Operations;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus operation operations (steps within flows).
/// </summary>
public sealed class OperationsService
{
    private readonly DirectusHttpClient _http;

    public OperationsService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusOperation>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusOperation>("operations", query, ct);

    public Task<DirectusOperation> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusOperation>($"operations/{id}", query, ct);

    public Task<DirectusOperation> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusOperation>("operations", body, query, ct);

    public Task<DirectusOperation> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusOperation>($"operations/{id}", data, query, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"operations/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("operations", ids, ct);
}
