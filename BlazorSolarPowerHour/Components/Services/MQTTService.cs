using BlazorSolarPowerHour.Models;
using BlazorSolarPowerHour.Services;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using System.Text;

namespace BlazorSolarPowerHour.Components.Services;
public class MQTTService : IAsyncDisposable
{
    private readonly string mqttHost;
    private readonly string mqttPort;
    private readonly MessagesDbService dbService;
    private MqttFactory mqttFactory = new();
    private IMqttClient? mqttClient;
    public bool IsSubscribed { get; set; }
    public MQTTService(IConfiguration config, MessagesDbService dbService)
    {
        mqttHost = config["MqttHost"] ?? throw new InvalidOperationException("Set the MqttHost for the application in configuration.");
        mqttPort = config["MqttPort"] ?? "1883";
        this.dbService = dbService;
    }

    public async Task StartAsync()
    {
        mqttClient = mqttFactory.CreateMqttClient();
        MqttClientOptions options = new MqttClientOptionsBuilder()
            .WithTcpServer(host: mqttHost, port: int.Parse(mqttPort))
            .Build();
        mqttClient.ApplicationMessageReceivedAsync += GotMessage;
        await mqttClient.ConnectAsync(options);

        var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(t => t.WithTopic("solar_assistant/#"))
            .Build();

        if (mqttClient is null) throw new InvalidOperationException("Client cannot be null when subscribing.");
        await mqttClient.SubscribeAsync(subscribeOptions, CancellationToken.None);
        IsSubscribed = true;
    }

    //public async Task SetupMQTT(Func<MqttApplicationMessageReceivedEventArgs, Task> messageDelegate)
    //{
    //    mqttClient = mqttFactory.CreateMqttClient();
    //    MqttClientOptions options = new MqttClientOptionsBuilder()
    //        .WithTcpServer(host: mqttHost, port: int.Parse(mqttPort))
    //        .Build();
    //    mqttClient.ApplicationMessageReceivedAsync += messageDelegate;
    //    await mqttClient.ConnectAsync(options);
    //}

    //public async Task SubscribeAsync()
    //{

        
    //}
    public async Task UnsubscribeAsync()
    {
        var options = mqttFactory.CreateUnsubscribeOptionsBuilder()
            .WithTopicFilter(new MqttTopicFilter() { Topic = "solar_assistant/#" })
            .Build();

        if (mqttClient is null) throw new InvalidOperationException("Client cannot be null when subscribing.");
        await mqttClient.UnsubscribeAsync(options, CancellationToken.None);
    }

    private async Task GotMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        Console.WriteLine(e.ApplicationMessage.Topic);

        // Read the topic string. I return a strong type that we can easily work with it instead of crazy strings.
        var topicName = TopicNameHelper.GetTopicName(e.ApplicationMessage.Topic);

        // Read the payload. Important! It is in the form of an ArraySegment<byte>, so we need to convert to byte[], then to ASCII.
        var decodedPayload = Encoding.ASCII.GetString(e.ApplicationMessage.PayloadSegment.ToArray());

        var item = new MqttDataItem { Topic = e.ApplicationMessage.Topic, Value = decodedPayload, Timestamp = DateTime.Now };

        await dbService.AddMeasurementAsync(item);

    }

    public async ValueTask DisposeAsync()
    {
        if (IsSubscribed)
        {
            await UnsubscribeAsync();
        }
        mqttClient?.Dispose();
    }
}
