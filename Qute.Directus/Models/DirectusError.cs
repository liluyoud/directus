namespace Qute.Directus.Models;

/// <summary>
/// Represents a single error object returned by the Directus API.
/// </summary>
public record DirectusError(
    string Message,
    int? StatusCode = null,
    string? Code = null,
    object? Extensions = null
);
