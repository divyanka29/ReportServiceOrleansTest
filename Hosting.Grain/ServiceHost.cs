//-----------------------------------------------------------------------
// <copyright file="ServiceHost.cs" company="MS">
// Copyright (c) MS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Hosting.Grain
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using NLog.Extensions.Hosting;
    using NLog.Extensions.Logging;
    using Orleans.Hosting;

    public class ServiceHost : IHostedService
    {
        private readonly ISiloHost[] hosts;

        public ServiceHost(
            IServiceInfo[] serviceInfos,
            Func<IServiceInfo, ISiloHostBuilder> hostFactory)
        {
            this.hosts = serviceInfos
                .Select(serviceInfo => hostFactory(serviceInfo).Build())
                .ToArray();
        }

        public static IConfiguration Configuration { get; set; }

        public static Task RunService(
            string[] args,
            IServiceInfo[] serviceInfos,
            Func<IHostBuilder, IHostBuilder> lifetimeConfigurer)
        {
            var isdevServer = args.Contains("--devserver");
            var isService =
                !Debugger.IsAttached
                && !args.Contains("--console")
                && !isdevServer;

            var environment = !isdevServer
                ? Environment.GetEnvironmentVariable("Environment")
                : "Development";

            Configuration =
                new ConfigurationBuilder()
                    .AddConfigurations(
                        args,
                        environment)
                    .Build();

            var hostBuilder = new HostBuilder()
                .UseNLog()
                .ConfigureHostConfiguration((c) => HostRegistrations.HostConfiguration(c, args))
                .ConfigureAppConfiguration((hostContext, configApp)
                    => HostRegistrations.AppConfiguration(hostContext, configApp, args))
                .ConfigureServices(HostRegistrations.ConfigureServices)
                .ConfigureLogging((loggingBuilder) => loggingBuilder.AddNLog())
                .UseServiceProviderFactory(
                    new AutofacServiceProviderFactory(
                        (cb) =>
                        {
                            _ = cb.RegisterInstance(serviceInfos);
                            HostRegistrations.RegisterComponents(
                                cb,
                                isdevServer);
                        }))
                ;

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            var host = (isService
                ? lifetimeConfigurer != null
                    ? lifetimeConfigurer(hostBuilder)
                    : hostBuilder.UseConsoleLifetime()
                : hostBuilder.UseConsoleLifetime()).Build();

            return host.RunAsync();
        }

        public Task StartAsync(
            CancellationToken cancellationToken = default)
            => Task.WhenAll(this.hosts.Select(_ => _.StartAsync()).ToList());

        public Task StopAsync(
            CancellationToken cancellationToken = default)
            => Task.WhenAll(this.hosts.Select(_ => _.StopAsync()).ToList());
    }
}
