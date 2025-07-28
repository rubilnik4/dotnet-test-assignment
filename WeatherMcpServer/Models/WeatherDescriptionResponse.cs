using System.Text.Json.Serialization;

namespace WeatherMcpServer.Models;

/// <summary>
/// Contains a short text description of the weather condition
/// </summary>
public sealed record WeatherDescription(
    [property: JsonPropertyName("description")] string Description);