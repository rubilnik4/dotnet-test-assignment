using System.ComponentModel.DataAnnotations;

namespace WeatherMcpServer.Settings;

/// <summary>
/// Configuration settings for accessing the OpenWeatherMap API.
/// </summary>
public sealed record OpenWeatherSettings
{
    [property: Required]
    public string ApiKey { get; init; } = string.Empty;
}