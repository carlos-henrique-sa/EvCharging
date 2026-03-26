namespace EvCharging.Models;

public class ChargingStation
{
    public int StationId { get; set; }
    public bool IsCharging { get; set; }
    public double PowerKw { get; set; }
    public DateTime StartTime { get; set; }
}
