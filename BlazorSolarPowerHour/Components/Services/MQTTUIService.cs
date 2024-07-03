using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;

namespace BlazorSolarPowerHour.Components.Services;
public class MQTTUIService
{
    private readonly string mqttHost;
    private readonly string mqttPort;
    private MqttFactory mqttFactory = new();
    private IMqttClient? mqttClient;
    public MQTTUIService(IConfiguration config)
    {
        mqttHost = config["MqttHost"] ?? throw new InvalidOperationException("Set the MqttHost for the application in configuration.");
        mqttPort = config["MqttPort"] ?? "1883";
    }

    public async Task SetupMQTT(Func<MqttApplicationMessageReceivedEventArgs, Task> messageDelegate)
    {
        mqttClient = mqttFactory.CreateMqttClient();
        MqttClientOptions options = new MqttClientOptionsBuilder()
            .WithTcpServer(host: mqttHost, port: int.Parse(mqttPort))
            .Build();
        mqttClient.ApplicationMessageReceivedAsync += messageDelegate;
        await mqttClient.ConnectAsync(options);
    }

    public async Task SubscribeAsync()
    {
        var options = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(t => t.WithTopic("solar_assistant/#"))
            .Build();

        if (mqttClient is null) throw new InvalidOperationException("Client cannot be null when subscribing.");
            await mqttClient.SubscribeAsync(options, CancellationToken.None);
    }
    public async Task UnsubscribeAsync()
    {
        var options = mqttFactory.CreateUnsubscribeOptionsBuilder()
            .WithTopicFilter(new MqttTopicFilter() { Topic = "solar_assistant/#" })
            .Build();

        if (mqttClient is null) throw new InvalidOperationException("Client cannot be null when subscribing.");
        await mqttClient.UnsubscribeAsync(options, CancellationToken.None);
    }
}
