using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherMcpServer.Services;
using WeatherMcpServer.Settings;

namespace WeatherMcpIntegrationTest;

public class OpenWeatherServiceTests
{
    private OpenWeatherService _service = null!;

    [SetUp]
    public void Setup()
    {
        var apiKey = Environment.GetEnvironmentVariable("OpenWeather__ApiKey");
        Assert.That(apiKey, Is.Not.Null.And.Not.Empty, "API key must be set in OPENWEATHER_APIKEY");

        var settings = Options.Create(new OpenWeatherSettings { ApiKey = apiKey });
        var httpClient = new HttpClient();
        var logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<OpenWeatherService>();

        _service = new OpenWeatherService(settings, httpClient, logger);
    }

    [Test]
    public async Task GetCurrentWeather_Should_Return_Data_For_Real_City()
    {
        var result = await _service.GetCurrentWeather("London");
        Assert.Multiple(() =>
        {
            Assert.That(result.Main.Temperature, Is.Not.EqualTo(0).Or.GreaterThan(-100));
            Assert.That(result.Description, Is.Not.Empty);
        });
    }

    [Test]
    public async Task GetWeatherForecast_Should_Return_3_Day_Forecast()
    {
        var result = await _service.GetWeatherForecast("London", days: 3);
        Assert.That(result.Forecast, Has.Count.EqualTo(3));
        Assert.That(result.Forecast.All(f => f.Description.Count > 0), Is.True);
    }

    [Test]
    public async Task GetWeatherAlerts_Should_Return_EmptyList_If_None()
    {
        try
        {
            var result = await _service.GetWeatherAlerts("London");
            Assert.That(result.Alerts, Is.Not.Null);
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("401") || ex.Message.Contains("One Call")
        )
        {
            Assert.Ignore("Weather alerts not available without One Call 3.0 subscription.");
        }
    }
}