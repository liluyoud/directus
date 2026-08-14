using System.Text.Json;

namespace Qute.Directus.Models.Panels;

/// <summary>
/// Represents a Directus panel (widget within a dashboard).
/// </summary>
public record DirectusPanel
{
    public string? Id { get; init; }
    public string? Dashboard { get; init; }
    public string? Name { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
    public bool? ShowHeader { get; init; }
    public string? Note { get; init; }
    public string? Type { get; init; }
    public int? PositionX { get; init; }
    public int? PositionY { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public JsonElement? Options { get; init; }
    public DateTime? DateCreated { get; init; }
    public string? UserCreated { get; init; }
}
