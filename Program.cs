using EvCharging.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração de logging para produção
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Serviços
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<IChargingService, ChargingService>();
builder.Services.AddSingleton<IChargingOcppService, ChargingOcppService>();
builder.Services.AddRazorPages();
var app = builder.Build();


// Middleware de tratamento global de erros
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

app.MapControllers();
app.UseHttpsRedirection();

app.MapRazorPages();

app.Run();
