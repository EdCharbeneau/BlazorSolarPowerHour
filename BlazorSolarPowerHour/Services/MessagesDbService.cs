using BlazorSolarPowerHour.Models;
using Microsoft.EntityFrameworkCore;
using Telerik.Blazor.Data;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;
namespace BlazorSolarPowerHour.Services;

public class MessagesDbService(MeasurementsDbContext dbContext)
{
    public async Task<List<MqttDataItem>> GetAllMeasurementsAsync()
    {
        return await dbContext.Measurements.ToListAsync();
    }

    public async Task<List<MqttDataItem>> GetMeasurementsAsync(DateTime start, DateTime end)
    {
        return await dbContext.Measurements.Where(i => i.Timestamp > start && i.Timestamp < end).ToListAsync();
    }

    public async Task<DataSourceResult> GetMeasurementsRequestAsync(DateTime start, DateTime end, DataSourceRequest dataSourceRequest)
    {
        return await dbContext.Measurements
            .Where(i => i.Timestamp > start && i.Timestamp < end)
            .ToDataSourceResultAsync(dataSourceRequest);
    }

    public async Task<MqttDataItem> AddMeasurementAsync(MqttDataItem dataItem)
    {
        dbContext.Measurements.Add(dataItem);

        await dbContext.SaveChangesAsync();

        return dataItem;
    }

    public async Task<MqttDataItem> UpdateMeasurementAsync(MqttDataItem item)
    {
        var productExist = dbContext.Measurements.FirstOrDefault(p => p.Id == item.Id);

        if (productExist != null)
        {
            dbContext.Update(item);

            await dbContext.SaveChangesAsync();
        }

        return item;
    }

    public async Task DeleteMeasurementAsync(MqttDataItem item)
    {
        dbContext.Measurements.Remove(item);

        await dbContext.SaveChangesAsync();
    }
}