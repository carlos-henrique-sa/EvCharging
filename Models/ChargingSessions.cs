namespace EvCharging.Models;

/// <summary>
/// Representa uma sessão de carregamento de veículo elétrico.
/// </summary>
public class ChargingSession
{
    /// <summary>
    /// Identificador da estação de carregamento.
    /// </summary>
    public string StationId { get; set; } = string.Empty;

    /// <summary>
    /// Indica se a estação está carregando.
    /// </summary>
    public bool IsCharging { get; set; }

    /// <summary>
    /// Potência em kW fornecida pela estação.
    /// </summary>
    public double PowerKw { get; set; }

    /// <summary>
    /// Data e hora de início da sessão.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Data e hora de fim da sessão (quando parada).
    /// </summary>
    public DateTime? EndTime { get; set; }
}
