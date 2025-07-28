using System.Text.Json.Serialization;

namespace WeatherMcpServer.Models;

/// <summary>
/// Represents the response from the OpenWeatherMap "current weather" API
/// </summary>
public sealed record WeatherResponse(
    [property: JsonPropertyName("main")] WeatherMainResponse Main,
    [property: JsonPropertyName("weather")] IReadOnlyList<WeatherDescription> Description
);


