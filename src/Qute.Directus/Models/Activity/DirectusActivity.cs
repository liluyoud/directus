using System.Text.Json;

namespace Qute.Directus.Models.Activity;

/// <summary>
/// Represents a Directus activity log entry.
/// </summary>
public record DirectusActivity
{
    public int? Id { get; init; }
    public string? Action { get; init; }
    public string? User { get; init; }
    public DateTime? Timestamp { get; init; }
    public string? Ip { get; init; }
    public string? UserAgent { get; init; }
    public string? Collection { get; init; }
    public string? Item { get; init; }
    public string? Comment { get; init; }
    public string? Origin { get; init; }
    public List<int>? Revisions { get; init; }
}
