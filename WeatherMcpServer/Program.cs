using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Settings;
using WeatherMcpServer.Tools;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<OpenWeatherSettings>()
    .Bind(builder.Configuration.GetSection("OpenWeather"))
    .ValidateDataAnnotations()
    .Validate(s => !string.IsNullOrWhiteSpace(s.ApiKey), "OpenWeather API key is required")
    .ValidateOnStart();

builder.Logging
    .AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Debug);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<WeatherTools>();

await builder.Build().RunAsync();