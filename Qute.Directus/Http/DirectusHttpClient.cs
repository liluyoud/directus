using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Qute.Directus.Models;
using Qute.Directus.Serialization;

namespace Qute.Directus.Http;

/// <summary>
/// Low-level HTTP client wrapper that handles authentication headers, request serialization,
/// response deserialization, and error handling for the Directus REST API.
/// </summary>
public sealed class DirectusHttpClient
{
    /// <summary>
    /// Well-known key used by the Blazor WebAssembly runtime to read the Fetch API
    /// <c>credentials</c> option from <see cref="HttpRequestMessage.Options"/>.
    /// </summary>
    private readonly HttpClient _httpClient;
    private readonly TokenManager _tokenManager;
    private readonly DirectusOptions _options;

    public DirectusHttpClient(HttpClient httpClient, TokenManager tokenManager, DirectusOptions options)
    {
        _httpClient = httpClient;
        _tokenManager = tokenManager;
        _options = options;

        _httpClient.BaseAddress ??= options.GetBaseUri();
    }

    /// <summary>The underlying <see cref="TokenManager"/> for this client.</summary>
    public TokenManager TokenManager => _tokenManager;

    // ─── GET ───────────────────────────────────────────────────────────

    /// <summary>Sends a GET request and deserializes the response as <c>{ "data": T }</c>.</summary>
    public async Task<T> GetAsync<T>(string path, QueryParameters? query = null, CancellationToken ct = default)
    {
        var url = BuildUrl(path, query);
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    /// <summary>Sends a GET request and deserializes the full wrapper <c>{ "data": T, "meta": ... }</c>.</summary>
    public async Task<DirectusResponse<T>> GetWithMetaAsync<T>(string path, QueryParameters? query = null, CancellationToken ct = default)
    {
        var url = BuildUrl(path, query);
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        return await ReadFullResponseAsync<T>(response, ct);
    }

    /// <summary>Sends a GET request and deserializes as <c>{ "data": List&lt;T&gt;, "meta": ... }</c>.</summary>
    public async Task<DirectusListResponse<T>> GetListAsync<T>(string path, QueryParameters? query = null, CancellationToken ct = default)
    {
        var url = BuildUrl(path, query);
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        return await ReadListResponseAsync<T>(response, ct);
    }

    /// <summary>Sends a GET request and returns the raw response stream (used for assets/downloads).</summary>
    public async Task<Stream> GetStreamAsync(string path, QueryParameters? query = null, CancellationToken ct = default)
    {
        var url = BuildUrl(path, query);
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        await ApplyAuthAsync(request, ct);

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadAsStreamAsync(ct);
    }

    /// <summary>Sends a GET request and returns raw string content.</summary>
    public async Task<string> GetStringAsync(string path, QueryParameters? query = null, CancellationToken ct = default)
    {
        var url = BuildUrl(path, query);
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadAsStringAsync(ct);
    }

    // ─── POST ──────────────────────────────────────────────────────────

    /// <summary>Sends a POST request with a JSON body and deserializes <c>{ "data": T }</c>.</summary>
    public async Task<T> PostAsync<T>(string path, object? body = null, QueryParameters? query = null, CancellationToken ct = default)
    {
        var url = BuildUrl(path, query);
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        SetJsonBody(request, body);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    /// <summary>Sends a POST request with a JSON body and deserializes list response.</summary>
    public async Task<DirectusListResponse<T>> PostListAsync<T>(string path, object? body = null, QueryParameters? query = null, CancellationToken ct = default)
    {
        var url = BuildUrl(path, query);
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        SetJsonBody(request, body);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        return await ReadListResponseAsync<T>(response, ct);
    }

    /// <summary>Sends a POST request with no expected response content (204).</summary>
    public async Task PostAsync(string path, object? body = null, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        SetJsonBody(request, body);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);
    }

    /// <summary>Sends a POST request with multipart form data (used for file uploads).</summary>
    public async Task<T> PostMultipartAsync<T>(string path, MultipartFormDataContent content, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = content };
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    /// <summary>Sends a raw POST and returns the full response for manual handling (e.g., login).</summary>
    public async Task<TResponse> PostRawAsync<TResponse>(string path, object? body = null, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        SetJsonBody(request, body);
        ApplyBrowserCredentials(request);
        // NOTE: no auth header applied – used for login
        using var response = await _httpClient.SendAsync(request, ct);
        return await ReadResponseAsync<TResponse>(response, ct);
    }

    /// <summary>Sends a POST with auth and returns <c>{ "data": T }</c>.</summary>
    public async Task<TResponse> PostAuthenticatedRawAsync<TResponse>(string path, object? body = null, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        SetJsonBody(request, body);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        return await ReadResponseAsync<TResponse>(response, ct);
    }

    // ─── PATCH ─────────────────────────────────────────────────────────

    /// <summary>Sends a PATCH request and deserializes <c>{ "data": T }</c>.</summary>
    public async Task<T> PatchAsync<T>(string path, object? body = null, QueryParameters? query = null, CancellationToken ct = default)
    {
        var url = BuildUrl(path, query);
        using var request = new HttpRequestMessage(HttpMethod.Patch, url);
        SetJsonBody(request, body);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    /// <summary>Sends a PATCH request and deserializes list response.</summary>
    public async Task<DirectusListResponse<T>> PatchListAsync<T>(string path, object? body = null, QueryParameters? query = null, CancellationToken ct = default)
    {
        var url = BuildUrl(path, query);
        using var request = new HttpRequestMessage(HttpMethod.Patch, url);
        SetJsonBody(request, body);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        return await ReadListResponseAsync<T>(response, ct);
    }

    /// <summary>Sends a PATCH with multipart form data (used for file update + replace).</summary>
    public async Task<T> PatchMultipartAsync<T>(string path, MultipartFormDataContent content, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, path) { Content = content };
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    // ─── DELETE ────────────────────────────────────────────────────────

    /// <summary>Sends a DELETE request with no response body.</summary>
    public async Task DeleteAsync(string path, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, path);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);
    }

    /// <summary>Sends a DELETE request with a JSON body (used for bulk deletes).</summary>
    public async Task DeleteAsync(string path, object body, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, path);
        SetJsonBody(request, body);
        await ApplyAuthAsync(request, ct);

        using var response = await _httpClient.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);
    }

    // ─── Internal helpers ──────────────────────────────────────────────

    private static string BuildUrl(string path, QueryParameters? query)
    {
        if (query is null)
            return path;

        var qs = query.ToQueryString();
        return string.IsNullOrEmpty(qs) ? path : $"{path}?{qs}";
    }

    private async Task ApplyAuthAsync(HttpRequestMessage request, CancellationToken ct)
    {
        ApplyBrowserCredentials(request);

        if (!string.IsNullOrEmpty(_options.StaticToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.StaticToken);
            return;
        }

        var token = await _tokenManager.GetAccessTokenAsync(ct);
        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    /// <summary>
    /// When <see cref="DirectusOptions.UseBrowserCredentials"/> is enabled, sets the Fetch API
    /// <c>credentials</c> option to <c>"include"</c> so the browser sends cookies and other
    /// stored credentials with cross-origin requests.
    /// </summary>
    private void ApplyBrowserCredentials(HttpRequestMessage request)
    {
        if (!_options.UseBrowserCredentials)
            return;
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
    }

    private static void SetJsonBody(HttpRequestMessage request, object? body)
    {
        if (body is null) return;

        var json = JsonSerializer.Serialize(body, DirectusJsonOptions.Default);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        await EnsureSuccessAsync(response, ct);

        var wrapper = await response.Content.ReadFromJsonAsync<DirectusResponse<T>>(DirectusJsonOptions.Default, ct)
            ?? throw new DirectusException(response.StatusCode, "Response was null.");
        return wrapper.Data;
    }

    private static async Task<DirectusResponse<T>> ReadFullResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        await EnsureSuccessAsync(response, ct);

        var wrapper = await response.Content.ReadFromJsonAsync<DirectusResponse<T>>(DirectusJsonOptions.Default, ct);
        return wrapper ?? throw new DirectusException(response.StatusCode, "Response was null.");
    }

    private static async Task<DirectusListResponse<T>> ReadListResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        await EnsureSuccessAsync(response, ct);

        var wrapper = await response.Content.ReadFromJsonAsync<DirectusListResponse<T>>(DirectusJsonOptions.Default, ct);
        return wrapper ?? new DirectusListResponse<T>();
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
            return;

        string body;
        try
        {
            body = await response.Content.ReadAsStringAsync(ct);
        }
        catch
        {
            throw new DirectusException(response.StatusCode, $"Request failed with status {(int)response.StatusCode}.");
        }

        try
        {
            var errorResponse = JsonSerializer.Deserialize<DirectusErrorResponse>(body, DirectusJsonOptions.Default);
            if (errorResponse?.Errors is { Count: > 0 })
                throw new DirectusException(response.StatusCode, errorResponse.Errors);
        }
        catch (JsonException) { /* not a structured error */ }

        throw new DirectusException(response.StatusCode, body);
    }
}
