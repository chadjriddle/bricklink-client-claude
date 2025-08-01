using BrickLink.Client.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BrickLink.Client.Tests.Auth;

/// <summary>
/// Unit tests for the <see cref="ServiceCollectionExtensions"/> class.
/// </summary>
public class ServiceCollectionExtensionsTests
{
    private const string ValidConsumerKey = "test-consumer-key";
    private const string ValidConsumerSecret = "test-consumer-secret";
    private const string ValidAccessToken = "test-access-token";
    private const string ValidAccessTokenSecret = "test-access-token-secret";

    private readonly BrickLinkCredentials _validCredentials;

    public ServiceCollectionExtensionsTests()
    {
        _validCredentials = new BrickLinkCredentials(
            ValidConsumerKey,
            ValidConsumerSecret,
            ValidAccessToken,
            ValidAccessTokenSecret);
    }

    #region AddBrickLinkAuthentication Tests

    [Fact]
    public void AddBrickLinkAuthentication_WithCredentials_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBrickLinkAuthentication(_validCredentials);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var credentials = serviceProvider.GetService<BrickLinkCredentials>();
        var authHandler = serviceProvider.GetService<IAuthenticationHandler>();
        var authHandlerConcrete = serviceProvider.GetService<AuthenticationHandler>();

        Assert.NotNull(credentials);
        Assert.Equal(_validCredentials.ConsumerKey, credentials.ConsumerKey);
        Assert.NotNull(authHandler);
        Assert.NotNull(authHandlerConcrete);
    }

    [Fact]
    public void AddBrickLinkAuthentication_WithNullServices_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddBrickLinkAuthentication(null!, _validCredentials));
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddBrickLinkAuthentication_WithNullCredentials_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddBrickLinkAuthentication((BrickLinkCredentials)null!));
        Assert.Equal("credentials", exception.ParamName);
    }

    [Fact]
    public void AddBrickLinkAuthentication_WithCredentialsFactory_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        Func<IServiceProvider, BrickLinkCredentials> factory = _ => _validCredentials;

        // Act
        services.AddBrickLinkAuthentication(factory);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var authHandler = serviceProvider.GetService<IAuthenticationHandler>();
        var authHandlerConcrete = serviceProvider.GetService<AuthenticationHandler>();

        Assert.NotNull(authHandler);
        Assert.NotNull(authHandlerConcrete);
    }

    [Fact]
    public void AddBrickLinkAuthentication_WithNullCredentialsFactory_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddBrickLinkAuthentication((Func<IServiceProvider, BrickLinkCredentials>)null!));
        Assert.Equal("credentialsFactory", exception.ParamName);
    }

    #endregion

    #region AddBrickLinkHttpClient Tests

    [Fact]
    public void AddBrickLinkHttpClient_WithCredentials_RegistersHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBrickLinkHttpClient(_validCredentials);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

        Assert.NotNull(httpClientFactory);

        var httpClient = httpClientFactory.CreateClient("BrickLinkApi");
        Assert.NotNull(httpClient);
        Assert.NotNull(httpClient.BaseAddress);
    }

    [Fact]
    public void AddBrickLinkHttpClient_WithCustomBaseUrl_ConfiguresCorrectBaseUrl()
    {
        // Arrange
        var services = new ServiceCollection();
        const string customBaseUrl = "https://custom.api.example.com/api/v1/";

        // Act
        services.AddBrickLinkHttpClient(_validCredentials, customBaseUrl);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        var httpClient = httpClientFactory?.CreateClient("BrickLinkApi");

        Assert.NotNull(httpClient);
        Assert.Equal(new Uri(customBaseUrl), httpClient.BaseAddress);
    }

    [Fact]
    public void AddBrickLinkHttpClient_WithNullServices_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddBrickLinkHttpClient(null!, _validCredentials));
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddBrickLinkHttpClient_WithNullCredentials_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddBrickLinkHttpClient((BrickLinkCredentials)null!));
        Assert.Equal("credentials", exception.ParamName);
    }

    [Fact]
    public void AddBrickLinkHttpClient_WithCredentialsFactory_RegistersHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();
        Func<IServiceProvider, BrickLinkCredentials> factory = _ => _validCredentials;

        // Act
        services.AddBrickLinkHttpClient(factory);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

        Assert.NotNull(httpClientFactory);

        var httpClient = httpClientFactory.CreateClient("BrickLinkApi");
        Assert.NotNull(httpClient);
        Assert.NotNull(httpClient.BaseAddress);
    }

    #endregion

    #region AddBrickLinkHttpClientWithHandlers Tests

    [Fact]
    public void AddBrickLinkHttpClientWithHandlers_WithCredentials_RegistersHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBrickLinkHttpClientWithHandlers(_validCredentials);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

        Assert.NotNull(httpClientFactory);

        var httpClient = httpClientFactory.CreateClient("BrickLinkApi");
        Assert.NotNull(httpClient);
        Assert.NotNull(httpClient.BaseAddress);
    }

    [Fact]
    public void AddBrickLinkHttpClientWithHandlers_WithNullServices_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddBrickLinkHttpClientWithHandlers(null!, _validCredentials));
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddBrickLinkHttpClientWithHandlers_WithNullCredentials_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddBrickLinkHttpClientWithHandlers(null!));
        Assert.Equal("credentials", exception.ParamName);
    }

    #endregion

    #region Configuration-based Registration Tests

    [Fact]
    public void AddBrickLinkAuthenticationFromConfiguration_WithValidConfiguration_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateTestConfiguration();

        // Act
        services.AddBrickLinkAuthenticationFromConfiguration(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var credentials = serviceProvider.GetService<BrickLinkCredentials>();
        var authHandler = serviceProvider.GetService<IAuthenticationHandler>();
        var options = serviceProvider.GetService<IOptions<BrickLinkAuthenticationOptions>>();

        Assert.NotNull(credentials);
        Assert.NotNull(authHandler);
        Assert.NotNull(options);
        Assert.Equal(ValidConsumerKey, credentials.ConsumerKey);
    }

    [Fact]
    public void AddBrickLinkAuthenticationFromConfiguration_WithNullServices_ThrowsArgumentNullException()
    {
        // Arrange
        var configuration = CreateTestConfiguration();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddBrickLinkAuthenticationFromConfiguration(null!, configuration));
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddBrickLinkAuthenticationFromConfiguration_WithNullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddBrickLinkAuthenticationFromConfiguration(null!));
        Assert.Equal("configuration", exception.ParamName);
    }

    [Fact]
    public void AddBrickLinkHttpClientFromConfiguration_WithValidConfiguration_RegistersHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateTestConfiguration();

        // Act
        services.AddBrickLinkHttpClientFromConfiguration(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

        Assert.NotNull(httpClientFactory);

        var httpClient = httpClientFactory.CreateClient("BrickLinkApi");
        Assert.NotNull(httpClient);
        Assert.NotNull(httpClient.BaseAddress);
    }

    [Fact]
    public void AddBrickLinkHttpClientFromConfiguration_WithCustomSectionName_UsesCustomSection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateTestConfiguration("CustomAuth");

        // Act
        services.AddBrickLinkHttpClientFromConfiguration(configuration, "CustomAuth");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

        Assert.NotNull(httpClientFactory);

        var httpClient = httpClientFactory.CreateClient("BrickLinkApi");
        Assert.NotNull(httpClient);
    }

    #endregion

    #region Service Lifetime Tests

    [Fact]
    public void AddBrickLinkAuthentication_RegistersCredentialsAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBrickLinkAuthentication(_validCredentials);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var credentials1 = serviceProvider.GetService<BrickLinkCredentials>();
        var credentials2 = serviceProvider.GetService<BrickLinkCredentials>();

        Assert.NotNull(credentials1);
        Assert.NotNull(credentials2);
        Assert.Same(credentials1, credentials2); // Should be the same instance (singleton)
    }

    [Fact]
    public void AddBrickLinkAuthentication_RegistersAuthHandlerAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBrickLinkAuthentication(_validCredentials);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var handler1 = serviceProvider.GetService<AuthenticationHandler>();
        var handler2 = serviceProvider.GetService<AuthenticationHandler>();

        Assert.NotNull(handler1);
        Assert.NotNull(handler2);
        Assert.NotSame(handler1, handler2); // Should be different instances (transient)
    }

    #endregion

    #region Helper Methods

    private static IConfiguration CreateTestConfiguration(string sectionName = BrickLinkAuthenticationOptions.SectionName)
    {
        var configurationData = new Dictionary<string, string?>
        {
            [$"{sectionName}:ConsumerKey"] = ValidConsumerKey,
            [$"{sectionName}:ConsumerSecret"] = ValidConsumerSecret,
            [$"{sectionName}:AccessToken"] = ValidAccessToken,
            [$"{sectionName}:AccessTokenSecret"] = ValidAccessTokenSecret,
            [$"{sectionName}:BaseUrl"] = "https://api.bricklink.com/api/store/v1/"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();
    }

    #endregion
}
