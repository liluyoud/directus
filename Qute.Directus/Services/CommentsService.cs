using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Comments;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus comment operations.
/// </summary>
public sealed class CommentsService
{
    private readonly DirectusHttpClient _http;

    public CommentsService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusComment>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusComment>("activity/comments", query, ct);

    public Task<DirectusComment> GetByIdAsync(int id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusComment>($"activity/comments/{id}", query, ct);

    public Task<DirectusComment> CreateAsync(object body, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusComment>("activity/comments", body, query, ct);

    public Task<DirectusComment> UpdateAsync(int id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusComment>($"activity/comments/{id}", data, query, ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => _http.DeleteAsync($"activity/comments/{id}", ct);
}
