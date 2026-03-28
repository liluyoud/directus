namespace Qute.Directus.Models.Folders;

/// <summary>
/// Represents a Directus virtual folder (used for organizing files).
/// </summary>
public record DirectusFolder
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Parent { get; init; }
}
