namespace Qute.Directus.Models.Comments;

/// <summary>
/// Represents a Directus comment on an item.
/// </summary>
public record DirectusComment
{
    public int? Id { get; init; }
    public string? Collection { get; init; }
    public string? Item { get; init; }
    public string? Comment { get; init; }
    public string? UserCreated { get; init; }
    public DateTime? DateCreated { get; init; }
    public DateTime? DateUpdated { get; init; }
    public string? UserUpdated { get; init; }
}
