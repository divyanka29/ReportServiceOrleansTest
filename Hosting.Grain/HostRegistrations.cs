//-----------------------------------------------------------------------
// <copyright file="HostRegistrations.cs" company="MS">
// Copyright (c) MS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Hosting.Grain
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Autofac;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Definition for HostRegistrations.
    /// </summary>
    public static class HostRegistrations
    {
        public static void HostConfiguration(
            IConfigurationBuilder configHost,
            string[] args)
        {
            _ = configHost.SetBasePath(Directory.GetCurrentDirectory())
                .AddCommandLine(args)
                .AddJsonFile("hostsettings.json", true);
        }

        public static void AppConfiguration(
            HostBuilderContext hostContext,
            IConfigurationBuilder configApp,
            string[] args)
        {
            _ = configApp
                .AddConfigurations(
                    args,
                    hostContext.HostingEnvironment.EnvironmentName);
        }

        public static void ConfigureServices(
            HostBuilderContext hostContext,
            IServiceCollection services)
        {
            ServiceHost.Configuration = hostContext.Configuration;

            _ = services
                .Configure<HostOptions>(o => o.ShutdownTimeout = TimeSpan.FromSeconds(20));
        }

        public static void RegisterComponents(
            this ContainerBuilder builder,
            bool isDevServer)
        {
            _ = builder
                .RegisterType<ServiceHost>()
                .As<IHostedService>();

            _ = builder
                .RegisterSilo(isDevServer);
        }

        public static IConfigurationBuilder AddConfigurations(
            this IConfigurationBuilder configurationBuilder,
            string[] args,
            string environmentName)
        {
            _ = configurationBuilder
                .AddJsonFile(
                    "appsettings.json",
                    false);

            if (!string.IsNullOrWhiteSpace(environmentName))
            {
                configurationBuilder = configurationBuilder
                    .AddJsonFile(
                        $"appsettings.{environmentName}.json",
                        optional: true);
            }

            return configurationBuilder
                .AddEnvironmentVariables("ORLEANS_")
                .AddCommandLine(args);
        }
    }
}
