using System.Text.Json.Serialization;

namespace RoMars.IoT.Ecovacs.Models.Responses;

/// <summary>
/// Represents the work state control data.
/// </summary>
public class WorkStateControl
{
    /// <summary>
    /// Gets or sets the work state data.
    /// </summary>
    [JsonPropertyName("data")]
    public WorkStateData? Data { get; set; }
}
