using System.Text.Json.Serialization;

namespace WeatherMcpServer.Models;

/// <summary>
/// Represents a forecast response containing a list of daily weather predictions
/// </summary>
public sealed record ForecastResponse(IReadOnlyList<ForecastEntry> Forecast);