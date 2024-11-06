using Microsoft.EntityFrameworkCore;

namespace BlazorSolarPowerHour.Models;

public class MeasurementsDbContext(DbContextOptions<MeasurementsDbContext> options, IConfiguration config) : DbContext(options)
{
    private readonly string topicPrefix = config["MQTT_TOPIC_PREFIX"] ?? "solar_assistant";

    public DbSet<MqttDataItem> Measurements { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MqttDataItem>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        base.OnModelCreating(modelBuilder);
    }
}