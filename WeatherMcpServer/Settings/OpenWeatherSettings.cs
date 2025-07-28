using System.ComponentModel.DataAnnotations;

namespace WeatherMcpServer.Settings;

/// <summary>
/// Configuration settings for accessing the OpenWeatherMap API.
/// </summary>
/// <param name="ApiKey">API key used to authenticate</param>
public sealed record OpenWeatherSettings(
    [property: Required] string ApiKey);