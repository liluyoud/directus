using System.Text.Json;

namespace Qute.Directus.Models.Shares;

/// <summary>
/// Represents a Directus share (public item link).
/// </summary>
public record DirectusShare
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Collection { get; init; }
    public string? Item { get; init; }
    public string? Role { get; init; }
    public string? Password { get; init; }
    public string? UserCreated { get; init; }
    public DateTime? DateCreated { get; init; }
    public DateTime? DateStart { get; init; }
    public DateTime? DateEnd { get; init; }
    public int? TimesUsed { get; init; }
    public int? MaxUses { get; init; }
}
