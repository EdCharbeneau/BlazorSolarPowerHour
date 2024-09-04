using BlazorSolarPowerHour.Models;
using BlazorSolarPowerHour.Services;
using Microsoft.AspNetCore.Components;
using Telerik.Blazor.Components;
using static BlazorSolarPowerHour.Models.TopicHelper;

namespace BlazorSolarPowerHour.Components.Pages;

public partial class Home
{
    [Inject]
    public MessagesDbService DataService { get; set; } = default!;

    [Inject]
    Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; } = default!;

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

    // Instead of this array, MqttDataItem[] SolarData = [], use the chart object type, so we can bind it to a chart
    //ObservableRangeCollection<ChartMqttDataItem> SolarPowerData { get; } = new() { MaximumCount = 120 };
    //ObservableRangeCollection<ChartMqttDataItem> LoadPowerData { get; } = new() { MaximumCount = 120 };
    //ObservableRangeCollection<ChartMqttDataItem> BatteryPowerData { get; } = new() { MaximumCount = 120 };
    //ObservableRangeCollection<ChartMqttDataItem> GridPowerData { get; } = new() { MaximumCount = 120 };
    //ObservableRangeCollection<ChartMqttDataItem> BatteryChargeData { get; } = new() { MaximumCount = 120 };

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
        await LocalStorage.SetItemAsync(localStorageKey, state);
    }

    async Task LoadState()
    {
        TileLayoutState? state = await LocalStorage.GetItemAsync<TileLayoutState>(localStorageKey);
        if (state is null) return;
        TileLayoutInstance?.SetState(state);
    }

    protected override async Task OnInitializedAsync()
    {
        await GetValues();
    }

    private async Task GetValues()
    {
        // Step 1. Get the last 2 minutes of data from the database
        var x = await DataService.GetMeasurementsAsync(DateTime.Now.AddMinutes(-2), DateTime.Now);

        // Step 2. Get the individual topics from the data
        var pvPowerValues = x.Where(item => item.Topic == GetTopic(TopicName.PvEnergy_Total));
        var gridPowerValues = x.Where(item => item.Topic == GetTopic(TopicName.GridPower_Inverter1));
        var loadPowerValues = x.Where(item => item.Topic == GetTopic(TopicName.LoadPower_Inverter1));
        var batteryPowerValues = x.Where(item => item.Topic == GetTopic(TopicName.BatteryPower_Total));
        var batteryChargeLevelValues = x.Where(item => item.Topic == GetTopic(TopicName.StateOfCharge_Battery1));

        // Step 3. Get the most recent value to show the most recent "live" value
        pvPower = pvPowerValues.FirstOrDefault()?.Value ?? "-";
        gridPower = gridPowerValues.FirstOrDefault()?.Value ?? "-";
        loadPower = loadPowerValues.FirstOrDefault()?.Value ?? "-";
        batteryPower = batteryPowerValues.FirstOrDefault()?.Value ?? "-";

        batterChargeLevelPercent = double.Parse(batteryChargeLevelValues.FirstOrDefault()?.Value ?? "0");
        batteryPowerValue = double.Parse(batteryPower);


        // Step 4. Charts - Now we have the data from step #2
        // SolarPowerData = pvPowerValues.Select(item => new ChartMqttDataItem { CurrentValue = double.Parse(item.Value), Timestamp = item.Timestamp });
        // LoadPowerData = loadPowerValues.Select(item => new ChartMqttDataItem {  CurrentValue = double.Parse(item.Value), Timestamp = item.Timestamp });
        // BatteryPowerData = batteryPowerValues.Select(item => new ChartMqttDataItem {  CurrentValue = double.Parse(item.Value), Timestamp = item.Timestamp });
        // GridPowerData = gridPowerValues.Select(item => new ChartMqttDataItem {  CurrentValue = double.Parse(item.Value), Timestamp = item.Timestamp });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadState();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

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
