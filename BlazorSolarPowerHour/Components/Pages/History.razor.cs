using BlazorSolarPowerHour.Models;
using BlazorSolarPowerHour.Services;
using Microsoft.AspNetCore.Components;
using Telerik.Blazor.Components;
using Telerik.DataSource;

namespace BlazorSolarPowerHour.Components.Pages;

public partial class History
{
    [Inject]
    public MessagesDbService DataService { get; set; } = default!;

    //private ObservableRangeCollection<MqttDataItem> Data { get; } = new();
    TelerikGrid<MqttDataItem>? Grid { get; set; }

    private List<MqttDataItem> Data;

    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-1);
    public DateTime EndDate { get; set; } = DateTime.Now;
    public int DebounceDelay { get; set; } = 333;

    // StartDate > dateChosen 
    // EndDate < dateChosen
    FilterDescriptor StartFilter() => new FilterDescriptor(nameof(MqttDataItem.Timestamp), FilterOperator.IsGreaterThan, StartDate);
    FilterDescriptor EndFilter() => new FilterDescriptor(nameof(MqttDataItem.Timestamp), FilterOperator.IsLessThan, EndDate);

    // private async void OnStartDateChange(object obj)
    // {
    //     await UpdateGridAsync();
    // }

    // private async void OnEndDateChange(object obj)
    // {
    //     await UpdateGridAsync();
    // }

    private async Task OnRead(GridReadEventArgs args)
    {
        DataSourceResult result = await DataService.GetMeasurementsRequestAsync(StartDate, EndDate, args.Request);
        args.Data = result.Data;
        args.Total = result.Total;
    }

    // private async Task UpdateGridAsync()
    // {
    //     Data.Clear();
    //     Data.AddRange(await DataService.GetMeasurementsAsync(StartDate, EndDate));
    // }

    void StartValueChangedHandler(DateTime currStart)
    {
        //you have to update the model manually because handling the <Parameter>Changed event does not let you use @bind-<Parameter>
        //not updating the model will effectively cancel the event
        StartDate = currStart;

        //Console.WriteLine($"start changed to: {currStart}");
    }

    async Task EndValueChangedHandler(DateTime currEnd)
    {
        // you have to update the model manually because handling the <Parameter>Changed event does not let you use @bind-<Parameter>
        // not updating the model will effectively cancel the event
        EndDate = currEnd;

        // sample check to execute logic only after the user has selected both ends of the range
        // if this does not pass, the user has only clicked once in the calendar popup
        if (currEnd != default(DateTime))
        {
            var state = Grid?.GetState();
            if (state is not null)
            {
                // clear TransactionDate filters
                state.FilterDescriptors = state.FilterDescriptors.Where(f => (f as FilterDescriptor)?.Member != nameof(MqttDataItem.Timestamp)).ToList();
                // assign new TransactionDate filters
                state.FilterDescriptors.Add(StartFilter());
                state.FilterDescriptors.Add(EndFilter());
                await Grid?.SetStateAsync(state)!;
            }
        }
    }
}
