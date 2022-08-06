using System.Collections.Concurrent;
using WeatherSimulator.Proto;

namespace WeatherSimulator.Client.Storage;

public static class ClientStorage
{
    public static readonly ConcurrentDictionary<string, List<SensorData>> SensorsData = new ();
}