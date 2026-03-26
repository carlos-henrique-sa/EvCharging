using EvCharging.Models;

namespace EvCharging.Services;

/// <summary>
/// Interface para operações de sessão de carregamento.
/// </summary>
public interface IChargingService
{
    /// <summary>
    /// Inicia uma sessão de carregamento para uma estação.
    /// </summary>
    ChargingSession Start(string stationId);

    /// <summary>
    /// Encerra uma sessão de carregamento para uma estação.
    /// </summary>
    ChargingSession Stop(string stationId);

    /// <summary>
    /// Obtém a sessão de carregamento de uma estação.
    /// </summary>
    ChargingSession Get(string stationId);

    /// <summary>
    /// Obtém todas as sessões (por estação).
    /// </summary>
    IEnumerable<ChargingSession> GetAll();

    /// <summary>
    /// Obtém apenas sessões ativas.
    /// </summary>
    IEnumerable<ChargingSession> GetActive();

    /// <summary>
    /// Inicia o carregamento se possível (não estava carregando).
    /// </summary>
    bool TryStart(string stationId, out ChargingSession session);

    /// <summary>
    /// Para o carregamento se possível.
    /// </summary>
    bool TryStop(string stationId, out ChargingSession session);

    /// <summary>
    /// Busca a sessão por estação.
    /// </summary>
    bool TryGet(string stationId, out ChargingSession session);


    bool AlreadyCharging(string stationId);
}