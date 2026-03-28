using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Translations;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus custom translation operations.
/// </summary>
public sealed class TranslationsService
{
    private readonly DirectusHttpClient _http;

    public TranslationsService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusTranslation>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusTranslation>("translations", query, ct);

    public Task<DirectusTranslation> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusTranslation>($"translations/{id}", query, ct);

    public Task<DirectusTranslation> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusTranslation>("translations", body, query, ct);

    public Task<DirectusListResponse<DirectusTranslation>> CreateManyAsync(IEnumerable<object> items, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostListAsync<DirectusTranslation>("translations", items, query, ct);

    public Task<DirectusTranslation> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusTranslation>($"translations/{id}", data, query, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"translations/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("translations", ids, ct);
}
