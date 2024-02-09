using iot_services.Models;

namespace iot_services.Services
{
    public interface IWriteService
    {
        Task Add(TempSensor tmpsen, DateTime time);
    }
}