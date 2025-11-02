using RoMars.IoT.Ecovacs.Infrastructure.JsonConverters;
using System.Text.Json.Serialization;

namespace RoMars.IoT.Ecovacs.Models.Enums;

/// <summary>
/// Represents the cleaning status of an Ecovacs device.
/// </summary>
[JsonConverter(typeof(StringValueEnumConverter))]
public enum CleaningStatus
{
    /// <summary>
    /// Unknown status.
    /// </summary>
    [JsonPropertyName("unknown")]
    Unknown = -1,

    /// <summary>
    /// Device is idle.
    /// </summary>
    [JsonPropertyName("h")]
    Idle,

    /// <summary>
    /// Device is currently cleaning.
    /// </summary>
    [JsonPropertyName("s")]
    Cleaning,

    /// <summary>
    /// Device is paused.
    /// </summary>
    [JsonPropertyName("p")]
    Paused
}
