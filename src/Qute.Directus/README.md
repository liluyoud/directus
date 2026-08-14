# Qute.Directus

A comprehensive C# client library for the [Directus](https://directus.io) REST API. Targets **Directus 12**, maps all publicly available REST endpoints, and gives you a fluent query builder for filtering, sorting, and paginating items.

- [Installation](#installation)
- [Quick start](#quick-start)
- [Dependency injection](#dependency-injection)
- [Modeling collection items](#modeling-collection-items)
- [Querying items — the fluent builder](#querying-items--the-fluent-builder)
- [Typed collection client (`Items.Of<T>()`)](#typed-collection-client-itemsoft)
- [CRUD examples](#crud-examples)
- [Working with dynamic/untyped data](#working-with-dynamicuntyped-data)
- [Iterating responses and pagination metadata](#iterating-responses-and-pagination-metadata)
- [Error handling](#error-handling)
- [Authentication](#authentication)
- [Available services](#available-services)
- [Versioning and breaking changes](#versioning-and-breaking-changes)

## Installation

```bash
dotnet add package Qute.Directus
```

## Quick start

### Static token

```csharp
using Qute.Directus;

var client = new DirectusClient(new DirectusOptions
{
    BaseUrl = "https://my-directus.com",
    StaticToken = "my-static-token"
});

var articles = await client.Items.GetManyAsync<Article>("articles", q => q
    .Fields("id", "title", "content")
    .NotArchived()
    .Where("category", "news")
    .Sort("-date_created")
    .Limit(25));

foreach (var article in articles) // DirectusListResponse<T> is IReadOnlyList<T>
    Console.WriteLine(article.Title);
```

### Email/password login

```csharp
var client = new DirectusClient(new DirectusOptions { BaseUrl = "https://my-directus.com" });

await client.Auth.LoginAsync("admin@example.com", "password");
// client.Items / client.Users / ... are now authenticated as this user,
// and the access token refreshes itself automatically before it expires.

var me = await client.Users.GetCurrentAsync();
```

`DirectusClient` implements `IDisposable` — dispose it (or let your DI container do it) when you own the `HttpClient`, e.g. via a `using` block in a console app.

## Dependency injection

```csharp
// Program.cs / Startup.cs
services.AddDirectus(options =>
{
    options.BaseUrl = "https://my-directus.com";
    options.StaticToken = builder.Configuration["Directus:Token"]; // or use LoginAsync instead
});
```

```csharp
// Anywhere else in your app
public class ArticlesService(DirectusClient directus)
{
    public Task<DirectusListResponse<Article>> GetLatestAsync(CancellationToken ct) =>
        directus.Items.GetManyAsync<Article>("articles", q => q.NotArchived().Sort("-date_created").Limit(10), ct);
}
```

`AddDirectus` registers `DirectusClient`, `TokenManager`, and an `HttpClient` via `IHttpClientFactory`. For Blazor WebAssembly apps that authenticate via HTTP-only cookies instead of bearer tokens, set `options.UseBrowserCredentials = true` — this makes every request include the Fetch API `credentials: "include"` option.

## Modeling collection items

Directus JSON is snake_case; `Qute.Directus` configures `System.Text.Json` with a snake_case naming policy, so a plain PascalCase C# record maps automatically — **no `[JsonPropertyName]` needed** for ordinary field names:

```csharp
public record Article
{
    public string Id { get; init; } = "";
    public string? Title { get; init; }
    public DateTime? DateCreated { get; init; } // maps to "date_created"
}
```

### `DirectusItem<TKey>` — system fields for free

Inherit from `DirectusItem` (UUID primary key) or `DirectusItem<TKey>` (e.g. `DirectusItem<int>` for auto-increment collections) to pick up `id`, `sort`, `date_created`, `date_updated`, `user_created`, `user_updated` without redeclaring them:

```csharp
using Qute.Directus.Models.Items;

public record Article : DirectusItem
{
    public string? Title { get; init; }
    public string? Content { get; init; }
}
```

### `ArchivableDirectusItem<TKey>` — the Directus 12+ `archived` convention

> **Directus 12 note:** new collections default to a boolean `archived` field instead of the
> old string `status` field. Existing collections created before Directus 12 keep working via
> the string `status` field — model those with `DirectusItem<TKey>` plus your own `Status`
> property, and filter with `WithStatus(...)` instead of `NotArchived()`/`OnlyArchived()`.

For collections that use the new convention, inherit from `ArchivableDirectusItem`/`ArchivableDirectusItem<TKey>` instead — it's `DirectusItem<TKey>` plus a `bool? Archived` property, ready to pair with `NotArchived()`/`OnlyArchived()`:

```csharp
using Qute.Directus.Models.Items;

public record Article : ArchivableDirectusItem
{
    public string? Title { get; init; }
}

var live = await client.Items.GetManyAsync<Article>("articles", q => q.NotArchived());
```

### `[DirectusCollection]` — binding a model to its collection name

Adding `[DirectusCollection("name")]` to a model lets you use the [typed collection client](#typed-collection-client-itemsoft) instead of repeating the collection name string at every call site:

```csharp
using Qute.Directus.Models.Items;

[DirectusCollection("articles")]
public record Article : ArchivableDirectusItem
{
    public string? Title { get; init; }
    public string? Content { get; init; }
}
```

## Querying items — the fluent builder

`QueryParameters` builds Directus's `fields`/`filter`/`sort`/`limit`/`offset`/`search`/`meta`/`deep` query string. Every `Filter`/`Where*` call composes with the others via logical AND — you never need to hand-build an `_and` wrapper yourself.

### Selecting fields

```csharp
q.Fields("id", "title", "content");       // specific fields
q.Fields("*", "author.name");             // wildcard + a related field
```

### Filtering — one named method per Directus operator

```csharp
q.Where("status", "published");                    // _eq
q.WhereNot("status", "draft");                      // _neq
q.WhereGreaterThan("views", 100);                   // _gt
q.WhereGreaterThanOrEqual("price", 9.99);           // _gte
q.WhereLessThan("stock", 5);                        // _lt
q.WhereLessThanOrEqual("age", 65);                  // _lte
q.WhereIn("category", "news", "tech", "sports");    // _in
q.WhereNotIn("status", "draft", "archived");        // _nin
q.WhereContains("title", "directus");               // _contains (case-sensitive)
q.WhereIContains("title", "DIRECTUS");              // _icontains (case-insensitive)
q.WhereStartsWith("slug", "how-to-");                // _starts_with
q.WhereEndsWith("email", "@example.com");           // _ends_with
q.WhereNull("deleted_at");                          // _null
q.WhereNotNull("published_at");                     // _nnull
q.WhereEmpty("tags");                                // _empty
q.WhereNotEmpty("tags");                             // _nempty
q.WhereBetween("date_created", start, end);         // _between

// Escape hatch for any operator not covered above (e.g. a new Directus release adds one):
q.WhereOperator("geo_location", "_intersects", someGeoJson);
```

All of these — and `Filter(object)` for a fully custom raw filter fragment — combine with AND:

```csharp
var results = await client.Items.GetManyAsync<Article>("articles", q => q
    .NotArchived()
    .WhereGreaterThanOrEqual("date_created", DateTime.UtcNow.AddDays(-30))
    .WhereIn("category", "news", "tech")
    .WhereIContains("title", "directus"));
```

### `Or` — grouping conditions with OR

```csharp
var q = new QueryParameters()
    .Where("category", "news")
    .Or(o => o.Where("featured", true).WhereGreaterThan("views", 1000));
// => articles where category == "news" AND (featured == true OR views > 1000)
```

`Or` accepts a lambda so you can nest as deep as you need — the conditions built inside it are combined with OR among themselves, and the resulting group is ANDed with the rest of the filter.

### `NotArchived` / `OnlyArchived` / `WithStatus` — publish-state shortcuts

```csharp
q.NotArchived();              // archived == false  (Directus 12+ collections)
q.OnlyArchived();              // archived == true
q.NotArchived("is_archived");  // custom field name, if you renamed it

q.WithStatus("published");     // status == "published" (pre-Directus-12 collections)
```

### Sorting, pagination, search

```csharp
q.Sort("-date_created", "title");    // descending date, then ascending title
q.Limit(25).Offset(50);              // page 3 of 25
q.Search("directus headless cms");    // full-text search across searchable fields
```

### Metadata, relational depth, versions

```csharp
q.Meta("total_count", "filter_count"); // ask Directus to include counts in the response
q.Deep(new { author = new { _limit = 3 } }); // limit/filter/sort relational fields
q.Version("draft-2024");                     // read a specific Content Version
q.NoRelationalWildcard();                    // exclude reverse relations from "*"
```

### Escape hatches: `Custom` and `Raw`

```csharp
q.Custom("export", "csv"); // any query parameter not covered by a dedicated method

// Or bypass the builder entirely and hand-write the query string:
var q2 = QueryParameters.FromRaw("fields=id,title&filter[status][_eq]=published");
```

## Typed collection client (`Items.Of<T>()`)

Once a model has `[DirectusCollection("...")]`, `client.Items.Of<T>()` returns a client scoped to that collection — no more repeating the collection name (and no risk of a typo drifting out of sync with the model):

```csharp
[DirectusCollection("articles")]
public record Article : ArchivableDirectusItem
{
    public string? Title { get; init; }
    public string? Content { get; init; }
}

var articles = client.Items.Of<Article>();

var recent = await articles.GetManyAsync(q => q.NotArchived().Sort("-date_created").Limit(10));
var one = await articles.GetByIdAsync("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
var created = await articles.CreateAsync(new { title = "Hello", content = "World" });
await articles.UpdateAsync(created.Id, new { title = "Hello, updated" });
await articles.DeleteAsync(created.Id);
```

`DirectusCollectionClient<T>` mirrors `ItemsService` (`GetManyAsync`, `GetByIdAsync`, `GetSingletonAsync`, `CreateAsync`, `CreateManyAsync`, `UpdateAsync`, `UpdateManyAsync`, `UpdateSingletonAsync`, `DeleteAsync`, `DeleteManyAsync`) — everything just already knows which collection it talks to. If `T` has no `[DirectusCollection]` attribute, `Of<T>()` throws `InvalidOperationException` with a message telling you to add one or fall back to the string-based methods below.

The original string-based API on `client.Items` still works unchanged, and is what you reach for when there's no fixed model — e.g. dynamic/admin tooling, or a collection name only known at runtime:

```csharp
await client.Items.GetManyAsync<Article>("articles", q => q.NotArchived());
```

## CRUD examples

```csharp
// Create
var article = await client.Items.CreateAsync<Article>("articles", new
{
    title = "Hello, Directus",
    content = "First post!",
    archived = false
});

// Create many
var created = await client.Items.CreateManyAsync<Article>("articles",
[
    new { title = "Post 1" },
    new { title = "Post 2" },
]);

// Read one
var byId = await client.Items.GetByIdAsync<Article>("articles", article.Id);

// Read many, with the fluent builder
var page = await client.Items.GetManyAsync<Article>("articles", q => q
    .NotArchived()
    .Sort("-date_created")
    .Limit(20)
    .Offset(0));

// Update one
var updated = await client.Items.UpdateAsync<Article>("articles", article.Id, new { title = "Updated title" });

// Update many (bulk, matching a query)
await client.Items.UpdateManyAsync<Article>("articles", new { archived = true },
    QueryParameters.Empty.WhereLessThan("date_created", DateTime.UtcNow.AddYears(-1)));

// Delete one
await client.Items.DeleteAsync("articles", article.Id);

// Delete many by ID
await client.Items.DeleteManyAsync("articles", new[] { "id1", "id2", "id3" });

// Singleton collections (e.g. a "site_settings" collection with one row)
var settings = await client.Items.GetSingletonAsync<SiteSettings>("site_settings");
await client.Items.UpdateSingletonAsync<SiteSettings>("site_settings", new { site_name = "My Site" });
```

## Working with dynamic/untyped data

When you don't want (or can't yet write) a typed model — prototyping, admin tooling, a collection whose shape you don't control — use the untyped overloads, which return `System.Text.Json.JsonElement`:

```csharp
var raw = await client.Items.GetManyAsync("articles", q => q.NotArchived().Limit(5));
foreach (var item in raw.Data)
{
    var title = item.GetProperty("title").GetString();
}

var one = await client.Items.GetByIdAsync("articles", "3f2504e0-4f89-11d3-9a0c-0305e82c3301");
```

## Iterating responses and pagination metadata

`DirectusListResponse<T>` implements `IReadOnlyList<T>`, so you can enumerate/index it directly — `.Data` is still there if you prefer it explicitly:

```csharp
var response = await client.Items.GetManyAsync<Article>("articles", q => q.Meta("total_count", "filter_count"));

foreach (var article in response) { /* ... */ }   // same as foreach (var a in response.Data)
var first = response[0];                           // same as response.Data[0]
Console.WriteLine($"{response.Count} of {response.Meta?.FilterCount} matched ({response.Meta?.TotalCount} total)");
```

`Meta` is only populated when you ask for it via `.Meta(...)` in the query.

## Error handling

Every non-2xx response is surfaced as a `DirectusException`, populated from Directus's structured `{ "errors": [...] }` response body when available:

```csharp
try
{
    await client.Items.CreateAsync<Article>("articles", new { title = "Duplicate slug" });
}
catch (DirectusException ex)
{
    Console.WriteLine($"{(int)ex.StatusCode}: {ex.Message}");
    foreach (var error in ex.Errors)
        Console.WriteLine($"  {error.Code}: {error.Message}");
}
```

## Authentication

### Static token

Simplest option — set `DirectusOptions.StaticToken` and every request uses it as a Bearer token. No refresh logic runs at all in this mode.

```csharp
var client = new DirectusClient(new DirectusOptions
{
    BaseUrl = "https://my-directus.com",
    StaticToken = "my-static-token"
});
```

### Login / refresh flow

```csharp
var client = new DirectusClient(new DirectusOptions { BaseUrl = "https://my-directus.com" });

var session = await client.Auth.LoginAsync("admin@example.com", "password");
// session.AccessToken / session.RefreshToken / session.Expires are also available directly,
// but you don't need them — the client stores and refreshes them for you automatically.

await client.Auth.LogoutAsync();
```

By default (`DirectusOptions.AutoRefreshToken = true`), the access token is refreshed automatically a configurable number of seconds before it expires (`TokenRefreshBufferSeconds`, default 30):

```csharp
var client = new DirectusClient(new DirectusOptions
{
    BaseUrl = "https://my-directus.com",
    AutoRefreshToken = true,
    TokenRefreshBufferSeconds = 60, // start refreshing a minute early
});
```

Set `AutoRefreshToken = false` to disable automatic refresh entirely — requests will keep using the current access token even past expiry, and you handle the resulting `401` (via `DirectusException`) yourself, e.g. to redirect to a login page instead of silently retrying.

If the access token is expired, `AutoRefreshToken` is `true`, but no login has happened yet (no refresh token available), `DirectusException` is thrown with a clear message instead of silently sending an expired token.

### Blazor WebAssembly cookie auth

```csharp
services.AddDirectus(options =>
{
    options.BaseUrl = "https://my-directus.com";
    options.UseBrowserCredentials = true; // include cookies on cross-origin requests
});
```

## Available services

| Service | Description |
|---|---|
| `Auth` | Login, logout, refresh, OAuth, password reset |
| `Items` | Generic CRUD on collection items (`Items.Of<T>()` for typed, collection-bound access) |
| `Users` | User management, invite, register, TFA |
| `Files` | Upload, import, CRUD |
| `Assets` | File retrieval with image transformations |
| `Collections` | Collection management |
| `Fields` | Field management |
| `Roles` | Role management |
| `Permissions` | Permission management |
| `Policies` | Access policy management |
| `Activity` | Activity log (read-only) |
| `Folders` | Virtual folder management |
| `Relations` | Relation management |
| `Revisions` | Revision log (read-only) |
| `Presets` | Saved view presets |
| `Settings` | Global settings |
| `Server` | Server info, health, ping |
| `Flows` | Automation flows |
| `Operations` | Flow operations/steps |
| `Dashboards` | Dashboard management |
| `Panels` | Dashboard panel management |
| `Notifications` | Notification management |
| `Shares` | Share link management |
| `Translations` | Custom translations |
| `Comments` | Item comments |
| `Extensions` | Extension management |
| `Versions` | Content versioning |
| `Schema` | Schema snapshot, diff, apply |
| `Utilities` | Hash, sort, random string, export |
| `Metrics` | Prometheus metrics |

## Versioning and breaking changes

### v11.0.1

Bug fix, no breaking changes:

- **`DirectusListResponse<T>` deserialization was broken.** Because the type implements `IReadOnlyList<T>` (for the ergonomic enumeration/indexing described above), `System.Text.Json`'s default reflection-based resolution treated it as a JSON array and ignored its `data`/`meta` properties entirely — every list-returning call (`Items.GetManyAsync`, `Files.GetManyAsync`, etc.) threw `JsonException: The JSON value could not be converted to ... DirectusListResponse\`1[...]` on any real response. A dedicated `JsonConverter` (`Serialization/DirectusListResponseConverter.cs`) now handles `DirectusListResponse<T>` explicitly, restoring correct `{ "data": [...], "meta": {...} }` (de)serialization.

### v11.0.0

Breaking behavior changes — see [`.docs/specs-001.md`](../.docs/specs-001.md) for the full rationale:

- **`AutoRefreshToken` is now honored.** Previously this option had no effect; setting it to `false` now actually disables automatic token refresh.
- **Expired token with no refresh available now throws.** Previously an expired access token was silently reused when no login/refresh token existed; it now throws `DirectusException` (401) immediately, before the request is even sent.
- **`GetManyAsync`/list endpoints now throw on an unexpectedly null response body**, instead of silently returning an empty list, matching the behavior already used by the single-item read methods.
- **`PostRawAsync` was renamed to `PostUnauthenticatedAsync`** to avoid confusion with `PostAuthenticatedRawAsync` (only relevant if you were calling it directly rather than through `client.Auth`).

### v10.x and earlier

Additive only — named filter operators (`WhereGreaterThan`, `WhereIn`, `WhereContains`, etc.), `Or(...)` grouping, `[DirectusCollection]` + `Items.Of<T>()`, `DirectusListResponse<T>` implementing `IReadOnlyList<T>`, and `ArchivableDirectusItem<TKey>` were all introduced without breaking existing code.

## License

MIT
