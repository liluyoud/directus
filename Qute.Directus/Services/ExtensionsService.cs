using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Extensions;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus extension operations.
/// </summary>
public sealed class ExtensionsService
{
    private readonly DirectusHttpClient _http;

    public ExtensionsService(DirectusHttpClient http) => _http = http;

    /// <summary>List all installed extensions.</summary>
    public Task<DirectusListResponse<DirectusExtension>> GetManyAsync(CancellationToken ct = default)
        => _http.GetListAsync<DirectusExtension>("extensions", ct: ct);

    /// <summary>Update an extension's configuration (enable/disable).</summary>
    public Task<DirectusExtension> UpdateAsync(string bundle, string name, object data, CancellationToken ct = default)
        => _http.PatchAsync<DirectusExtension>($"extensions/{bundle}/{name}", data, ct: ct);
}
