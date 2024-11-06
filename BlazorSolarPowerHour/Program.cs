using Blazored.LocalStorage;
using BlazorSolarPowerHour.Components;
using BlazorSolarPowerHour.Models;
using BlazorSolarPowerHour.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MeasurementsDbContext>(o => 
{
    o.UseSqlite("Data Source=NewMeasurements.db", b => b.MigrationsAssembly("BlazorSolarPowerHour")); 
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddTelerikBlazor();
builder.Services.AddBlazoredLocalStorage();

// Set up the database service as a normal scoped service
builder.Services.AddScoped<MessagesDbService>();

// Helper class that supports multiple inverter manufacturers
builder.Services.AddSingleton<MqttTopicUtilities>();

// Using MqttService as a background service https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services
builder.Services.AddSingleton<MqttService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<MqttService>());


var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<MeasurementsDbContext>();

    await dbContext.Database.EnsureCreatedAsync();

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        await dbContext.Database.MigrateAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
