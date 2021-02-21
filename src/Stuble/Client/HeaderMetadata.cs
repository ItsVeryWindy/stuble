using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace Stuble.Client
{
    public class HeaderMetadata : IHeaderMetadata
    {
        public IReadOnlyDictionary<string, StringValues> Headers { get; }

        public HeaderMetadata(IEnumerable<KeyValuePair<string, StringValues>> headers)
        {
            Headers = headers.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
