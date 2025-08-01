using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace BrickLink.Client.Auth;

/// <summary>
/// Extension methods for IServiceCollection to configure BrickLink authentication services.
/// Provides convenient methods for dependency injection of authentication components.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds BrickLink authentication services to the dependency injection container.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="credentials">The BrickLink OAuth credentials to use for authentication.</param>
    /// <returns>The IServiceCollection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or credentials is null.</exception>
    public static IServiceCollection AddBrickLinkAuthentication(this IServiceCollection services, BrickLinkCredentials credentials)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (credentials == null)
            throw new ArgumentNullException(nameof(credentials));

        // Register the credentials as a singleton
        services.TryAddSingleton(credentials);

        // Register the authentication handler as a transient service
        // Transient because each HttpClient should have its own handler instance
        services.TryAddTransient<IAuthenticationHandler, AuthenticationHandler>();
        services.TryAddTransient<AuthenticationHandler>();

        return services;
    }

    /// <summary>
    /// Adds BrickLink authentication services with a credential factory.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="credentialsFactory">A factory function to create credentials.</param>
    /// <returns>The IServiceCollection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or credentialsFactory is null.</exception>
    public static IServiceCollection AddBrickLinkAuthentication(this IServiceCollection services, Func<IServiceProvider, BrickLinkCredentials> credentialsFactory)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (credentialsFactory == null)
            throw new ArgumentNullException(nameof(credentialsFactory));

        // Register the credentials factory as a singleton
        services.TryAddSingleton(credentialsFactory);

        // Register the authentication handler as a transient service
        services.TryAddTransient<IAuthenticationHandler>(provider =>
        {
            var credentials = credentialsFactory(provider);
            return new AuthenticationHandler(credentials);
        });
        services.TryAddTransient<AuthenticationHandler>(provider =>
        {
            var credentials = credentialsFactory(provider);
            return new AuthenticationHandler(credentials);
        });

        return services;
    }

    /// <summary>
    /// Adds a configured HttpClient with BrickLink authentication to the dependency injection container.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="credentials">The BrickLink OAuth credentials to use for authentication.</param>
    /// <param name="baseUrl">Optional custom base URL. If null, uses the default BrickLink API URL.</param>
    /// <returns>The IServiceCollection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or credentials is null.</exception>
    public static IServiceCollection AddBrickLinkHttpClient(this IServiceCollection services, BrickLinkCredentials credentials, string? baseUrl = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (credentials == null)
            throw new ArgumentNullException(nameof(credentials));

        // Add authentication services first
        services.AddBrickLinkAuthentication(credentials);

        // Configure HttpClient with authentication
        services.AddHttpClient("BrickLinkApi", client =>
        {
            var apiBaseUrl = string.IsNullOrWhiteSpace(baseUrl)
                ? Http.BrickLinkHttpClient.DefaultBaseUrl
                : baseUrl;

            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "BrickLink-Client/1.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<AuthenticationHandler>();

        return services;
    }

    /// <summary>
    /// Adds a configured HttpClient with BrickLink authentication using a credential factory.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="credentialsFactory">A factory function to create credentials.</param>
    /// <param name="baseUrl">Optional custom base URL. If null, uses the default BrickLink API URL.</param>
    /// <returns>The IServiceCollection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or credentialsFactory is null.</exception>
    public static IServiceCollection AddBrickLinkHttpClient(this IServiceCollection services, Func<IServiceProvider, BrickLinkCredentials> credentialsFactory, string? baseUrl = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (credentialsFactory == null)
            throw new ArgumentNullException(nameof(credentialsFactory));

        // Add authentication services first
        services.AddBrickLinkAuthentication(credentialsFactory);

        // Configure HttpClient with authentication
        services.AddHttpClient("BrickLinkApi", client =>
        {
            var apiBaseUrl = string.IsNullOrWhiteSpace(baseUrl)
                ? Http.BrickLinkHttpClient.DefaultBaseUrl
                : baseUrl;

            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "BrickLink-Client/1.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<AuthenticationHandler>();

        return services;
    }

    /// <summary>
    /// Adds a configured HttpClient with BrickLink authentication and additional delegating handlers.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="credentials">The BrickLink OAuth credentials to use for authentication.</param>
    /// <param name="configureHandlers">An action to configure additional message handlers.</param>
    /// <param name="baseUrl">Optional custom base URL. If null, uses the default BrickLink API URL.</param>
    /// <returns>The IServiceCollection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or credentials is null.</exception>
    public static IServiceCollection AddBrickLinkHttpClientWithHandlers(
        this IServiceCollection services,
        BrickLinkCredentials credentials,
        Action<IHttpClientBuilder>? configureHandlers = null,
        string? baseUrl = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (credentials == null)
            throw new ArgumentNullException(nameof(credentials));

        // Add authentication services first
        services.AddBrickLinkAuthentication(credentials);

        // Configure HttpClient with authentication
        var builder = services.AddHttpClient("BrickLinkApi", client =>
        {
            var apiBaseUrl = string.IsNullOrWhiteSpace(baseUrl)
                ? Http.BrickLinkHttpClient.DefaultBaseUrl
                : baseUrl;

            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "BrickLink-Client/1.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<AuthenticationHandler>();

        // Allow additional handler configuration
        configureHandlers?.Invoke(builder);

        return services;
    }

    /// <summary>
    /// Adds BrickLink authentication services configured from application settings.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration instance to bind options from.</param>
    /// <param name="sectionName">The configuration section name. Defaults to "BrickLinkAuthentication".</param>
    /// <returns>The IServiceCollection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configuration is null.</exception>
    public static IServiceCollection AddBrickLinkAuthenticationFromConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = BrickLinkAuthenticationOptions.SectionName)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // Configure options from configuration
        services.Configure<BrickLinkAuthenticationOptions>(
            configuration.GetSection(sectionName));

        // Register credentials factory that uses options
        services.TryAddSingleton<BrickLinkCredentials>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<BrickLinkAuthenticationOptions>>().Value;
            return options.ToCredentials();
        });

        // Register authentication handlers
        services.TryAddTransient<IAuthenticationHandler, AuthenticationHandler>();
        services.TryAddTransient<AuthenticationHandler>();

        return services;
    }

    /// <summary>
    /// Adds a configured HttpClient with BrickLink authentication from application settings.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration instance to bind options from.</param>
    /// <param name="sectionName">The configuration section name. Defaults to "BrickLinkAuthentication".</param>
    /// <returns>The IServiceCollection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configuration is null.</exception>
    public static IServiceCollection AddBrickLinkHttpClientFromConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = BrickLinkAuthenticationOptions.SectionName)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // Add authentication services from configuration
        services.AddBrickLinkAuthenticationFromConfiguration(configuration, sectionName);

        // Configure HttpClient with authentication
        services.AddHttpClient("BrickLinkApi", (provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<BrickLinkAuthenticationOptions>>().Value;
            var apiBaseUrl = string.IsNullOrWhiteSpace(options.BaseUrl)
                ? Http.BrickLinkHttpClient.DefaultBaseUrl
                : options.BaseUrl;

            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "BrickLink-Client/1.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<AuthenticationHandler>();

        return services;
    }
}
