using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Settings;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus global settings (singleton).
/// </summary>
public sealed class SettingsService
{
    private readonly DirectusHttpClient _http;

    public SettingsService(DirectusHttpClient http) => _http = http;

    /// <summary>Retrieve global settings.</summary>
    public Task<DirectusSettings> GetAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusSettings>("settings", query, ct);

    /// <summary>Update global settings.</summary>
    public Task<DirectusSettings> UpdateAsync(object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusSettings>("settings", data, query, ct);
}
