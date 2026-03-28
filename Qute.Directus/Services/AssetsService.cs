using Qute.Directus.Http;
using Qute.Directus.Models;

namespace Qute.Directus.Services;

/// <summary>
/// Service for retrieving Directus assets (files with optional image transformations).
/// </summary>
public sealed class AssetsService
{
    private readonly DirectusHttpClient _http;

    public AssetsService(DirectusHttpClient http) => _http = http;

    /// <summary>
    /// Get an asset (file) as a stream, optionally applying image transformations.
    /// </summary>
    /// <param name="id">File ID.</param>
    /// <param name="width">Width in pixels for the transformed image.</param>
    /// <param name="height">Height in pixels for the transformed image.</param>
    /// <param name="fit">Resizing behavior: <c>cover</c>, <c>contain</c>, <c>inside</c>, <c>outside</c>.</param>
    /// <param name="quality">JPEG/WebP quality (1-100).</param>
    /// <param name="format">Output format: <c>jpg</c>, <c>png</c>, <c>webp</c>, <c>tiff</c>, <c>avif</c>.</param>
    /// <param name="key">Preset key for pre-configured transformations.</param>
    /// <param name="ct">Cancellation token.</param>
    public Task<Stream> GetAsync(
        string id,
        int? width = null,
        int? height = null,
        string? fit = null,
        int? quality = null,
        string? format = null,
        string? key = null,
        CancellationToken ct = default)
    {
        var query = new QueryParameters();
        if (width.HasValue) query.Custom("width", width.Value.ToString());
        if (height.HasValue) query.Custom("height", height.Value.ToString());
        if (fit is not null) query.Custom("fit", fit);
        if (quality.HasValue) query.Custom("quality", quality.Value.ToString());
        if (format is not null) query.Custom("format", format);
        if (key is not null) query.Custom("key", key);

        return _http.GetStreamAsync($"assets/{id}", query, ct);
    }
}
