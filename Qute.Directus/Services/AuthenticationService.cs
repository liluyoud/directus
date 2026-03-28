using Qute.Directus.Http;
using Qute.Directus.Models.Auth;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus authentication operations.
/// </summary>
public sealed class AuthenticationService
{
    private readonly DirectusHttpClient _http;
    private readonly TokenManager _tokenManager;

    public AuthenticationService(DirectusHttpClient http, TokenManager tokenManager)
    {
        _http = http;
        _tokenManager = tokenManager;

        // Wire up the token manager's refresh function
        _tokenManager.SetRefreshFunction(RefreshInternalAsync);
    }

    /// <summary>
    /// Authenticate as a user with email and password.
    /// Tokens are stored automatically for subsequent requests.
    /// </summary>
    public async Task<LoginResponse> LoginAsync(string email, string password, string? otp = null, CancellationToken ct = default)
    {
        var request = new LoginRequest
        {
            Email = email,
            Password = password,
            Mode = "json",
            Otp = otp
        };

        var response = await _http.PostRawAsync<LoginResponse>("auth/login", request, ct);
        _tokenManager.SetTokens(response.AccessToken, response.RefreshToken, response.Expires);
        return response;
    }

    /// <summary>
    /// Invalidate the current refresh token, destroying the user's session.
    /// </summary>
    public async Task LogoutAsync(string? refreshToken = null, CancellationToken ct = default)
    {
        var request = new LogoutRequest
        {
            RefreshToken = refreshToken,
            Mode = "json"
        };

        await _http.PostAsync("auth/logout", request, ct);
        _tokenManager.ClearTokens();
    }

    /// <summary>
    /// Refresh the access token using a refresh token.
    /// </summary>
    public async Task<LoginResponse> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var response = await RefreshInternalAsync(refreshToken, ct);
        _tokenManager.SetTokens(response.AccessToken, response.RefreshToken, response.Expires);
        return response;
    }

    /// <summary>
    /// List all configured auth providers.
    /// </summary>
    public async Task<List<string>> ListProvidersAsync(CancellationToken ct = default)
    {
        return await _http.GetAsync<List<string>>("auth/oauth", ct: ct);
    }

    /// <summary>
    /// Request a password reset email for the given user.
    /// </summary>
    public async Task RequestPasswordResetAsync(string email, string? resetUrl = null, CancellationToken ct = default)
    {
        var request = new PasswordRequestRequest { Email = email, ResetUrl = resetUrl };
        await _http.PostAsync("auth/password/request", request, ct);
    }

    /// <summary>
    /// Reset a user's password using the token received via email.
    /// </summary>
    public async Task ResetPasswordAsync(string token, string newPassword, CancellationToken ct = default)
    {
        var request = new PasswordResetRequest { Token = token, Password = newPassword };
        await _http.PostAsync("auth/password/reset", request, ct);
    }

    private async Task<LoginResponse> RefreshInternalAsync(string refreshToken, CancellationToken ct)
    {
        var request = new RefreshRequest { RefreshToken = refreshToken, Mode = "json" };
        return await _http.PostRawAsync<LoginResponse>("auth/refresh", request, ct);
    }
}
