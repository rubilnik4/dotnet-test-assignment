using System.Text.Json.Serialization;
using WeatherMcpServer.Services;

namespace WeatherMcpServer.Models;

/// <summary>
/// Represents a single weather forecast entry for a specific date and time.
/// </summary>
public sealed record ForecastEntry(
    [property: JsonPropertyName("dt_txt")] 
    [property: JsonConverter(typeof(OpenWeatherDateTimeConverter))] 
    DateTime Date,
    
    [property: JsonPropertyName("main")] WeatherMainResponse Main,
    
    [property: JsonPropertyName("weather")] IReadOnlyList<WeatherDescription> Description
);