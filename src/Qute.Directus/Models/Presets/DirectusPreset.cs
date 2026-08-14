using System.Text.Json;

namespace Qute.Directus.Models.Presets;

/// <summary>
/// Represents a Directus preset (saved view configuration).
/// </summary>
public record DirectusPreset
{
    public int? Id { get; init; }
    public string? Bookmark { get; init; }
    public string? User { get; init; }
    public string? Role { get; init; }
    public string? Collection { get; init; }
    public string? Search { get; init; }
    public JsonElement? Layout { get; init; }
    public string? LayoutQuery { get; init; }
    public string? LayoutOptions { get; init; }
    public int? RefreshInterval { get; init; }
    public JsonElement? Filter { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
}
