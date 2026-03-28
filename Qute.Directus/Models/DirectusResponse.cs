namespace Qute.Directus.Models;

/// <summary>
/// Generic wrapper for a single-item Directus API response: <c>{ "data": T }</c>.
/// </summary>
public record DirectusResponse<T>
{
    public T Data { get; init; } = default!;
    public ResponseMeta? Meta { get; init; }
}

/// <summary>
/// Generic wrapper for a list Directus API response: <c>{ "data": [T], "meta": {} }</c>.
/// </summary>
public record DirectusListResponse<T>
{
    public List<T> Data { get; init; } = [];
    public ResponseMeta? Meta { get; init; }
}

/// <summary>
/// Represents the optional metadata block in Directus responses.
/// </summary>
public record ResponseMeta
{
    /// <summary>Total item count of the collection (unfiltered).</summary>
    public int? TotalCount { get; init; }

    /// <summary>Filtered item count.</summary>
    public int? FilterCount { get; init; }
}

/// <summary>
/// Wrapper for error responses from Directus: <c>{ "errors": [...] }</c>.
/// </summary>
public record DirectusErrorResponse
{
    public List<DirectusError> Errors { get; init; } = [];
}
