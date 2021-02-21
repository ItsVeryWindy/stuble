using Stuble.Client;
using System.Threading.Tasks;

namespace Stuble.Server
{
    public class Connect : IConnect
    {
        private readonly StubEndpoints _endpoints;

        public Connect(StubEndpoints endpoints)
        {
            _endpoints = endpoints;
        }

        public Task ConnectAsync(IStubleClient client)
        {
            return _endpoints.ResetEndpointsAsync(client);
        }
    }
}
