using System.Text.Json.Serialization;

namespace RoMars.IoT.Ecovacs.Models.Responses;

/// <summary>
/// Represents a generic API response from the Ecovacs service.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Gets or sets the response message.
    /// </summary>
    [JsonPropertyName("msg")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the response code.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; } = -1;

    /// <summary>
    /// Gets or sets the response data.
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}
