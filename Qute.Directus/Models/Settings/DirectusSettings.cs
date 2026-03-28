using System.Text.Json;

namespace Qute.Directus.Models.Settings;

/// <summary>
/// Represents Directus global settings (singleton).
/// </summary>
public record DirectusSettings
{
    public int? Id { get; init; }
    public string? ProjectName { get; init; }
    public string? ProjectDescriptor { get; init; }
    public string? ProjectUrl { get; init; }
    public string? ProjectColor { get; init; }
    public string? ProjectLogo { get; init; }
    public string? PublicForeground { get; init; }
    public string? PublicBackground { get; init; }
    public string? PublicFavicon { get; init; }
    public string? PublicNote { get; init; }
    public JsonElement? AuthLoginAttempts { get; init; }
    public string? AuthPasswordPolicy { get; init; }
    public string? StorageAssetTransform { get; init; }
    public JsonElement? StorageAssetPresets { get; init; }
    public JsonElement? CustomCss { get; init; }
    public string? StorageDefaultFolder { get; init; }
    public JsonElement? Basemaps { get; init; }
    public string? MapboxKey { get; init; }
    public JsonElement? ModuleBar { get; init; }
    public string? DefaultLanguage { get; init; }
    public JsonElement? CustomAspectRatios { get; init; }
    public string? PublicRegistration { get; init; }
    public string? PublicRegistrationVerifyEmail { get; init; }
    public string? PublicRegistrationRole { get; init; }
    public string? PublicRegistrationEmailFilter { get; init; }
    public JsonElement? ThemeLightOverrides { get; init; }
    public JsonElement? ThemeDarkOverrides { get; init; }
}
