using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stuble.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stuble.Client
{
    public class StubleClient : BackgroundService
    {
        private readonly HubConnection _connection;
        private readonly IEnumerable<ICreateEndpoint> _createEndpoints;
        private readonly ILogger<StubleClient> _logger;
        private readonly StubleOptions _options;
        private readonly Random _random = new Random();

        public StubleClient(IOptions<StubleOptions> options, HubConnection connection, IEnumerable<ICreateEndpoint> createEndpoints, ILogger<StubleClient> logger)
        {
            _connection = connection;
            _createEndpoints = createEndpoints;
            _logger = logger;
            _options = options.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_options.Mode == StubleModes.Server)
                return Task.CompletedTask;

            _connection.On<Guid, StubEndpoint>("CreateEndpoint", OnCreateEndpoint);

            _connection.Closed += (error) =>
            {
                return RetryConnection(stoppingToken);
            };

            return RetryConnection(stoppingToken);
        }

        private async Task OnCreateEndpoint(Guid id, StubEndpoint endpoint)
        {
            foreach(var createEndpoint in _createEndpoints)
            {
                try
                {
                    await createEndpoint.CreateEndpointAsync(id, endpoint);
                }
                catch (Exception ex)
                {
                    _logger.LogUnhandledException(ex);
                }
            }
        }

        private Task RandomDelay() => Task.Delay(_random.Next(0, 5) * 1000);

        private async Task RetryConnection(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _connection.StartAsync(stoppingToken);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogUnhandledException(ex);
                }

                await RandomDelay();
            }
        }
    }
}
