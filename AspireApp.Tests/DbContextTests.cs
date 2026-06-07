using AspireApp.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AspireApp.Tests;

public class DbContextTests
{
    [Fact]
    public void DbContext_ShouldNotHavePendingModelChanges()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=(localdb)\\mssqllocaldb;Database=AspireAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
            })
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>();
        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.MigrationsAssembly("AspireApp.MigrationService")
        );

        using var context = new ApiDbContext(optionsBuilder.Options);

        // Act
        var hasPendingModelChanges = context.Database.HasPendingModelChanges();

        // Assert
        Assert.False(hasPendingModelChanges,
            "The database model has pending changes. Please create a migration using: " +
            "dotnet ef migrations add <MigrationName> -p AspireApp.MigrationService -s AspireApp.MigrationService");
    }
}
