using Blazored.LocalStorage;
using BlazorSolarPowerHour.Components.DashboardComponents;
using BlazorSolarPowerHour.Models;
using BlazorSolarPowerHour.Services;
using CommonHelpers.Collections;
using Microsoft.AspNetCore.Components;
using Telerik.Blazor.Components;

namespace BlazorSolarPowerHour.Components.Pages;

public partial class Home : IDisposable
{
    [Inject]
    MessagesDbService DataService { get; set; } = default!;
    
    [Inject]
    MqttTopicUtilities MqttTopicUtilities { get; set; } = default!;

    [Inject]
    IConfiguration Config { get; set; } = default!;

    [Inject]
    ILocalStorageService LocalStorage { get; set; } = default!;

    BatteryLiveGauges? BatteryLiveGauges { get; set; }
    TelerikLinearGauge? BatteryPowerGauge { get; set; }
    TelerikTileLayout? TileLayoutInstance { get; set; }
    TelerikChart? SystemPowerChartRef { get; set; }
    TelerikChart? BatteryPercentageChartRef { get; set; }

    // For charts:
    ObservableRangeCollection<ChartMqttDataItem> SolarPowerData { get; } = new() { MaximumCount = 120 };
    ObservableRangeCollection<ChartMqttDataItem> LoadPowerData { get; } = new() { MaximumCount = 120 };
    ObservableRangeCollection<ChartMqttDataItem> BatteryPowerData { get; } = new() { MaximumCount = 120 };
    ObservableRangeCollection<ChartMqttDataItem> GridPowerData { get; } = new() { MaximumCount = 120 };
    ObservableRangeCollection<ChartMqttDataItem> BatteryChargeData { get; } = new() { MaximumCount = 120 };

    static readonly List<string> TimeRanges = ["1 hour", "6 hours", "12 hours", "1 day", "7 days"];
    string ActiveTimeRange { get; set; } = TimeRanges[0];
    DateTime StartDateTime { get; set; } = DateTime.Now.AddHours(-1);
    DateTime EndDateTime { get; set; } = DateTime.Now;

    readonly string localStorageKey = "tile-layout-state";
    string batteryPower = "";
    string batteryCharge = "";
    string pvPower = "";
    string gridPower = "";
    string loadPower = "";
    string inverterMode = "Solar/Battery/Grid";
    string chargerSourcePriority = "Solar";
    double batteryChargeValue;
    double batteryPowerValue;

    string batteryVoltage = "0";
    string backToBatteryVoltage = "0";
    string pvVoltage = "0";
    string pv1Voltage = "0";
    string pvCurrent = "0";
    string pv1Current = "0";
    string busVoltage = "0";
    string outputVoltage = "0";

    string outputFrequency = "0";
    string gridFrequency = "0";

    CancellationTokenSource? cts;
    int LoadDataInterval { get; set; } = 2000;
    bool IsTimerRunning { get; set; }

    protected override Task OnInitializedAsync() => GetValues(); // Fire first GetValues before relying on timers

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadState();

            cts = new CancellationTokenSource();

            IsTimerRunning = true;

            await IntervalDataUpdate();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    async Task ToggleTimer()
    {
        if (!IsTimerRunning)
        {
            if (cts?.Token == null)
            {
                cts = new CancellationTokenSource();

                await IntervalDataUpdate();
            }
        }
        else
        {
            if (cts != null)
                await cts.CancelAsync();
        }
    }

    async Task IntervalDataUpdate()
    {
        while (cts?.Token != null)
        {
            await Task.Delay(LoadDataInterval, cts.Token);

            await GetValues();

            StateHasChanged();
        }
    }

    async Task GetValues()
    {
        // Get the range of data based on the time range selection (1,6,12, or 24 hours)
        var items = await DataService.GetMeasurementsAsync(StartDateTime, EndDateTime);

        // Get the individual topics from the data, separating topics by inverter model

        loadPower = MqttTopicUtilities.GetNewestValue(items, TopicName.LoadPower_Inverter1, "0");
        pvPower = MqttTopicUtilities.GetNewestValue(items, TopicName.PvPower_Inverter1, "0");
        gridPower = MqttTopicUtilities.GetNewestValue(items, TopicName.GridPower_Inverter1, "0");
        batteryPower = MqttTopicUtilities.GetNewestValue(items, TopicName.BatteryPower_Total, "0");
        batteryCharge = MqttTopicUtilities.GetNewestValue(items, TopicName.BatteryStateOfCharge_Total, "0");
        inverterMode = MqttTopicUtilities.GetNewestValue(items, TopicName.DeviceMode_Inverter1, "unknown");
        chargerSourcePriority = MqttTopicUtilities.GetNewestValue(items, TopicName.ChargerSourcePriority_Inverter1, "unknown");

        batteryPowerValue = Convert.ToDouble(batteryPower);
        batteryChargeValue = Convert.ToDouble(batteryCharge);

        gridFrequency = MqttTopicUtilities.GetNewestValue(items, TopicName.GridFrequency_Inverter1, "0");
        outputFrequency = MqttTopicUtilities.GetNewestValue(items, TopicName.AcOutputFrequency_Inverter1, "0");

        outputVoltage = MqttTopicUtilities.GetNewestValue(items, TopicName.AcOutputVoltage_Inverter1, "0");
        batteryVoltage = MqttTopicUtilities.GetNewestValue(items, TopicName.BatteryVoltage_Inverter1, "0");
        backToBatteryVoltage = MqttTopicUtilities.GetNewestValue(items, TopicName.BackToBatteryVoltage_Inverter1, "0");

        busVoltage = MqttTopicUtilities.GetNewestValue(items, TopicName.BusVoltage_Total, "0");
        

        switch (Config["INVERTER_MANUFACTURER"]?.ToLower())
        {
            case "must":
                // MUST only topics
                pvVoltage = MqttTopicUtilities.GetNewestValue(items, TopicName.PvVoltage_Inverter1, "0");
                pvCurrent = MqttTopicUtilities.GetNewestValue(items, TopicName.PvCurrent_Inverter1, "0");
                break;
            case "growatt":
                // Growatt only topics
                pv1Voltage = MqttTopicUtilities.GetNewestValue(items, TopicName.PvVoltage1_Inverter1, "0");
                pv1Current = MqttTopicUtilities.GetNewestValue(items, TopicName.PvCurrent1_Inverter1, "0");
                break;
        }


        // Get some historical data for charts
        foreach (var item in items)
        {
            var topicName = MqttTopicUtilities.GetTopicName(item.Topic ?? "");

            switch (topicName)
            {
                case TopicName.LoadPower_Inverter1:
                    LoadPowerData.Add(new ChartMqttDataItem { Category = topicName, CurrentValue = Convert.ToDouble(item.Value), Timestamp = item.Timestamp });
                    break;
                case TopicName.PvPower_Inverter1:
                    SolarPowerData.Add(new ChartMqttDataItem { Category = topicName, CurrentValue = Convert.ToDouble(item.Value), Timestamp = item.Timestamp });
                    break;
                case TopicName.BatteryPower_Total:
                    BatteryPowerData.Add(new ChartMqttDataItem { Category = topicName, CurrentValue = Convert.ToDouble(item.Value), Timestamp = item.Timestamp });
                    break;
                case TopicName.GridPower_Inverter1:
                    GridPowerData.Add(new ChartMqttDataItem { Category = topicName, CurrentValue = Convert.ToDouble(item.Value), Timestamp = item.Timestamp });
                    break;
                case TopicName.BatteryStateOfCharge_Total:
                    BatteryChargeData.Add(new ChartMqttDataItem { Category = topicName, CurrentValue = Convert.ToDouble(item.Value), Timestamp = item.Timestamp });
                    break;
            }
        }
    }

    private async Task OnTimeRangeChanged(string newValue)
    {
        ActiveTimeRange = newValue;

        SolarPowerData.Clear();
        LoadPowerData.Clear();
        BatteryPowerData.Clear();
        GridPowerData.Clear();
        BatteryChargeData.Clear();

        StartDateTime = newValue switch
        {
            "1 hour" => DateTime.Now.AddHours(-1),
            "6 hours" => DateTime.Now.AddHours(-6),
            "12 hours" => DateTime.Now.AddHours(-12),
            "1 day" => DateTime.Now.AddDays(-1),
            "7 days" => DateTime.Now.AddDays(-7),
            _ => StartDateTime
        };

        // regardless if timer is running or not, we want to refresh the data
        await GetValues();
    }

    async Task ItemResize()
    {
        BatteryLiveGauges?.Refresh();
        BatteryPowerGauge?.Refresh();
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


    async Task ClearTileLayout()
    {
        TileLayoutItemState[] DefaultState = new[]
        {
            new TileLayoutItemState { Order = 0, ColSpan = 1, RowSpan = 1 },
            new TileLayoutItemState { Order = 1, ColSpan = 1, RowSpan = 1 },
            new TileLayoutItemState { Order = 2, ColSpan = 1, RowSpan = 1 },
            new TileLayoutItemState { Order = 3, ColSpan = 1, RowSpan = 1 },
            new TileLayoutItemState { Order = 4, ColSpan = 2, RowSpan = 1 },
            new TileLayoutItemState { Order = 5, ColSpan = 2, RowSpan = 1 }
        };
        await LocalStorage.RemoveItemAsync(localStorageKey);
        TileLayoutInstance?.SetState(new() { ItemStates= DefaultState });
    }

    public void Dispose()
    {
        cts?.Cancel();
    }
}