# Blazor SOLAR Power Hour

This is the source code for the project the Blazor Solar Power Hour team, [@EdCharbeneau](https://github.com/EdCharbeneau), [@LanceMcCarthy](https://github.com/LanceMcCarthy) and [@JustinR34](https://github.com/JustinR34), built over the summer of 2024 while streaming on [Twitch.tv/CodeItLive](https://www.twitch.tv/codeitlive).

| Workflow | Status |
|----------|--------|
| main | [![Main](https://github.com/EdCharbeneau/BlazorSolarPowerHour/actions/workflows/master.yml/badge.svg)](https://github.com/EdCharbeneau/BlazorSolarPowerHour/actions/workflows/master.yml) |
| release | [![Releases](https://github.com/EdCharbeneau/BlazorSolarPowerHour/actions/workflows/release.yml/badge.svg)](https://github.com/EdCharbeneau/BlazorSolarPowerHour/actions/workflows/release.yml) |

![image](https://github.com/user-attachments/assets/d4d9d634-d650-475c-9df5-8a439004cdc2)


## Highlights

Here are just a few of the highlights and takeaways:

- **MQTT** - The project has a true scope-less background service that connects to an MQTT broker. The broker publishes topics from a SolarAssistant installation's inverter. See the [Services/MqttService.cs](BlazorSolarPowerHour/Services/MqttService.cs) class.
- **EntityFramework** - We need to store data, the app curently uses a local Sqllite database, but it can be easily swapped out for any data system that EntityFramerwork supports. See [Services/MessagesDbService.cs](BlazorSolarPowerHour/Services/MessagesDbService.cs) class.
- **Telerik UI for Blazor** - Now, we need a beautiful UI, none other than [Telerik's Blazor components](https://www.telerik.com/blazor-ui) to the rescue. Not just UI controls, but also a bunch of little thngs that make writing a Blazor app more powerful and pleasurable experience. See the [Home](BlazorSolarPowerHour/Components/Pages/Home.razor) and [History](BlazorSolarPowerHour/Components/Pages/History.razor) razor pages.


## Deployment Options

Before running any of the available options, you need to know two pieces of information: 

- The MQTT Broker host - This can be either an IP address or a domain and is set using the `MQTT_HOST` environment variable.
- The MQTT service port - This the port which the MQTT service uses and is set using the `MQTT_PORT` environment variable. This is **1883** by default, but some domain-based servers just serve over 80 (http) or 443 (https).

### Docker

For quick deployments and fast upgrades, we recommend using the `ghcr.io/edcharbeneau/powerproduction:latest` docker image (has both `linux-arm64` and `linux-x64` support).

```
docker run -d -p 8080:8080 -e MQTT_HOST='broker.hivemq.com' -e MQTT_PORT='1883' ghcr.io/edcharbeneau/powerproduction:latest
```

> [!IMPORTANT]
> You must have 8080 as the container port, but you can use any port you prefer for the host port `-p <host port>:8080`.

### Docker Compose

This is similar to a docker CLI command, except everything is done for you based on yaml inside a `docker-compose.yml` file.

```yaml
version: '3'
services:
  app:
    image: 'ghcr.io/edcharbeneau/powerproduction:latest'
    restart: unless-stopped
    ports:
      - '8080:8080'  # host-port:container-port
    environment:
      - MQTT_HOST="broker.hivemq.com"  # Your-mqtt-server's ip-address or-domain name
      - MQTT_PORT=""  # This is 1883 by default, but some domain-based servers just serve over 80 (http) or 443 (https).
```

With that file in the current directory, all you need to do is run the following command:

`docker compose up -d`


### Azure | Windows | Windows Server

You will find a `<version>_net8.0-win-x64.zip` artifact attached to every GitHub Actions release workflow. You can host this build with IIS or any system that supports hosting .NET web applications.