using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using EvCharging.Hubs;
using EvCharging.Models;
using EvCharging.Services;

namespace EvCharging.Controllers;


[ApiController]
[Route("charging")]
public class ChargingOcppController(IChargingOcppService _service, IHubContext<ChargingHub> hubContext) : ControllerBase
{
    [HttpGet("ocpp/registered")]
    public IActionResult GetRegisteredChargePoints()
    {
        var chargePoints = _service.GetAllRegisteredStations();
        return Ok(chargePoints);
    }

    [HttpPost("{stationId}/ocpp/register")]
    public async Task<IActionResult> RegisterOcpp(string stationId, [FromQuery] string vendor, [FromQuery] string model)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            return BadRequest("StationId is required.");

        try
        {
            var chargePoint = _service.RegisterOcppChargePoint(stationId, vendor ?? "Unknown", model ?? "Unknown");

            // Notifica todos os clientes conectados via SignalR que uma nova estação foi registrada
            var ocppStatus = _service.GetOcppStatus(stationId);

            var stationRegistered = new
             {
                stationId,
                ocppStatus,
                session = (object?)null
            };
            await hubContext.Clients.All.SendAsync("StationRegistered", stationRegistered);

            return Ok(chargePoint);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        } 
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