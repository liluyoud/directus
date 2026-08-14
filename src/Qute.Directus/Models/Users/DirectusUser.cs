using System.Text.Json;

namespace Qute.Directus.Models.Users;

/// <summary>
/// Represents a Directus user.
/// </summary>
public record DirectusUser
{
    public string? Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
    public string? Location { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public List<string>? Tags { get; init; }
    public string? Avatar { get; init; }
    public string? Language { get; init; }
    public string? TfaSecret { get; init; }
    public string? Status { get; init; }
    public string? Role { get; init; }
    public string? Token { get; init; }
    public object? Policies { get; init; }
    public DateTime? LastAccess { get; init; }
    public string? LastPage { get; init; }
    public string? Provider { get; init; }
    public string? ExternalIdentifier { get; init; }
    public JsonElement? AuthData { get; init; }
    public bool? EmailNotifications { get; init; }
    public string? Appearance { get; init; }
    public string? ThemeDark { get; init; }
    public string? ThemeLight { get; init; }
    public JsonElement? ThemeLightOverrides { get; init; }
    public JsonElement? ThemeDarkOverrides { get; init; }
}

/// <summary>
/// Request body for <c>POST /users/invite</c>.
/// </summary>
public record UserInviteRequest
{
    public required string Email { get; init; }
    public required string Role { get; init; }
    public string? InviteUrl { get; init; }
}

/// <summary>
/// Request body for <c>POST /users/invite/accept</c>.
/// </summary>
public record AcceptInviteRequest
{
    public required string Token { get; init; }
    public required string Password { get; init; }
}

/// <summary>
/// Request body for <c>POST /users/register</c>.
/// </summary>
public record RegisterUserRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? VerificationUrl { get; init; }
}

/// <summary>
/// Request body for enabling TFA.
/// </summary>
public record TfaEnableRequest
{
    public required string Secret { get; init; }
    public required string Otp { get; init; }
}

/// <summary>
/// Request body for disabling TFA.
/// </summary>
public record TfaDisableRequest
{
    public required string Otp { get; init; }
}

/// <summary>
/// Response from generating a TFA secret.
/// </summary>
public record TfaGenerateResponse
{
    public string? Secret { get; init; }
    public string? OtpauthUrl { get; init; }
}
