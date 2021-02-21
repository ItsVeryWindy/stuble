using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Stuble.Server
{
    public class StubResponse
    {
        public Dictionary<string, StringValues> Headers { get; set; }

        public string Body { get; set; }

        public int StatusCode { get; set; } = 200;
    }
}
