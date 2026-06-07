using AspireApp.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.Persistence.Data;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    public DbSet<WeatherForecast> WeatherForecasts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.TemperatureC).IsRequired();
            entity.Property(e => e.Summary).HasMaxLength(100);
        });
    }
}
