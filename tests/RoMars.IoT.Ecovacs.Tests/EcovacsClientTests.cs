using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using RoMars.IoT.Ecovacs.Configuration;
using RoMars.IoT.Ecovacs.Exceptions;
using RoMars.IoT.Ecovacs.Models.Responses;
using RoMars.IoT.Ecovacs.Services;
using System.Net;
using System.Text.Json;
using Xunit;

namespace RoMars.IoT.Ecovacs.Tests;

public class EcovacsClientTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<EcovacsClient>> _loggerMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly EcovacsClientOptions _options;

    public EcovacsClientTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<EcovacsClient>>();
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _options = new EcovacsClientOptions
        {
            BaseUrl = "https://test.ecovacs.com",
            ApiKey = "test-api-key"
        };
    }

    [Fact]
    public void Constructor_InitializesServices()
    {
        // Arrange
        var httpClientMock = new Mock<HttpClient>();
        _httpClientFactoryMock.Setup(x => x.CreateClient("EcovacsClient")).Returns(httpClientMock.Object);

        // Act
        var client = CreateClient();

        // Assert
        Assert.NotNull(client.Devices);
        Assert.NotNull(client.Control);
        Assert.IsType<EcovacsDeviceService>(client.Devices);
        Assert.IsType<EcovacsControlService>(client.Control);
    }

    [Fact]
    public async Task SendPostRequestAsync_SuccessfulRequest_ReturnsDeserializedResponse()
    {
        // Arrange
        var expectedResponse = new ApiResponse<TestData>
        {
            Code = 0,
            Message = "success",
            Data = new TestData { Value = "test" }
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://test.ecovacs.com")
        };
        _httpClientFactoryMock.Setup(x => x.CreateClient("EcovacsClient")).Returns(httpClient);

        var client = CreateClient();
        var parameters = new { test = "value" };

        // Act
        var result = await client.SendPostRequestAsync<TestData>("/test/endpoint", parameters);

        // Assert
        Assert.Equal(expectedResponse.Code, result.Code);
        Assert.Equal(expectedResponse.Message, result.Message);
        Assert.Equal(expectedResponse.Data.Value, result.Data.Value);
    }

    [Fact]
    public async Task SendPostRequestAsync_HttpError_ThrowsEcovacsApiException()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Bad Request")
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://test.ecovacs.com")
        };
        _httpClientFactoryMock.Setup(x => x.CreateClient("EcovacsClient")).Returns(httpClient);

        var client = CreateClient();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EcovacsApiException>(
            () => client.SendPostRequestAsync<TestData>("/test/endpoint", new { }));

        Assert.Contains("Request failed", exception.Message);
    }

    [Fact]
    public async Task SendPostRequestAsync_JsonDeserializationError_ThrowsEcovacsApiException()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("invalid json")
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://test.ecovacs.com")
        };
        _httpClientFactoryMock.Setup(x => x.CreateClient("EcovacsClient")).Returns(httpClient);

        var client = CreateClient();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EcovacsApiException>(
            () => client.SendPostRequestAsync<TestData>("/test/endpoint", new { }));

        Assert.Contains("Failed to deserialize", exception.Message);
    }

    [Fact]
    public async Task SendGetRequestAsync_SuccessfulRequest_ReturnsDeserializedResponse()
    {
        // Arrange
        var expectedResponse = new ApiResponse<TestData>
        {
            Code = 0,
            Message = "success",
            Data = new TestData { Value = "test" }
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://test.ecovacs.com")
        };
        _httpClientFactoryMock.Setup(x => x.CreateClient("EcovacsClient")).Returns(httpClient);

        var client = CreateClient();
        var parameters = new { test = "value" };

        // Act
        var result = await client.SendGetRequestAsync<TestData>("/test/endpoint", parameters);

        // Assert
        Assert.Equal(expectedResponse.Code, result.Code);
        Assert.Equal(expectedResponse.Message, result.Message);
        Assert.Equal(expectedResponse.Data.Value, result.Data.Value);
    }

    [Fact]
    public async Task SendGetRequestAsync_AddsApiKeyToQueryParameters()
    {
        // Arrange
        var expectedResponse = new ApiResponse<TestData> { Code = 0, Message = "success" };
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://test.ecovacs.com")
        };
        _httpClientFactoryMock.Setup(x => x.CreateClient("EcovacsClient")).Returns(httpClient);

        var client = CreateClient();

        // Act
        await client.SendGetRequestAsync<TestData>("/test/endpoint", new { test = "value" });

        // Assert
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("ak=test-api-key")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendPostRequestAsync_AddsApiKeyToRequestBody()
    {
        // Arrange
        var expectedResponse = new ApiResponse<TestData> { Code = 0, Message = "success" };
        var capturedContent = string.Empty;

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>(async (request, _) =>
            {
                capturedContent = await request.Content.ReadAsStringAsync();
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://test.ecovacs.com")
        };
        _httpClientFactoryMock.Setup(x => x.CreateClient("EcovacsClient")).Returns(httpClient);

        var client = CreateClient();

        // Act
        await client.SendPostRequestAsync<TestData>("/test/endpoint", new { test = "value" });

        // Assert
        Assert.Contains("\"ak\":\"test-api-key\"", capturedContent);
        Assert.Contains("\"test\":\"value\"", capturedContent);
    }

    private EcovacsClient CreateClient()
    {
        var optionsMock = new Mock<IOptions<EcovacsClientOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_options);

        return new EcovacsClient(
            _httpClientFactoryMock.Object,
            optionsMock.Object,
            _loggerMock.Object,
            _loggerFactoryMock.Object);
    }

    private class TestData
    {
        public string Value { get; set; } = string.Empty;
    }
}
