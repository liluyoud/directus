using System.Text.Json;

namespace Qute.Directus.Models.Versions;

/// <summary>
/// Represents a Directus Content Version.
/// </summary>
public record DirectusVersion
{
    public string? Id { get; init; }
    public string? Key { get; init; }
    public string? Name { get; init; }
    public string? Collection { get; init; }
    public string? Item { get; init; }
    public string? HashValue { get; init; }
    public DateTime? DateCreated { get; init; }
    public DateTime? DateUpdated { get; init; }
    public string? UserCreated { get; init; }
    public string? UserUpdated { get; init; }
    public JsonElement? Delta { get; init; }
}
