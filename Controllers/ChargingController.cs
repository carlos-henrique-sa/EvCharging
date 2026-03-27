using Microsoft.AspNetCore.Mvc;
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
}