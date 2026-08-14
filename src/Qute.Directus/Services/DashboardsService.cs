using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Dashboards;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus dashboard operations.
/// </summary>
public sealed class DashboardsService
{
    private readonly DirectusHttpClient _http;

    public DashboardsService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusDashboard>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusDashboard>("dashboards", query, ct);

    public Task<DirectusDashboard> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusDashboard>($"dashboards/{id}", query, ct);

    public Task<DirectusDashboard> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusDashboard>("dashboards", body, query, ct);

    public Task<DirectusDashboard> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusDashboard>($"dashboards/{id}", data, query, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"dashboards/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("dashboards", ids, ct);
}
