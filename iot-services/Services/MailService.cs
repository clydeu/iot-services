﻿using MimeKit;
using Serilog;
using SmtpServer;
using SmtpServer.Authentication;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System.Buffers;
using System.Text.RegularExpressions;
using ILogger = Serilog.ILogger;
using ServiceProvider = SmtpServer.ComponentModel.ServiceProvider;

namespace iot_services.Services
{
    public class MailService : MessageStore, IUserAuthenticator
    {
        private readonly SmtpServer.SmtpServer smtpServer;
        private readonly ILogger logger = Log.ForContext<MailService>();
        private readonly string serverName = "localhost";
        private readonly int serverPort = 587;
        private readonly Func<string, Task> motionDetected;
        private readonly string[] emailSubjects;

        public MailService(Func<string, Task> motionDetected)
        {
            emailSubjects = Environment.GetEnvironmentVariable("SWANN_EMAIL_SUBJECTS")?.Split(";") ??
                ["Alert from your NVR/DVR --- A Human and Vehicle event has been detected"];

            var options = new SmtpServerOptionsBuilder()
                            .ServerName(serverName)
                            .Endpoint(
                                endpointBuilder =>
                                {
                                    endpointBuilder.Port(serverPort, false).AllowUnsecureAuthentication(true).AuthenticationRequired();
                                }
                            ).Build();

            var serviceProvider = new ServiceProvider();
            serviceProvider.Add(this as IMessageStore);
            serviceProvider.Add(this as IUserAuthenticator);

            smtpServer = new SmtpServer.SmtpServer(options, serviceProvider);
            this.motionDetected = motionDetected;
        }

        public Task<bool> AuthenticateAsync(ISessionContext context, string user, string password, CancellationToken cancellationToken)
        {
            logger.Debug("Authentication for {user}", user);
            return Task.FromResult(true);
        }

        public Task RunAsync()
        {
            logger.Debug($"SMTP server running on port {serverPort}");
            return smtpServer.StartAsync(CancellationToken.None);
        }

        public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
        {
            logger.Debug($"Saving email with {buffer.Length} length");
            await using var stream = new MemoryStream();

            var position = buffer.GetPosition(0);
            while (buffer.TryGet(ref position, out var memory))
            {
                await stream.WriteAsync(memory, cancellationToken);
            }

            stream.Position = 0;

            var message = await MimeMessage.LoadAsync(stream, cancellationToken);

            logger.Debug("{from}: {subject}", message.From, message.Subject);
            logger.Debug("body: {body}", string.Join(", ", message.TextBody.Trim().Split(Environment.NewLine).Where(x => x != string.Empty)));
            if (emailSubjects.Contains(message.Subject))
            {
                var channel = GetChannel(message.TextBody);
                logger.Debug("Channel: {channel}", channel);
                if (!string.IsNullOrWhiteSpace(channel))
                    await motionDetected(channel);
            }

            return SmtpResponse.Ok;
        }

        public string GetChannel(string bodyText)
        {
            var strReader = new StringReader(bodyText);
            string? str = null;
            string? channel = null;
            while ((str = strReader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(str))
                    continue;

                var keyVal = str.Split(": ");
                if (keyVal.Length != 2)
                    continue;

                if (Regex.IsMatch(keyVal[0], @"Channel Name \[IP-CH[1-6]\]"))
                {
                    channel = keyVal[1];
                    break;
                }
            }

            return Regex.Replace((channel ?? "").ToLower(), @"\s+", "-");
        }
    }
}