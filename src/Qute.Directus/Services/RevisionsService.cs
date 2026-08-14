using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Revisions;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus revision operations (read-only).
/// </summary>
public sealed class RevisionsService
{
    private readonly DirectusHttpClient _http;

    public RevisionsService(DirectusHttpClient http) => _http = http;

    public Task<DirectusListResponse<DirectusRevision>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusRevision>("revisions", query, ct);

    public Task<DirectusRevision> GetByIdAsync(int id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusRevision>($"revisions/{id}", query, ct);
}
