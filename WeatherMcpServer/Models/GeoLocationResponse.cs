using System.Text.Json.Serialization;

namespace WeatherMcpServer.Models;

/// <summary>
/// Represents a geographical location returned by the OpenWeatherMap Geocoding API
/// </summary>
public sealed record GeoLocationResponse(
    [property: JsonPropertyName("lat")] double Latitude,
    [property: JsonPropertyName("lon")] double Longitude
);