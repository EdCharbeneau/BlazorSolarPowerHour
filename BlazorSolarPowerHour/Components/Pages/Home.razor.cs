using BlazorSolarPowerHour.Components.Services;
using MQTTnet.Client;
using System.Collections.ObjectModel;
using Telerik.Blazor.Components;
using static BlazorSolarPowerHour.Components.Services.TopicNameHelper;

namespace BlazorSolarPowerHour.Components.Pages;
public partial class Home
{
    string localStorageKey = "tile-layout-state";
    int UIupdateRate = 2; //TODO: Move to app settings
    string message = "Undefined";
    string batteryPower = "";
    double batteryPowerValue;
    double batterChargeLevelPercent;
    string pvPower = "";
    string gridPower = "";
    string loadPower = "";
    ObservableCollection<string> LiveMessages = new();
    bool isSubscribed = false;
    DateTime lastRender = DateTime.Now;
    TelerikArcGauge? BatteryLevelPercentageGauge { get; set; }
    TelerikLinearGauge? BatteryPowerGauge { get; set; }
    TelerikLinearGauge? BatteryLevelGauge { get; set; }
    TelerikTileLayout? TileLayoutInstance { get; set; }
    async Task ItemResize()
    {
        BatteryLevelPercentageGauge?.Refresh();
        BatteryPowerGauge?.Refresh();
        BatteryLevelGauge?.Refresh();
       await SaveState();
    }

    async Task OnReorder()
    {
        await SaveState();
    }

    async Task SaveState()
    {
        TileLayoutState? state = TileLayoutInstance?.GetState();
        if (state is null) return;
        await localStorage.SetItemAsync(localStorageKey, state);
    }

    async Task LoadState()
    {
        TileLayoutState? state = await localStorage.GetItemAsync<TileLayoutState>(localStorageKey);
        if (state is null) return;
        TileLayoutInstance?.SetState(state);
    }

    protected override async Task OnInitializedAsync()
    {
        await service.SetupMQTT(GotMessage);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadState();
        }
        lastRender = DateTime.Now;
        base.OnAfterRender(firstRender);
    }

    async Task ToggleConnection()
    {
        isSubscribed = !isSubscribed;

        if (isSubscribed)
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
            batterChargeLevelPercent = double.Parse(e.ApplicationMessage.PayloadSegment.GetTopicValue());
        }
        if (topicName == TopicName.BatteryPower_Total)
        {
            // update UI with value of the battery's current power
            batteryPower = e.ApplicationMessage.PayloadSegment.GetTopicValue();
            batteryPowerValue = double.Parse(batteryPower);
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
