using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stuble.Client
{
    public class StubleEndpointDataSource : EndpointDataSource, IDisposable
    {
        private List<Endpoint> _endpoints = new List<Endpoint>();
        private CancellationChangeToken _changeToken;
        private CancellationTokenSource _cts;
        private readonly SemaphoreSlim _semaphore;

        public override IReadOnlyList<Endpoint> Endpoints => _endpoints;

        public StubleEndpointDataSource()
        {
            _cts = new CancellationTokenSource();
            _changeToken = new CancellationChangeToken(_cts.Token);
            _semaphore = new SemaphoreSlim(1);
        }

        public async Task AddEndpointAsync(StubRequest request)
        {
            await _semaphore.WaitAsync();

            try
            {
                _endpoints.Add(request.CreateEndpoint());

                Refresh();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override IChangeToken GetChangeToken() => _changeToken;

        public async Task ResetEndpointsAsync(IReadOnlyCollection<StubRequest> requests)
        {
            await _semaphore.WaitAsync();

            try
            {
                var endpoints = new List<Endpoint>();

                foreach (var request in requests)
                {
                    endpoints.Add(request.CreateEndpoint());
                }

                _endpoints = endpoints;

                Refresh();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void Refresh()
        {
            var cts = _cts;

            _cts = new CancellationTokenSource();
            _changeToken = new CancellationChangeToken(_cts.Token);

            cts.Cancel();
            cts.Dispose();
        }

        public void Dispose()
        {
            _cts.Dispose();
        }
    }
}
