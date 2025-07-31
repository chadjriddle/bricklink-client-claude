using System.Net;
using BrickLink.Client.Http;
using BrickLink.Client.Retry;
using Moq;

namespace BrickLink.Client.Tests.Http;

public sealed class BrickLinkHttpClientExtensionsTests
{
    [Fact]
    public async Task GetWithRetryAsync_WithNullClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            BrickLinkHttpClientExtensions.GetWithRetryAsync(null!, "test"));
    }

    [Fact]
    public async Task GetWithRetryAsync_WithStringUri_CallsRetryHandler()
    {
        // Arrange
        using var httpClient = new HttpClient();
        using var client = new BrickLinkHttpClient(httpClient);
        var mockRetryHandler = new Mock<RetryHandler>();
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);

        mockRetryHandler.Setup(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await client.GetWithRetryAsync("test", mockRetryHandler.Object);

        // Assert
        Assert.Same(expectedResponse, result);
        mockRetryHandler.Verify(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetWithRetryAsync_WithUriObject_CallsRetryHandler()
    {
        // Arrange
        using var httpClient = new HttpClient();
        using var client = new BrickLinkHttpClient(httpClient);
        var uri = new Uri("https://example.com/test");
        var mockRetryHandler = new Mock<RetryHandler>();
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);

        mockRetryHandler.Setup(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await client.GetWithRetryAsync(uri, mockRetryHandler.Object);

        // Assert
        Assert.Same(expectedResponse, result);
        mockRetryHandler.Verify(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetWithRetryAsync_WithNullRetryHandler_UsesDefaultHandler()
    {
        // Arrange
        using var handler = new HttpClientHandler();
        using var httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://httpbin.org/");
        using var client = new BrickLinkHttpClient(httpClient);

        // Act & Assert (should not throw - using real HTTP endpoint)
        var result = await client.GetWithRetryAsync("status/200", null);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        result.Dispose();
    }

    [Fact]
    public async Task PostWithRetryAsync_WithStringUri_CallsRetryHandler()
    {
        // Arrange
        using var httpClient = new HttpClient();
        using var client = new BrickLinkHttpClient(httpClient);
        var content = new StringContent("test");
        var mockRetryHandler = new Mock<RetryHandler>();
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);

        mockRetryHandler.Setup(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await client.PostWithRetryAsync("test", content, mockRetryHandler.Object);

        // Assert
        Assert.Same(expectedResponse, result);
        mockRetryHandler.Verify(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task PostWithRetryAsync_WithUriObject_CallsRetryHandler()
    {
        // Arrange
        using var httpClient = new HttpClient();
        using var client = new BrickLinkHttpClient(httpClient);
        var uri = new Uri("https://example.com/test");
        var content = new StringContent("test");
        var mockRetryHandler = new Mock<RetryHandler>();
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);

        mockRetryHandler.Setup(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await client.PostWithRetryAsync(uri, content, mockRetryHandler.Object);

        // Assert
        Assert.Same(expectedResponse, result);
        mockRetryHandler.Verify(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task PutWithRetryAsync_WithStringUri_CallsRetryHandler()
    {
        // Arrange
        using var httpClient = new HttpClient();
        using var client = new BrickLinkHttpClient(httpClient);
        var content = new StringContent("test");
        var mockRetryHandler = new Mock<RetryHandler>();
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);

        mockRetryHandler.Setup(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await client.PutWithRetryAsync("test", content, mockRetryHandler.Object);

        // Assert
        Assert.Same(expectedResponse, result);
        mockRetryHandler.Verify(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task PutWithRetryAsync_WithUriObject_CallsRetryHandler()
    {
        // Arrange
        using var httpClient = new HttpClient();
        using var client = new BrickLinkHttpClient(httpClient);
        var uri = new Uri("https://example.com/test");
        var content = new StringContent("test");
        var mockRetryHandler = new Mock<RetryHandler>();
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);

        mockRetryHandler.Setup(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await client.PutWithRetryAsync(uri, content, mockRetryHandler.Object);

        // Assert
        Assert.Same(expectedResponse, result);
        mockRetryHandler.Verify(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteWithRetryAsync_WithStringUri_CallsRetryHandler()
    {
        // Arrange
        using var httpClient = new HttpClient();
        using var client = new BrickLinkHttpClient(httpClient);
        var mockRetryHandler = new Mock<RetryHandler>();
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);

        mockRetryHandler.Setup(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await client.DeleteWithRetryAsync("test", mockRetryHandler.Object);

        // Assert
        Assert.Same(expectedResponse, result);
        mockRetryHandler.Verify(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteWithRetryAsync_WithUriObject_CallsRetryHandler()
    {
        // Arrange
        using var httpClient = new HttpClient();
        using var client = new BrickLinkHttpClient(httpClient);
        var uri = new Uri("https://example.com/test");
        var mockRetryHandler = new Mock<RetryHandler>();
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);

        mockRetryHandler.Setup(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await client.DeleteWithRetryAsync(uri, mockRetryHandler.Object);

        // Assert
        Assert.Same(expectedResponse, result);
        mockRetryHandler.Verify(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetWithRetryAsync_WithCancellationToken_PassesCancellationToken()
    {
        // Arrange
        using var httpClient = new HttpClient();
        using var client = new BrickLinkHttpClient(httpClient);
        var mockRetryHandler = new Mock<RetryHandler>();
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
        using var cts = new CancellationTokenSource();

        mockRetryHandler.Setup(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), cts.Token))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await client.GetWithRetryAsync("test", mockRetryHandler.Object, cts.Token);

        // Assert
        Assert.Same(expectedResponse, result);
        mockRetryHandler.Verify(h => h.ExecuteHttpAsync(It.IsAny<Func<Task<HttpResponseMessage>>>(), cts.Token), Times.Once);
    }

    [Fact]
    public async Task PostWithRetryAsync_WithNullClient_ThrowsArgumentNullException()
    {
        // Arrange
        var content = new StringContent("test");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            BrickLinkHttpClientExtensions.PostWithRetryAsync(null!, "test", content));
    }

    [Fact]
    public async Task PutWithRetryAsync_WithNullClient_ThrowsArgumentNullException()
    {
        // Arrange
        var content = new StringContent("test");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            BrickLinkHttpClientExtensions.PutWithRetryAsync(null!, "test", content));
    }

    [Fact]
    public async Task DeleteWithRetryAsync_WithNullClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            BrickLinkHttpClientExtensions.DeleteWithRetryAsync(null!, "test"));
    }

    [Fact]
    public async Task GetWithRetryAsync_WithStringUri_WithoutRetryHandler_UsesDefaultHandler()
    {
        // Arrange
        using var handler = new HttpClientHandler();
        using var httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://httpbin.org/");
        using var client = new BrickLinkHttpClient(httpClient);

        // Act & Assert (should not throw - using real HTTP endpoint)
        var result = await client.GetWithRetryAsync("status/200");
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        result.Dispose();
    }

    [Fact]
    public async Task PostWithRetryAsync_WithStringUri_WithoutRetryHandler_UsesDefaultHandler()
    {
        // Arrange
        using var handler = new HttpClientHandler();
        using var httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://httpbin.org/");
        using var client = new BrickLinkHttpClient(httpClient);
        var content = new StringContent("test");

        // Act & Assert (should not throw - using real HTTP endpoint)
        var result = await client.PostWithRetryAsync("post", content);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        result.Dispose();
    }

    [Fact]
    public async Task PutWithRetryAsync_WithStringUri_WithoutRetryHandler_UsesDefaultHandler()
    {
        // Arrange
        using var handler = new HttpClientHandler();
        using var httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://httpbin.org/");
        using var client = new BrickLinkHttpClient(httpClient);
        var content = new StringContent("test");

        // Act & Assert (should not throw - using real HTTP endpoint)
        var result = await client.PutWithRetryAsync("put", content);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        result.Dispose();
    }

    [Fact]
    public async Task DeleteWithRetryAsync_WithStringUri_WithoutRetryHandler_UsesDefaultHandler()
    {
        // Arrange
        using var handler = new HttpClientHandler();
        using var httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://httpbin.org/");
        using var client = new BrickLinkHttpClient(httpClient);

        // Act & Assert (should not throw - using real HTTP endpoint)
        var result = await client.DeleteWithRetryAsync("delete");
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        result.Dispose();
    }
}
