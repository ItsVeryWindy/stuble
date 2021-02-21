using System;
using System.Collections.Generic;
using System.Linq;

namespace Stuble.Razor
{
    public class RazorTemplateCompilationException : Exception
    {
        public IReadOnlyCollection<RazorTemplateError> Errors { get; }

        public RazorTemplateCompilationException(IEnumerable<RazorTemplateError> errors) : base("Unable to compile razor template")
        {
            Errors = errors.ToList();
        }
    }
}
