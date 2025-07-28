using System.Text.Json.Serialization;

namespace WeatherMcpServer.Models;

/// <summary>
/// Contains primary weather metrics such as temperature
/// </summary>
public sealed record WeatherMainResponse(
    [property: JsonPropertyName("temp")] decimal Temperature);