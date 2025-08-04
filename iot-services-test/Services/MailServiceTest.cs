using FluentAssertions;
using iot_services.Services;

namespace iot_services_test.Services
{
    public class MailServiceTest
    {
        [Fact]
        public void GetSwannChannel()
        {
            var bodyText = @"

Device Name: NVR8-8580RN
Event Type: Human and Vehicle

Channel Name [IP-CH1]: Front Yard
AlarmType: Human
Time: 2024-02-07 13:08:59
Alert from your NVR/DVR --- A Human and Vehicle event has been detected on one of your cameras; please check the attached image for reference.";

            new MailService((camera) => Task.FromResult(0)).SwannGetChannel(bodyText).Should().Be("front-yard");
        }

        [Fact]
        public void GetReolinkChannel()
        {
            var mailService = new MailService((camera) => Task.FromResult(0));

            mailService.ReolinkGetChannel("Alarm Camera Name:Living Room Camera, Alarm Event:Motion Detection, Alarm Input Channel No.:1, Alarm Device Name:Living Room Camera").Should().Be("living-room");
            mailService.ReolinkGetChannel("Alarm Camera Name:Garage Camera, Alarm Event:Motion Detection, Alarm Input Channel No.:1, Alarm Device Name:Garage Camera").Should().Be("garage");
        }
    }
}