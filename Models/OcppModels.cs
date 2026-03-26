namespace EvCharging.Models;

public class OcppChargePoint
{
    public string StationId { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public DateTime RegisteredAtUtc { get; set; }
}

public class OcppStatus
{
    public string StationId { get; set; } = string.Empty;
    public bool IsCharging { get; set; }
    public string Status { get; set; } = "Unknown";
    public DateTime TimestampUtc { get; set; }
    public string? Message { get; set; }
}

public class OcppCommandRequest
{
    public string Action { get; set; } = string.Empty;
    public string? Payload { get; set; }
}

public class OcppStatusUpdateRequest
{
    public bool IsCharging { get; set; }
    public string Status { get; set; } = "Unknown";
    public string? Message { get; set; }
}