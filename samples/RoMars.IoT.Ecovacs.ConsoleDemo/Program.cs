using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoMars.IoT.Ecovacs.Configuration;
using RoMars.IoT.Ecovacs.Exceptions;
using RoMars.IoT.Ecovacs.Interfaces;
using RoMars.IoT.Ecovacs.Models.Enums;

// Configure services
var services = new ServiceCollection();
services.AddLogging();

// Configure Ecovacs client with options
services.AddEcovacsClient(options =>
{
    options.ApiKey = "aXm974U5WINpkhJ4jviLXNxqXQA85p3c";
    options.BaseUrl = "https://open.ecovacs.com/";
});

var serviceProvider = services.BuildServiceProvider();
var ecovacsClient = serviceProvider.GetRequiredService<IEcovacsClient>();

try
{
    // Get available devices
    Console.WriteLine("Fetching available devices...");
    var deviceResponse = await ecovacsClient.Devices.GetDeviceListAsync();

    if (deviceResponse.Code != 0 || deviceResponse.Data == null || deviceResponse.Data.Count == 0)
    {
        Console.WriteLine("No devices found or API error occurred.");
        return;
    }

    var deviceNickname = deviceResponse.Data[0];
    Console.WriteLine($"Found device: {deviceNickname}");

    // Get current work state
    Console.WriteLine("\nFetching current work state...");
    var workStateResponse = await ecovacsClient.Control.GetWorkStateAsync(deviceNickname);

    if (workStateResponse.Code == 0 && workStateResponse.Data?.Data?.Control?.Data != null)
    {
        var workState = workStateResponse.Data.Data.Control.Data;
        Console.WriteLine($"Cleaning Status: {workState.CleaningStatus}");
        Console.WriteLine($"Charging Status: {workState.ChargingStatus}");
        Console.WriteLine($"Station Status: {workState.StationStatus}");
    }

    // Example: Send a cleaning command (commented out for safety)

    Console.WriteLine("\nStarting cleaning...");
    var cleanResponse = await ecovacsClient.Control.SendCleaningCommandAsync(deviceNickname, CleaningAction.Start);
    Console.WriteLine($"Clean command result: {cleanResponse.Message}");


    Console.WriteLine("\nDemo completed successfully!");
}
catch (EcovacsApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message} (Code: {ex.ErrorCode})");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
