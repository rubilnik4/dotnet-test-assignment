using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WeatherMcpServer.Services;

/// <summary>
/// Converts OpenWeatherMap's "dt_txt" date/time strings (e.g., "2025-07-28 12:00:00")
/// </summary>
public class OpenWeatherDateTimeConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        return DateTime.ParseExact(str!, Format, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}