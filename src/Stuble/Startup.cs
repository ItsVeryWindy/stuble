using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Stuble.Client;
using Stuble.Razor;
using Stuble.Server;
using System;

namespace Stuble
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    var options = endpoints.ServiceProvider.GetRequiredService<IOptions<StubleOptions>>().Value;

                    if (options.Mode == StubleModes.Server)
                    {
                        endpoints.MapHub<StubleHub>("/hub");
                        endpoints.MapControllers();
                    }

                    if (options.Mode == StubleModes.Client)
                    {
                        endpoints.DataSources.Add(endpoints.ServiceProvider.GetRequiredService<StubleEndpointDataSource>());
                    }
                });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddRazorPages();

            services.AddSingleton<IRazorTemplateFactory, RazorTemplateFactory>();

            services
                .AddOptions<StubleOptions>()
                .Bind(_configuration)
                .ValidateDataAnnotations();

            services
                .AddOptions<StubleClientOptions>()
                .Bind(_configuration)
                .ValidateDataAnnotations();

            services.AddSingleton<StubEndpoints>();

            services
                .AddSignalR();

            services
                .AddSingleton<MatcherPolicy, HeaderMatcherPolicy>()
                .AddSingleton<MatcherPolicy, QueryStringMatcherPolicy>();

            services.AddSingleton<StubleEndpointDataSource>();
            services.AddSingleton<ICreateEndpoint, CreateEndpoint>();
            services.AddSingleton<IConnect, Connect>();

            services.TryAddSingleton(x => new HubConnectionBuilder().WithUrl(new Uri(x.GetRequiredService<IOptions<StubleClientOptions>>().Value.ServerUrl ?? new Uri("http://localhost"), "/hub")).Build());

            services.AddHostedService<StubleClient>();
        }
    }
}
