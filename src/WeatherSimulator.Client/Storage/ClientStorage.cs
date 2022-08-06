using System.Collections.Concurrent;
using WeatherSimulator.Proto;

namespace WeatherSimulator.Client.Storage;

public static class ClientStorage
{
    public static readonly ConcurrentDictionary<string, ConcurrentBag<SensorData>> SensorsData = new ();
}