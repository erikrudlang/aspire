var builder = WebApplication.CreateBuilder(args);

ILogger? logger = null;

try
{
    // Add service defaults & Aspire client integrations.
    builder.AddServiceDefaults(logger);

    // Add services to the container.
    builder.Services.AddProblemDetails();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Get logger after app is built
    logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Application starting up...");

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

    logger.LogInformation("Application configured successfully, starting web host...");

    app.Run();
}
catch (Exception ex)
{
    if (logger != null)
    {
        logger.LogCritical(ex, "Application failed to start");
    }
    else
    {
        Console.WriteLine($"FATAL ERROR during startup: {ex}");
    }
    throw;
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
