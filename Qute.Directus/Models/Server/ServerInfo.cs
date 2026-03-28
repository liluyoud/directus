using System.Text.Json;

namespace Qute.Directus.Models.Server;

/// <summary>
/// Represents Directus server info from <c>GET /server/info</c>.
/// </summary>
public record ServerInfo
{
    public string? ProjectName { get; init; }
    public JsonElement? Project { get; init; }
    public JsonElement? Directus { get; init; }
    public JsonElement? Node { get; init; }
    public JsonElement? Os { get; init; }
}

/// <summary>
/// Represents Directus server health from <c>GET /server/health</c>.
/// </summary>
public record ServerHealth
{
    public string? Status { get; init; }
    public string? ReleaseId { get; init; }
    public string? ServiceId { get; init; }
    public JsonElement? Checks { get; init; }
}
