using System.Text;

namespace BlazorSolarPowerHour.Models;

public class MqttTopicUtilities(IConfiguration config)
{
    // the owner of the inverter might have a custom topic prefix, so we need to allow it to be set as a config var.
    private readonly string topicPrefix = config["MQTT_TOPIC_PREFIX"] ?? "solar_assistant";

    public string GetNewestValue(List<MqttDataItem> items, TopicName topic, string defaultValue)
        => items.Where(d => d.Topic == GetTopic(topic)).OrderByDescending(d => d.Timestamp).FirstOrDefault()?.Value ?? defaultValue;

    public TopicName GetTopicName(string topic)
    {
        if (topic == $"{topicPrefix}/total/battery_power/state") return TopicName.BatteryPower_Total;
        if (topic == $"{topicPrefix}/total/battery_state_of_charge/state") return TopicName.BatteryStateOfCharge_Total;
        if (topic == $"{topicPrefix}/total/battery_energy_in/state") return TopicName.BatteryEnergyIn_Total;
        if (topic == $"{topicPrefix}/total/battery_energy_out/state") return TopicName.BatteryEnergyOut_Total;
        if (topic == $"{topicPrefix}/total/bus_voltage/state") return TopicName.BusVoltage_Total;
        if (topic == $"{topicPrefix}/total/grid_energy_in/state") return TopicName.GridEnergyIn_Total;
        if (topic == $"{topicPrefix}/total/grid_energy_out/state") return TopicName.GridEnergyOut_Total;
        if (topic == $"{topicPrefix}/total/load_energy/state") return TopicName.LoadEnergy_Total;
        if (topic == $"{topicPrefix}/total/pv_energy/state") return TopicName.PvEnergy_Total;
        if (topic == $"{topicPrefix}/inverter_1/pv_current/state") return TopicName.PvCurrent_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/pv_current_1/state") return TopicName.PvCurrent1_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/pv_current_2/state") return TopicName.PvCurrent2_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/pv_voltage/state") return TopicName.PvVoltage_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/pv_voltage_1/state") return TopicName.PvVoltage1_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/pv_voltage_2/state") return TopicName.PvVoltage2_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/pv_power/state") return TopicName.PvPower_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/pv_power_1/state") return TopicName.PvPower1_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/pv_power_2/state") return TopicName.PvPower2_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/load_apparent_power/state") return TopicName.LoadApparentPower_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/temperature/state") return TopicName.Temperature_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/load_percentage/state") return TopicName.LoadPercentage_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/grid_power/state") return TopicName.GridPower_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/grid_frequency/state") return TopicName.GridFrequency_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/grid_voltage/state") return TopicName.GridVoltage_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/ac_output_frequency/state") return TopicName.AcOutputFrequency_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/ac_output_voltage/state") return TopicName.AcOutputVoltage_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/load_power/state") return TopicName.LoadPower_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/device_mode/state") return TopicName.DeviceMode_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/charger_source_priority/state") return TopicName.ChargerSourcePriority_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/output_source_priority/state") return TopicName.OutputSourcePriority_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/battery_voltage/state") return TopicName.BatteryVoltage_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/battery_absorption_charge_voltage/state") return TopicName.BatteryAbsorptionChargeVoltage_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/battery_current/state") return TopicName.BatteryCurrent_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/battery_float_charge_voltage/state") return TopicName.BatteryFloatChargeVoltage_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/shutdown_battery_voltage/state") return TopicName.ShutdownBatteryVoltage_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/back_to_battery_voltage/state") return TopicName.BackToBatteryVoltage_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/to_grid_battery_voltage/state") return TopicName.ToGridBatteryVoltage_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/max_grid_charge_current/state") return TopicName.MaxGridChargeCurrent_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/max_charge_current/state") return TopicName.MaxChargeCurrent_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/serial_number/state") return TopicName.SerialNumber_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/power_saving/state") return TopicName.PowerSaving_Inverter1;
        if (topic == $"{topicPrefix}/battery_1/current/state") return TopicName.Current_Battery1;
        if (topic == $"{topicPrefix}/battery_1/state_of_charge/state") return TopicName.StateOfCharge_Battery1;
        if (topic == $"{topicPrefix}/battery_1/voltage/state") return TopicName.Voltage_Battery1;
        if (topic == $"{topicPrefix}/battery_1/power/state") return TopicName.Power_Battery1;
        if (topic == $"{topicPrefix}/inverter_1/solar_power_priority/state") return TopicName.SolarPowerPriority;
        if (topic == $"{topicPrefix}/inverter_1/export_to_grid/state") return TopicName.ExportToGrid_Inverter1;
        if (topic == $"{topicPrefix}/inverter_1/max_discharge_current/state") return TopicName.MaxDischargeCurrent_Inverter1;

        throw new ArgumentOutOfRangeException(
            $"The topic name {topic} has no match. Please update the TopicName.cs enum and TopicNameHelper.GetTopicName method to support reading this new topic.");
    }

    public string GetTopic(TopicName topicName)
    {
        if (topicName == TopicName.BatteryPower_Total) return $"{topicPrefix}/total/battery_power/state";
        if (topicName == TopicName.BatteryStateOfCharge_Total) return $"{topicPrefix}/total/battery_state_of_charge/state";
        if (topicName == TopicName.BatteryEnergyIn_Total) return $"{topicPrefix}/total/battery_energy_in/state";
        if (topicName == TopicName.BatteryEnergyOut_Total) return $"{topicPrefix}/total/battery_energy_out/state";
        if (topicName == TopicName.BusVoltage_Total) return $"{topicPrefix}/total/bus_voltage/state";
        if (topicName == TopicName.GridEnergyIn_Total) return $"{topicPrefix}/total/grid_energy_in/state";
        if (topicName == TopicName.GridEnergyOut_Total) return $"{topicPrefix}/total/grid_energy_out/state";
        if (topicName == TopicName.LoadEnergy_Total) return $"{topicPrefix}/total/load_energy/state";
        if (topicName == TopicName.PvEnergy_Total) return $"{topicPrefix}/total/pv_energy/state";
        if (topicName == TopicName.GridFrequency_Inverter1) return $"{topicPrefix}/inverter_1/grid_frequency/state";
        if (topicName == TopicName.PvCurrent_Inverter1) return $"{topicPrefix}/inverter_1/pv_current/state";
        if (topicName == TopicName.PvCurrent1_Inverter1) return $"{topicPrefix}/inverter_1/pv_current_1/state";
        if (topicName == TopicName.PvCurrent2_Inverter1) return $"{topicPrefix}/inverter_1/pv_current_2/state";
        if (topicName == TopicName.PvVoltage_Inverter1) return $"{topicPrefix}/inverter_1/pv_voltage/state";
        if (topicName == TopicName.PvVoltage1_Inverter1) return $"{topicPrefix}/inverter_1/pv_voltage_1/state";
        if (topicName == TopicName.PvVoltage2_Inverter1) return $"{topicPrefix}/inverter_1/pv_voltage_2/state";
        if (topicName == TopicName.PvPower_Inverter1) return $"{topicPrefix}/inverter_1/pv_power/state";
        if (topicName == TopicName.PvPower1_Inverter1) return $"{topicPrefix}/inverter_1/pv_power_1/state";
        if (topicName == TopicName.BatteryVoltage_Inverter1) return $"{topicPrefix}/inverter_1/battery_voltage/state";
        if (topicName == TopicName.LoadApparentPower_Inverter1) return $"{topicPrefix}/inverter_1/load_apparent_power/state";
        if (topicName == TopicName.Temperature_Inverter1) return $"{topicPrefix}/inverter_1/temperature/state";
        if (topicName == TopicName.LoadPercentage_Inverter1) return $"{topicPrefix}/inverter_1/load_percentage/state";
        if (topicName == TopicName.BatteryCurrent_Inverter1) return $"{topicPrefix}/inverter_1/battery_current/state";
        if (topicName == TopicName.GridPower_Inverter1) return $"{topicPrefix}/inverter_1/grid_power/state";
        if (topicName == TopicName.DeviceMode_Inverter1) return $"{topicPrefix}/inverter_1/device_mode/state";
        if (topicName == TopicName.GridVoltage_Inverter1) return $"{topicPrefix}/inverter_1/grid_voltage/state";
        if (topicName == TopicName.AcOutputFrequency_Inverter1) return $"{topicPrefix}/inverter_1/ac_output_frequency/state";
        if (topicName == TopicName.AcOutputVoltage_Inverter1) return $"{topicPrefix}/inverter_1/ac_output_voltage/state";
        if (topicName == TopicName.LoadPower_Inverter1) return $"{topicPrefix}/inverter_1/load_power/state";
        if (topicName == TopicName.PvPower2_Inverter1) return $"{topicPrefix}/inverter_1/pv_power_2/state";
        if (topicName == TopicName.ChargerSourcePriority_Inverter1) return $"{topicPrefix}/inverter_1/charger_source_priority/state";
        if (topicName == TopicName.BatteryAbsorptionChargeVoltage_Inverter1) return $"{topicPrefix}/inverter_1/battery_absorption_charge_voltage/state";
        if (topicName == TopicName.MaxChargeCurrent_Inverter1) return $"{topicPrefix}/inverter_1/max_charge_current/state";
        if (topicName == TopicName.BatteryFloatChargeVoltage_Inverter1) return $"{topicPrefix}/inverter_1/battery_float_charge_voltage/state";
        if (topicName == TopicName.MaxGridChargeCurrent_Inverter1) return $"{topicPrefix}/inverter_1/max_grid_charge_current/state";
        if (topicName == TopicName.OutputSourcePriority_Inverter1) return $"{topicPrefix}/inverter_1/output_source_priority/state";
        if (topicName == TopicName.ToGridBatteryVoltage_Inverter1) return $"{topicPrefix}/inverter_1/to_grid_battery_voltage/state";
        if (topicName == TopicName.ShutdownBatteryVoltage_Inverter1) return $"{topicPrefix}/inverter_1/shutdown_battery_voltage/state";
        if (topicName == TopicName.BackToBatteryVoltage_Inverter1) return $"{topicPrefix}/inverter_1/back_to_battery_voltage/state";
        if (topicName == TopicName.SerialNumber_Inverter1) return $"{topicPrefix}/inverter_1/serial_number/state";
        if (topicName == TopicName.PowerSaving_Inverter1) return $"{topicPrefix}/inverter_1/power_saving/state";
        if (topicName == TopicName.Current_Battery1) return $"{topicPrefix}/battery_1/current/state";
        if (topicName == TopicName.StateOfCharge_Battery1) return $"{topicPrefix}/battery_1/state_of_charge/state";
        if (topicName == TopicName.Voltage_Battery1) return $"{topicPrefix}/battery_1/voltage/state";
        if (topicName == TopicName.Power_Battery1) return $"{topicPrefix}/battery_1/power/state";
        if (topicName == TopicName.ExportToGrid_Inverter1) return $"{topicPrefix}/inverter_1/export_to_grid/state";
        if (topicName == TopicName.SolarPowerPriority) return $"{topicPrefix}/inverter_1/solar_power_priority/state";
        if (topicName == TopicName.MaxDischargeCurrent_Inverter1) return $"{topicPrefix}/inverter_1/max_discharge_current/state";

        throw new ArgumentOutOfRangeException(
            $"The enum {topicName} has no known string match. Please update GetTopicFromTopicName method to support reading this new TopicName.");
    }
}