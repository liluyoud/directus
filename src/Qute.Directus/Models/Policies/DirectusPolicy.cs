using System.Text.Json;

namespace Qute.Directus.Models.Policies;

/// <summary>
/// Represents a Directus access policy.
/// </summary>
public record DirectusPolicy
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Icon { get; init; }
    public string? Description { get; init; }
    public JsonElement? IpAccess { get; init; }
    public bool? EnforceTfa { get; init; }
    public bool? AdminAccess { get; init; }
    public bool? AppAccess { get; init; }
    public List<string>? Permissions { get; init; }
    public List<string>? Users { get; init; }
    public List<string>? Roles { get; init; }
}
