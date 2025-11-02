using RoMars.IoT.Ecovacs.Infrastructure.JsonConverters;
using System.Text.Json.Serialization;

namespace RoMars.IoT.Ecovacs.Models.Enums;

/// <summary>
/// Represents the station status of an Ecovacs device.
/// </summary>
[JsonConverter(typeof(StringValueEnumConverter))]
public enum StationStatus
{
    /// <summary>
    /// Unknown status.
    /// </summary>
    [JsonPropertyName("unknown")]
    Unknown = -1,

    /// <summary>
    /// Station is idle.
    /// </summary>
    [JsonPropertyName("i")]
    Idle,

    /// <summary>
    /// Station is washing.
    /// </summary>
    [JsonPropertyName("wash")]
    Washing,

    /// <summary>
    /// Station is drying.
    /// </summary>
    [JsonPropertyName("dry")]
    Drying,

    /// <summary>
    /// Station is collecting dust.
    /// </summary>
    [JsonPropertyName("dust")]
    DustCollecting
}
