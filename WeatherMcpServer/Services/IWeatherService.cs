using System.Threading.Tasks;
using WeatherMcpServer.Models;

namespace WeatherMcpServer.Services
{
    /// <summary>
    /// Defines the contract for a weather service
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Gets the current weather for a specified location
        /// </summary>
        Task<WeatherResponse> GetCurrentWeather(string city, string? countryCode = null);

        /// <summary>
        /// Gets the weather forecast for a specified location
        /// </summary>
        Task<ForecastResponse> GetWeatherForecast(string city, string? countryCode = null, int days = 3);

        /// <summary>
        /// Gets weather alerts/warnings for a specified location
        /// </summary>
        Task<WeatherAlertResponse> GetWeatherAlerts(string city, string? countryCode = null);
    }
}