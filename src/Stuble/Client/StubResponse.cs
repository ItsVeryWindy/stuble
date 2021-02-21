using Microsoft.AspNetCore.Http;
using Stuble.Razor;
using System.Threading.Tasks;

namespace Stuble.Client
{
    public class StubResponse
    {
        public IHeaderDictionary Headers { get; }

        public int StatusCode { get; set; } = 200;

        public IRazorTemplate Body { get; set; }

        public StubResponse()
        {
            Headers = new HeaderDictionary();
        }

        public Task Execute(HttpContext context)
        {
            foreach (var header in Headers)
            {
                context.Response.Headers.Add(header);
            }

            if (Body != null)
            {
                context.Response.WriteAsync(Body.Render());
            }

            return Task.CompletedTask;
        }
    }
}
