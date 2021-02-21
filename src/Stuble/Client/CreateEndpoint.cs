using Stuble.Razor;
using Stuble.Server;
using System;
using System.Threading.Tasks;

namespace Stuble.Client
{
    public class CreateEndpoint : ICreateEndpoint
    {
        private readonly StubleEndpointDataSource _dataSource;
        private readonly IRazorTemplateFactory _factory;

        public CreateEndpoint(StubleEndpointDataSource dataSource, IRazorTemplateFactory factory)
        {
            _dataSource = dataSource;
            _factory = factory;
        }

        async Task ICreateEndpoint.CreateEndpointAsync(Guid id, StubEndpoint endpoint)
        {
            var response = new StubResponse
            {
                Body = _factory.Create(endpoint.Response.Body)
            };

            var request = new StubRequest(id, response)
            {
                Path = endpoint.Request.Path
            };

            await _dataSource.AddEndpointAsync(request);
        }
    }
}
