using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Qute.Directus;
using Qute.Directus.Models.Users;

namespace Qute.PWA.Auth;

/// <summary>
/// Custom <see cref="AuthenticationStateProvider"/> that checks the current Directus session
/// by calling <c>/users/me</c>. Works with both cookie-based and token-based auth.
/// </summary>
public sealed class DirectusAuthStateProvider : AuthenticationStateProvider
{
    private readonly DirectusClient _client;
    private DirectusUser? _cachedUser;

    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public DirectusAuthStateProvider(DirectusClient client)
    {
        _client = client;
    }

    /// <summary>
    /// The currently authenticated Directus user, or null if not authenticated.
    /// </summary>
    public DirectusUser? CurrentUser => _cachedUser;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var user = await _client.Users.GetCurrentAsync();
            _cachedUser = user;
            return BuildAuthState(user);
        }
        catch
        {
            _cachedUser = null;
            return Anonymous;
        }
    }

    /// <summary>
    /// Performs login via email/password and notifies the auth state has changed.
    /// </summary>
    public async Task<DirectusUser> LoginAsync(string email, string password)
    {
        await _client.Auth.LoginAsync(email, password);
        var user = await _client.Users.GetCurrentAsync();
        _cachedUser = user;

        NotifyAuthenticationStateChanged(Task.FromResult(BuildAuthState(user)));
        return user;
    }

    /// <summary>
    /// Logs out and notifies the auth state has changed.
    /// </summary>
    public async Task LogoutAsync()
    {
        try
        {
            await _client.Auth.LogoutAsync();
        }
        catch
        {
            // Ignore errors during logout (session may already be invalid)
        }

        _cachedUser = null;
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    /// <summary>
    /// Forces a re-check of the authentication state against the Directus API.
    /// </summary>
    public void Refresh()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static AuthenticationState BuildAuthState(DirectusUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
        };

        if (user.Role is not null)
            claims.Add(new Claim(ClaimTypes.Role, user.Role));

        var identity = new ClaimsIdentity(claims, "Directus");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
