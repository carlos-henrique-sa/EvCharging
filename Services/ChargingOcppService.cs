using EvCharging.Models;
using System.Collections.Concurrent;

namespace EvCharging.Services;

/// <summary>
/// Service responsible for managing OCPP interactions.
/// </summary>
public class ChargingOcppService(IChargingService _chargingService) : IChargingOcppService
{
    private readonly ConcurrentDictionary<string, OcppChargePoint> _ocppChargePoints = new();
    private readonly ConcurrentDictionary<string, OcppStatus> _ocppStatuses = new();

    /// <inheritdoc/>
    public OcppChargePoint RegisterOcppChargePoint(string stationId, string vendor, string model)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            throw new ArgumentException("stationId is required", nameof(stationId));

        var chargePoint = new OcppChargePoint
        {
            StationId = stationId,
            Vendor = vendor,
            Model = model,
            RegisteredAtUtc = DateTime.UtcNow
        };

        _ocppChargePoints[stationId] = chargePoint;
        _ocppStatuses[stationId] = new OcppStatus
        {
            StationId = stationId,
            IsCharging = false,
            Status = "Available",
            TimestampUtc = DateTime.UtcNow
        };

        return chargePoint;
    }

    /// <inheritdoc/>
    public OcppStatus? GetOcppStatus(string stationId)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            throw new ArgumentException("stationId is required", nameof(stationId));

        _ocppStatuses.TryGetValue(stationId, out var status);
        return status;
    }

    /// <inheritdoc/>
    public OcppStatus UpdateOcppStatus(string stationId, OcppStatusUpdateRequest statusUpdate)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            throw new ArgumentException("stationId is required", nameof(stationId));

        if (statusUpdate == null)
            throw new ArgumentNullException(nameof(statusUpdate));

        if (!_ocppChargePoints.ContainsKey(stationId))
            throw new InvalidOperationException($"Charge point '{stationId}' is not registered.");

        var newStatus = new OcppStatus
        {
            StationId = stationId,
            IsCharging = statusUpdate.IsCharging,
            Status = statusUpdate.Status,
            TimestampUtc = DateTime.UtcNow,
            Message = statusUpdate.Message
        };

        _ocppStatuses[stationId] = newStatus;

        if (statusUpdate.IsCharging)
            _chargingService.Start(stationId);
        else
            _chargingService.Stop(stationId);

        return newStatus;
    }

    /// <inheritdoc/>
    public OcppCommandRequest SendOcppCommand(string stationId, OcppCommandRequest command)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            throw new ArgumentException("stationId is required", nameof(stationId));

        if (command == null)
            throw new ArgumentNullException(nameof(command));

        if (!_ocppChargePoints.ContainsKey(stationId))
            throw new InvalidOperationException($"Charge point '{stationId}' is not registered.");

        // Simulação: processa comando local e altera estado conforme need
        switch (command.Action.ToLowerInvariant())
        {
            case "startcharging":
                _chargingService.Start(stationId);
                _ocppStatuses[stationId] = new OcppStatus
                {
                    StationId = stationId,
                    IsCharging = true,
                    Status = "Charging",
                    TimestampUtc = DateTime.UtcNow,
                    Message = "Charge point started via OCPP command"
                };
                break;
            case "stopcharging":
                _chargingService.Stop(stationId);
                _ocppStatuses[stationId] = new OcppStatus
                {
                    StationId = stationId,
                    IsCharging = false,
                    Status = "Available",
                    TimestampUtc = DateTime.UtcNow,
                    Message = "Charge point stopped via OCPP command"
                };
                break;
            default:
                _ocppStatuses[stationId] = new OcppStatus
                {
                    StationId = stationId,
                    IsCharging = _ocppStatuses[stationId].IsCharging,
                    Status = _ocppStatuses[stationId].Status,
                    TimestampUtc = DateTime.UtcNow,
                    Message = $"Unknown command '{command.Action}' simulated"
                };
                break;
        }

        return command;
    }
}