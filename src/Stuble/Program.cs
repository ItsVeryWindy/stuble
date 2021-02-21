using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Stuble
{
    public class Program
    {
        static void Main(string[] args)
        {
            new HostBuilder().ConfigureWebHost(whb =>
            {
                whb
                    .UseKestrel()
                    .UseStartup<Startup>();
            }).Build().Start();
        }
    }
}
