using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using iot_services.Models;

namespace iot_services.Services
{
    public class InfluxDBService : IWriteService
    {
        private readonly string url;
        private readonly string token;
        private readonly string bucket;
        private readonly string org;
        private readonly InfluxDBClient client;
        private readonly WriteApiAsync writeApi;

        public InfluxDBService(string? org, string? bucket)
        {
            var u = Environment.GetEnvironmentVariable("IOT_LOGGER_INFLUX_DB_URL");
            if (u == null)
                throw new InvalidOperationException("Environment variable IOT_LOGGER_INFLUX_DB_URL is null.");

            if (Uri.TryCreate(u, UriKind.Absolute, out var uriResult) &&
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                throw new InvalidOperationException("IOT_LOGGER_INFLUX_DB_URL is not a valid URL.");

            var t = Environment.GetEnvironmentVariable("IOT_LOGGER_INFLUX_DB_TOKEN");
            if (t == null)
                throw new InvalidOperationException("Environment variable IOT_LOGGER_INFLUX_DB_TOKEN is null.");

            if (bucket == null)
                throw new InvalidOperationException("Bucket is null.");

            if (org == null)
                throw new InvalidOperationException("Org is null.");

            url = u;
            token = t;
            this.bucket = bucket;
            this.org = org;

            client = new InfluxDBClient(url, token);
            writeApi = client.GetWriteApiAsync();
        }

        public async Task Add(TempSensor tmpsen, DateTime time)
        {
            var point = PointData.Measurement(tmpsen.Sensor)
                .Field(tmpsen.Measurement, tmpsen.Value)
                .Timestamp(time, WritePrecision.Ns);

            await writeApi.WritePointAsync(point, bucket, org);
        }
    }
}