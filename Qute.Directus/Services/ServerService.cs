using System.Text.Json;
using Qute.Directus.Http;
using Qute.Directus.Models.Server;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus server information endpoints.
/// </summary>
public sealed class ServerService
{
    private readonly DirectusHttpClient _http;

    public ServerService(DirectusHttpClient http) => _http = http;

    /// <summary>Get server info (project details, Directus version, etc.).</summary>
    public Task<ServerInfo> GetInfoAsync(CancellationToken ct = default)
        => _http.GetAsync<ServerInfo>("server/info", ct: ct);

    /// <summary>Get server health status.</summary>
    public async Task<ServerHealth> GetHealthAsync(CancellationToken ct = default)
    {
        var json = await _http.GetStringAsync("server/health", ct: ct);
        return JsonSerializer.Deserialize<ServerHealth>(json, Serialization.DirectusJsonOptions.Default)
            ?? new ServerHealth();
    }

    /// <summary>Ping the server. Returns "pong" if the server is reachable.</summary>
    public Task<string> PingAsync(CancellationToken ct = default)
        => _http.GetStringAsync("server/ping", ct: ct);
}
