using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Panels;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus panel operations (dashboard widgets).
/// </summary>
public sealed class PanelsService
{
    private readonly DirectusHttpClient _http;

    public PanelsService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusPanel>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusPanel>("panels", query, ct);

    public Task<DirectusPanel> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusPanel>($"panels/{id}", query, ct);

    public Task<DirectusPanel> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusPanel>("panels", body, query, ct);

    public Task<DirectusPanel> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusPanel>($"panels/{id}", data, query, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"panels/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("panels", ids, ct);
}
