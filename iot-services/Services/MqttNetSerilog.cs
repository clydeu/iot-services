using MQTTnet.Diagnostics;
using Serilog;
using ILogger = Serilog.ILogger;

namespace iot_services.Services
{
    internal class MqttNetSerilog : IMqttNetLogger
    {
        private readonly ILogger logger = Log.ForContext("SourceContext", "MqttNet");
        public bool IsEnabled => true;

        public void Publish(MqttNetLogLevel logLevel, string source, string message, object[] parameters, Exception exception)
        {
            switch (logLevel)
            {
                case MqttNetLogLevel.Verbose:
                    logger.Debug($"{source}: {message}", parameters);
                    break;

                case MqttNetLogLevel.Info:
                    logger.Information($"{source}: {message}", parameters);
                    break;

                case MqttNetLogLevel.Warning:
                    logger.Warning(exception, $"{source}: {message}", parameters);
                    break;

                case MqttNetLogLevel.Error:
                    logger.Error(exception, $"{source}: {message}", parameters);
                    break;
            }
        }
    }
}