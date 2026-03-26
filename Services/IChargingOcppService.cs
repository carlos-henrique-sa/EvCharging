using EvCharging.Models;
namespace EvCharging.Services;

public interface IChargingOcppService
{
    // OCPP simulation methods

    /// <summary>
    /// Register an OCPP charger (charge point).
    /// </summary>
    OcppChargePoint RegisterOcppChargePoint(string stationId, string vendor, string model);

    /// <summary>
    /// Get the OCPP status of a charger.
    /// </summary>
    OcppStatus? GetOcppStatus(string stationId);

    /// <summary>
    /// Receive status update from the charger via OCPP.
    /// </summary>
    OcppStatus UpdateOcppStatus(string stationId, OcppStatusUpdateRequest statusUpdate);

    /// <summary>
    /// Send an OCPP command to the charger.
    /// </summary>
    OcppCommandRequest SendOcppCommand(string stationId, OcppCommandRequest command);
}