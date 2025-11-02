using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoMars.IoT.Ecovacs.Configuration;
using RoMars.IoT.Ecovacs.Interfaces;
using RoMars.IoT.Ecovacs.Services;
using Xunit;

namespace RoMars.IoT.Ecovacs.Tests;

public class IntegrationTests
{
    [Fact]
    public void AddEcovacsClient_WithConfiguration_RegistersServicesCorrectly()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Ecovacs:BaseUrl"] = "https://test.ecovacs.com",
                ["Ecovacs:ApiKey"] = "test-api-key",
                ["Ecovacs:Timeout"] = "00:00:45",
                ["Ecovacs:MaxRetries"] = "5",
                ["Ecovacs:RetryDelay"] = "00:00:02"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddEcovacsClient(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetRequiredService<IEcovacsClient>();
        Assert.NotNull(client);
        Assert.NotNull(client.Devices);
        Assert.NotNull(client.Control);

        // Verify options are configured correctly
        var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<EcovacsClientOptions>>();
        Assert.Equal("https://test.ecovacs.com", options.Value.BaseUrl);
        Assert.Equal("test-api-key", options.Value.ApiKey);
        Assert.Equal(TimeSpan.FromSeconds(45), options.Value.Timeout);
        Assert.Equal(5, options.Value.MaxRetries);
        Assert.Equal(TimeSpan.FromSeconds(2), options.Value.RetryDelay);
    }

    [Fact]
    public void AddEcovacsClient_WithAction_RegistersServicesCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddEcovacsClient(options =>
        {
            options.BaseUrl = "https://custom.ecovacs.com";
            options.ApiKey = "custom-api-key";
            options.Timeout = TimeSpan.FromMinutes(1);
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetRequiredService<IEcovacsClient>();
        Assert.NotNull(client);
        Assert.NotNull(client.Devices);
        Assert.NotNull(client.Control);

        // Verify options are configured correctly
        var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<EcovacsClientOptions>>();
        Assert.Equal("https://custom.ecovacs.com", options.Value.BaseUrl);
        Assert.Equal("custom-api-key", options.Value.ApiKey);
        Assert.Equal(TimeSpan.FromMinutes(1), options.Value.Timeout);
    }

    [Fact]
    public void AddEcovacsClient_WithoutConfiguration_UsesDefaultOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddEcovacsClient();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetRequiredService<IEcovacsClient>();
        Assert.NotNull(client);

        // Verify default options
        var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<EcovacsClientOptions>>();
        Assert.Equal("https://open.ecovacs.com", options.Value.BaseUrl);
        Assert.Null(options.Value.ApiKey);
        Assert.Equal(TimeSpan.FromSeconds(30), options.Value.Timeout);
        Assert.Equal(3, options.Value.MaxRetries);
        Assert.Equal(TimeSpan.FromSeconds(1), options.Value.RetryDelay);
    }

    [Fact]
    public async Task FullIntegration_ClientCanBeResolvedAndUsed()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddEcovacsClient(options =>
        {
            options.BaseUrl = "https://test.ecovacs.com";
            options.ApiKey = "test-key";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var client = serviceProvider.GetRequiredService<IEcovacsClient>();

        // Assert - Just verify the client and services are properly resolved
        Assert.NotNull(client);
        Assert.NotNull(client.Devices);
        Assert.NotNull(client.Control);
        Assert.IsType<EcovacsDeviceService>(client.Devices);
        Assert.IsType<EcovacsControlService>(client.Control);

        // Note: We can't actually call the methods without mocking HTTP calls,
        // but this verifies the DI setup works correctly
    }
}
