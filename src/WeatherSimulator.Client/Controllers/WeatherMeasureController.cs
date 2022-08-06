using Microsoft.AspNetCore.Mvc;
using WeatherSimulator.Client.Storage;
using WeatherSimulator.Proto;

namespace WeatherSimulator.Client.Controllers;

[ApiController]
[Route("api/weather-measure")]
public class WeatherMeasureController : ControllerBase
{
    private readonly WeatherSimulatorService.WeatherSimulatorServiceClient _weatherClient;

    public WeatherMeasureController(WeatherSimulatorService.WeatherSimulatorServiceClient weatherClient)
    {
        _weatherClient = weatherClient;
    }

    [HttpGet]
    [Route("sensor-data")]
    public async Task<SensorData> GetSensorData(string id)
    {
        var response = await _weatherClient.GetSensorDataAsync(new SensorInfo {SensorId = id});

        return response;
    }

    [HttpGet]
    [Route("history")]
    public Task<List<SensorData>> GetHistory(string id)
    {
        ClientStorage.SensorsData.TryGetValue(id, out var history);

        return Task.FromResult(history ?? new List<SensorData>());
    }
}