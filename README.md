# Blazor SOLAR Power Hour

This is the source code for the project the Blazor Solar Power Hour team, [@EdCharbeneau](https://github.com/EdCharbeneau), [@LanceMcCarthy](https://github.com/LanceMcCarthy) and [@JustinR34](https://github.com/JustinR34), built over the summer of 2024 while streaming on [Twitch.tv/CodeItLive](https://www.twitch.tv/codeitlive).

![image](https://github.com/user-attachments/assets/d4d9d634-d650-475c-9df5-8a439004cdc2)


## Highlights

Here are just a few of the highlights and takeaways:

- **MQTT** - The project has a true scope-less background service that connects to an MQTT broker. The broker publishes topics from a SolarAssistant installation's inverter. See the [Services/MqttService.cs](BlazorSolarPowerHour/Services/MqttService.cs) class.
- **EntityFramework** - We need to store data, the app curently uses a local Sqllite database, but it can be easily swapped out for any data system that EntityFramerwork supports. See [Services/MessagesDbService.cs](BlazorSolarPowerHour/Services/MessagesDbService.cs) class.
- **Telerik UI for Blazor** - Now, we need a beautiful UI, none other than [Telerik's Blazor components](https://www.telerik.com/blazor-ui) to the rescue. Not just UI controls, but also a bunch of little thngs that make writing a Blazor app more powerful and pleasurable experience. See the [Home](BlazorSolarPowerHour/Components/Pages/Home.razor) and [History](BlazorSolarPowerHour/Components/Pages/History.razor) razor pages.
