using System.Threading.Tasks;
using NUnit.Framework;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Stuble.Server;
using Stuble.Client;
using System;
using System.Threading;

namespace Stuble.Tests
{
    public class Class1 : TestBase
    {
        [Test]
        public async Task ShouldCreateNewEndpoint()
        {
            var createEndpoint = new StubCreateEndpoint();
            var connect = new StubConnect();

            var server = CreateServer(x => x.AddSingleton<IConnect>(connect));

            var client = CreateClient(server, x => x.AddSingleton<ICreateEndpoint>(createEndpoint));

            connect.Wait();

            var createdEndpoint = await CreateEndpoint(server, new StubEndpoint
            {
                Request = new Server.StubRequest
                {
                    Path = "hello"
                },
                Response = new Server.StubResponse
                {
                    Body = "hello"
                }
            });

            createEndpoint.Wait();

            var response = await client.CreateClient().GetAsync("hello");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("hello"));
        }

        class StubCreateEndpoint : ICreateEndpoint
        {
            private readonly ManualResetEventSlim _manualResetEvent = new ManualResetEventSlim();

            public Task CreateEndpointAsync(Guid id, StubEndpoint endpoint)
            {
                _manualResetEvent.Set();

                return Task.CompletedTask;
            }

            public void Wait() => _manualResetEvent.Wait(Debugger.IsAttached ? -1 : 5000);
        }

        class StubConnect : IConnect
        {
            private readonly ManualResetEventSlim _manualResetEvent = new ManualResetEventSlim();

            public Task ConnectAsync(IStubleClient client)
            {
                _manualResetEvent.Set();

                return Task.CompletedTask;
            }

            public void Wait() => _manualResetEvent.Wait(Debugger.IsAttached ? -1 : 5000);
        }
    }
}
