using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Stuble.Server;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using Stuble.Client;

namespace Stuble.Tests
{
    public class TestBase
    {
        private List<IHost> _hosts;

        public TestServer CreateServer(Action<IServiceCollection> configureServices) => CreateClient(StubleModes.Server, null, configureServices);

        public TestServer CreateClient(TestServer server, Action<IServiceCollection> configureServices) => CreateClient(StubleModes.Client, server, configureServices);

        private TestServer CreateClient(StubleModes mode, TestServer server, Action<IServiceCollection> configureServices)
        {
            var host = new HostBuilder().ConfigureWebHost(builder =>
            {
                builder
                    .UseStartup<Startup>()
                    .UseTestServer()
                    .ConfigureAppConfiguration(x => x.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Mode"] = mode.ToString("D"),
                        ["ServerUrl"] = server?.BaseAddress.ToString()
                    }))
                    .ConfigureLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Warning));

                if (server != null)
                {
                    builder.ConfigureServices(x => x.AddSingleton(new HubConnectionBuilder()
                           .WithUrl(new Uri(server.BaseAddress, "hub"),
                           x => x.HttpMessageHandlerFactory =
                           _ => server.CreateHandler()).Build()));
                }
                else
                {
                    builder.ConfigureServices(x => 
                    x.AddSingleton(x => new HubConnectionBuilder().WithUrl(new Uri(x.GetRequiredService<IOptions<StubleClientOptions>>().Value.ServerUrl ?? new Uri("http://looocalhost"), "/hub")).Build()));
                }

                builder.ConfigureTestServices(configureServices);

            }).Build();

            _hosts.Add(host);

            host.Start();

            return host.GetTestServer();
        }

        [SetUp]
        public void SetUp()
        {
            _hosts = new List<IHost>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach(var host in _hosts)
            {
                host.Dispose();
            }
        }

        protected async Task<Uri> CreateEndpoint(TestServer server, StubEndpoint endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "endpoints")
            {
                Content = new StringContent(JsonConvert.SerializeObject(endpoint), Encoding.UTF8, "application/json")
            };

            var response = await server.CreateClient().SendAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            return response.Headers.Location;
        }
    }
}