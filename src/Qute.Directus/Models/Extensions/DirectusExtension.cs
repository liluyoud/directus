using System.Text.Json;

namespace Qute.Directus.Models.Extensions;

/// <summary>
/// Represents a Directus extension.
/// </summary>
public record DirectusExtension
{
    public string? Name { get; init; }
    public string? Bundle { get; init; }
    public JsonElement? Schema { get; init; }
    public JsonElement? Meta { get; init; }
}
