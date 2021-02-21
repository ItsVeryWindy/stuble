using Stuble.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stuble.Client
{
    public interface IStubleClient
    {
        public Task ResetEndpoints(Dictionary<Guid, StubEndpoint> endpoints);
        Task CreateEndpoint(Guid id, StubEndpoint endpoint);
    }
}
