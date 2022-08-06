namespace WeatherSimulator.Client.Configuration;

public class ClientOptions
{
    public string ServerUri { get; set; }

    public IEnumerable<Guid> Sensors { get; set; }
}