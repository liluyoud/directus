using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Qute.Directus.Http;

namespace Qute.Directus.Extensions;

/// <summary>
/// Extension methods for registering the Directus client in the Microsoft DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="DirectusClient"/> and its dependencies in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="DirectusOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDirectus(options =>
    /// {
    ///     options.BaseUrl = "https://my-directus.com";
    ///     options.StaticToken = "my-static-token";
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddDirectus(this IServiceCollection services, Action<DirectusOptions> configure)
    {
        services.Configure(configure);

        services.AddSingleton<TokenManager>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DirectusOptions>>().Value;
            return new TokenManager(options.TokenRefreshBufferSeconds);
        });

        services.AddHttpClient<DirectusHttpClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<DirectusOptions>>().Value;
            client.BaseAddress = options.GetBaseUri();
        });

        services.AddTransient<DirectusHttpClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DirectusOptions>>().Value;
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = factory.CreateClient(options.HttpClientName);
            httpClient.BaseAddress ??= options.GetBaseUri();
            var tokenManager = sp.GetRequiredService<TokenManager>();
            return new DirectusHttpClient(httpClient, tokenManager, options);
        });

        services.AddTransient<DirectusClient>(sp =>
        {
            var http = sp.GetRequiredService<DirectusHttpClient>();
            var tokenManager = sp.GetRequiredService<TokenManager>();
            return new DirectusClient(http, tokenManager);
        });

        return services;
    }

    /// <summary>
    /// Registers <see cref="DirectusClient"/> using a pre-configured <see cref="DirectusOptions"/> instance.
    /// </summary>
    public static IServiceCollection AddDirectus(this IServiceCollection services, DirectusOptions options)
    {
        return services.AddDirectus(o =>
        {
            o.BaseUrl = options.BaseUrl;
            o.StaticToken = options.StaticToken;
            o.AutoRefreshToken = options.AutoRefreshToken;
            o.TokenRefreshBufferSeconds = options.TokenRefreshBufferSeconds;
            o.HttpClientName = options.HttpClientName;
            o.UseBrowserCredentials = options.UseBrowserCredentials;
        });
    }
}
