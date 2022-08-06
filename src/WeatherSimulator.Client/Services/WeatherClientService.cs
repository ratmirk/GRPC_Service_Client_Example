using System.Collections.Concurrent;
using Grpc.Core;
using Microsoft.Extensions.Options;
using WeatherSimulator.Client.Configuration;
using WeatherSimulator.Client.Storage;
using WeatherSimulator.Proto;

namespace WeatherSimulator.Client.Services;

public class WeatherClientService : BackgroundService
{
    private readonly ILogger<WeatherClientService> _logger;
    private readonly WeatherSimulatorService.WeatherSimulatorServiceClient _weatherClient;
    private readonly IOptionsMonitor<ClientOptions> _configuration;
    private ConcurrentBag<Guid> _subscriptions;

    public WeatherClientService(WeatherSimulatorService.WeatherSimulatorServiceClient weatherClient,
        IOptionsMonitor<ClientOptions> configuration, ILogger<WeatherClientService> logger)
    {
        _weatherClient = weatherClient;
        _configuration = configuration;
        _logger = logger;
        _subscriptions = new ConcurrentBag<Guid>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var stream = _weatherClient.GetSensorsStream(cancellationToken: stoppingToken);

        var responseTask = ReadResponse(stoppingToken, stream);
        var requestTask = RequestTask(stoppingToken, stream);

        await Task.WhenAny(requestTask, responseTask);
    }

    private async Task RequestTask(CancellationToken stoppingToken,
        AsyncDuplexStreamingCall<ToServerMessage, SensorData> stream)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = new ToServerMessage();
            message.SubscribeSensorsIds.AddRange(GetSensorIdsForSubscribe());
            message.UnsubscribeSensorsIds.Add(GetSensorIdsForUnsubscribe());
            UpdateSubscriptions();

            try
            {
                await stream.RequestStream.WriteAsync(message, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "Возникла ошибка {Message}", e.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ReadResponse(CancellationToken stoppingToken,
        AsyncDuplexStreamingCall<ToServerMessage, SensorData> stream)
    {
        await foreach (var sensorData in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (!ClientStorage.SensorsData.ContainsKey(sensorData.SensorId))
                ClientStorage.SensorsData.TryAdd(sensorData.SensorId, new ConcurrentBag<SensorData>());

            ClientStorage.SensorsData[sensorData.SensorId].Add(sensorData);
        }
    }

    private IEnumerable<string> GetSensorIdsForSubscribe()
    {
        return _configuration.CurrentValue.Sensors.Except(_subscriptions).Select(x => x.ToString());
    }

    private IEnumerable<string> GetSensorIdsForUnsubscribe()
    {
        return _subscriptions.Except(_configuration.CurrentValue.Sensors).Select(x => x.ToString());
    }

    private void UpdateSubscriptions()
    {
        _subscriptions.Clear();
        foreach (var sensor in _configuration.CurrentValue.Sensors) _subscriptions.Add(sensor);
    }
}