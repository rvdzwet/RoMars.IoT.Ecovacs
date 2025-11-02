using Microsoft.Extensions.Logging;
using Moq;
using RoMars.IoT.Ecovacs.Interfaces;
using RoMars.IoT.Ecovacs.Models.Enums;
using RoMars.IoT.Ecovacs.Models.Responses;
using RoMars.IoT.Ecovacs.Services;
using System.Text.Json;
using Xunit;

namespace RoMars.IoT.Ecovacs.Tests;

public class EcovacsControlServiceTests
{
    private readonly Mock<IEcovacsClient> _clientMock;
    private readonly Mock<ILogger<EcovacsControlService>> _loggerMock;
    private readonly EcovacsControlService _service;

    public EcovacsControlServiceTests()
    {
        _clientMock = new Mock<IEcovacsClient>();
        _loggerMock = new Mock<ILogger<EcovacsControlService>>();
        _service = new EcovacsControlService(_clientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetWorkStateAsync_CallsClientWithCorrectParameters()
    {
        // Arrange
        var nickname = "test-device";
        var expectedResponse = new ApiResponse<WorkStateInnerData>
        {
            Code = 0,
            Message = "success",
            Data = new WorkStateInnerData
            {
                Code = 0,
                Message = "success",
                Data = new WorkStateInnerControl()
            }
        };

        object capturedParameters = null;
        _clientMock.Setup(x => x.SendPostRequestAsync<WorkStateInnerData>("robot/ctl", It.IsAny<object>()))
            .Callback<string, object>((endpoint, parameters) => capturedParameters = parameters)
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetWorkStateAsync(nickname);

        // Assert
        Assert.Equal(expectedResponse, result);
        Assert.NotNull(capturedParameters);

        var nickNameProperty = capturedParameters.GetType().GetProperty("nickName");
        var cmdProperty = capturedParameters.GetType().GetProperty("cmd");
        var actProperty = capturedParameters.GetType().GetProperty("act");

        Assert.Equal(nickname, nickNameProperty.GetValue(capturedParameters));
        Assert.Equal("GetWorkState", cmdProperty.GetValue(capturedParameters));
        Assert.Equal("", actProperty.GetValue(capturedParameters));
    }

    [Theory]
    [InlineData(CleaningAction.Start, "s")]
    [InlineData(CleaningAction.Resume, "r")]
    [InlineData(CleaningAction.Pause, "p")]
    [InlineData(CleaningAction.Stop, "h")]
    public async Task SendCleaningCommandAsync_MapsActionsCorrectly(CleaningAction action, string expectedActionString)
    {
        // Arrange
        var nickname = "test-device";
        var expectedResponse = new ApiResponse<JsonElement>
        {
            Code = 0,
            Message = "success"
        };

        object capturedParameters = null;
        _clientMock.Setup(x => x.SendPostRequestAsync<JsonElement>("robot/ctl", It.IsAny<object>()))
            .Callback<string, object>((endpoint, parameters) => capturedParameters = parameters)
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.SendCleaningCommandAsync(nickname, action);

        // Assert
        Assert.Equal(expectedResponse, result);
        Assert.NotNull(capturedParameters);

        var nickNameProperty = capturedParameters.GetType().GetProperty("nickName");
        var cmdProperty = capturedParameters.GetType().GetProperty("cmd");
        var actProperty = capturedParameters.GetType().GetProperty("act");

        Assert.Equal(nickname, nickNameProperty.GetValue(capturedParameters));
        Assert.Equal("Clean", cmdProperty.GetValue(capturedParameters));
        Assert.Equal(expectedActionString, actProperty.GetValue(capturedParameters));
    }

    [Theory]
    [InlineData(ChargingAction.GoStart, "go-start")]
    [InlineData(ChargingAction.StopGo, "stopGo")]
    public async Task SendChargingCommandAsync_MapsActionsCorrectly(ChargingAction action, string expectedActionString)
    {
        // Arrange
        var nickname = "test-device";
        var expectedResponse = new ApiResponse<JsonElement>
        {
            Code = 0,
            Message = "success"
        };

        object capturedParameters = null;
        _clientMock.Setup(x => x.SendPostRequestAsync<JsonElement>("robot/ctl", It.IsAny<object>()))
            .Callback<string, object>((endpoint, parameters) => capturedParameters = parameters)
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.SendChargingCommandAsync(nickname, action);

        // Assert
        Assert.Equal(expectedResponse, result);
        Assert.NotNull(capturedParameters);

        var nickNameProperty = capturedParameters.GetType().GetProperty("nickName");
        var cmdProperty = capturedParameters.GetType().GetProperty("cmd");
        var actProperty = capturedParameters.GetType().GetProperty("act");

        Assert.Equal(nickname, nickNameProperty.GetValue(capturedParameters));
        Assert.Equal("Charge", cmdProperty.GetValue(capturedParameters));
        Assert.Equal(expectedActionString, actProperty.GetValue(capturedParameters));
    }

    [Fact]
    public async Task GetWorkStateAsync_WhenClientThrowsException_RethrowsException()
    {
        // Arrange
        var nickname = "test-device";
        var expectedException = new Exception("API Error");
        _clientMock.Setup(x => x.SendPostRequestAsync<WorkStateInnerData>("robot/ctl", It.IsAny<object>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.GetWorkStateAsync(nickname));
        Assert.Equal(expectedException, exception);
    }

    [Fact]
    public async Task SendCleaningCommandAsync_WhenClientThrowsException_RethrowsException()
    {
        // Arrange
        var nickname = "test-device";
        var expectedException = new Exception("API Error");
        _clientMock.Setup(x => x.SendPostRequestAsync<JsonElement>("robot/ctl", It.IsAny<object>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.SendCleaningCommandAsync(nickname, CleaningAction.Start));
        Assert.Equal(expectedException, exception);
    }

    [Fact]
    public async Task SendChargingCommandAsync_WhenClientThrowsException_RethrowsException()
    {
        // Arrange
        var nickname = "test-device";
        var expectedException = new Exception("API Error");
        _clientMock.Setup(x => x.SendPostRequestAsync<JsonElement>("robot/ctl", It.IsAny<object>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.SendChargingCommandAsync(nickname, ChargingAction.GoStart));
        Assert.Equal(expectedException, exception);
    }
}
