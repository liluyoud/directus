using System.Text.Json;
using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Shares;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus share operations.
/// </summary>
public sealed class SharesService
{
    private readonly DirectusHttpClient _http;

    public SharesService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusShare>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusShare>("shares", query, ct);

    public Task<DirectusShare> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusShare>($"shares/{id}", query, ct);

    public Task<DirectusShare> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusShare>("shares", body, query, ct);

    public Task<DirectusShare> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusShare>($"shares/{id}", data, query, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"shares/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("shares", ids, ct);

    /// <summary>Get info about a share (public, no auth required).</summary>
    public Task<JsonElement> GetInfoAsync(string id, CancellationToken ct = default)
        => _http.PostAsync<JsonElement>("shares/info", new { share = id }, ct: ct);
}
