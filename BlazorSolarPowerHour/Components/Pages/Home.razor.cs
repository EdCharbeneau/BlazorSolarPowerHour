using BlazorSolarPowerHour.Models;
using BlazorSolarPowerHour.Services;
using CommonHelpers.Collections;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
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

    protected override Task OnInitializedAsync() => GetValues(); // Fire first GetValues before relying on timers

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


    async Task ClearTileLayout()
    {
        TileLayoutItemState[] DefaultState = new TileLayoutItemState[]
        {
            new TileLayoutItemState { ColSpan = 1, Order = 0, RowSpan = 1 },
            new TileLayoutItemState { ColSpan = 2, Order = 1, RowSpan = 1 },
            new TileLayoutItemState { ColSpan = 1, Order = 2, RowSpan = 1 },
            new TileLayoutItemState { ColSpan = 4, Order = 3, RowSpan = 1 }
        };
        await LocalStorage.RemoveItemAsync(localStorageKey);
        TileLayoutInstance?.SetState(new() { ItemStates= DefaultState });
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

        loadPower = items.GetNewestValue(TopicName.LoadPower_Inverter1, "0");
        pvPower = items.GetNewestValue(TopicName.PvPower_Inverter1, "0");
        gridPower = items.GetNewestValue(TopicName.GridPower_Inverter1, "0");
        batteryPower = items.GetNewestValue(TopicName.BatteryPower_Total, "0");
        batteryCharge = items.GetNewestValue(TopicName.BatteryStateOfCharge_Total, "0");
        inverterMode = items.GetNewestValue(TopicName.DeviceMode_Inverter1, "unknown");
        chargerSourcePriority = items.GetNewestValue(TopicName.ChargerSourcePriority_Inverter1, "unknown");

        batteryPowerValue = Convert.ToDouble(batteryPower);
        batteryChargeValue = Convert.ToDouble(batteryCharge);

        // *** Step 3. Get some historical data for charts. *** //

        foreach (var item in items.Take(60))
        {
            var topicName = GetTopicName(item.Topic ?? "");
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

    public void Dispose()
    {
        cts?.Cancel();
    }
}