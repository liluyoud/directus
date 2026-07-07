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
