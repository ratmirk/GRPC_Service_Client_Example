using WeatherSimulator.Client.Configuration;
using WeatherSimulator.Client.Services;
using WeatherSimulator.Proto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Configuration.AddJsonFile("appsettings.json", false, true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ClientOptions>(builder.Configuration.GetSection("ClientOptions"));

// HostedServices.
builder.Services.AddHostedService<WeatherClientService>();

// Grpc.
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<WeatherSimulatorService.WeatherSimulatorServiceClient>((sp, x) =>
{
    var stringUri = builder.Configuration.GetValue<string>("ClientOptions:ServerUri");
    x.Address = new Uri(stringUri);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();