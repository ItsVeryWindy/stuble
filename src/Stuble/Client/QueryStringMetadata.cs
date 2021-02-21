using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace Stuble.Client
{
    public class QueryStringMetadata : IQueryStringMetadata
    {
        public IReadOnlyDictionary<string, StringValues> Query { get; }

        public QueryStringMetadata(IEnumerable<KeyValuePair<string, StringValues>> query)
        {
            Query = query.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
