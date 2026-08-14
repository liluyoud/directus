using System.Text.Json;

namespace Qute.Directus.Models.Permissions;

/// <summary>
/// Represents a Directus permission rule.
/// </summary>
public record DirectusPermission
{
    public int? Id { get; init; }
    public string? Collection { get; init; }
    public string? Action { get; init; }
    public JsonElement? Permissions { get; init; }
    public JsonElement? Validation { get; init; }
    public JsonElement? Presets { get; init; }
    public object? Fields { get; init; }
    public string? Policy { get; init; }
}
