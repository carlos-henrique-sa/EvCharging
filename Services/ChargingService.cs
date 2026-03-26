using EvCharging.Models;
using System.Collections.Concurrent;

namespace EvCharging.Services;

/// <summary>
/// Service responsible for managing charging sessions.
/// </summary>
public class ChargingService : IChargingService
{
    private readonly ConcurrentDictionary<string, ChargingSession> _sessions = new();
    private readonly ConcurrentDictionary<string, OcppChargePoint> _ocppChargePoints = new();
    private readonly ConcurrentDictionary<string, OcppStatus> _ocppStatuses = new();

    /// <inheritdoc/>
    public ChargingSession Start(string stationId)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            throw new ArgumentException("stationId is required", nameof(stationId));
        

        var newSession = new ChargingSession
        {
            StationId = stationId,
            IsCharging = true,
            PowerKw = 22,
            StartTime = DateTime.UtcNow,
            EndTime = null
        };

        var session = _sessions.AddOrUpdate(stationId, newSession, (key, old) => newSession);

        return session;
    }

    /// <inheritdoc/>
    public ChargingSession Stop(string stationId)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            throw new ArgumentException("stationId is required", nameof(stationId));

        if (!_sessions.TryGetValue(stationId, out var existing))
            return null;

        if (!existing.IsCharging)
            return existing;

        existing.IsCharging = false;
        existing.EndTime = DateTime.UtcNow;
        return existing;
    }

    /// <inheritdoc/>
    public ChargingSession Get(string stationId)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            throw new ArgumentException("stationId is required", nameof(stationId));

        return _sessions.TryGetValue(stationId, out var session) ? session : null!;
    }

    /// <inheritdoc/>
    public IEnumerable<ChargingSession> GetAll() => _sessions.Values;

    /// <inheritdoc/>
    public IEnumerable<ChargingSession> GetActive() => _sessions.Values.Where(s => s.IsCharging);

    /// <inheritdoc/>
    public bool TryStart(string stationId, out ChargingSession session)
    {
        session = new ChargingSession
        {
            StationId = stationId,
            IsCharging = true,
            PowerKw = 22,
            StartTime = DateTime.UtcNow
        };

        return _sessions.TryAdd(stationId, session);
    }

    /// <inheritdoc/>
    public bool TryStop(string stationId, out ChargingSession session)
    {
        if (!_sessions.TryGetValue(stationId, out var existing))
        {
            session = null!;
            return false;
        }

        if (!existing.IsCharging)
        {
            session = existing;
            return false;
        }

        var updated = new ChargingSession
        {
            StationId = existing.StationId,
            IsCharging = false,
            PowerKw = existing.PowerKw,
            StartTime = existing.StartTime,
            EndTime = DateTime.UtcNow
        };

        _sessions[stationId] = updated;
        session = updated;
        return true;
    }

    /// <inheritdoc/>
    public bool TryGet(string stationId, out ChargingSession session)
    {
        session = Get(stationId);
        return session != null;
    }

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
            Start(stationId);
        else
            Stop(stationId);

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
                Start(stationId);
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
                Stop(stationId);
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

    public bool AlreadyCharging(string stationId)
    {
        return _sessions.TryGetValue(stationId, out var s) && s.IsCharging;
    }
}