using System.Text.Json;

namespace Qute.Directus.Models.Relations;

/// <summary>
/// Represents a Directus relation between collections.
/// </summary>
public record DirectusRelation
{
    public int? Id { get; init; }
    public string? ManyCollection { get; init; }
    public string? ManyField { get; init; }
    public string? OneCollection { get; init; }
    public string? OneField { get; init; }
    public string? OneCollectionField { get; init; }
    public string? OneAllowedCollections { get; init; }
    public string? JunctionField { get; init; }
    public string? SortField { get; init; }
    public string? OneDeselectAction { get; init; }
    public JsonElement? Schema { get; init; }
    public JsonElement? Meta { get; init; }
}
