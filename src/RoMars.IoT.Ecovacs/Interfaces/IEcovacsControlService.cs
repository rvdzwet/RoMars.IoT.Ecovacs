using RoMars.IoT.Ecovacs.Models.Enums;
using RoMars.IoT.Ecovacs.Models.Responses;
using System.Text.Json;

namespace RoMars.IoT.Ecovacs.Interfaces;

/// <summary>
/// Service interface for controlling Ecovacs devices.
/// </summary>
public interface IEcovacsControlService
{
    /// <summary>
    /// Gets the current work state of a device.
    /// </summary>
    /// <param name="nickname">The device nickname.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<ApiResponse<WorkStateInnerData>> GetWorkStateAsync(string nickname);

    /// <summary>
    /// Sends a cleaning command to a device.
    /// </summary>
    /// <param name="nickname">The device nickname.</param>
    /// <param name="action">The cleaning action to perform.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<ApiResponse<JsonElement>> SendCleaningCommandAsync(string nickname, CleaningAction action);

    /// <summary>
    /// Sends a charging command to a device.
    /// </summary>
    /// <param name="nickname">The device nickname.</param>
    /// <param name="action">The charging action to perform.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<ApiResponse<JsonElement>> SendChargingCommandAsync(string nickname, ChargingAction action);
}
