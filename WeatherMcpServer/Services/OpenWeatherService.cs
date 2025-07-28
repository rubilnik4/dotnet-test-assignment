using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherMcpServer.Models;
using WeatherMcpServer.Settings;

namespace WeatherMcpServer.Services;

/// <summary>
/// Provides access to current weather data, forecasts, and alerts using the OpenWeatherMap API
/// </summary>
public class OpenWeatherService(
    IOptions<OpenWeatherSettings> options,
    HttpClient httpClient,
    ILogger<OpenWeatherService> logger) : IWeatherService
{
    private readonly OpenWeatherSettings _settings = options.Value;

    private const string BaseUrl = "https://api.openweathermap.org";

    /// <summary>
    /// Gets the current weather for a specified location
    /// </summary>
    public async Task<WeatherResponse> GetCurrentWeather(string city, string? countryCode = null)
    {
        var location = GetLocationQuery(city, countryCode);
        var url = $"{BaseUrl}/data/2.5/weather?q={location}&appid={_settings.ApiKey}&units=metric&lang=en";

        logger.LogDebug("Calling GetCurrentWeather for city {City}, country {Country}", city, countryCode);

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var error = await GetErrorMessage(response);
            Fail($"Weather API returned {response.StatusCode} for '{city}' - {error}");
        }
            

        await using var stream = await response.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<WeatherResponse>(stream);
        if (data is null || data.Description.Count == 0)
            Fail($"Invalid or empty weather data for '{city}'");
        return data;
    }

    /// <summary>
    /// Gets the weather forecast for a specified location
    /// </summary>
    public async Task<ForecastResponse> GetWeatherForecast(string city, string? countryCode = null, int days = 3)
    {
        var location = GetLocationQuery(city, countryCode);
        var url = $"{BaseUrl}/data/2.5/forecast?q={location}&appid={_settings.ApiKey}&units=metric&lang=en";

        logger.LogDebug("Calling GetWeatherForecast for city {City}, country {Country}", city, countryCode);

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var error = await GetErrorMessage(response);
            Fail($"Forecast API returned {response.StatusCode} for '{city}' - {error}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<ForecastResponse>(stream);
        if (data is null || data.Forecast.Count == 0)
            Fail($"Malformed forecast data for '{city}'");

        var forecast = data.Forecast.Where((_, i) => i % 8 == 0).Take(days).ToList();
        return forecast.Count == 0
            ? new ForecastResponse(new List<ForecastEntry>())
            : new ForecastResponse(forecast);
    }

    /// <summary>
    /// Retrieves active weather alerts for a given location
    /// </summary>
    public async Task<WeatherAlertResponse> GetWeatherAlerts(string city, string? countryCode = null)
    {
        var geoLocation = await GetCoordinates(city, countryCode);

        logger.LogDebug("Calling GetWeatherAlerts for city {City}, country {Country}", city, countryCode);
        var alertUrl =
            $"{BaseUrl}/data/3.0/onecall?lat={geoLocation.Latitude}&lon={geoLocation.Longitude}&appid={_settings.ApiKey}" +
            $"&exclude=current,minutely,hourly,daily&units=metric&lang=en";
        var response = await httpClient.GetAsync(alertUrl);
        if (!response.IsSuccessStatusCode)
        {
            var error = await GetErrorMessage(response);
            Fail($"Weather alert API returned {response.StatusCode} for '{city}' - {error}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<WeatherAlertResponse>(stream);
        if (data?.Alerts is null)
            Fail($"No weather alerts for {city}.");

        return data.Alerts.Count == 0
            ? new WeatherAlertResponse(new List<WeatherEventResponse>())
            : data;
    }

    /// <summary>
    /// Resolves latitude and longitude coordinates for the specified city and optional country code
    /// </summary>
    private async Task<GeoLocationResponse> GetCoordinates(string city, string? countryCode = null)
    {
        var location = GetLocationQuery(city, countryCode);
        var url = $"{BaseUrl}/geo/1.0/direct?q={location}&limit=1&appid={_settings.ApiKey}";

        logger.LogDebug("Calling GetCoordinates for city {City}, country {Country}", city, countryCode);

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var error = await GetErrorMessage(response);
            Fail($"Coordinates API returned {response.StatusCode} for '{city}' - {error}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        var locations = await JsonSerializer.DeserializeAsync<List<GeoLocationResponse>>(stream);

        var geoLocation = locations?.FirstOrDefault();
        if (geoLocation is null)
            Fail($"Location '{location}' not found");

        return geoLocation;
    }

    private static async Task<string> GetErrorMessage(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        if (doc.RootElement.TryGetProperty("message", out var message))
            return message.GetString() ?? "Unknown error message";
        return "No message in error response";
    }

    [DoesNotReturn]
    private void Fail(string message)
    {
        logger.LogError("Weather service failure: {Message}", message);
        throw new InvalidOperationException(message);
    }

    private static string GetLocationQuery(string city, string? countryCode) =>
        !string.IsNullOrWhiteSpace(countryCode) ? $"{city},{countryCode}" : city;
}