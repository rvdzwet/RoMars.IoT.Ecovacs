using RoMars.IoT.Ecovacs.Models.Responses;

namespace RoMars.IoT.Ecovacs.Interfaces;

/// <summary>
/// Service interface for managing Ecovacs devices.
/// </summary>
public interface IEcovacsDeviceService
{
    /// <summary>
    /// Gets a list of available devices.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<ApiResponse<List<string>>> GetDeviceListAsync();
}
