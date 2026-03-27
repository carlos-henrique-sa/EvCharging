using EvCharging.Models;

namespace EvCharging.Services;

/// <summary>
/// Interface for operations of charging sessions.
/// </summary>
public interface IChargingService
{
    /// <summary>
    /// Starts the charging session for the station.
    /// </summary>
    ChargingSession Start(string stationId);

    /// <summary>
    /// Stops the charging session for the station.
    /// </summary>
    ChargingSession Stop(string stationId);

    /// <summary>
    /// Get the session for a station.
    /// </summary>
    ChargingSession Get(string stationId);

    /// <summary>
    /// Start the charging.
    /// </summary>
    bool TryStart(string stationId, out ChargingSession session);

    /// <summary>
    /// Stop the charging.
    /// </summary>
    bool TryStop(string stationId, out ChargingSession session);

    /// <summary>
    /// Search for station.
    /// </summary>
    bool TryGet(string stationId, out ChargingSession session);

    /// <summary>
    /// Verify if station is charging.
    /// </summary>
    bool AlreadyCharging(string stationId);
}