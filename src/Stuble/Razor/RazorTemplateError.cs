namespace Stuble.Razor
{
    public class RazorTemplateError
    {
        public int Line { get; }
        public int Character { get; }
        public string Message { get; }

        public RazorTemplateError(int line, int character, string message)
        {
            Line = line;
            Character = character;
            Message = message;
        }
    }
}
