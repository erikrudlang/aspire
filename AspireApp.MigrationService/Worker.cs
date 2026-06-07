using AspireApp.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.MigrationService;

public class MigrationWorker(ILogger<MigrationWorker> logger, IServiceProvider serviceProvider, IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting database migration...");

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

            // Ensure database is created
            logger.LogInformation("Ensuring database exists...");
            await dbContext.Database.EnsureCreatedAsync(stoppingToken);

            // Apply pending migrations
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(stoppingToken);
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                await dbContext.Database.MigrateAsync(stoppingToken);
                logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No pending migrations found.");
            }

            logger.LogInformation("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
        finally
        {
            // Stop the application after migration is complete
            lifetime.StopApplication();
        }
    }
}
