using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Notifications;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus notification operations.
/// </summary>
public sealed class NotificationsService
{
    private readonly DirectusHttpClient _http;

    public NotificationsService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusNotification>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusNotification>("notifications", query, ct);

    public Task<DirectusNotification> GetByIdAsync(int id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusNotification>($"notifications/{id}", query, ct);

    public Task<DirectusNotification> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusNotification>("notifications", body, query, ct);

    public Task<DirectusNotification> UpdateAsync(int id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusNotification>($"notifications/{id}", data, query, ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => _http.DeleteAsync($"notifications/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<int> ids, CancellationToken ct = default)
        => _http.DeleteAsync("notifications", ids, ct);
}
