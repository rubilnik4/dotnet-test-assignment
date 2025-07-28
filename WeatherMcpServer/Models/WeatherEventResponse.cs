using System.Text.Json.Serialization;

namespace WeatherMcpServer.Models;

/// <summary>
/// Represents a weather alert issued for a specific location
/// </summary>
public sealed record WeatherEventResponse(
    [property: JsonPropertyName("event")] string Event,
    [property: JsonPropertyName("description")] string Description
);