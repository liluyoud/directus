namespace Qute.Directus.Models.Items;

/// <summary>
/// Optional base record for typed collection item models, covering the system fields
/// Directus tracks on every collection (primary key and create/update audit metadata).
/// Inherit from this instead of redeclaring these fields on every model.
/// </summary>
/// <typeparam name="TKey">
/// The collection's primary key type — <see cref="int"/> for auto-increment collections,
/// <see cref="string"/> for the default Directus UUID.
/// </typeparam>
public abstract record DirectusItem<TKey>
{
    public TKey Id { get; init; } = default!;
    public int? Sort { get; init; }
    public DateTime? DateCreated { get; init; }
    public DateTime? DateUpdated { get; init; }
    public string? UserCreated { get; init; }
    public string? UserUpdated { get; init; }
}

/// <summary>Shorthand for collections using the default Directus UUID primary key.</summary>
public abstract record DirectusItem : DirectusItem<string>;

/// <summary>
/// Like <see cref="DirectusItem{TKey}"/>, but also declares the boolean <c>archived</c> field
/// introduced by the Directus 12+ Data Studio convention. Use this base for collections that use
/// that convention, and query them with <see cref="Qute.Directus.Models.QueryParameters.NotArchived"/>/
/// <see cref="Qute.Directus.Models.QueryParameters.OnlyArchived"/>. For collections created before Directus 12 that still
/// use a string <c>status</c> field, inherit from <see cref="DirectusItem{TKey}"/> instead and declare
/// your own <c>Status</c> property.
/// </summary>
public abstract record ArchivableDirectusItem<TKey> : DirectusItem<TKey>
{
    public bool? Archived { get; init; }
}

/// <summary>Shorthand for <see cref="ArchivableDirectusItem{TKey}"/> using the default Directus UUID primary key.</summary>
public abstract record ArchivableDirectusItem : ArchivableDirectusItem<string>;
