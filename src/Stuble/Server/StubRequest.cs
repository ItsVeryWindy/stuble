using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Stuble.Server
{
    public class StubRequest
    {
        public string Path { get; set; }

        public Dictionary<string, StringValues> Query { get; set; }

        public Dictionary<string, StringValues> Headers { get; set; }

        public string Method { get; set; }

        public string Body { get; set; }
    }
}
