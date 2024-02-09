namespace iot_services.Helpers
{
    public static class DateTimeExtension
    {
        public static DateTime RemoveMillisecond(this DateTime dateTime)
        {
            return dateTime.AddMilliseconds(-dateTime.Millisecond);
        }
    }
}