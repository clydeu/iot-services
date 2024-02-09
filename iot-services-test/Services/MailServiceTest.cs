using FluentAssertions;
using iot_services.Services;

namespace iot_services_test.Services
{
    public class MailServiceTest
    {
        [Fact]
        public void GetChannel()
        {
            var bodyText = @"

Device Name: NVR8-8580RN
Event Type: Human and Vehicle

Channel Name [IP-CH1]: Front Yard
AlarmType: Human
Time: 2024-02-07 13:08:59
Alert from your NVR/DVR --- A Human and Vehicle event has been detected on one of your cameras; please check the attached image for reference.";

            new MailService((camera) => Task.FromResult(0)).GetChannel(bodyText).Should().Be("front-yard");
        }
    }
}