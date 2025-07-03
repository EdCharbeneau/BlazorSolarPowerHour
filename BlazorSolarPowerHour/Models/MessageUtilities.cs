using System.Buffers;
using System.Text;

namespace BlazorSolarPowerHour.Models;

    public static class MessageUtilities
    {
        public static string GetNewestValue(this List<MqttDataItem> items, TopicName topic, string defaultValue) 
            => items.Where(d => d.Topic == GetTopic(topic)).OrderByDescending(d => d.Timestamp).FirstOrDefault()?.Value ?? defaultValue;

        public static string GetTopicValue(this ReadOnlySequence<byte> bytes) 
            => Encoding.ASCII.GetString(bytes.ToArray());

        public static TopicName GetTopicName(string topic)
        {
            switch (topic)
            {
                case "solar_assistant/total/battery_power/state":
                    return TopicName.BatteryPower_Total;
                case "solar_assistant/total/battery_state_of_charge/state":
                    return TopicName.BatteryStateOfCharge_Total;
                case "solar_assistant/total/battery_energy_in/state":
                    return TopicName.BatteryEnergyIn_Total;
                case "solar_assistant/total/battery_energy_out/state":
                    return TopicName.BatteryEnergyOut_Total;
                case "solar_assistant/total/bus_voltage/state":
                    return TopicName.BusVoltage_Total;
                case "solar_assistant/total/grid_energy_in/state":
                    return TopicName.GridEnergyIn_Total;
                case "solar_assistant/total/grid_energy_out/state":
                    return TopicName.GridEnergyOut_Total;
                case "solar_assistant/total/load_energy/state":
                    return TopicName.LoadEnergy_Total;
                case "solar_assistant/total/pv_energy/state":
                    return TopicName.PvEnergy_Total;
                case "solar_assistant/inverter_1/grid_frequency/state":
                    return TopicName.GridFrequency_Inverter1;
                case "solar_assistant/inverter_1/pv_current_1/state":
                    return TopicName.PvCurrent1_Inverter1;
                case "solar_assistant/inverter_1/pv_power/state":
                    return TopicName.PvPower_Inverter1;
                case "solar_assistant/inverter_1/battery_voltage/state":
                    return TopicName.BatteryVoltage_Inverter1;
                case "solar_assistant/inverter_1/load_apparent_power/state":
                    return TopicName.LoadApparentPower_Inverter1;
                case "solar_assistant/inverter_1/pv_current_2/state":
                    return TopicName.PvCurrent2_Inverter1;
                case "solar_assistant/inverter_1/temperature/state":
                    return TopicName.Temperature_Inverter1;
                case "solar_assistant/inverter_1/load_percentage/state":
                    return TopicName.LoadPercentage_Inverter1;
                case "solar_assistant/inverter_1/battery_current/state":
                    return TopicName.BatteryCurrent_Inverter1;
                case "solar_assistant/inverter_1/grid_power/state":
                    return TopicName.GridPower_Inverter1;
                case "solar_assistant/inverter_1/grid_power_1/state":
                    return TopicName.GridPower1_Inverter1;
                case "solar_assistant/inverter_1/grid_power_2/state":
                    return TopicName.GridPower2_Inverter1;
                case "solar_assistant/inverter_1/pv_voltage_1/state":
                    return TopicName.PvVoltage1_Inverter1;
                case "solar_assistant/inverter_1/pv_voltage_2/state":
                    return TopicName.PvVoltage2_Inverter1;
                case "solar_assistant/inverter_1/pv_power_1/state":
                    return TopicName.PvPower1_Inverter1;
                case "solar_assistant/inverter_1/device_mode/state":
                    return TopicName.DeviceMode_Inverter1;
                case "solar_assistant/inverter_1/grid_voltage/state":
                    return TopicName.GridVoltage_Inverter1;
                case "solar_assistant/inverter_1/grid_voltage_1/state":
                    return TopicName.GridVoltage1_Inverter1;
                case "solar_assistant/inverter_1/grid_voltage_2/state":
                    return TopicName.GridVoltage2_Inverter1;
                case "solar_assistant/inverter_1/ac_output_frequency/state":
                    return TopicName.AcOutputFrequency_Inverter1;
                case "solar_assistant/inverter_1/ac_output_voltage/state":
                    return TopicName.AcOutputVoltage_Inverter1;
                case "solar_assistant/inverter_1/load_power/state":
                    return TopicName.LoadPower_Inverter1;
                case "solar_assistant/inverter_1/load_power_1/state":
                    return TopicName.LoadPower1_Inverter1;
                case "solar_assistant/inverter_1/load_power_2/state":
                    return TopicName.LoadPower2_Inverter1;
                case "solar_assistant/inverter_1/pv_power_2/state":
                    return TopicName.PvPower2_Inverter1;
                case "solar_assistant/inverter_1/charger_source_priority/state":
                    return TopicName.ChargerSourcePriority_Inverter1;
                case "solar_assistant/inverter_1/battery_absorption_charge_voltage/state":
                    return TopicName.BatteryAbsorptionChargeVoltage_Inverter1;
                case "solar_assistant/inverter_1/max_charge_current/state":
                    return TopicName.MaxChargeCurrent_Inverter1;
                case "solar_assistant/inverter_1/battery_float_charge_voltage/state":
                    return TopicName.BatteryFloatChargeVoltage_Inverter1;
                case "solar_assistant/inverter_1/max_grid_charge_current/state":
                    return TopicName.MaxGridChargeCurrent_Inverter1;
                case "solar_assistant/inverter_1/output_source_priority/state":
                    return TopicName.OutputSourcePriority_Inverter1;
                case "solar_assistant/inverter_1/to_grid_battery_voltage/state":
                    return TopicName.ToGridBatteryVoltage_Inverter1;
                case "solar_assistant/inverter_1/shutdown_battery_voltage/state":
                    return TopicName.ShutdownBatteryVoltage_Inverter1;
                case "solar_assistant/inverter_1/back_to_battery_voltage/state":
                    return TopicName.BackToBatteryVoltage_Inverter1;
                case "solar_assistant/inverter_1/serial_number/state":
                    return TopicName.SerialNumber_Inverter1;
                case "solar_assistant/inverter_1/power_saving/state":
                    return TopicName.PowerSaving_Inverter1;
                case "solar_assistant/battery_1/current/state":
                    return TopicName.Current_Battery1;
                case "solar_assistant/battery_1/state_of_charge/state":
                    return TopicName.StateOfCharge_Battery1;
                case "solar_assistant/battery_1/voltage/state":
                    return TopicName.Voltage_Battery1;
                case "solar_assistant/battery_1/power/state":
                    return TopicName.Power_Battery1;
                case "solar_assistant/inverter_1/solar_feed_to_grid/state":
                    return TopicName.SolarFeedToGrid_Inverter1;
                default:
                    Console.WriteLine($"The topic name {topic} has no match. Please update the TopicName.cs enum and TopicNameHelper.GetTopicName method to support reading this new topic.");
                    return TopicName.Unknown;
                //default:
                //    throw new ArgumentOutOfRangeException(
                //        $"The topic name {topic} has no match. Please update the TopicName.cs enum and TopicNameHelper.GetTopicName method to support reading this new topic.");
        }
        }

        public static string GetTopic(TopicName topicName)
        {
            switch (topicName)
            {
                case TopicName.BatteryPower_Total:
                    return "solar_assistant/total/battery_power/state";
                case TopicName.BatteryStateOfCharge_Total:
                    return "solar_assistant/total/battery_state_of_charge/state";
                case TopicName.BatteryEnergyIn_Total:
                    return "solar_assistant/total/battery_energy_in/state";
                case TopicName.BatteryEnergyOut_Total:
                    return "solar_assistant/total/battery_energy_out/state";
                case TopicName.BusVoltage_Total:
                    return "solar_assistant/total/bus_voltage/state";
                case TopicName.GridEnergyIn_Total:
                    return "solar_assistant/total/grid_energy_in/state";
                case TopicName.GridEnergyOut_Total:
                    return "solar_assistant/total/grid_energy_out/state";
                case TopicName.LoadEnergy_Total:
                    return "solar_assistant/total/load_energy/state";
                case TopicName.PvEnergy_Total:
                    return "solar_assistant/total/pv_energy/state";
                case TopicName.GridFrequency_Inverter1:
                    return "solar_assistant/inverter_1/grid_frequency/state";
                case TopicName.PvCurrent1_Inverter1:
                    return "solar_assistant/inverter_1/pv_current_1/state";
                case TopicName.PvPower_Inverter1:
                    return "solar_assistant/inverter_1/pv_power/state";
                case TopicName.BatteryVoltage_Inverter1:
                    return "solar_assistant/inverter_1/battery_voltage/state";
                case TopicName.LoadApparentPower_Inverter1:
                    return "solar_assistant/inverter_1/load_apparent_power/state";
                case TopicName.PvCurrent2_Inverter1:
                    return "solar_assistant/inverter_1/pv_current_2/state";
                case TopicName.Temperature_Inverter1:
                    return "solar_assistant/inverter_1/temperature/state";
                case TopicName.LoadPercentage_Inverter1:
                    return "solar_assistant/inverter_1/load_percentage/state";
                case TopicName.BatteryCurrent_Inverter1:
                    return "solar_assistant/inverter_1/battery_current/state";
                case TopicName.GridPower_Inverter1:
                    return "solar_assistant/inverter_1/grid_power/state";
                case TopicName.GridPower1_Inverter1:
                    return "solar_assistant/inverter_1/grid_power_1/state";
                case TopicName.GridPower2_Inverter1:
                    return "solar_assistant/inverter_1/grid_power_2/state";
                case TopicName.PvVoltage1_Inverter1:
                    return "solar_assistant/inverter_1/pv_voltage_1/state";
                case TopicName.PvVoltage2_Inverter1:
                    return "solar_assistant/inverter_1/pv_voltage_2/state";
                case TopicName.PvPower1_Inverter1:
                    return "solar_assistant/inverter_1/pv_power_1/state";
                case TopicName.DeviceMode_Inverter1:
                    return "solar_assistant/inverter_1/device_mode/state";
                case TopicName.GridVoltage_Inverter1:
                    return "solar_assistant/inverter_1/grid_voltage/state";
                case TopicName.GridVoltage1_Inverter1:
                    return "solar_assistant/inverter_1/grid_voltage_1/state";
                case TopicName.GridVoltage2_Inverter1:
                    return "solar_assistant/inverter_1/grid_voltage_2/state";
                case TopicName.AcOutputFrequency_Inverter1:
                    return "solar_assistant/inverter_1/ac_output_frequency/state";
                case TopicName.AcOutputVoltage_Inverter1:
                    return "solar_assistant/inverter_1/ac_output_voltage/state";
                case TopicName.LoadPower_Inverter1:
                    return "solar_assistant/inverter_1/load_power/state";
                case TopicName.LoadPower2_Inverter1:
                    return "solar_assistant/inverter_1/load_power_2/state";
                case TopicName.PvPower2_Inverter1:
                    return "solar_assistant/inverter_1/pv_power_2/state";
                case TopicName.ChargerSourcePriority_Inverter1:
                    return "solar_assistant/inverter_1/charger_source_priority/state";
                case TopicName.BatteryAbsorptionChargeVoltage_Inverter1:
                    return "solar_assistant/inverter_1/battery_absorption_charge_voltage/state";
                case TopicName.MaxChargeCurrent_Inverter1:
                    return "solar_assistant/inverter_1/max_charge_current/state";
                case TopicName.BatteryFloatChargeVoltage_Inverter1:
                    return "solar_assistant/inverter_1/battery_float_charge_voltage/state";
                case TopicName.MaxGridChargeCurrent_Inverter1:
                    return "solar_assistant/inverter_1/max_grid_charge_current/state";
                case TopicName.OutputSourcePriority_Inverter1:
                    return "solar_assistant/inverter_1/output_source_priority/state";
                case TopicName.ToGridBatteryVoltage_Inverter1:
                    return "solar_assistant/inverter_1/to_grid_battery_voltage/state";
                case TopicName.ShutdownBatteryVoltage_Inverter1:
                    return "solar_assistant/inverter_1/shutdown_battery_voltage/state";
                case TopicName.BackToBatteryVoltage_Inverter1:
                    return "solar_assistant/inverter_1/back_to_battery_voltage/state";
                case TopicName.SerialNumber_Inverter1:
                    return "solar_assistant/inverter_1/serial_number/state";
                case TopicName.PowerSaving_Inverter1:
                    return "solar_assistant/inverter_1/power_saving/state";
                case TopicName.Current_Battery1:
                    return "solar_assistant/battery_1/current/state";
                case TopicName.StateOfCharge_Battery1:
                    return "solar_assistant/battery_1/state_of_charge/state";
                case TopicName.Voltage_Battery1:
                    return "solar_assistant/battery_1/voltage/state";
                case TopicName.Power_Battery1:
                    return "solar_assistant/battery_1/power/state";
                case TopicName.SolarFeedToGrid_Inverter1:
                    return "solar_assistant/inverter_1/solar_feed_to_grid/state";
                case TopicName.Unknown:
                default:
                    Console.WriteLine($"The enum {topicName} has no known string match. Please update GetTopicFromTopicName method to support reading this new TopicName.");
                    return "";
                //default:
                //    throw new ArgumentOutOfRangeException(
                //        $"The enum {topicName} has no known string match. Please update GetTopicFromTopicName method to support reading this new TopicName.");
            }
        }
    }
