using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stuble.Client
{
    public class HeaderMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        public override int Order => 0;

        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            foreach(var endpoint in endpoints)
            {
                if (endpoint.Metadata.GetMetadata<IHeaderMetadata>() != null)
                    return true;
            }

            return false;
        }

        public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
        {
            for (var i = 0; i < candidates.Count; i++)
            {
                var metadata = candidates[i].Endpoint?.Metadata.GetMetadata<IHeaderMetadata>();

                if (metadata == null)
                    continue;

                if (!candidates.IsValidCandidate(i))
                    continue;

                Apply(httpContext, candidates, i, metadata);
            }

            return Task.CompletedTask;
        }

        private void Apply(HttpContext httpContext, CandidateSet candidates, int index, IHeaderMetadata metadata)
        {
            foreach (var header in metadata.Headers)
            {
                if (!httpContext.Request.Headers.TryGetValue(header.Key, out var values))
                {
                    candidates.SetValidity(index, false);
                    return;
                }

                if(!header.Value.All(x => values.Contains(x)))
                {
                    candidates.SetValidity(index, false);
                    return;
                }
            }
        }
    }
}
