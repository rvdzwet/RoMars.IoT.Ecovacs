using Microsoft.Extensions.Logging;
using RoMars.IoT.Ecovacs.Interfaces;
using RoMars.IoT.Ecovacs.Models.Responses;

namespace RoMars.IoT.Ecovacs.Services;

/// <summary>
/// Service for managing Ecovacs devices.
/// </summary>
public class EcovacsDeviceService : IEcovacsDeviceService
{
    private readonly IEcovacsClient _client;
    private readonly ILogger<EcovacsDeviceService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EcovacsDeviceService"/> class.
    /// </summary>
    /// <param name="client">The Ecovacs client.</param>
    /// <param name="logger">The logger.</param>
    public EcovacsDeviceService(IEcovacsClient client, ILogger<EcovacsDeviceService> logger)
    {
        _client = client;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<List<string>>> GetDeviceListAsync()
    {
        _logger.LogInformation("Retrieving device list");

        try
        {
            var response = await _client.SendGetRequestAsync<List<string>>("/robot/deviceList", new { });
            _logger.LogInformation("Retrieved {Count} devices", response.Data?.Count ?? 0);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve device list");
            throw;
        }
    }
}
