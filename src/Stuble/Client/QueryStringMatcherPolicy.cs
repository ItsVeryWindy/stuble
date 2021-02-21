using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stuble.Client
{
    public class QueryStringMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        public override int Order => 0;

        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            foreach (var endpoint in endpoints)
            {
                if (endpoint.Metadata.GetMetadata<IQueryStringMetadata>() != null)
                    return true;
            }

            return false;
        }

        public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
        {
            for (var i = 0; i < candidates.Count; i++)
            {
                var metadata = candidates[i].Endpoint?.Metadata.GetMetadata<IQueryStringMetadata>();

                if (metadata == null)
                    continue;

                if (!candidates.IsValidCandidate(i))
                    continue;

                Apply(httpContext, candidates, i, metadata);
            }

            return Task.CompletedTask;
        }

        private void Apply(HttpContext httpContext, CandidateSet candidates, int index, IQueryStringMetadata metadata)
        {
            foreach (var query in metadata.Query)
            {
                if (!httpContext.Request.Query.TryGetValue(query.Key, out var values))
                {
                    candidates.SetValidity(index, false);
                    return;
                }

                if (!query.Value.All(x => values.Contains(x)))
                {
                    candidates.SetValidity(index, false);
                    return;
                }
            }
        }
    }
}
