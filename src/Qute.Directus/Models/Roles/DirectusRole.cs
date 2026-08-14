using System.Text.Json;

namespace Qute.Directus.Models.Roles;

/// <summary>
/// Represents a Directus role.
/// </summary>
public record DirectusRole
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Icon { get; init; }
    public string? Description { get; init; }
    public string? Parent { get; init; }
    public List<string>? Children { get; init; }
    public List<string>? Policies { get; init; }
    public List<string>? Users { get; init; }
}
