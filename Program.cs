using EvCharging.Hubs;
using EvCharging.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ==================== SERVIÇOS ====================
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// === SIGNALR ===
builder.Services.AddSignalR();                    // ← ESSA LINHA ESTAVA FALTANDO

// Seus serviços existentes
builder.Services.AddSingleton<IChargingService, ChargingService>();
builder.Services.AddSingleton<IChargingOcppService, ChargingOcppService>();

var app = builder.Build();

// ==================== MIDDLEWARE ====================
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\":\"Ocorreu um erro inesperado. Tente novamente mais tarde.\"}");
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ==================== ENDPOINTS ====================
app.MapHub<ChargingHub>("/chargingHub");     // Agora vai funcionar

app.MapControllers();
app.UseHttpsRedirection();
app.MapRazorPages();

app.Run();