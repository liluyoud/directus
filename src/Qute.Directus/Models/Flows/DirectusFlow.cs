using System.Text.Json;

namespace Qute.Directus.Models.Flows;

/// <summary>
/// Represents a Directus Flow (automation workflow).
/// </summary>
public record DirectusFlow
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
    public string? Description { get; init; }
    public string? Status { get; init; }
    public string? Trigger { get; init; }
    public string? Accountability { get; init; }
    public JsonElement? Options { get; init; }
    public string? Operation { get; init; }
    public DateTime? DateCreated { get; init; }
    public string? UserCreated { get; init; }
    public List<string>? Operations { get; init; }
}
