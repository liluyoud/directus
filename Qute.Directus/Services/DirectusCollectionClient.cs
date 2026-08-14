using Qute.Directus.Models;

namespace Qute.Directus.Services;

/// <summary>
/// Strongly-typed client scoped to a single Directus collection, resolved from
/// <typeparamref name="T"/>'s <see cref="Models.Items.DirectusCollectionAttribute"/>.
/// Mirrors <see cref="ItemsService"/> without repeating the collection name at every call.
/// Obtained via <see cref="ItemsService.Of{T}"/>.
/// </summary>
public sealed class DirectusCollectionClient<T> where T : class
{
    private readonly ItemsService _items;
    private readonly string _collection;

    internal DirectusCollectionClient(ItemsService items, string collection)
    {
        _items = items;
        _collection = collection;
    }

    /// <summary>List all items in the collection.</summary>
    public Task<DirectusListResponse<T>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _items.GetManyAsync<T>(_collection, query, ct);

    /// <summary>List all items in the collection using a query builder.</summary>
    public Task<DirectusListResponse<T>> GetManyAsync(Action<QueryParameters> configure, CancellationToken ct = default)
        => _items.GetManyAsync<T>(_collection, configure, ct);

    /// <summary>Retrieve a single item by ID.</summary>
    public Task<T> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _items.GetByIdAsync<T>(_collection, id, query, ct);

    /// <summary>Retrieve a single item by ID using a query builder.</summary>
    public Task<T> GetByIdAsync(string id, Action<QueryParameters> configure, CancellationToken ct = default)
        => _items.GetByIdAsync<T>(_collection, id, configure, ct);

    /// <summary>Retrieve the singleton item for this collection.</summary>
    public Task<T> GetSingletonAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _items.GetSingletonAsync<T>(_collection, query, ct);

    /// <summary>Create a single item.</summary>
    public Task<T> CreateAsync(object item, QueryParameters? query = null, CancellationToken ct = default)
        => _items.CreateAsync<T>(_collection, item, query, ct);

    /// <summary>Create multiple items.</summary>
    public Task<DirectusListResponse<T>> CreateManyAsync(IEnumerable<object> items, QueryParameters? query = null, CancellationToken ct = default)
        => _items.CreateManyAsync<T>(_collection, items, query, ct);

    /// <summary>Update a single item by ID.</summary>
    public Task<T> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _items.UpdateAsync<T>(_collection, id, data, query, ct);

    /// <summary>Update multiple items at once.</summary>
    public Task<DirectusListResponse<T>> UpdateManyAsync(object data, QueryParameters? query = null, CancellationToken ct = default)
        => _items.UpdateManyAsync<T>(_collection, data, query, ct);

    /// <summary>Update the singleton item.</summary>
    public Task<T> UpdateSingletonAsync(object data, QueryParameters? query = null, CancellationToken ct = default)
        => _items.UpdateSingletonAsync<T>(_collection, data, query, ct);

    /// <summary>Delete a single item by ID.</summary>
    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _items.DeleteAsync(_collection, id, ct);

    /// <summary>Delete multiple items by IDs or query.</summary>
    public Task DeleteManyAsync(object keysOrQuery, CancellationToken ct = default)
        => _items.DeleteManyAsync(_collection, keysOrQuery, ct);
}
