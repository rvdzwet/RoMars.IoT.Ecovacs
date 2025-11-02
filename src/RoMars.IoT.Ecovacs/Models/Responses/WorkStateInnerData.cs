using System.Text.Json.Serialization;

namespace RoMars.IoT.Ecovacs.Models.Responses;

/// <summary>
/// Represents the inner work state data.
/// </summary>
public class WorkStateInnerData
{
    /// <summary>
    /// Gets or sets the response code.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; } = -1;

    /// <summary>
    /// Gets or sets the response message.
    /// </summary>
    [JsonPropertyName("msg")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the inner data.
    /// </summary>
    [JsonPropertyName("data")]
    public WorkStateInnerControl? Data { get; set; }
}
