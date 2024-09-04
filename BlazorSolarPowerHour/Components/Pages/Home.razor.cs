using BlazorSolarPowerHour.Components.Services;
using BlazorSolarPowerHour.Models;
using BlazorSolarPowerHour.Services;
using Microsoft.AspNetCore.Components;
using MQTTnet.Client;
using System.Collections.ObjectModel;
using Telerik.Blazor.Components;
using static BlazorSolarPowerHour.Components.Services.TopicNameHelper;
using System.Text;

namespace BlazorSolarPowerHour.Components.Pages;
public partial class Home
{
    [Inject]
    public MessagesDbService DataService { get; set; } = default!;

    [Inject]
    Blazored.LocalStorage.ILocalStorageService localStorage { get; set; } = default!;

    TelerikArcGauge? BatteryLevelPercentageGauge { get; set; }
    TelerikLinearGauge? BatteryPowerGauge { get; set; }
    TelerikLinearGauge? BatteryLevelGauge { get; set; }
    TelerikTileLayout? TileLayoutInstance { get; set; }

    string localStorageKey = "tile-layout-state";
    string message = "Undefined";
    string batteryPower = "";
    double batteryPowerValue;
    double batterChargeLevelPercent;
    string pvPower = "";
    string gridPower = "";
    string loadPower = "";
    MqttDataItem[] SolarData = [];

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
        await GetValues();
    }

    private async Task GetValues()
    {
        var x = await DataService.GetMeasurementsAsync(DateTime.Now.AddMinutes(-2), DateTime.Now);
        pvPower = x.FirstOrDefault(item => item?.Topic == TopicNameHelper.GetTopicFromTopicName(TopicName.PvEnergy_Total))?.Value?.ToString() ?? "-";
        batterChargeLevelPercent = double.Parse(x.FirstOrDefault(item => item?.Topic == TopicNameHelper.GetTopicFromTopicName(TopicName.StateOfCharge_Battery1))?.Value ?? "0");
        //pvPower = x.FirstOrDefault(item => item?.Topic == TopicNameHelper.GetTopicFromTopicName(TopicName.PvEnergy_Total))?.ToString() ?? "-";
        // gridPower = "";
        // loadPower = "";

         //if (topicName == TopicName.StateOfCharge_Battery1)
    //    {
    //        // update UI with value of the battery's current power
    //        batterChargeLevelPercent = double.Parse(decodedPayload);
    //    }
    //    if (topicName == TopicName.BatteryPower_Total)
    //    {
    //        // update UI with value of the battery's current power
    //        batteryPower = decodedPayload;
    //        batteryPowerValue = double.Parse(batteryPower);
    //    }
    //    if (topicName == TopicName.PvPower1_Inverter1)
    //    {
    //        // update UI - solar panel's current power
    //        pvPower = decodedPayload;
    //    }
    //    if (topicName == TopicName.GridPower_Inverter1)
    //    {
    //        // update UI - grid's current power
    //        gridPower = decodedPayload;
    //    }
    //    if (topicName == TopicName.LoadPower_Inverter1)
    //    {
    //        // update UI - load's current power
    //        loadPower = decodedPayload;
    //    }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadState();
        }
        base.OnAfterRender(firstRender);
    }


    //private async Task GotMessage(MqttApplicationMessageReceivedEventArgs e)
    //{
    //    Console.WriteLine(e.ApplicationMessage.Topic);

    //    // *************************************************************************** //
    //    // ******** Step 1 - Get the topic and payload data out of the message ******* //
    //    // *************************************************************************** //

    //    // Read the topic string. I return a strong type that we can easily work with it instead of crazy strings.
    //    var topicName = TopicNameHelper.GetTopicName(e.ApplicationMessage.Topic);

    //    // Read the payload. Important! It is in the form of an ArraySegment<byte>, so we need to convert to byte[], then to ASCII.
    //    var decodedPayload = Encoding.ASCII.GetString(e.ApplicationMessage.PayloadSegment.ToArray());

    //    var item = new MqttDataItem { Topic = e.ApplicationMessage.Topic, Value = decodedPayload, Timestamp = DateTime.Now };


    //    // *************************************************************************** //
    //    // ************ Step 2 - Store the item in the long term storage ************* //
    //    // *************************************************************************** //

    //    if (IsDatabaseEnabled)
    //    {
    //        // Save item to database
    //        await DataService.AddMeasurementAsync(item);
    //    }


    //    // *************************************************************************** //
    //    // ************* Step 3 - Now we can do something with that data ************* //
    //    // *************************************************************************** //

    //    if (topicName == TopicName.StateOfCharge_Battery1)
    //    {
    //        // update UI with value of the battery's current power
    //        batterChargeLevelPercent = double.Parse(decodedPayload);
    //    }
    //    if (topicName == TopicName.BatteryPower_Total)
    //    {
    //        // update UI with value of the battery's current power
    //        batteryPower = decodedPayload;
    //        batteryPowerValue = double.Parse(batteryPower);
    //    }
    //    if (topicName == TopicName.PvPower1_Inverter1)
    //    {
    //        // update UI - solar panel's current power
    //        pvPower = decodedPayload;
    //    }
    //    if (topicName == TopicName.GridPower_Inverter1)
    //    {
    //        // update UI - grid's current power
    //        gridPower = decodedPayload;
    //    }
    //    if (topicName == TopicName.LoadPower_Inverter1)
    //    {
    //        // update UI - load's current power
    //        loadPower = decodedPayload;
    //    }

    //    //TODO: Fix dealy of first GotMessage render
    //    if ((DateTime.Now - lastRender).TotalSeconds > UIupdateRate)
    //    {
    //        await InvokeAsync(StateHasChanged);
    //    }

    //    LiveMessages.Add(e.ApplicationMessage.Topic);
    //}



    // For future use
    // ObservableRangeCollection<ChartMqttDataItem> SolarPowerData { get; } = new(){ MaximumCount = 120 };
    // ObservableRangeCollection<ChartMqttDataItem> LoadPowerData { get; } = new(){ MaximumCount = 120 };
    // ObservableRangeCollection<ChartMqttDataItem> BatteryPowerData { get; } = new(){ MaximumCount = 120 };
    // ObservableRangeCollection<ChartMqttDataItem> GridPowerData { get; } = new(){ MaximumCount = 120 };
    // ObservableRangeCollection<ChartMqttDataItem> BatteryChargeData { get; } = new(){ MaximumCount = 120 };
    // double BatteryChargePercentage { get; set; } = 0;
    // string CurrentSolar { get; set; } = "0";
    // string CurrentLoad { get; set; } = "0";
    // string CurrentBatteryPowerTotal { get; set; } = "0";
    // string CurrentGridTotal { get; set; } = "0";
    // string CurrentInverterMode { get; set; } = "Solar/Battery/Grid";
    // string ChargerSourcePriority { get; set; } = "Solar";
}
