using RoMars.IoT.Ecovacs.Infrastructure.JsonConverters;
using System.Text.Json.Serialization;

namespace RoMars.IoT.Ecovacs.Models.Enums;

/// <summary>
/// Represents the charging status of an Ecovacs device.
/// </summary>
[JsonConverter(typeof(StringValueEnumConverter))]
public enum ChargingStatus
{
    /// <summary>
    /// Unknown status.
    /// </summary>
    [JsonPropertyName("unknown")]
    Unknown = -1,

    /// <summary>
    /// Device is idle (not charging).
    /// </summary>
    [JsonPropertyName("i")]
    Idle,

    /// <summary>
    /// Device is going home to charge.
    /// </summary>
    [JsonPropertyName("g")]
    GoingHome,

    /// <summary>
    /// Device is currently charging.
    /// </summary>
    [JsonPropertyName("charging")]
    Charging
}
