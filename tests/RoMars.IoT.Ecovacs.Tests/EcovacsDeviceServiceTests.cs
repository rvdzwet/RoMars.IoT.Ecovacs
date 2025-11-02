using Microsoft.Extensions.Logging;
using Moq;
using RoMars.IoT.Ecovacs.Interfaces;
using RoMars.IoT.Ecovacs.Models.Responses;
using RoMars.IoT.Ecovacs.Services;
using Xunit;

namespace RoMars.IoT.Ecovacs.Tests;

public class EcovacsDeviceServiceTests
{
    private readonly Mock<IEcovacsClient> _clientMock;
    private readonly Mock<ILogger<EcovacsDeviceService>> _loggerMock;
    private readonly EcovacsDeviceService _service;

    public EcovacsDeviceServiceTests()
    {
        _clientMock = new Mock<IEcovacsClient>();
        _loggerMock = new Mock<ILogger<EcovacsDeviceService>>();
        _service = new EcovacsDeviceService(_clientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetDeviceListAsync_CallsClientWithCorrectEndpoint()
    {
        // Arrange
        var expectedResponse = new ApiResponse<List<string>>
        {
            Code = 0,
            Message = "success",
            Data = new List<string> { "device1", "device2" }
        };

        _clientMock.Setup(x => x.SendGetRequestAsync<List<string>>("/robot/deviceList", It.IsAny<object>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetDeviceListAsync();

        // Assert
        Assert.Equal(expectedResponse.Code, result.Code);
        Assert.Equal(expectedResponse.Message, result.Message);
        Assert.Equal(expectedResponse.Data, result.Data);
        _clientMock.Verify(x => x.SendGetRequestAsync<List<string>>("/robot/deviceList", It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task GetDeviceListAsync_WhenClientThrowsException_RethrowsException()
    {
        // Arrange
        var expectedException = new Exception("API Error");
        _clientMock.Setup(x => x.SendGetRequestAsync<List<string>>("/robot/deviceList", It.IsAny<object>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.GetDeviceListAsync());
        Assert.Equal(expectedException, exception);
    }

    [Fact]
    public async Task GetDeviceListAsync_ReturnsEmptyListWhenNoDevices()
    {
        // Arrange
        var expectedResponse = new ApiResponse<List<string>>
        {
            Code = 0,
            Message = "success",
            Data = new List<string>()
        };

        _clientMock.Setup(x => x.SendGetRequestAsync<List<string>>("/robot/deviceList", It.IsAny<object>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetDeviceListAsync();

        // Assert
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }
}
