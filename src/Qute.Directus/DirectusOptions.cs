namespace Qute.Directus;

/// <summary>
/// Configuration options for the Directus client.
/// </summary>
public sealed class DirectusOptions
{
    /// <summary>
    /// Base URL of the Directus instance (e.g., "https://my-directus.com").
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Static token for authentication. When set, token-based auth is used instead of login/refresh flow.
    /// </summary>
    public string? StaticToken { get; set; }

    /// <summary>
    /// Whether to automatically refresh access tokens before they expire. Default is true.
    /// </summary>
    public bool AutoRefreshToken { get; set; } = true;

    /// <summary>
    /// Buffer in seconds before token expiration to trigger a refresh. Default is 30 seconds.
    /// </summary>
    public int TokenRefreshBufferSeconds { get; set; } = 30;

    /// <summary>
    /// Name used when registering the HttpClient via IHttpClientFactory.
    /// </summary>
    public string HttpClientName { get; set; } = "DirectusClient";

    /// <summary>
    /// When <c>true</c>, every outgoing request will include browser-managed credentials
    /// (cookies, TLS client certificates, Authorization headers) by setting the Fetch API's
    /// <c>credentials</c> option to <c>"include"</c>.
    /// <para>
    /// This is required in <b>Blazor WebAssembly</b> scenarios where the Directus session
    /// is managed via HTTP-only cookies instead of bearer tokens.
    /// </para>
    /// </summary>
    public bool UseBrowserCredentials { get; set; }

    internal Uri GetBaseUri()
    {
        var url = BaseUrl.TrimEnd('/');
        return new Uri(url);
    }
}
