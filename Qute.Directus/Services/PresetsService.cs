using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Presets;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus preset operations.
/// </summary>
public sealed class PresetsService
{
    private readonly DirectusHttpClient _http;

    public PresetsService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusPreset>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusPreset>("presets", query, ct);

    public Task<DirectusPreset> GetByIdAsync(int id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusPreset>($"presets/{id}", query, ct);

    public Task<DirectusPreset> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusPreset>("presets", body, query, ct);

    public Task<DirectusPreset> UpdateAsync(int id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusPreset>($"presets/{id}", data, query, ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => _http.DeleteAsync($"presets/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<int> ids, CancellationToken ct = default)
        => _http.DeleteAsync("presets", ids, ct);
}
