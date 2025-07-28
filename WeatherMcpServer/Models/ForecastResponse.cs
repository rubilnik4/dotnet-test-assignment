using System.Text.Json.Serialization;

namespace WeatherMcpServer.Models;

/// <summary>
/// Represents a forecast response containing a list of daily weather predictions
/// </summary>
public sealed record ForecastResponse(
    [property: JsonPropertyName("list")] IReadOnlyList<ForecastEntry> Forecast);