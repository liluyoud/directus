using DotNetEnv;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Qute.Directus.Extensions;
using Qute.View;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

Env.Load();
var baseUrl = Environment.GetEnvironmentVariable("DIRECTUS_URL")
    ?? builder.Configuration.GetValue<string>("Directus:BaseUrl") 
    ?? builder.HostEnvironment.BaseAddress;

// ─── Directus Client ──────────────────────────────────────────────────
// Uses browser credentials (cookies) — no login required.
builder.Services.AddDirectus(options =>
{
    options.BaseUrl = baseUrl ?? builder.HostEnvironment.BaseAddress;
    options.UseBrowserCredentials = true;
});

await builder.Build().RunAsync();
