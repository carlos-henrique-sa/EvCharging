using EvCharging.Models;
using System.Collections.Concurrent;

namespace EvCharging.Services;

/// <summary>
/// Service responsible for managing charging sessions.
/// </summary>
public class ChargingService : IChargingService
{
    private readonly ConcurrentDictionary<string, ChargingSession> _sessions = new();

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
            return null!;

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
    public bool AlreadyCharging(string stationId)
    {
        return _sessions.TryGetValue(stationId, out var s) && s.IsCharging;
    }
}