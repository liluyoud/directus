using System.Net;
using Qute.Directus.Models;

namespace Qute.Directus.Http;

/// <summary>
/// Exception thrown when a Directus API request fails.
/// </summary>
public class DirectusException : Exception
{
    /// <summary>
    /// HTTP status code returned by the Directus API.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Structured error details returned by the Directus API.
    /// </summary>
    public IReadOnlyList<DirectusError> Errors { get; }

    public DirectusException(HttpStatusCode statusCode, IReadOnlyList<DirectusError> errors)
        : base(BuildMessage(statusCode, errors))
    {
        StatusCode = statusCode;
        Errors = errors;
    }

    public DirectusException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = [new DirectusError(message, (int)statusCode)];
    }

    public DirectusException(string message, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = HttpStatusCode.InternalServerError;
        Errors = [new DirectusError(message)];
    }

    private static string BuildMessage(HttpStatusCode statusCode, IReadOnlyList<DirectusError> errors)
    {
        if (errors.Count == 0)
            return $"Directus API returned {(int)statusCode} ({statusCode}).";

        var messages = string.Join("; ", errors.Select(e => e.Message));
        return $"Directus API returned {(int)statusCode}: {messages}";
    }
}
