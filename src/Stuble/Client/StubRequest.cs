using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stuble.Client
{
    public class StubRequest
    {
        private readonly StubResponse _response;

        public string Path { get; set; }
        public QueryCollection Query { get; }

        public IHeaderDictionary Headers { get; }

        public string Method { get; }

        public StubRequest(Guid id, StubResponse response)
        {
            Query = new QueryCollection();
            Headers = new HeaderDictionary();

            _response = response;
        }

        public Endpoint CreateEndpoint()
        {
            var pattern = RoutePatternFactory.Parse(Path);

            var items = new List<object>();

            if (Method != null)
            {
                items.Add(new HttpMethodMetadata(Enumerable.Repeat(Method, 1)));
            }

            if (Headers.Count > 0)
            {
                items.Add(new HeaderMetadata(Headers));
            }

            if (Query.Count > 0)
            {
                items.Add(new QueryStringMetadata(Query));
            }

            var metadata = new EndpointMetadataCollection(items);

            var endpoint = new RouteEndpoint(Execute, pattern, 0, metadata, null);

            return endpoint;
        }

        private Task Execute(HttpContext context)
        {
            return _response.Execute(context);
        }
    }
}
