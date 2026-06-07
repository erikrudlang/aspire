using AspireApp.Persistence.Data;
using AspireApp.Persistence.Models;
using Microsoft.EntityFrameworkCore;

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

    // Configure DbContext with migrations assembly
    builder.Services.AddDbContext<ApiDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection") ??
            "Server=(localdb)\\mssqllocaldb;Database=AspireAppDb;Trusted_Connection=True;MultipleActiveResultSets=true",
            sqlOptions => sqlOptions.MigrationsAssembly("AspireApp.MigrationService")
        ));

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

    app.MapGet("/weatherforecast", async (ApiDbContext db) =>
    {
        var forecasts = await db.WeatherForecasts.ToListAsync();

        // If no data exists, seed some sample data
        if (!forecasts.Any())
        {
            string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

            var newForecasts = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
                .ToList();

            db.WeatherForecasts.AddRange(newForecasts);
            await db.SaveChangesAsync();

            forecasts = newForecasts;
        }

        return forecasts;
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
