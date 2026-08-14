using DotNetEnv;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Qute.Directus;
using Qute.Directus.Extensions;
using Qute.PWA;
using Qute.PWA.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


Env.Load();
var baseUrl = Environment.GetEnvironmentVariable("DIRECTUS_URL")
    ?? builder.Configuration.GetValue<string>("Directus:BaseUrl")
    ?? builder.HostEnvironment.BaseAddress;

// ─── Directus Client ──────────────────────────────────────────────────
builder.Services.AddDirectus(options =>
{
    // Use the same origin as the PWA itself (Directus is reverse-proxied or same-origin).
    // Change this if Directus runs on a different URL.
    options.BaseUrl = baseUrl ?? builder.HostEnvironment.BaseAddress;
    options.UseBrowserCredentials = true;
});

// ─── Authentication ───────────────────────────────────────────────────
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<DirectusAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<DirectusAuthStateProvider>());

await builder.Build().RunAsync();
