# Qute.Directus

A comprehensive C# client library for the [Directus](https://directus.io) REST API.

## Installation

```bash
dotnet add package Qute.Directus
```

## Quick Start

### Direct Usage

```csharp
using Qute.Directus;

var client = new DirectusClient(new DirectusOptions
{
    BaseUrl = "https://my-directus.com",
    StaticToken = "my-static-token"
});

// Or authenticate with email/password
await client.Auth.LoginAsync("admin@example.com", "password");

// Query items
var articles = await client.Items.GetManyAsync<Article>("articles", q => q
    .Fields("id", "title", "content")
    .Filter(new { status = new { _eq = "published" } })
    .Sort("-date_created")
    .Limit(25));
```

### With Dependency Injection

```csharp
// In Program.cs or Startup.cs
services.AddDirectus(options =>
{
    options.BaseUrl = "https://my-directus.com";
    options.StaticToken = "my-static-token";
});

// In your service
public class MyService(DirectusClient directus)
{
    public async Task DoWorkAsync()
    {
        var users = await directus.Users.GetManyAsync();
    }
}
```

## Available Services

| Service | Description |
|---|---|
| `Auth` | Login, logout, refresh, OAuth, password reset |
| `Items` | Generic CRUD on collection items |
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

## License

MIT
