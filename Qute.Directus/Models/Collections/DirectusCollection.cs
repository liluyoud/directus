using System.Text.Json;

namespace Qute.Directus.Models.Collections;

/// <summary>
/// Represents a Directus collection.
/// </summary>
public record DirectusCollection
{
    public string? Collection { get; init; }
    public CollectionMeta? Meta { get; init; }
    public JsonElement? Schema { get; init; }
}

/// <summary>
/// Metadata for a Directus collection.
/// </summary>
public record CollectionMeta
{
    public string? Collection { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
    public string? Note { get; init; }
    public string? DisplayTemplate { get; init; }
    public bool? Hidden { get; init; }
    public bool? Singleton { get; init; }
    public object? Translations { get; init; }
    public string? ArchiveField { get; init; }
    public bool? ArchiveAppFilter { get; init; }
    public string? ArchiveValue { get; init; }
    public string? UnarchiveValue { get; init; }
    public string? SortField { get; init; }
    public string? Accountability { get; init; }
    public object? ItemDuplicationFields { get; init; }
    public int? Sort { get; init; }
    public string? Group { get; init; }
    public string? Collapse { get; init; }
    public string? PreviewUrl { get; init; }
    public bool? Versioning { get; init; }
}
