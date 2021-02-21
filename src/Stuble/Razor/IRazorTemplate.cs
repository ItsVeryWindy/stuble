using System;

namespace Stuble.Razor
{
    public interface IRazorTemplate : IDisposable
    {
        string Render();
    }
}
