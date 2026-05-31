var builder = WebApplication.CreateBuilder(args);

// Create an early logger using the builder's logging
using var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});
var logger = loggerFactory.CreateLogger<Program>();

try
{
    logger.LogInformation("Starting application configuration...");

    // Add service defaults & Aspire client integrations.
    builder.AddServiceDefaults(logger);

    // Add services to the container.
    builder.Services.AddProblemDetails();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    logger.LogInformation("Building application...");
    var app = builder.Build();

    // Get the DI logger after app is built
    var appLogger = app.Services.GetRequiredService<ILogger<Program>>();
    appLogger.LogInformation("Application starting up...");

    // Configure the HTTP request pipeline.
    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    app.MapGet("/", () => "API service is running. Navigate to /weatherforecast to see sample data.");

    app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

    app.MapDefaultEndpoints();

    appLogger.LogInformation("Application configured successfully, starting web host...");

    app.Run();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Application failed to start");
    Console.WriteLine($"FATAL ERROR during startup: {ex}");
    throw;
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
