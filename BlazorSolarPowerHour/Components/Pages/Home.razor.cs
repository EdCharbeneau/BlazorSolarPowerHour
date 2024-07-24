using BlazorSolarPowerHour.Components.Services;
using MQTTnet.Client;
using System.Collections.ObjectModel;
using static BlazorSolarPowerHour.Components.Services.TopicNameHelper;

namespace BlazorSolarPowerHour.Components.Pages;
public partial class Home
{
    int UIupdateRate = 2; //TODO: Move to app settings
    string message = "Undefined";
    string batteryPower = "";
    double batteryPowerValue;
    string pvPower = "";
    string gridPower = "";
    string loadPower = "";
    ObservableCollection<string> LiveMessages = new();
    bool isSubscribed = false;
    DateTime lastRender = DateTime.Now;

    protected override async Task OnInitializedAsync()
    {
        await service.SetupMQTT(GotMessage);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        lastRender = DateTime.Now;
        base.OnAfterRender(firstRender);
    }

    async Task ToggleConnection(bool value)
    {
        isSubscribed = value;

        if (value)
        {
            await service.SubscribeAsync();
        }
        else
        {
            await service.UnsubscribeAsync();
        }
    }

    private async Task GotMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        Console.WriteLine(e.ApplicationMessage.Topic);
        var topicName = GetTopicName(e.ApplicationMessage.Topic);
        if (topicName == TopicName.StateOfCharge_Battery1)
        {
            // update UI with value of the battery's current power
            batteryPowerValue = double.Parse(e.ApplicationMessage.PayloadSegment.GetTopicValue());
        }
        if (topicName == TopicName.BatteryPower_Total)
        {
            // update UI with value of the battery's current power
            batteryPower = e.ApplicationMessage.PayloadSegment.GetTopicValue();
        }
        if (topicName == TopicName.PvPower1_Inverter1)
        {
            // update UI - solar panel's current power
            pvPower = e.ApplicationMessage.PayloadSegment.GetTopicValue();
        }
        if (topicName == TopicName.GridPower_Inverter1)
        {
            // update UI - grid's current power
            gridPower = e.ApplicationMessage.PayloadSegment.GetTopicValue();
        }
        if (topicName == TopicName.LoadPower_Inverter1)
        {
            // update UI - load's current power
            loadPower = e.ApplicationMessage.PayloadSegment.GetTopicValue();
            
        }

        //TODO: Fix dealy of first GotMessage render
        if ((DateTime.Now - lastRender).TotalSeconds > UIupdateRate)
        {
            await InvokeAsync(StateHasChanged);
        }
        LiveMessages.Add(e.ApplicationMessage.Topic);
    }
}
