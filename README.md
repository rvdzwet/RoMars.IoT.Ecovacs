# Ecovacs

A modern, SOLID-compliant .NET client library for controlling Ecovacs IoT devices (robot vacuums, etc.) via their official API.

## Features

- **SOLID Principles**: Clean architecture following Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion principles
- **Microsoft Conventions**: Uses `IHttpClientFactory`, `IOptions<>`, and `ILogger<>` for proper .NET integration
- **Comprehensive Logging**: Structured logging with correlation IDs for request tracing
- **Dependency Injection**: Easy integration with .NET dependency injection containers
- **Type Safety**: Strongly typed enums and models with proper JSON serialization
- **Error Handling**: Custom exceptions with detailed error information
- **Async/Await**: Fully asynchronous API for high performance

## Installation

```bash
dotnet add package Ecovacs
```

## Quick Start

### 1. Installation

```bash
dotnet add package RoMars.IoT.Ecovacs
```

### 2. Configure Services

```csharp
using Microsoft.Extensions.DependencyInjection;
using RoMars.IoT.Ecovacs.Configuration;

var services = new ServiceCollection();

// Option 1: Configure via code (not recommended for production)
services.AddEcovacsClient(options =>
{
    options.ApiKey = "your-api-key-here"; // ⚠️  Avoid hardcoding secrets
    options.BaseUrl = "https://open.ecovacs.com";
});

// Option 2: Configure via IConfiguration (recommended)
services.AddEcovacsClient(configuration);
```

### 3. Configure Secrets

Create `appsettings.Development.json` in your project root (this file is gitignored):

```json
{
  "Ecovacs": {
    "ApiKey": "your-actual-api-key-here"
  }
}
```

For production, use environment variables or user secrets:

```bash
# Environment variables
set Ecovacs__ApiKey=your-api-key-here

# Or user secrets (development)
dotnet user-secrets set "Ecovacs:ApiKey" "your-api-key-here"
```

### 4. Configure appsettings.json

Your main `appsettings.json` should contain non-sensitive defaults:

```json
{
  "Ecovacs": {
    "BaseUrl": "https://open.ecovacs.com",
    "Timeout": "00:00:30",
    "MaxRetries": 3,
    "RetryDelay": "00:00:01"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "RoMars.IoT.Ecovacs": "Debug"
    }
  }
}
```

### 5. Use the Client

```csharp
using Ecovacs;

public class RobotController
{
    private readonly IEcovacsClient _ecovacsClient;

    public RobotController(IEcovacsClient ecovacsClient)
    {
        _ecovacsClient = ecovacsClient;
    }

    public async Task ControlRobotAsync()
    {
        // Get available devices
        var devices = await _ecovacsClient.Devices.GetDeviceListAsync();
        var robotNickname = devices.Data?.FirstOrDefault();

        if (robotNickname != null)
        {
            // Get current status
            var status = await _ecovacsClient.Control.GetWorkStateAsync(robotNickname);

            // Start cleaning
            await _ecovacsClient.Control.SendCleaningCommandAsync(robotNickname, CleaningAction.Start);

            // Stop cleaning after some time
            await Task.Delay(TimeSpan.FromMinutes(30));
            await _ecovacsClient.Control.SendCleaningCommandAsync(robotNickname, CleaningAction.Stop);
        }
    }
}
```

## API Reference

### IEcovacsClient

The main entry point for all Ecovacs operations.

```csharp
public interface IEcovacsClient
{
    IEcovacsDeviceService Devices { get; }
    IEcovacsControlService Control { get; }
    Task<ApiResponse<T>> SendRequestAsync<T>(string endpoint, object parameters);
}
```

### IEcovacsDeviceService

Handles device discovery and management.

```csharp
public interface IEcovacsDeviceService
{
    Task<ApiResponse<List<string>>> GetDeviceListAsync();
}
```

### IEcovacsControlService

Controls robot operations like cleaning and charging.

```csharp
public interface IEcovacsControlService
{
    Task<ApiResponse<WorkStateInnerData>> GetWorkStateAsync(string nickname);
    Task<ApiResponse<JsonElement>> SendCleaningCommandAsync(string nickname, CleaningAction action);
    Task<ApiResponse<JsonElement>> SendChargingCommandAsync(string nickname, ChargingAction action);
}
```

## Enums

### CleaningAction
- `Start` - Start cleaning
- `Resume` - Resume paused cleaning
- `Pause` - Pause current cleaning
- `Stop` - Stop cleaning and return home

### ChargingAction
- `GoStart` - Go home and start charging
- `StopGo` - Stop going home/charging

### CleaningStatus
- `Unknown` - Status unknown
- `Idle` - Robot is idle
- `Cleaning` - Robot is actively cleaning
- `Paused` - Cleaning is paused

### ChargingStatus
- `Unknown` - Status unknown
- `Idle` - Not charging
- `GoingHome` - Going home to charge
- `Charging` - Currently charging

### StationStatus
- `Unknown` - Status unknown
- `Idle` - Station is idle
- `Washing` - Station is washing
- `Drying` - Station is drying
- `DustCollecting` - Station is collecting dust

## Configuration

```csharp
public class EcovacsClientOptions
{
    public string BaseUrl { get; set; } = "https://open.ecovacs.com";
    public string? ApiKey { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
}
```

## Error Handling

The library throws `EcovacsApiException` for API-related errors:

```csharp
try
{
    var response = await ecovacsClient.Control.SendCleaningCommandAsync(nickname, CleaningAction.Start);
}
catch (EcovacsApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message} (Code: {ex.ErrorCode})");
}
```

## Logging

The library uses Microsoft.Extensions.Logging. Configure logging levels:

```json
{
  "Logging": {
    "LogLevel": {
      "Ecovacs": "Debug"
    }
  }
}
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## License

MIT License - see LICENSE file for details.

## Support

For issues and questions, please create an issue on GitHub.
