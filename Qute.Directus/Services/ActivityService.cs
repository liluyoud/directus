using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Activity;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus activity log (read-only).
/// </summary>
public sealed class ActivityService
{
    private readonly DirectusHttpClient _http;

    public ActivityService(DirectusHttpClient http) => _http = http;

    /// <summary>List activity actions.</summary>
    public Task<DirectusListResponse<DirectusActivity>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusActivity>("activity", query, ct);

    /// <summary>Retrieve a specific activity action.</summary>
    public Task<DirectusActivity> GetByIdAsync(int id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusActivity>($"activity/{id}", query, ct);
}
