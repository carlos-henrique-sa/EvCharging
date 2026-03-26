using Microsoft.AspNetCore.Mvc;
using EvCharging.Models;
using EvCharging.Services;

namespace EvCharging.Controllers;

[ApiController]
[Route("charging")]
public class ChargingController(IChargingService _service) : ControllerBase
{

    [HttpGet]
    public IActionResult GetAll()
    {
        var sessions = _service.GetAll();
        return Ok(sessions);
    }

    [HttpGet("active")]
    public IActionResult GetActive()
    {
        var sessions = _service.GetActive();
        return Ok(sessions);
    }

    [HttpGet("{stationId}")]
    public IActionResult Get(string stationId)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            return BadRequest("StationId is required.");

        if (!_service.TryGet(stationId, out var result))
            return NotFound();

        return Ok(result);
    }

    [HttpPost("start/{stationId}")]
    public IActionResult Start(string stationId)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            return BadRequest("StationId is required.");

        if (_service.AlreadyCharging(stationId)) {
            return Conflict($"Station '{stationId}' is already charging.");
        }

        _service.TryStart(stationId, out var session);

    
        return Ok(session);
    }

    [HttpPost("stop/{stationId}")]
    public IActionResult Stop(string stationId)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            return BadRequest("StationId is required.");

        if (!_service.TryStop(stationId, out var session))
            return NotFound();

        return Ok(session);
    }

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