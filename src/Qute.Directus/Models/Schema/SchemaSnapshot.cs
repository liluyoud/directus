using System.Text.Json;

namespace Qute.Directus.Models.Schema;

/// <summary>
/// Represents a Directus schema snapshot from <c>GET /schema/snapshot</c>.
/// </summary>
public record SchemaSnapshot
{
    public string? Version { get; init; }
    public string? DirectusVersion { get; init; }
    public string? Vendor { get; init; }
    public JsonElement? Collections { get; init; }
    public JsonElement? Fields { get; init; }
    public JsonElement? Relations { get; init; }
}

/// <summary>
/// Represents the result of a schema diff operation.
/// </summary>
public record SchemaDiff
{
    public JsonElement? Diff { get; init; }
}

/// <summary>
/// Request body for <c>POST /schema/apply</c>.
/// </summary>
public record SchemaApplyRequest
{
    public required JsonElement Diff { get; init; }
}
