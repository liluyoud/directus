using System.Text.Json;
using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Flows;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus Flow (automation) operations.
/// </summary>
public sealed class FlowsService
{
    private readonly DirectusHttpClient _http;

    public FlowsService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusFlow>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusFlow>("flows", query, ct);

    public Task<DirectusFlow> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusFlow>($"flows/{id}", query, ct);

    public Task<DirectusFlow> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusFlow>("flows", body, query, ct);

    public Task<DirectusFlow> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusFlow>($"flows/{id}", data, query, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"flows/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("flows", ids, ct);

    /// <summary>Trigger a manual flow by ID.</summary>
    public Task<JsonElement> TriggerAsync(string id, object? body = null, CancellationToken ct = default)
        => _http.PostAsync<JsonElement>($"flows/trigger/{id}", body, ct: ct);
}
