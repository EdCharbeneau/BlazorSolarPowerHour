using BlazorSolarPowerHour.Models;
using BlazorSolarPowerHour.Services;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using System.Text;

namespace BlazorSolarPowerHour.Components.Services;

public class MQTTService : IAsyncDisposable
{
    private readonly MessagesDbService dbService;
    private readonly string mqttHost;
    private readonly string mqttPort;
    private readonly MqttFactory? mqttFactory;
    private readonly IMqttClient? mqttClient;

    public MQTTService(IConfiguration config, MessagesDbService dbService)
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
            Value = Encoding.ASCII.GetString(e.ApplicationMessage.PayloadSegment.ToArray()), 
            Timestamp = DateTime.Now
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (IsSubscribed)
        {
            // Unsubscribe, be nice to the broker
            var options = mqttFactory?.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(new MqttTopicFilter() { Topic = "solar_assistant/#" })
                .Build();

            await mqttClient!.UnsubscribeAsync(options, CancellationToken.None);
        }

        mqttClient?.Dispose();
    }

    //public async Task SubscribeAsync()
    //{
    //    // Connect to the MQTT server

    //    var port = int.TryParse(mqttPort, out var portNumber) ? portNumber : 1883;
    //    var clientOptions = new MqttClientOptionsBuilder()
    //        .WithTcpServer(mqttHost, port)
    //        .Build();

    //    await mqttClient!.ConnectAsync(clientOptions, CancellationToken.None);

    //    // Subscribe to the topics
    //    var mqttSubscribeOptions = mqttFactory?.CreateSubscribeOptionsBuilder()
    //        .WithTopicFilter(f => { f.WithTopic("solar_assistant/#"); })
    //        .Build();

    //    await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
    //}

    //public async Task UnsubscribeAsync()
    //{
    //    var options = mqttFactory.CreateUnsubscribeOptionsBuilder()
    //        .WithTopicFilter(new MqttTopicFilter() { Topic = "solar_assistant/#" })
    //        .Build();

    //    if (mqttClient is null) throw new InvalidOperationException("Client cannot be null when subscribing.");
    //    await mqttClient.UnsubscribeAsync(options, CancellationToken.None);
    //}
}
