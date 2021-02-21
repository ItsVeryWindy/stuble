using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Stuble.Server
{
    [Route("endpoints")]
    [ApiController]
    public class EndpointsController : Controller
    {
        private readonly StubEndpoints _endpoints;

        public EndpointsController(StubEndpoints endpoints)
        {
            _endpoints = endpoints;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEndpoint(StubEndpoint endpoint)
        {
            var id = await _endpoints.CreateAsync(endpoint);

            return Created("endpoints", id);
        }
    }
}
