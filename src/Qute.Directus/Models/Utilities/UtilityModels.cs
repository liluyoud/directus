namespace Qute.Directus.Models.Utilities;

/// <summary>
/// Request body for <c>POST /utils/hash/generate</c>.
/// </summary>
public record HashGenerateRequest
{
    public required string String { get; init; }
}

/// <summary>
/// Response from <c>POST /utils/hash/generate</c>.
/// </summary>
public record HashGenerateResponse
{
    public string? Hash { get; init; }
}

/// <summary>
/// Request body for <c>POST /utils/hash/verify</c>.
/// </summary>
public record HashVerifyRequest
{
    public required string String { get; init; }
    public required string Hash { get; init; }
}

/// <summary>
/// Response from <c>POST /utils/hash/verify</c>.
/// </summary>
public record HashVerifyResponse
{
    public bool Valid { get; init; }
}

/// <summary>
/// Request body for <c>POST /utils/sort/{collection}</c>.
/// </summary>
public record SortRequest
{
    public required string Item { get; init; }
    public required int To { get; init; }
}

/// <summary>
/// Response from <c>GET /utils/random/string</c>.
/// </summary>
public record RandomStringResponse
{
    public string? Random { get; init; }
}
