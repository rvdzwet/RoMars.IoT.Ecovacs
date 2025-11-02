using Microsoft.Extensions.Logging;
using RoMars.IoT.Ecovacs.Interfaces;
using RoMars.IoT.Ecovacs.Models.Enums;
using RoMars.IoT.Ecovacs.Models.Responses;
using System.Text.Json;

namespace RoMars.IoT.Ecovacs.Services;

/// <summary>
/// Service for controlling Ecovacs devices.
/// </summary>
public class EcovacsControlService : IEcovacsControlService
{
    private readonly IEcovacsClient _client;
    private readonly ILogger<EcovacsControlService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EcovacsControlService"/> class.
    /// </summary>
    /// <param name="client">The Ecovacs client.</param>
    /// <param name="logger">The logger.</param>
    public EcovacsControlService(IEcovacsClient client, ILogger<EcovacsControlService> logger)
    {
        _client = client;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<WorkStateInnerData>> GetWorkStateAsync(string nickname)
    {
        _logger.LogInformation("Getting work state for device {Nickname}", nickname);

        try
        {
            var parameters = new
            {
                nickName = nickname,
                cmd = "GetWorkState",
                act = ""
            };

            var response = await _client.SendPostRequestAsync<WorkStateInnerData>("robot/ctl", parameters);
            _logger.LogInformation("Retrieved work state for device {Nickname}", nickname);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get work state for device {Nickname}", nickname);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<JsonElement>> SendCleaningCommandAsync(string nickname, CleaningAction action)
    {
        var actionString = GetCleaningActionString(action);
        _logger.LogInformation("Sending cleaning command {Action} to device {Nickname}", action, nickname);

        try
        {
            var parameters = new
            {
                nickName = nickname,
                cmd = "Clean",
                act = actionString
            };

            var response = await _client.SendPostRequestAsync<JsonElement>("robot/ctl", parameters);
            _logger.LogInformation("Successfully sent cleaning command {Action} to device {Nickname}", action, nickname);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send cleaning command {Action} to device {Nickname}", action, nickname);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<JsonElement>> SendChargingCommandAsync(string nickname, ChargingAction action)
    {
        var actionString = GetChargingActionString(action);
        _logger.LogInformation("Sending charging command {Action} to device {Nickname}", action, nickname);

        try
        {
            var parameters = new
            {
                nickName = nickname,
                cmd = "Charge",
                act = actionString
            };

            var response = await _client.SendPostRequestAsync<JsonElement>("robot/ctl", parameters);
            _logger.LogInformation("Successfully sent charging command {Action} to device {Nickname}", action, nickname);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send charging command {Action} to device {Nickname}", action, nickname);
            throw;
        }
    }

    private static string GetCleaningActionString(CleaningAction action) => action switch
    {
        CleaningAction.Start => "s",
        CleaningAction.Resume => "r",
        CleaningAction.Pause => "p",
        CleaningAction.Stop => "h",
        _ => throw new ArgumentException($"Unsupported cleaning action: {action}")
    };

    private static string GetChargingActionString(ChargingAction action) => action switch
    {
        ChargingAction.GoStart => "go-start",
        ChargingAction.StopGo => "stopGo",
        _ => throw new ArgumentException($"Unsupported charging action: {action}")
    };
}
