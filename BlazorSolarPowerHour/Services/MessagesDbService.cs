using BlazorSolarPowerHour.Models;
using Microsoft.EntityFrameworkCore;
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
        await dbContext.AddAsync(dataItem);

        await dbContext.SaveChangesAsync();

        return dataItem;
    }

    public async Task UpdateMeasurementAsync(MqttDataItem updatedItem)
    {
        // Optimized approach
        var originalItem = await dbContext.FindAsync<MqttDataItem>(updatedItem.Id);

        if (originalItem != null)
        {
            dbContext.Entry(originalItem).State = EntityState.Detached;

            dbContext.Update(updatedItem);
 
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteMeasurementAsync(MqttDataItem item)
    {
        var originalItem = await dbContext.FindAsync<MqttDataItem>(item.Id);

        if(originalItem == null)
        {
            dbContext.Remove(item);

            await dbContext.SaveChangesAsync();
        }
    }
}