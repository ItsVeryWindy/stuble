using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Stuble.Client
{
    public interface IHeaderMetadata
    {
        IReadOnlyDictionary<string, StringValues> Headers { get; }
    }
}
