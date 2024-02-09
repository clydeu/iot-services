namespace iot_services.Services
{
    public class TempSensorInfluxDBService : InfluxDBService
    {
        public TempSensorInfluxDBService() :
            base(Environment.GetEnvironmentVariable("IOT_LOGGER_INFLUX_TEMP_ORG"),
                Environment.GetEnvironmentVariable("IOT_LOGGER_INFLUX_TEMP_BUCKET"))
        {
        }
    }
}