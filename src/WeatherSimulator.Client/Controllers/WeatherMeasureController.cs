using System.Net;
using Microsoft.AspNetCore.Mvc;
using WeatherSimulator.Client.Exceptions;
using WeatherSimulator.Client.Storage;
using WeatherSimulator.Proto;

namespace WeatherSimulator.Client.Controllers;

[ApiController]
[Route("api/weather-measure")]
[AppExceptionFilter]
[ProducesResponseType(typeof(SensorData), (int)HttpStatusCode.OK)]
[ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
[ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
public class WeatherMeasureController : ControllerBase
{
    // TODO: Написать тесты
    private readonly WeatherSimulatorService.WeatherSimulatorServiceClient _weatherClient;

    public WeatherMeasureController(WeatherSimulatorService.WeatherSimulatorServiceClient weatherClient)
    {
        _weatherClient = weatherClient;
    }

    [HttpGet]
    [Route("sensor-data")]
    public async Task<SensorData> GetSensorData(string id)
    {
        ValidateId(id);

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
        ValidateId(id);

        ClientStorage.SensorsData.TryGetValue(id, out var history);
        var historyList = (history ?? throw new KeyNotFoundException("История для указанного Sensor Id не найдена")).ToList();

        if (count > 0 && historyList.Count > count)
            return Task.FromResult(historyList.Skip(historyList.Count - count).ToList());

        return Task.FromResult(historyList);
    }

    // TODO: Сделать нормальную валидацию, типа FluentValidation или другой вариант.
    private void ValidateId(string id)
    {
        if (!Guid.TryParse(id, out _))
            throw new ArgumentException("Некорректный SensorId");
    }
}