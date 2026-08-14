using Qute.Directus.Http;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus metrics endpoint (Prometheus format).
/// </summary>
public sealed class MetricsService
{
    private readonly DirectusHttpClient _http;

    public MetricsService(DirectusHttpClient http) => _http = http;

    /// <summary>
    /// Get Prometheus-formatted metrics (requires METRICS_ENABLED to be true on the server).
    /// </summary>
    public Task<string> GetAsync(CancellationToken ct = default)
        => _http.GetStringAsync("metrics", ct: ct);
}
