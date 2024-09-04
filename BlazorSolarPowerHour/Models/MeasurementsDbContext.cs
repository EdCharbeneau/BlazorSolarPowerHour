using Microsoft.EntityFrameworkCore;

namespace BlazorSolarPowerHour.Models;

public class MeasurementsDbContext(DbContextOptions<MeasurementsDbContext> options) : DbContext(options)
{
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

        modelBuilder.Entity<MqttDataItem>()
            .HasData(GenerateSeedData());

        base.OnModelCreating(modelBuilder);
    }

    private static IEnumerable<MqttDataItem> GenerateSeedData()
    {
        return
        [
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/pv_power/state", Value = "1400", Timestamp = DateTime.Now.AddSeconds(-1) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/grid_power/state", Value = "0", Timestamp = DateTime.Now.AddSeconds(-1) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/load_power/state", Value = "875", Timestamp = DateTime.Now.AddSeconds(-1) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/total/battery_power/state", Value = "525", Timestamp = DateTime.Now.AddSeconds(-1) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/total/battery_state_of_charge/state", Value = "100", Timestamp = DateTime.Now.AddSeconds(-1) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/charger_source_priority/state", Value = "Solar/Battery/Grid", Timestamp = DateTime.Now.AddSeconds(-1) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/device_mode/state", Value = "Solar", Timestamp = DateTime.Now.AddSeconds(-1) },
            
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/pv_power/state", Value = "1600", Timestamp = DateTime.Now.AddSeconds(-2) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/grid_power/state", Value = "0", Timestamp = DateTime.Now.AddSeconds(-2) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/load_power/state", Value = "875", Timestamp = DateTime.Now.AddSeconds(-2) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/total/battery_power/state", Value = "725", Timestamp = DateTime.Now.AddSeconds(-2) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/total/battery_state_of_charge/state", Value = "100", Timestamp = DateTime.Now.AddSeconds(-2) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/charger_source_priority/state", Value = "Solar/Battery/Grid", Timestamp = DateTime.Now.AddSeconds(-2) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/device_mode/state", Value = "Solar", Timestamp = DateTime.Now.AddSeconds(-2) },

            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/pv_power/state", Value = "1700", Timestamp = DateTime.Now.AddSeconds(-3) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/grid_power/state", Value = "0", Timestamp = DateTime.Now.AddSeconds(-3) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/load_power/state", Value = "1050", Timestamp = DateTime.Now.AddSeconds(-3) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/total/battery_power/state", Value = "650", Timestamp = DateTime.Now.AddSeconds(-3) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/total/battery_state_of_charge/state", Value = "100", Timestamp = DateTime.Now.AddSeconds(-3) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/charger_source_priority/state", Value = "Solar/Battery/Grid", Timestamp = DateTime.Now.AddSeconds(-3) },
            new() { Id  = Guid.NewGuid(), Topic = "solar_assistant/inverter_1/device_mode/state", Value = "Solar", Timestamp = DateTime.Now.AddSeconds(-3) }
        ];
    }
}