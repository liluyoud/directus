using System.Text.Json;

namespace Qute.Directus.Models.Revisions;

/// <summary>
/// Represents a Directus revision (change history entry).
/// </summary>
public record DirectusRevision
{
    public int? Id { get; init; }
    public int? Activity { get; init; }
    public string? Collection { get; init; }
    public string? Item { get; init; }
    public JsonElement? Data { get; init; }
    public JsonElement? Delta { get; init; }
    public string? Parent { get; init; }
    public string? Version { get; init; }
}
