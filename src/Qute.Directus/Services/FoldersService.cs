using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Folders;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus virtual folder operations.
/// </summary>
public sealed class FoldersService
{
    private readonly DirectusHttpClient _http;

    public FoldersService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusFolder>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusFolder>("folders", query, ct);

    public Task<DirectusFolder> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusFolder>($"folders/{id}", query, ct);

    public Task<DirectusFolder> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusFolder>("folders", body, query, ct);

    public Task<DirectusFolder> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusFolder>($"folders/{id}", data, query, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"folders/{id}", ct);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("folders", ids, ct);
}
