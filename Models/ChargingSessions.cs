namespace EvCharging.Models;

/// <summary>
/// This represents an electric vehicle charging session.
/// </summary>
public class ChargingSession
{
    /// <summary>
    /// Charging station identifier.
    /// </summary>
    public string StationId { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the station is charging.
    /// </summary>
    public bool IsCharging { get; set; }

    /// <summary>
    /// Power in kW supplied by the station.
    /// </summary>
    public double PowerKw { get; set; }

    /// <summary>
    /// Start date and time of the session.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// End date and time of the session (when stopped).
    /// </summary>
    public DateTime? EndTime { get; set; }
}
