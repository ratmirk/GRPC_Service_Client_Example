using System.Collections.Concurrent;
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
        return await _weatherClient.GetSensorDataAsync(new SensorIdRequest {SensorId = id});
    }

    /// <summary> Получить сохраненные данные для конкретного датчика. </summary>
    /// <param name="id"> Идентификатор датчика</param>
    /// <param name="count">
    /// Последние N штук.
    /// Если параметр равен 0, то возвращаем всю историю.
    /// </param>
    [HttpGet]
    [Route("history")]
    public Task<List<SensorData>> GetHistory(string id, int count = 0)
    {
        ClientStorage.SensorsData.TryGetValue(id, out var history);
        var historyList = history?.ToList();

        if (historyList is null)
            return Task.FromResult(new List<SensorData>());

        if (count > 0 && historyList.Count > count)
            return Task.FromResult(historyList.Skip(historyList.Count - count).ToList());

        return Task.FromResult(historyList);
    }
}