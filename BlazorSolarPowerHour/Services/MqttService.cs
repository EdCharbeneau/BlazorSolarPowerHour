using BlazorSolarPowerHour.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using static BlazorSolarPowerHour.Models.MessageUtilities;

namespace BlazorSolarPowerHour.Services;

public class MqttService : IAsyncDisposable
{
    private readonly MessagesDbService dbService;
    private readonly string mqttHost;
    private readonly string mqttPort;
    private readonly MqttFactory? mqttFactory;
    private readonly IMqttClient? mqttClient;

    public MqttService(IConfiguration config, MessagesDbService dbService)
    {
        this.dbService = dbService;

        mqttHost = config["MqttHost"] ?? throw new InvalidOperationException("Set the MqttHost for the application in configuration.");
        mqttPort = config["MqttPort"] ?? "1883";

        this.mqttFactory = new MqttFactory();
        this.mqttClient = mqttFactory?.CreateMqttClient();

        if(mqttClient == null)
            throw new NullReferenceException("The MQTT client could not be created.");

        this.mqttClient.ApplicationMessageReceivedAsync += GotMessage;
    }

    public bool IsSubscribed { get; set; }

    public async Task StartAsync()
    {
        // Connect to the MQTT server
        var port = int.TryParse(mqttPort, out var portNumber) ? portNumber : 1883;
        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(mqttHost, port)
            .Build();
        await mqttClient!.ConnectAsync(clientOptions, CancellationToken.None);

        // Subscribe to the topics
        var mqttSubscribeOptions = mqttFactory?.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => { f.WithTopic("solar_assistant/#"); })
            .Build();
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        // Public signal to let the rest of the application know we are subscribed.
        IsSubscribed = true;
    }

    private async Task GotMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        Console.WriteLine(e.ApplicationMessage.Topic);

        await dbService.AddMeasurementAsync(new MqttDataItem
        {
            Topic = e.ApplicationMessage.Topic, 
            Value = e.ApplicationMessage.PayloadSegment.GetTopicValue(), 
            Timestamp = DateTime.Now
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (IsSubscribed)
        {
            // Unsubscribe, be nice to the broker
            var options = mqttFactory?.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(new MqttTopicFilter { Topic = "solar_assistant/#" })
                .Build();

            await mqttClient!.UnsubscribeAsync(options, CancellationToken.None);

            IsSubscribed = false;
        }

        mqttClient?.Dispose();
    }
}


// *** For explanation during live stream *** //
// The service can now operate as a scopeless background task by implementing IHostedService
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services

//public class MqttService2(IConfiguration config, IServiceProvider serviceProvider) : BackgroundService, IAsyncDisposable
//{
//    // Service setup
//    private MqttFactory? mqttFactory;
//    private IMqttClient? mqttClient;
//    private readonly string mqttHost = config["MQTT_HOST"] ?? throw new NullReferenceException("A value for the MQTT_HOST environment variable must be set before starting the application.");
//    private readonly string mqttPort = config["MQTT_PORT"] ?? string.Empty;

//    // LIVE Values
//    public bool IsSubscribed { get; set; }
//    public double LiveBatteryChargePercentage { get; set; } = 0;
//    public string LiveSolar { get; set; } = "0";
//    public string LiveLoad { get; set; } = "0";
//    public string LiveBatteryPowerTotal { get; set; } = "0";
//    public string LiveGridTotal { get; set; } = "0";
//    public string LiveInverterMode { get; set; } = "Solar/Battery/Grid";
//    public string LiveSourcePriority { get; set; } = "Solar";

//    // This is required in order to get the scoped DbService in a BackgroundService (we cannot inject it in the CTOR because it is scoped)
//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        this.mqttFactory = new MqttFactory();
//        this.mqttClient = mqttFactory?.CreateMqttClient();

//        if(mqttClient == null)
//            throw new NullReferenceException("The MQTT client could not be created.");

//        // Add event handler for when a message comes in.
//        mqttClient.ApplicationMessageReceivedAsync += GotMessage;

//        // Connect to the MQTT server
//        var port = int.TryParse(mqttPort, out var portNumber) ? portNumber : 1883;
//        var clientOptions = new MqttClientOptionsBuilder().WithTcpServer(mqttHost, port).Build();
//        await mqttClient!.ConnectAsync(clientOptions, CancellationToken.None);

//        // start listening for messages
//        await mqttClient.SubscribeAsync(
//            mqttFactory?.CreateSubscribeOptionsBuilder()
//                .WithTopicFilter(f => { f.WithTopic("solar_assistant/#"); })
//                .Build(), 
//            CancellationToken.None);
//    }


//    private async Task GotMessage(MqttApplicationMessageReceivedEventArgs e)
//    {
//        // Get the value from the payload
//        var decodedPayload = e.ApplicationMessage.PayloadSegment.GetTopicValue();

//        // Update live values
//        await ProcessDataItem(decodedPayload, GetTopicName(e.ApplicationMessage.Topic));

//        // Important: Create a temporary scope in order to access the DbService and add the item.
//        using var scope = serviceProvider.CreateScope();
//        var dbService = scope.ServiceProvider.GetRequiredService<MessagesDbService>();
//        await dbService.AddMeasurementAsync(new MqttDataItem
//        {
//            Topic = e.ApplicationMessage.Topic,
//            Value = decodedPayload, Timestamp = DateTime.Now

//        });
//    }


//    // This is called by IHostedService
//    public override async Task StopAsync(CancellationToken cancellationToken)
//    {
//        if (IsSubscribed)
//        {
//            // Unsubscribe, be nice to the broker
//            var options = mqttFactory?.CreateUnsubscribeOptionsBuilder()
//                .WithTopicFilter(new MqttTopicFilter { Topic = "solar_assistant/#" })
//                .Build();

//            await mqttClient!.UnsubscribeAsync(options, CancellationToken.None);

//            IsSubscribed = false;
//        }
//    }

//    // Be a good dev
//    public async ValueTask DisposeAsync()
//    {
//        await StopAsync(CancellationToken.None);

//        mqttClient?.Dispose();
//    }

//    // for live values only
//    private Task ProcessDataItem(string decodedPayload, TopicName messageTopic)
//    {
//        var item = new ChartMqttDataItem { Category = messageTopic, Timestamp = DateTime.Now };

//        switch (messageTopic)
//        {
//            case TopicName.DeviceMode_Inverter1:
//                LiveInverterMode = decodedPayload;
//                break;

//            case TopicName.ChargerSourcePriority_Inverter1:
//                LiveSourcePriority = decodedPayload;
//                break;

//            case TopicName.LoadPower_Inverter1:
//                item.CurrentValue = Convert.ToDouble(decodedPayload);
//                LiveLoad = $"{item.CurrentValue}";
//                break;

//            case TopicName.PvPower_Inverter1:
//                item.CurrentValue = Convert.ToDouble(decodedPayload);
//                LiveSolar = $"{item.CurrentValue}";
//                break;

//            case TopicName.BatteryPower_Total:
//                item.CurrentValue = Convert.ToDouble(decodedPayload);
//                LiveBatteryPowerTotal = $"{item.CurrentValue}";
//                break;

//            case TopicName.GridPower_Inverter1:
//                item.CurrentValue = Convert.ToDouble(decodedPayload);
//                LiveGridTotal = $"{item.CurrentValue}";
//                break;

//            case TopicName.BatteryStateOfCharge_Total:
//                LiveBatteryChargePercentage = item.CurrentValue = Convert.ToDouble(decodedPayload);
//                break;
//            // Other topics available...
//            //case TopicName.PvEnergy_Total:
//            //case TopicName.LoadEnergy_Total:
//            //case TopicName.BatteryEnergyIn_Total:
//            //case TopicName.BatteryEnergyOut_Total:
//            //case TopicName.GridEnergyIn_Total:
//            //case TopicName.GridEnergyOut_Total:
//            //case TopicName.BusVoltage_Total:
//            //case TopicName.GridFrequency_Inverter1:
//            //case TopicName.PvCurrent1_Inverter1:
//            //case TopicName.BatteryVoltage_Inverter1:
//            //case TopicName.LoadApparentPower_Inverter1:
//            //case TopicName.PvCurrent2_Inverter1:
//            //case TopicName.Temperature_Inverter1:
//            //case TopicName.LoadPercentage_Inverter1:
//            //case TopicName.BatteryCurrent_Inverter1:
//            //case TopicName.PvVoltage1_Inverter1:
//            //case TopicName.PvVoltage2_Inverter1:
//            //case TopicName.PvPower1_Inverter1:
//            //case TopicName.GridVoltage_Inverter1:
//            //case TopicName.AcOutputFrequency_Inverter1:
//            //case TopicName.AcOutputVoltage_Inverter1:
//            //case TopicName.PvPower2_Inverter1:
//            //case TopicName.BatteryAbsorptionChargeVoltage_Inverter1:
//            //case TopicName.MaxChargeCurrent_Inverter1:
//            //case TopicName.BatteryFloatChargeVoltage_Inverter1:
//            //case TopicName.MaxGridChargeCurrent_Inverter1:
//            //case TopicName.OutputSourcePriority_Inverter1:
//            //case TopicName.ToGridBatteryVoltage_Inverter1:
//            //case TopicName.ShutdownBatteryVoltage_Inverter1:
//            //case TopicName.BackToBatteryVoltage_Inverter1:
//            //case TopicName.SerialNumber_Inverter1:
//            //case TopicName.PowerSaving_Inverter1:
//            //case TopicName.Current_Battery1:
//            //case TopicName.Voltage_Battery1:
//            //case TopicName.Power_Battery1:
//            //case TopicName.StateOfCharge_Battery1:
//            case TopicName.Unknown:
//            default:
//                break;
//        }

//        return Task.CompletedTask;
//    }
//}
