namespace Qute.Directus.Models.Auth;

/// <summary>
/// Request body for <c>POST /auth/login</c>.
/// </summary>
public record LoginRequest
{
    /// <summary>Email address of the user.</summary>
    public required string Email { get; init; }

    /// <summary>Password of the user.</summary>
    public required string Password { get; init; }

    /// <summary>
    /// Whether to retrieve the refresh token in the JSON response or in an httpOnly cookie.
    /// Default is <c>"json"</c>.
    /// </summary>
    public string? Mode { get; init; }

    /// <summary>One-time password for MFA-enabled accounts.</summary>
    public string? Otp { get; init; }
}

/// <summary>
/// Response body from <c>POST /auth/login</c> and <c>POST /auth/refresh</c>.
/// </summary>
public record LoginResponse
{
    /// <summary>JWT access token.</summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>Token validity in seconds.</summary>
    public int Expires { get; init; }

    /// <summary>Refresh token (only present when mode is "json").</summary>
    public string? RefreshToken { get; init; }
}

/// <summary>
/// Request body for <c>POST /auth/refresh</c>.
/// </summary>
public record RefreshRequest
{
    /// <summary>The refresh token.</summary>
    public required string RefreshToken { get; init; }

    /// <summary>Token mode: <c>"json"</c> or <c>"cookie"</c>.</summary>
    public string? Mode { get; init; }
}

/// <summary>
/// Request body for <c>POST /auth/password/request</c>.
/// </summary>
public record PasswordRequestRequest
{
    /// <summary>Email address of the user requesting a password reset.</summary>
    public required string Email { get; init; }

    /// <summary>Optional custom URL for the password reset link.</summary>
    public string? ResetUrl { get; init; }
}

/// <summary>
/// Request body for <c>POST /auth/password/reset</c>.
/// </summary>
public record PasswordResetRequest
{
    /// <summary>One-time JWT token from the reset email.</summary>
    public required string Token { get; init; }

    /// <summary>New password for the user.</summary>
    public required string Password { get; init; }
}

/// <summary>
/// Request body for <c>POST /auth/logout</c>.
/// </summary>
public record LogoutRequest
{
    /// <summary>The refresh token to invalidate.</summary>
    public string? RefreshToken { get; init; }

    /// <summary>Token mode: <c>"json"</c> or <c>"cookie"</c>.</summary>
    public string? Mode { get; init; }
}
