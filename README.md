# ☀️ Weather MCP Server

This is a Model Context Protocol (MCP) server that provides real-time weather data using the [OpenWeatherMap API](https://openweathermap.org/). It is implemented in .NET 8 using the MCP .NET server library.

## ✅ Features

* Get current weather for any city (`GetCurrentWeather`)
* Get 3-day weather forecast (`GetWeatherForecast`)
* Get active weather alerts (`GetWeatherAlerts`)
* Real API integration (no mocks)
* Runs as an MCP server (stdin transport)
* CI integration tests against real data via GitHub Actions

## 🛠 Requirements

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [OpenWeatherMap API Key](https://openweathermap.org/appid) (free tier works)

## 🔧 Configuration

The API key must be provided via an environment variable:

```bash
OpenWeather__ApiKey=your_actual_key_here
```

You can either:

* Set it globally:
  `export OpenWeather__ApiKey=...` (Linux/macOS)
  `set OpenWeather__ApiKey=...` (Windows)

* Or use `appsettings.json` locally:

```json
{
  "OpenWeather": {
    "ApiKey": "your_actual_key_here"
  }
}
```

## 🚀 Running the Server

You can run the server manually:

```bash
dotnet run --project WeatherMcpServer
```

Or test it with [MCP Inspector](https://github.com/modelcontextprotocol/inspector):

```bash
npx @modelcontextprotocol/inspector
```

Make sure the server is running and your API key is configured.

## 🧲 Running Integration Tests

Integration tests hit the real OpenWeather API using your key.

```bash
export OpenWeather__ApiKey=your_actual_key
dotnet test WeatherMcpIntegrationTest
```

Or via GitHub Actions:
API key must be configured as a secret: `OPENWEATHER_API_KEY`

## 📁 Project Structure

```
.
├── WeatherMcpServer             # Main MCP server project
├── WeatherMcpIntegrationTest   # Integration tests with real API
├── WeatherMcpServer.sln
└── README.md
```

## 📦 NuGet Packages Used

* `Microsoft.Extensions.AI.Server`
* `ModelContextProtocol`
* `Microsoft.Extensions.Http`
* `NUnit`

## 📘 Notes

* This server uses `IOptions<>` to bind settings.
* Errors from the OpenWeather API are logged and reported to the user.
* `GetWeatherAlerts` requires the "One Call by Call" plan from OpenWeather.

---

Made with ☁️ for FastMCP
