namespace Qute.Directus.Models.Items;

/// <summary>
/// Declares which Directus collection a typed item model maps to, enabling
/// <see cref="Qute.Directus.Services.ItemsService.Of{T}"/> to resolve the collection name
/// without repeating it as a string at every call site.
/// </summary>
/// <example>
/// <code>
/// [DirectusCollection("articles")]
/// public record Article : DirectusItem
/// {
///     public string? Title { get; init; }
/// }
///
/// var articles = await client.Items.Of&lt;Article&gt;().GetManyAsync(q => q.NotArchived());
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class DirectusCollectionAttribute(string name) : Attribute
{
    /// <summary>The Directus collection name (e.g. <c>"articles"</c>).</summary>
    public string Name { get; } = name;
}
