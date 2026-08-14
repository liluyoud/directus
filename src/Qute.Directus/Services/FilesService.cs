using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Files;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus file operations (upload, import, CRUD).
/// </summary>
public sealed class FilesService
{
    private readonly DirectusHttpClient _http;

    public FilesService(DirectusHttpClient http) => _http = http;

    // ─── Read ──────────────────────────────────────────────────────────

    /// <summary>List all files.</summary>
    public Task<DirectusListResponse<DirectusFile>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusFile>("files", query, ct);

    /// <summary>Retrieve a single file by ID.</summary>
    public Task<DirectusFile> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusFile>($"files/{id}", query, ct);

    // ─── Upload ────────────────────────────────────────────────────────

    /// <summary>Upload a file using multipart form data.</summary>
    public Task<DirectusFile> UploadAsync(Stream fileStream, string fileName, string? contentType = null, DirectusFile? metadata = null, CancellationToken ct = default)
    {
        var content = new MultipartFormDataContent();

        if (metadata is not null)
        {
            if (metadata.Title is not null) content.Add(new StringContent(metadata.Title), "title");
            if (metadata.Description is not null) content.Add(new StringContent(metadata.Description), "description");
            if (metadata.Folder is not null) content.Add(new StringContent(metadata.Folder), "folder");
        }

        var streamContent = new StreamContent(fileStream);
        if (!string.IsNullOrEmpty(contentType))
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

        content.Add(streamContent, "file", fileName);

        return _http.PostMultipartAsync<DirectusFile>("files", content, ct);
    }

    /// <summary>Import a file from a URL.</summary>
    public Task<DirectusFile> ImportAsync(string url, DirectusFile? metadata = null, CancellationToken ct = default)
    {
        var request = new FileImportRequest { Url = url, Data = metadata };
        return _http.PostAsync<DirectusFile>("files/import", request, ct: ct);
    }

    // ─── Update ────────────────────────────────────────────────────────

    /// <summary>Update file metadata (and optionally replace the file).</summary>
    public Task<DirectusFile> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusFile>($"files/{id}", data, query, ct);

    /// <summary>Update file with a new file stream (replace).</summary>
    public Task<DirectusFile> UpdateWithFileAsync(string id, Stream fileStream, string fileName, string? contentType = null, CancellationToken ct = default)
    {
        var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(fileStream);
        if (!string.IsNullOrEmpty(contentType))
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        content.Add(streamContent, "file", fileName);

        return _http.PatchMultipartAsync<DirectusFile>($"files/{id}", content, ct);
    }

    /// <summary>Update multiple files at once.</summary>
    public Task<DirectusListResponse<DirectusFile>> UpdateManyAsync(object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchListAsync<DirectusFile>("files", data, query, ct);

    // ─── Delete ────────────────────────────────────────────────────────

    /// <summary>Delete a file by ID (also removes from disk).</summary>
    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"files/{id}", ct);

    /// <summary>Delete multiple files by IDs.</summary>
    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("files", ids, ct);
}
