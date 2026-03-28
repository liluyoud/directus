using System.Text.Json;

namespace Qute.Directus.Models.Operations;

/// <summary>
/// Represents a Directus Operation (step within a Flow).
/// </summary>
public record DirectusOperation
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Key { get; init; }
    public string? Type { get; init; }
    public int? PositionX { get; init; }
    public int? PositionY { get; init; }
    public JsonElement? Options { get; init; }
    public string? Resolve { get; init; }
    public string? Reject { get; init; }
    public string? Flow { get; init; }
    public DateTime? DateCreated { get; init; }
    public string? UserCreated { get; init; }
}
