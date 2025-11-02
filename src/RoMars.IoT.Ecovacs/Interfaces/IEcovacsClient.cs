using RoMars.IoT.Ecovacs.Models.Responses;

namespace RoMars.IoT.Ecovacs.Interfaces;

/// <summary>
/// Main interface for the Ecovacs client.
/// Provides access to device management and control services.
/// </summary>
public interface IEcovacsClient
{
    /// <summary>
    /// Gets the device service for managing Ecovacs devices.
    /// </summary>
    IEcovacsDeviceService Devices { get; }

    /// <summary>
    /// Gets the control service for controlling Ecovacs devices.
    /// </summary>
    IEcovacsControlService Control { get; }

    /// <summary>
    /// Sends a POST request to the Ecovacs API and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The expected type of the data field in the ApiResponse.</typeparam>
    /// <param name="endpoint">The API endpoint (path) to send the request to.</param>
    /// <param name="parameters">The request parameters to be serialized into the POST body (as application/json).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the API response object.</returns>
    Task<ApiResponse<T>> SendPostRequestAsync<T>(string endpoint, object parameters);

    /// <summary>
    /// Sends a GET request to the Ecovacs API and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The expected type of the data field in the ApiResponse.</typeparam>
    /// <param name="endpoint">The API endpoint (path) to send the request to.</param>
    /// <param name="parameters">The request parameters to be converted into a URL query string.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the API response object.</returns>
    Task<ApiResponse<T>> SendGetRequestAsync<T>(string endpoint, object parameters);
}