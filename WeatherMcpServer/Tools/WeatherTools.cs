using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using WeatherMcpServer.Services;

namespace WeatherMcpServer.Tools;

/// <summary>
/// Defines MCP-accessible tools for retrieving real-time weather data,
/// including current conditions, forecasts, and alerts.
/// </summary>
public sealed class WeatherTools(IWeatherService weatherService, ILogger<WeatherTools> logger)
{
    /// <summary>
    /// Retrieves the current weather conditions for the specified city using real-time data.
    /// </summary>
    [McpServerTool]
    [Description("Gets the current weather conditions for the specified city")]
    public async Task<string> GetCurrentWeather(
        [Description("The city name (e.g., 'London')")] string city,
        [Description("Optional country code (e.g., 'GB')")] string? countryCode = null)
    {
        logger.LogInformation("LLM called GetCurrentWeather: City = {City}, Country = {CountryCode}", 
            city, countryCode);
        try
        {
            var weather = await weatherService.GetCurrentWeather(city, countryCode);
            var temperature = weather.Main.Temperature;
            var description = weather.Description[0];

            return $"Current weather in {city}: {temperature}°C, {description}.";
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Weather API error in GetCurrentWeather");
            return $"Could not retrieve weather data for '{city}'.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in GetCurrentWeather for city: {City}", city);
            return $"Unexpected error while fetching weather for '{city}'.";
        }
    }
    
    /// <summary>
    /// Gets a multi-day weather forecast for a specified location via MCP.
    /// </summary>
    [McpServerTool]
    [Description("Gets a multi-day weather forecast for the specified city.")]
    public async Task<string> GetWeatherForecast(
        [Description("The city name (e.g., 'Berlin')")] string city,
        [Description("Optional country code (e.g., 'DE')")] string? countryCode = null,
        [Description("Number of days to include in the forecast (default is 3)")] int days = 3)
    {
        logger.LogInformation("LLM called GetWeatherForecast: City = {City}, Country = {CountryCode}, Days = {Days}", 
            city, countryCode, days);
        try
        {
            var forecast = await weatherService.GetWeatherForecast(city, countryCode, days);
            if (forecast.Forecast.Count == 0)
                return $"No forecast data available for '{city}'.";

            var formatted = forecast.Forecast
                .Select(entry => $"{entry.Date:yyyy-MM-dd}: {entry.Main.Temperature}°C, {entry.Description[0]}");
            return $"Forecast for {city}:\n" + string.Join("\n", formatted);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Weather API failure in GetWeatherForecast: {City}", city);
            return $"Could not retrieve forecast data for '{city}'";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in GetWeatherForecast: {City}", city);
            return $"Unexpected error occurred while fetching the forecast for '{city}'.";
        }
    }
    
    /// <summary>
    /// Retrieves active weather alerts for the specified city, if any are available.
    /// </summary>
    [McpServerTool]
    [Description("Gets active weather alerts (e.g., storms, floods) for the specified city.")]
    public async Task<string> GetWeatherAlerts(
        [Description("The city name (e.g., 'New York')")] string city,
        [Description("Optional country code (e.g., 'US')")] string? countryCode = null)
    {
        logger.LogInformation("LLM called GetWeatherAlerts: City = {City}, Country = {CountryCode}", city, countryCode);
        try
        {
            var alertResponse = await weatherService.GetWeatherAlerts(city, countryCode);

            if (alertResponse.Alerts is null || alertResponse.Alerts.Count == 0)
                return $"There are no active weather alerts for '{city}'.";

            var formatted = alertResponse.Alerts.Select(alert =>$"{alert.Event}, {alert.Description}");
            return $"Weather alerts for {city}:\n\n" + string.Join("\n\n", formatted);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Weather API failure in GetWeatherAlerts: {City}", city);
            return $"Could not retrieve weather alerts data for '{city}'";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Weather API failure in GetWeatherAlerts: {City}", city);
            return $"Weather alert service failed for '{city}'. Please try again later.";
        }
    }
}