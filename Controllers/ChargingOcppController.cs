using Microsoft.AspNetCore.Mvc;
using EvCharging.Models;
using EvCharging.Services;

namespace EvCharging.Controllers;


[ApiController]
[Route("charging")]
public class ChargingOcppController(IChargingOcppService _service) : ControllerBase
{
    [HttpPost("{stationId}/ocpp/register")]
    public IActionResult RegisterOcpp(string stationId, [FromQuery] string vendor, [FromQuery] string model)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            return BadRequest("StationId is required.");

        var chargePoint = _service.RegisterOcppChargePoint(stationId, vendor ?? "Unknown", model ?? "Unknown");
        return Ok(chargePoint);
    }

    [HttpGet("{stationId}/ocpp/status")]
    public IActionResult GetOcppStatus(string stationId)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            return BadRequest("StationId is required.");

        var status = _service.GetOcppStatus(stationId);
        if (status == null)
            return NotFound();

        return Ok(status);
    }

    [HttpPost("{stationId}/ocpp/status")]
    public IActionResult UpdateOcppStatus(string stationId, [FromBody] OcppStatusUpdateRequest updateRequest)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            return BadRequest("StationId is required.");

        try
        {
            var status = _service.UpdateOcppStatus(stationId, updateRequest);
            return Ok(status);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{stationId}/ocpp/command")]
    public IActionResult SendOcppCommand(string stationId, [FromBody] OcppCommandRequest command)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            return BadRequest("StationId is required.");

        try
        {
            var response = _service.SendOcppCommand(stationId, command);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}