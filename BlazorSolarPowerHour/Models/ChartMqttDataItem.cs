namespace BlazorSolarPowerHour.Models;

public class ChartMqttDataItem
{
    public TopicName Category { get; set; }
    public double CurrentValue { get; set; }
    public DateTime Timestamp { get; set; }
}