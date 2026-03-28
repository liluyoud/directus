using System.Text.Json;

namespace Qute.Directus.Models.Files;

/// <summary>
/// Represents a Directus file object.
/// </summary>
public record DirectusFile
{
    public string? Id { get; init; }
    public string? Storage { get; init; }
    public string? FilenameDisk { get; init; }
    public string? FilenameDownload { get; init; }
    public string? Title { get; init; }
    public string? Type { get; init; }
    public string? Folder { get; init; }
    public string? UploadedBy { get; init; }
    public DateTime? CreatedOn { get; init; }
    public string? Charset { get; init; }
    public long? Filesize { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public int? Duration { get; init; }
    public string? Embed { get; init; }
    public string? Description { get; init; }
    public string? Location { get; init; }
    public List<string>? Tags { get; init; }
    public JsonElement? Metadata { get; init; }
    public DateTime? UploadedOn { get; init; }
    public DateTime? ModifiedOn { get; init; }
    public string? FocalPointX { get; init; }
    public string? FocalPointY { get; init; }
}

/// <summary>
/// Request body for <c>POST /files/import</c>.
/// </summary>
public record FileImportRequest
{
    /// <summary>URL to download the file from.</summary>
    public required string Url { get; init; }

    /// <summary>Optional file metadata to set on the imported file.</summary>
    public DirectusFile? Data { get; init; }
}
