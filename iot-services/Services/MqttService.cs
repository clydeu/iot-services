using MQTTnet;
using MQTTnet.Server;
using Serilog;
using ILogger = Serilog.ILogger;

namespace iot_services.Services
{
    internal class MqttService : IDisposable
    {
        private readonly MqttServer mqttServer;
        private readonly ILogger logger = Log.ForContext<MqttService>();
        private readonly MqttServerOptions mqttServerOptions;

        public MqttService()
        {
            var mqttFactory = new MqttFactory(new MqttNetSerilog());
            mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();
            bool.TryParse(Environment.GetEnvironmentVariable("IOT_MQTT_KEEP_ALIVE"), out var result);
            mqttServerOptions.DefaultEndpointOptions.KeepAlive = result;
            mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);
        }

        public void Dispose()
        {
            mqttServer.Dispose();
        }

        public async Task RunAsync()
        {
            await mqttServer.StartAsync();
            logger.Debug("MQTT server running on port {serverPort}", mqttServerOptions.DefaultEndpointOptions.Port);
        }

        public async Task PublishMotionDetected(string camera)
        {
            var message = new MqttApplicationMessageBuilder().WithTopic($"{camera}-motion-detected").WithPayload("1").Build();
            await mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message));
        }
    }
}