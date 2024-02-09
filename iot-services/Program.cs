using iot_services.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>
      loggingBuilder.AddSerilog(dispose: true));

builder.Services.AddSingleton<TempSensorInfluxDBService>();
builder.Services.AddSingleton<MqttService>();
builder.Services.AddSingleton(x =>
{
    var mqtt = x.GetService<MqttService>();
    return new MailService(mqtt.PublishMotionDetected);
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Configure the HTTP request pipeline.
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapControllers();

try
{
    var logger = Log.ForContext("SourceContext", "Program");
    using var mqttService = new MqttService();

    var services = new List<Task>
    {
        app.Services.GetService<MqttService>()?.RunAsync() ?? throw new SystemException("Mqtt Service cannot be resolved."),
        app.Services.GetService<MailService>()?.RunAsync() ?? throw new SystemException("Mail Service cannot be resolved."),
        app.RunAsync()
    };

    logger.Information("Starting services...");
    await Task.WhenAll(services);
}
finally
{
    Log.CloseAndFlush();
}