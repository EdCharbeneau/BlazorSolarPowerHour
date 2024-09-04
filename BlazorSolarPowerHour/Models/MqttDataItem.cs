﻿using System.ComponentModel.DataAnnotations;

namespace BlazorSolarPowerHour.Models;

public class MqttDataItem
{
    /// <summary>
    /// Autogenerated key value by EFCore
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// String representation of the MQTT topic.
    /// </summary>
    public string? Topic { get; set; }

    /// <summary>
    /// Value of the MQTT message.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Time stamp of when the MQTT message was received.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
