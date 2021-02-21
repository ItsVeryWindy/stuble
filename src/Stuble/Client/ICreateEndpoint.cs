using Stuble.Server;
using System;
using System.Threading.Tasks;

namespace Stuble.Client
{
    public interface ICreateEndpoint
    {
        Task CreateEndpointAsync(Guid id, StubEndpoint endpoint);
    }
}
