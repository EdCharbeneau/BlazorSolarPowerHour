using BlazorSolarPowerHour.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;

namespace BlazorSolarPowerHour.Services;

public class MqttService(IConfiguration config, IServiceProvider serviceProvider) : BackgroundService, IAsyncDisposable
{
    // Service setup
    private MqttFactory? mqttFactory;
    private IMqttClient? mqttClient;
    private readonly string mqttHost = config["MQTT_HOST"] ?? throw new NullReferenceException("A value for the MQTT_HOST environment variable must be set before starting the application.");
    private readonly string mqttPort = config["MQTT_PORT"] ?? string.Empty;

    // External acknowledgement of live connection
    public bool IsSubscribedToTopic { get; set; }

    // Event for detecting changes to MQTT subscription status
    public delegate Task SubscribedChanged();
    public event SubscribedChanged? SubscriptionChanged;

    // This is required in order to get the scoped DbService in a BackgroundService (we cannot inject it in the CTOR because it is scoped)
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.mqttFactory = new MqttFactory();
        this.mqttClient = mqttFactory?.CreateMqttClient();

        if (mqttClient == null)
            throw new NullReferenceException("The MQTT client could not be created.");

        // Add event handler for when a message comes in.
        mqttClient.ApplicationMessageReceivedAsync += GotMessage;

        // Connect to the MQTT server
        var port = int.TryParse(mqttPort, out var portNumber) ? portNumber : 1883;
        var clientOptions = new MqttClientOptionsBuilder().WithTcpServer(mqttHost, port).Build();
        await mqttClient!.ConnectAsync(clientOptions, CancellationToken.None);

        // start listening for messages
        await mqttClient.SubscribeAsync(
            mqttFactory?.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic("solar_assistant/#"); })
                .Build(),
            CancellationToken.None);

        IsSubscribedToTopic = true;
        SubscriptionChanged?.Invoke();
    }

    // This is called every time a new message comes in (once for each topic)
    private async Task GotMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        // Get the value from the payload
        var decodedPayload = e.ApplicationMessage.PayloadSegment.GetTopicValue();

        // Important: Create a temporary scope in order to access the DbService and add the item.
        using var scope = serviceProvider.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<MessagesDbService>();
        await dbService.AddMeasurementAsync(new MqttDataItem
        {
            Topic = e.ApplicationMessage.Topic,
            Value = decodedPayload,
            Timestamp = DateTime.Now
        });
    }


    // This is called by IHostedService
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (IsSubscribedToTopic)
        {
            // Unsubscribe, be nice to the broker
            var options = mqttFactory?.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(new MqttTopicFilter { Topic = "solar_assistant/#" })
                .Build();

            await mqttClient!.UnsubscribeAsync(options, CancellationToken.None);

            IsSubscribedToTopic = false;
            SubscriptionChanged?.Invoke();
        }
    }

    // Be a good dev
    public async ValueTask DisposeAsync()
    {
        await StopAsync(CancellationToken.None);

        mqttClient?.Dispose();
    }
}
