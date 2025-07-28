using System.Text.Json.Serialization;

namespace WeatherMcpServer.Models;

/// <summary>
/// Represents the alert section of OpenWeatherMap One Call API response.
/// </summary>
public sealed record WeatherAlertResponse(
    [property: JsonPropertyName("alerts")] IReadOnlyList<WeatherEventResponse>? Alerts
);