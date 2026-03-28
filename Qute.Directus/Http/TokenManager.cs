using Qute.Directus.Models.Auth;
using Qute.Directus.Serialization;
using System.Text.Json;

namespace Qute.Directus.Http;

/// <summary>
/// Manages access and refresh tokens, providing thread-safe automatic token refresh.
/// </summary>
public sealed class TokenManager : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _expiresAt = DateTime.MinValue;
    private readonly int _bufferSeconds;
    private Func<string, CancellationToken, Task<LoginResponse>>? _refreshFunc;

    public TokenManager(int bufferSeconds = 30)
    {
        _bufferSeconds = bufferSeconds;
    }

    /// <summary>Whether a valid (or refreshable) session exists.</summary>
    public bool HasToken => _accessToken is not null || _refreshToken is not null;

    /// <summary>
    /// Registers the function used to refresh the access token.
    /// </summary>
    public void SetRefreshFunction(Func<string, CancellationToken, Task<LoginResponse>> refreshFunc)
    {
        _refreshFunc = refreshFunc;
    }

    /// <summary>
    /// Stores the tokens from a login or refresh response.
    /// </summary>
    public void SetTokens(string accessToken, string? refreshToken, int expiresInSeconds)
    {
        _accessToken = accessToken;
        _refreshToken = refreshToken;
        _expiresAt = DateTime.UtcNow.AddSeconds(expiresInSeconds);
    }

    /// <summary>
    /// Clears all stored tokens (used on logout).
    /// </summary>
    public void ClearTokens()
    {
        _accessToken = null;
        _refreshToken = null;
        _expiresAt = DateTime.MinValue;
    }

    /// <summary>
    /// Gets the current access token, refreshing it automatically if expired or about to expire.
    /// </summary>
    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_accessToken is null)
            return null;

        if (DateTime.UtcNow.AddSeconds(_bufferSeconds) < _expiresAt)
            return _accessToken;

        // Token is expired or about to expire: attempt refresh
        if (_refreshToken is null || _refreshFunc is null)
            return _accessToken;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check after acquiring the lock
            if (DateTime.UtcNow.AddSeconds(_bufferSeconds) < _expiresAt)
                return _accessToken;

            var response = await _refreshFunc(_refreshToken, cancellationToken);
            SetTokens(response.AccessToken, response.RefreshToken, response.Expires);
            return _accessToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}
