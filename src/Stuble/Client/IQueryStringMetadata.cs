using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Stuble.Client
{
    public interface IQueryStringMetadata
    {
        IReadOnlyDictionary<string, StringValues> Query { get; }
    }
}
