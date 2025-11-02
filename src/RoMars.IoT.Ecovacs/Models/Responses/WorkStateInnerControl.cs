using System.Text.Json.Serialization;

namespace RoMars.IoT.Ecovacs.Models.Responses;

/// <summary>
/// Represents the inner work state control.
/// </summary>
public class WorkStateInnerControl
{
    /// <summary>
    /// Gets or sets the control data.
    /// </summary>
    [JsonPropertyName("ctl")]
    public WorkStateControl? Control { get; set; }
}
