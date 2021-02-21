using System.IO;
using System.Threading.Tasks;

namespace Stuble.Razor
{
    public abstract class StubTemplateBase
    {
        private StringWriter _writer;

        public abstract Task ExecuteAsync();

        protected void Write(object value) => WriteLiteral(value.ToString());

        protected void WriteLiteral(string value)
        {
            _writer.Write(value);
        }

        public override string ToString()
        {
            _writer = new StringWriter();

            ExecuteAsync().GetAwaiter().GetResult();

            return _writer.ToString();
        }
    }
}
