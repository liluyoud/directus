namespace Qute.Directus.Models.Translations;

/// <summary>
/// Represents a Directus custom translation string.
/// </summary>
public record DirectusTranslation
{
    public string? Id { get; init; }
    public string? Key { get; init; }
    public string? Value { get; init; }
    public string? Language { get; init; }
}
