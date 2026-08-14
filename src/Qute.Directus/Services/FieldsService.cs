using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Fields;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus field operations.
/// </summary>
public sealed class FieldsService
{
    private readonly DirectusHttpClient _http;

    public FieldsService(DirectusHttpClient http) => _http = http;

    /// <summary>List all fields across all collections.</summary>
    public Task<DirectusListResponse<DirectusField>> GetAllAsync(CancellationToken ct = default)
        => _http.GetListAsync<DirectusField>("fields", ct: ct);

    /// <summary>List all fields in a specific collection.</summary>
    public Task<DirectusListResponse<DirectusField>> GetByCollectionAsync(string collection, CancellationToken ct = default)
        => _http.GetListAsync<DirectusField>($"fields/{collection}", ct: ct);

    /// <summary>Retrieve a specific field in a collection.</summary>
    public Task<DirectusField> GetByIdAsync(string collection, string field, CancellationToken ct = default)
        => _http.GetAsync<DirectusField>($"fields/{collection}/{field}", ct: ct);

    /// <summary>Create a new field in a collection.</summary>
    public Task<DirectusField> CreateAsync(string collection, object body, CancellationToken ct = default)
        => _http.PostAsync<DirectusField>($"fields/{collection}", body, ct: ct);

    /// <summary>Update a field in a collection.</summary>
    public Task<DirectusField> UpdateAsync(string collection, string field, object data, CancellationToken ct = default)
        => _http.PatchAsync<DirectusField>($"fields/{collection}/{field}", data, ct: ct);

    /// <summary>Delete a field from a collection.</summary>
    public Task DeleteAsync(string collection, string field, CancellationToken ct = default)
        => _http.DeleteAsync($"fields/{collection}/{field}", ct);
}
