using Blazored.LocalStorage;
using BlazorSolarPowerHour.Components;
using BlazorSolarPowerHour.Models;
using BlazorSolarPowerHour.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MeasurementsDbContext>(o => 
{
    o.UseSqlite("Data Source=Measurements.db", b => b.MigrationsAssembly("BlazorSolarPowerHour")); 
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddTelerikBlazor();
builder.Services.AddBlazoredLocalStorage();

// Using MqttService as a background service https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services
builder.Services.AddScoped<MessagesDbService>();
builder.Services.AddHostedService<MqttService>();


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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
