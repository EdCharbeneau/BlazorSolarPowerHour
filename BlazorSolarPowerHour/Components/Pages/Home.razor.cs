using BlazorSolarPowerHour.Models;
using BlazorSolarPowerHour.Services;
using CommonHelpers.Collections;
using Microsoft.AspNetCore.Components;
using Telerik.Blazor.Components;
using static BlazorSolarPowerHour.Models.MessageUtilities;

namespace BlazorSolarPowerHour.Components.Pages;

public partial class Home : IDisposable
{
    [Inject]
    public MessagesDbService DataService { get; set; } = default!;

    

    [Inject]
    Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; } = default!;

    TelerikArcGauge? BatteryLevelPercentageGauge { get; set; }
    TelerikLinearGauge? BatteryPowerGauge { get; set; }
    TelerikLinearGauge? BatteryLevelGauge { get; set; }
    TelerikTileLayout? TileLayoutInstance { get; set; }
    TelerikChart? SystemPowerChartRef { get; set; }

    // For charts:
    ObservableRangeCollection<ChartMqttDataItem> SolarPowerData { get; } = new() { MaximumCount = 120 };
    ObservableRangeCollection<ChartMqttDataItem> LoadPowerData { get; } = new() { MaximumCount = 120 };
    ObservableRangeCollection<ChartMqttDataItem> BatteryPowerData { get; } = new() { MaximumCount = 120 };
    ObservableRangeCollection<ChartMqttDataItem> GridPowerData { get; } = new() { MaximumCount = 120 };
    ObservableRangeCollection<ChartMqttDataItem> BatteryChargeData { get; } = new() { MaximumCount = 120 };

    readonly string localStorageKey = "tile-layout-state";
    string batteryPower = "";
    double batteryPowerValue;
    string batteryCharge = "";
    double batteryChargeValue;
    string pvPower = "";
    string gridPower = "";
    string loadPower = "";
    string inverterMode = "Solar/Battery/Grid";
    string chargerSourcePriority = "Solar";

    CancellationTokenSource? cts;
    int LoadDataInterval { get; set; } = 2000;
    bool IsTimerRunning { get; set; }

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
        // *** Step 1. Make a call to the database to get some data. *** //

        // Get the last X minutes of data
        var items = await DataService.GetMeasurementsAsync(DateTime.Now.AddMinutes(-2), DateTime.Now);


        // *** Step 2. Get the individual topics from the data *** //

        // energy being consumed by the house (string, no parsing needed)
        loadPower = items.Where(d => d.Topic == GetTopic(TopicName.LoadPower_Inverter1)).OrderBy(d => d.Timestamp).LastOrDefault()?.Value ?? "0";

        // solar panels' output power (string, no parsing needed)
        pvPower = items.Where(d => d.Topic == GetTopic(TopicName.PvPower_Inverter1)).OrderBy(d => d.Timestamp).LastOrDefault()?.Value ?? "0";

        // energy being consumed from the grid (string, no parsing needed)
        gridPower = items.Where(d => d.Topic == GetTopic(TopicName.GridPower_Inverter1)).OrderBy(d => d.Timestamp).LastOrDefault()?.Value ?? "0";

        // battery level in percentage (string, needs to be parsed for gauge render)
        batteryCharge = items.Where(d => d.Topic == GetTopic(TopicName.BatteryStateOfCharge_Total)).OrderBy(d => d.Timestamp).LastOrDefault()?.Value ?? "0";
        batteryChargeValue = Convert.ToDouble(batteryCharge);

        // power entering/leaving the batteries
        batteryPower = items.Where(d => d.Topic == GetTopic(TopicName.BatteryPower_Total)).OrderBy(d=>d.Timestamp).LastOrDefault()?.Value ?? "0";
        batteryPowerValue = Convert.ToDouble(batteryPower);

        // the mode of the inverter (which source is the priority)
        inverterMode = items.Where(d => d.Topic == GetTopic(TopicName.DeviceMode_Inverter1)).OrderBy(d => d.Timestamp).LastOrDefault()?.Value ?? "unknown";

        // the battery charging source priority (usually solar, but can be Grid when there's an incoming storm)
        chargerSourcePriority = items.Where(d => d.Topic == GetTopic(TopicName.ChargerSourcePriority_Inverter1)).OrderBy(d => d.Timestamp).LastOrDefault()?.Value ?? "unknown";


        // *** Step 3. Get some historical data for charts. *** //

        foreach (var item in items.Take(60))
        {
            var topicName = GetTopicName(item.Topic ?? "");
            switch (topicName)
            {
                case TopicName.LoadPower_Inverter1:
                    LoadPowerData.Add(new ChartMqttDataItem { Category = topicName, CurrentValue = Convert.ToDouble(item.Value), Timestamp = DateTime.Now });
                    break;
                case TopicName.PvPower_Inverter1:
                    SolarPowerData.Add(new ChartMqttDataItem { Category = topicName, CurrentValue = Convert.ToDouble(item.Value), Timestamp = DateTime.Now });
                    break;
                case TopicName.BatteryPower_Total:
                    BatteryPowerData.Add(new ChartMqttDataItem { Category = topicName, CurrentValue = Convert.ToDouble(item.Value), Timestamp = DateTime.Now });
                    break;
                case TopicName.GridPower_Inverter1:
                    GridPowerData.Add(new ChartMqttDataItem { Category = topicName, CurrentValue = Convert.ToDouble(item.Value), Timestamp = DateTime.Now });
                    break;
                case TopicName.BatteryStateOfCharge_Total:
                    BatteryChargeData.Add(new ChartMqttDataItem { Category = topicName, CurrentValue = Convert.ToDouble(item.Value), Timestamp = DateTime.Now });
                    break;
            }
        }
    }

    public void Dispose()
    {
        cts?.Cancel();
    }
}