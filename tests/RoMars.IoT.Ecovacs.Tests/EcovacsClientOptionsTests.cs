using RoMars.IoT.Ecovacs.Configuration;
using Xunit;

namespace RoMars.IoT.Ecovacs.Tests;

public class EcovacsClientOptionsTests
{
    [Fact]
    public void EcovacsClientOptions_HasCorrectDefaultValues()
    {
        // Arrange & Act
        var options = new EcovacsClientOptions();

        // Assert
        Assert.Equal("https://open.ecovacs.com", options.BaseUrl);
        Assert.Null(options.ApiKey);
        Assert.Equal(TimeSpan.FromSeconds(30), options.Timeout);
        Assert.Equal(3, options.MaxRetries);
        Assert.Equal(TimeSpan.FromSeconds(1), options.RetryDelay);
    }

    [Fact]
    public void EcovacsClientOptions_CanSetCustomValues()
    {
        // Arrange & Act
        var options = new EcovacsClientOptions
        {
            BaseUrl = "https://custom.ecovacs.com",
            ApiKey = "custom-api-key",
            Timeout = TimeSpan.FromMinutes(2),
            MaxRetries = 5,
            RetryDelay = TimeSpan.FromSeconds(5)
        };

        // Assert
        Assert.Equal("https://custom.ecovacs.com", options.BaseUrl);
        Assert.Equal("custom-api-key", options.ApiKey);
        Assert.Equal(TimeSpan.FromMinutes(2), options.Timeout);
        Assert.Equal(5, options.MaxRetries);
        Assert.Equal(TimeSpan.FromSeconds(5), options.RetryDelay);
    }

    [Fact]
    public void EcovacsClientOptions_ApiKeyCanBeNull()
    {
        // Arrange & Act
        var options = new EcovacsClientOptions
        {
            ApiKey = null
        };

        // Assert
        Assert.Null(options.ApiKey);
    }

    [Fact]
    public void EcovacsClientOptions_TimeoutCanBeCustomized()
    {
        // Arrange & Act
        var options = new EcovacsClientOptions
        {
            Timeout = TimeSpan.FromHours(1)
        };

        // Assert
        Assert.Equal(TimeSpan.FromHours(1), options.Timeout);
    }

    [Fact]
    public void EcovacsClientOptions_RetrySettingsCanBeCustomized()
    {
        // Arrange & Act
        var options = new EcovacsClientOptions
        {
            MaxRetries = 10,
            RetryDelay = TimeSpan.FromMinutes(1)
        };

        // Assert
        Assert.Equal(10, options.MaxRetries);
        Assert.Equal(TimeSpan.FromMinutes(1), options.RetryDelay);
    }
}
