namespace Stuble.Razor
{
    public interface IRazorTemplateFactory
    {
        IRazorTemplate Create(string content);
    }
}
