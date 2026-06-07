using AspireApp.MigrationService;
using AspireApp.Persistence.Data;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Create an early logger
using var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});
var logger = loggerFactory.CreateLogger("MigrationService");

// Add service defaults
builder.AddServiceDefaults(logger);

// Configure DbContext with migrations assembly
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        "Server=(localdb)\\mssqllocaldb;Database=AspireAppDb;Trusted_Connection=True;MultipleActiveResultSets=true",
        sqlOptions => sqlOptions.MigrationsAssembly("AspireApp.MigrationService")
    ));

// Add migration worker
builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
host.Run();
