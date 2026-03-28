using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Utilities;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus utility operations (hash, sort, random, export).
/// </summary>
public sealed class UtilitiesService
{
    private readonly DirectusHttpClient _http;

    public UtilitiesService(DirectusHttpClient http) => _http = http;

    /// <summary>Generate a hash for the given string.</summary>
    public Task<string> HashGenerateAsync(string value, CancellationToken ct = default)
        => _http.PostAsync<string>("utils/hash/generate", new HashGenerateRequest { String = value }, ct: ct);

    /// <summary>Verify a string against a hash.</summary>
    public Task<bool> HashVerifyAsync(string value, string hash, CancellationToken ct = default)
        => _http.PostAsync<bool>("utils/hash/verify", new HashVerifyRequest { String = value, Hash = hash }, ct: ct);

    /// <summary>Re-sort items in a collection (manual sorting).</summary>
    public Task SortAsync(string collection, string item, int to, CancellationToken ct = default)
        => _http.PostAsync($"utils/sort/{collection}", new SortRequest { Item = item, To = to }, ct);

    /// <summary>Generate a random string of the given length.</summary>
    public async Task<string> RandomStringAsync(int? length = null, CancellationToken ct = default)
    {
        var query = length.HasValue ? new QueryParameters().Custom("length", length.Value.ToString()) : null;
        return await _http.GetAsync<string>("utils/random/string", query, ct);
    }

    /// <summary>Export items from a collection.</summary>
    public Task ExportAsync(string collection, object body, CancellationToken ct = default)
        => _http.PostAsync($"utils/export/{collection}", body, ct);
}
