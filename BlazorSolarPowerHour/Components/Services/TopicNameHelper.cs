namespace BlazorSolarPowerHour.Components.Services;

using System.Diagnostics;
using System.Reflection;

public static class TopicNameHelper
{
    /// <summary>
    /// Converts the topic string (e.g. 'solar_assistant/inverter_1/battery_current/state') to a more easily workable enum
    /// </summary>
    /// <param name="topic">MQTT topic (e.g. 'solar_assistant/inverter_1/battery_current/state')</param>
    /// <returns>The matching enum equivalent.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the topic has no known match, an out of range exception wil be thrown. If needed, add the nw unknown type to the enum.</exception>
    public static TopicName GetTopicName(string topic)
    {
        return topic switch
        {
            "solar_assistant/total/battery_power/state" => TopicName.BatteryPower_Total,
            "solar_assistant/total/battery_state_of_charge/state" => TopicName.BatteryStateOfCharge_Total,
            "solar_assistant/total/battery_energy_in/state" => TopicName.BatteryEnergyIn_Total,
            "solar_assistant/total/battery_energy_out/state" => TopicName.BatteryEnergyOut_Total,
            "solar_assistant/total/bus_voltage/state" => TopicName.BusVoltage_Total,
            "solar_assistant/total/grid_energy_in/state" => TopicName.GridEnergyIn_Total,
            "solar_assistant/total/grid_energy_out/state" => TopicName.GridEnergyOut_Total,
            "solar_assistant/total/load_energy/state" => TopicName.LoadEnergy_Total,
            "solar_assistant/total/pv_energy/state" => TopicName.PvEnergy_Total,
            "solar_assistant/inverter_1/grid_frequency/state" => TopicName.GridFrequency_Inverter1,
            "solar_assistant/inverter_1/pv_current_1/state" => TopicName.PvCurrent1_Inverter1,
            "solar_assistant/inverter_1/pv_power/state" => TopicName.PvPower_Inverter1,
            "solar_assistant/inverter_1/battery_voltage/state" => TopicName.BatteryVoltage_Inverter1,
            "solar_assistant/inverter_1/load_apparent_power/state" => TopicName.LoadApparentPower_Inverter1,
            "solar_assistant/inverter_1/pv_current_2/state" => TopicName.PvCurrent2_Inverter1,
            "solar_assistant/inverter_1/temperature/state" => TopicName.Temperature_Inverter1,
            "solar_assistant/inverter_1/load_percentage/state" => TopicName.LoadPercentage_Inverter1,
            "solar_assistant/inverter_1/battery_current/state" => TopicName.BatteryCurrent_Inverter1,
            "solar_assistant/inverter_1/grid_power/state" => TopicName.GridPower_Inverter1,
            "solar_assistant/inverter_1/pv_voltage_1/state" => TopicName.PvVoltage1_Inverter1,
            "solar_assistant/inverter_1/pv_voltage_2/state" => TopicName.PvVoltage2_Inverter1,
            "solar_assistant/inverter_1/pv_power_1/state" => TopicName.PvPower1_Inverter1,
            "solar_assistant/inverter_1/device_mode/state" => TopicName.DeviceMode_Inverter1,
            "solar_assistant/inverter_1/grid_voltage/state" => TopicName.GridVoltage_Inverter1,
            "solar_assistant/inverter_1/ac_output_frequency/state" => TopicName.AcOutputFrequency_Inverter1,
            "solar_assistant/inverter_1/ac_output_voltage/state" => TopicName.AcOutputVoltage_Inverter1,
            "solar_assistant/inverter_1/load_power/state" => TopicName.LoadPower_Inverter1,
            "solar_assistant/inverter_1/pv_power_2/state" => TopicName.PvPower2_Inverter1,
            "solar_assistant/inverter_1/charger_source_priority/state" => TopicName.ChargerSourcePriority_Inverter1,
            "solar_assistant/inverter_1/battery_absorption_charge_voltage/state" => TopicName.BatteryAbsorptionChargeVoltage_Inverter1,
            "solar_assistant/inverter_1/max_charge_current/state" => TopicName.MaxChargeCurrent_Inverter1,
            "solar_assistant/inverter_1/battery_float_charge_voltage/state" => TopicName.BatteryFloatChargeVoltage_Inverter1,
            "solar_assistant/inverter_1/max_grid_charge_current/state" => TopicName.MaxGridChargeCurrent_Inverter1,
            "solar_assistant/inverter_1/output_source_priority/state" => TopicName.OutputSourcePriority_Inverter1,
            "solar_assistant/inverter_1/to_grid_battery_voltage/state" => TopicName.ToGridBatteryVoltage_Inverter1,
            "solar_assistant/inverter_1/shutdown_battery_voltage/state" => TopicName.ShutdownBatteryVoltage_Inverter1,
            "solar_assistant/inverter_1/back_to_battery_voltage/state" => TopicName.BackToBatteryVoltage_Inverter1,
            "solar_assistant/inverter_1/serial_number/state" => TopicName.SerialNumber_Inverter1,
            "solar_assistant/inverter_1/power_saving/state" => TopicName.PowerSaving_Inverter1,
            "solar_assistant/battery_1/current/state" => TopicName.Current_Battery1,
            "solar_assistant/battery_1/state_of_charge/state" => TopicName.StateOfCharge_Battery1,
            "solar_assistant/battery_1/voltage/state" => TopicName.Voltage_Battery1,
            "solar_assistant/battery_1/power/state" => TopicName.Power_Battery1,
            _ => throw new ArgumentOutOfRangeException($"The topic name {topic} has no match. Please update the TopicName.cs enum and TopicNameHelper.GetTopicName method to support reading this new topic.")
        };
    }

    public static string GetTopicFromTopicName(TopicName topicName)
    {
        return topicName switch
        {
            TopicName.BatteryPower_Total => "solar_assistant/total/battery_power/state",
            TopicName.BatteryStateOfCharge_Total => "solar_assistant/total/battery_state_of_charge/state",
            TopicName.BatteryEnergyIn_Total => "solar_assistant/total/battery_energy_in/state",
            TopicName.BatteryEnergyOut_Total => "solar_assistant/total/battery_energy_out/state",
            TopicName.BusVoltage_Total => "solar_assistant/total/bus_voltage/state",
            TopicName.GridEnergyIn_Total => "solar_assistant/total/grid_energy_in/state",
            TopicName.GridEnergyOut_Total => "solar_assistant/total/grid_energy_out/state",
            TopicName.LoadEnergy_Total => "solar_assistant/total/load_energy/state",
            TopicName.PvEnergy_Total => "solar_assistant/total/pv_energy/state",
            TopicName.GridFrequency_Inverter1 => "solar_assistant/inverter_1/grid_frequency/state",
            TopicName.PvCurrent1_Inverter1 => "solar_assistant/inverter_1/pv_current_1/state",
            TopicName.PvPower_Inverter1 => "solar_assistant/inverter_1/pv_power/state",
            TopicName.BatteryVoltage_Inverter1 => "solar_assistant/inverter_1/battery_voltage/state",
            TopicName.LoadApparentPower_Inverter1 => "solar_assistant/inverter_1/load_apparent_power/state",
            TopicName.PvCurrent2_Inverter1 => "solar_assistant/inverter_1/pv_current_2/state",
            TopicName.Temperature_Inverter1 => "solar_assistant/inverter_1/temperature/state",
            TopicName.LoadPercentage_Inverter1 => "solar_assistant/inverter_1/load_percentage/state",
            TopicName.BatteryCurrent_Inverter1 => "solar_assistant/inverter_1/battery_current/state",
            TopicName.GridPower_Inverter1 => "solar_assistant/inverter_1/grid_power/state",
            TopicName.PvVoltage1_Inverter1 => "solar_assistant/inverter_1/pv_voltage_1/state",
            TopicName.PvVoltage2_Inverter1 => "solar_assistant/inverter_1/pv_voltage_2/state",
            TopicName.PvPower1_Inverter1 => "solar_assistant/inverter_1/pv_power_1/state",
            TopicName.DeviceMode_Inverter1 => "solar_assistant/inverter_1/device_mode/state",
            TopicName.GridVoltage_Inverter1 => "solar_assistant/inverter_1/grid_voltage/state",
            TopicName.AcOutputFrequency_Inverter1 => "solar_assistant/inverter_1/ac_output_frequency/state",
            TopicName.AcOutputVoltage_Inverter1 => "solar_assistant/inverter_1/ac_output_voltage/state",
            TopicName.LoadPower_Inverter1 => "solar_assistant/inverter_1/load_power/state",
            TopicName.PvPower2_Inverter1 => "solar_assistant/inverter_1/pv_power_2/state",
            TopicName.ChargerSourcePriority_Inverter1 => "solar_assistant/inverter_1/charger_source_priority/state",
            TopicName.BatteryAbsorptionChargeVoltage_Inverter1 => "solar_assistant/inverter_1/battery_absorption_charge_voltage/state",
            TopicName.MaxChargeCurrent_Inverter1 => "solar_assistant/inverter_1/max_charge_current/state",
            TopicName.BatteryFloatChargeVoltage_Inverter1 => "solar_assistant/inverter_1/battery_float_charge_voltage/state",
            TopicName.MaxGridChargeCurrent_Inverter1 => "solar_assistant/inverter_1/max_grid_charge_current/state",
            TopicName.OutputSourcePriority_Inverter1 => "solar_assistant/inverter_1/output_source_priority/state",
            TopicName.ToGridBatteryVoltage_Inverter1 => "solar_assistant/inverter_1/to_grid_battery_voltage/state",
            TopicName.ShutdownBatteryVoltage_Inverter1 => "solar_assistant/inverter_1/shutdown_battery_voltage/state",
            TopicName.BackToBatteryVoltage_Inverter1 => "solar_assistant/inverter_1/back_to_battery_voltage/state",
            TopicName.SerialNumber_Inverter1 => "solar_assistant/inverter_1/serial_number/state",
            TopicName.PowerSaving_Inverter1 => "solar_assistant/inverter_1/power_saving/state",
            TopicName.Current_Battery1 => "solar_assistant/battery_1/current/state",
            TopicName.StateOfCharge_Battery1 => "solar_assistant/battery_1/state_of_charge/state",
            TopicName.Voltage_Battery1 => "solar_assistant/battery_1/voltage/state",
            TopicName.Power_Battery1 => "solar_assistant/battery_1/power/state",
            _ => throw new ArgumentOutOfRangeException($"The enum {topicName} has no known string match. Please update GetTopicFromTopicName method to support reading this new TopicName."),
        };
    }

    // example values
    // 
    // to_grid_battery_voltage
    // ******************************************************************* //
    // solar_assistant/total/battery_power/state: 65
    // solar_assistant/total/battery_state_of_charge/state: 100
    // solar_assistant/total/bus_voltage/state: 240.6
    // solar_assistant/inverter_1/grid_frequency/state: 60.01
    // solar_assistant/inverter_1/pv_current_1/state: 2.4
    // solar_assistant/inverter_1/pv_power/state: 165
    // solar_assistant/inverter_1/battery_voltage/state: 27.2
    // solar_assistant/inverter_1/load_apparent_power/state: 121
    // solar_assistant/inverter_1/pv_current_2/state: 0
    // solar_assistant/inverter_1/temperature/state: 98.4
    // solar_assistant/inverter_1/load_percentage/state: 4
    // solar_assistant/inverter_1/battery_current/state: 2.4
    // solar_assistant/inverter_1/grid_power/state: 0
    // solar_assistant/inverter_1/pv_voltage_1/state: 69.5
    // solar_assistant/inverter_1/pv_voltage_2/state: 0
    // solar_assistant/inverter_1/pv_power_1/state: 165
    // solar_assistant/inverter_1/device_mode/state: Solar/Battery
    // solar_assistant/inverter_1/grid_voltage/state: 119.4
    // solar_assistant/inverter_1/ac_output_frequency/state: 60
    // solar_assistant/inverter_1/ac_output_voltage/state: 120.1
    // solar_assistant/inverter_1/pv_power_2/state: 0
    // solar_assistant/inverter_1/load_power/state: 79
}
