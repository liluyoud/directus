using System.Text.Json;

namespace Qute.Directus.Models.Fields;

/// <summary>
/// Represents a Directus field.
/// </summary>
public record DirectusField
{
    public string? Collection { get; init; }
    public string? Field { get; init; }
    public string? Type { get; init; }
    public JsonElement? Schema { get; init; }
    public FieldMeta? Meta { get; init; }
}

/// <summary>
/// Metadata for a Directus field.
/// </summary>
public record FieldMeta
{
    public int? Id { get; init; }
    public string? Collection { get; init; }
    public string? Field { get; init; }
    public string? Special { get; init; }
    public string? Interface { get; init; }
    public JsonElement? Options { get; init; }
    public string? Display { get; init; }
    public JsonElement? DisplayOptions { get; init; }
    public bool? Readonly { get; init; }
    public bool? Hidden { get; init; }
    public int? Sort { get; init; }
    public string? Width { get; init; }
    public object? Translations { get; init; }
    public string? Note { get; init; }
    public JsonElement? Conditions { get; init; }
    public bool? Required { get; init; }
    public string? Group { get; init; }
    public JsonElement? Validation { get; init; }
    public string? ValidationMessage { get; init; }
}
