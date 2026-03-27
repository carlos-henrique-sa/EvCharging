using Microsoft.AspNetCore.SignalR;
using EvCharging.Services;
using EvCharging.Models;

namespace EvCharging.Hubs;

public class ChargingHub(IChargingOcppService ocppService, IChargingService chargingService) : Hub
{
    private readonly IChargingOcppService _ocppService = ocppService;
    private readonly IChargingService _chargingService = chargingService;

    // Calls when client wants to start/stop charger or send any OCPP command
    public async Task SendCommand(string stationId, string action)
    {
        if (string.IsNullOrWhiteSpace(stationId))
            return;

        try
        {
            var command = new OcppCommandRequest { Action = action };
            _ocppService.SendOcppCommand(stationId, command);

            // Após processar, notifica TODOS os clients conectados
            await BroadcastStatusUpdate(stationId);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    // Internal method to notify all clients when status changes
    private async Task BroadcastStatusUpdate(string stationId)
    {
        var ocppStatus = _ocppService.GetOcppStatus(stationId);
        var session = _chargingService.Get(stationId);

        var payload = new
        {
            stationId,
            ocppStatus,
            session,
            timestamp = DateTime.UtcNow
        };

        await Clients.All.SendAsync("StatusUpdated", payload);
    }
}