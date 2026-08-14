using System.Text.Json;

namespace Qute.Directus.Models.Dashboards;

/// <summary>
/// Represents a Directus dashboard.
/// </summary>
public record DirectusDashboard
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Icon { get; init; }
    public string? Note { get; init; }
    public string? Color { get; init; }
    public DateTime? DateCreated { get; init; }
    public string? UserCreated { get; init; }
    public List<string>? Panels { get; init; }
}
