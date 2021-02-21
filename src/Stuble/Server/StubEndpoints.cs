using Microsoft.AspNetCore.SignalR;
using Stuble.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stuble.Server
{
    public class StubEndpoints
    {
        private readonly IHubContext<StubleHub, IStubleClient> _context;
        private Dictionary<Guid, StubEndpoint> _endpoints;
        private readonly SemaphoreSlim _semaphore;

        public StubEndpoints(IHubContext<StubleHub, IStubleClient> context)
        {
            _context = context;
            _endpoints = new Dictionary<Guid, StubEndpoint>();
            _semaphore = new SemaphoreSlim(1);
        }

        public async Task ResetEndpointsAsync(IStubleClient client)
        {
            var endpoints = new Dictionary<Guid, StubEndpoint>();

            await _semaphore.WaitAsync();

            try
            {
                foreach(var endpoint in _endpoints)
                {
                    endpoints.Add(endpoint.Key, endpoint.Value);
                }

                await client.ResetEndpoints(endpoints);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Guid> CreateAsync(StubEndpoint endpoint)
        {
            var id = Guid.NewGuid();

            await _semaphore.WaitAsync();

            try
            {
                _endpoints[id] = endpoint;

                await _context.Clients.All.CreateEndpoint(id, endpoint);

            }
            finally
            {
                _semaphore.Release();
            }

            return id;
        }
    }
}
