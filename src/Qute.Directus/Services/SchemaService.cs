using System.Text.Json;
using Qute.Directus.Http;
using Qute.Directus.Models.Schema;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus schema management (snapshot, diff, apply).
/// </summary>
public sealed class SchemaService
{
    private readonly DirectusHttpClient _http;

    public SchemaService(DirectusHttpClient http) => _http = http;

    /// <summary>Retrieve a snapshot of the current schema.</summary>
    public Task<SchemaSnapshot> GetSnapshotAsync(CancellationToken ct = default)
        => _http.GetAsync<SchemaSnapshot>("schema/snapshot", ct: ct);

    /// <summary>Compute the diff between the current schema and a provided snapshot.</summary>
    public Task<SchemaDiff> DiffAsync(object snapshot, CancellationToken ct = default)
        => _http.PostAsync<SchemaDiff>("schema/diff", snapshot, ct: ct);

    /// <summary>Apply a schema diff to update the database.</summary>
    public Task ApplyAsync(object diff, CancellationToken ct = default)
        => _http.PostAsync("schema/apply", diff, ct);
}
