using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TcpListener
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptions<TcpListenerConfiguration> _options;

        public Worker(ILogger<Worker> logger, IOptions<TcpListenerConfiguration> options)
        {
            _logger = logger;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"TcpListener service started with:" +
                $"\r\nActivationPhrase: {_options.Value.ActivationPhrase}" +
                $"\r\nPort: {_options.Value.Port}" +
                $"\r\nCommand: {_options.Value.Command}" +
                $"\r\nVariables: {_options.Value.Variables}");

            var listener = new System.Net.Sockets.TcpListener(IPAddress.Any, _options.Value.Port);
            listener.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                while(true)
                {
                    _logger.LogInformation("Waiting for client...");

                    var client = listener.AcceptTcpClient();
                    IPEndPoint remoteIpEndPoint = client.Client.RemoteEndPoint as IPEndPoint;

                    _logger.LogInformation("Client connected from {0}", remoteIpEndPoint.Address);

                    var clientStream = client.GetStream();
                    StreamWriter sw = new StreamWriter(clientStream);
                    StreamReader sr = new StreamReader(clientStream);

                    var command = sr.ReadLine();
                    _logger.LogInformation($"Client send: { command }");

                    var result = command.Trim().Equals(_options.Value.ActivationPhrase);
                    if (result)
                    {
                        _logger.LogInformation($"Command accepted");
                        sw.Write($"... Acknowledged{ Environment.NewLine }");
                        sw.Flush();

                        client.Close();
                        Process.Start(new ProcessStartInfo(_options.Value.Command, _options.Value.Variables) { UseShellExecute = true });
                        break;
                    }
                    _logger.LogInformation($"Command not recognized, ignored");
                    sw.Write($"... Ok{ Environment.NewLine }");
                    sw.Flush();

                    client.Close();
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("TcpListener service stopping");
        }
    }
}
