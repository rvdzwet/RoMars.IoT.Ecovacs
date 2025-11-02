using RoMars.IoT.Ecovacs.Models.Enums;
using System.Text.Json.Serialization;

namespace RoMars.IoT.Ecovacs.Models.Responses;

/// <summary>
/// Represents the work state data of an Ecovacs device.
/// </summary>
public class WorkStateData
{
    /// <summary>
    /// Gets or sets the result string.
    /// </summary>
    [JsonPropertyName("ret")]
    public string Result { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cleaning status.
    /// </summary>
    [JsonPropertyName("cleanSt")]
    public CleaningStatus CleaningStatus { get; set; } = CleaningStatus.Unknown;

    /// <summary>
    /// Gets or sets the charging status.
    /// </summary>
    [JsonPropertyName("chargeSt")]
    public ChargingStatus ChargingStatus { get; set; } = ChargingStatus.Unknown;

    /// <summary>
    /// Gets or sets the station status.
    /// </summary>
    [JsonPropertyName("stationSt")]
    public StationStatus StationStatus { get; set; } = StationStatus.Unknown;

    /// <summary>
    /// Gets or sets the waterbox status.
    /// </summary>
    [JsonPropertyName("waterboxSt")]
    public int WaterboxStatus { get; set; } = 0;

    /// <summary>
    /// Gets or sets the sleep status.
    /// </summary>
    [JsonPropertyName("sleepSt")]
    public int SleepStatus { get; set; } = 0;
}
