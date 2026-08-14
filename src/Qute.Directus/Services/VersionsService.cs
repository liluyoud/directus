using System.Text.Json;
using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Versions;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus Content Version operations.
/// </summary>
public sealed class VersionsService
{
    private readonly DirectusHttpClient _http;

    public VersionsService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusVersion>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusVersion>("versions", query, ct);

    public Task<DirectusVersion> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusVersion>($"versions/{id}", query, ct);

    public Task<DirectusVersion> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusVersion>("versions", body, query, ct);

    public Task<DirectusVersion> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusVersion>($"versions/{id}", data, query, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"versions/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("versions", ids, ct);

    /// <summary>Save changes to a content version.</summary>
    public Task<JsonElement> SaveAsync(string id, object data, CancellationToken ct = default)
        => _http.PostAsync<JsonElement>($"versions/{id}/save", data, ct: ct);

    /// <summary>Promote a content version to the main item.</summary>
    public Task PromoteAsync(string id, object? body = null, CancellationToken ct = default)
        => _http.PostAsync($"versions/{id}/promote", body, ct);
}
