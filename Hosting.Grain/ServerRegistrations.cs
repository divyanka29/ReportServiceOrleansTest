//-----------------------------------------------------------------------
// <copyright file="ServerRegistrations.cs" company="MS">
// Copyright (c) MS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Hosting.Grain
{
    using System;
    using System.Linq;
    using System.Net;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Hosting.Grain.Plugins;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;
    using Orleans;
    using Orleans.Configuration;
    using Orleans.Hosting;
    using Orleans.Runtime;

    public static class ServerRegistrations
    {
        public static ContainerBuilder RegisterSilo(
            this ContainerBuilder builder,
            bool isDevelopmentEnv)
        {
            _ = builder.Register<Action<IServiceInfo, ISiloHostBuilder>>((containerContext) =>
            {
                  //var icc = containerContext.Resolve<IComponentContext>();
                  return (IServiceInfo serviceInfo, ISiloHostBuilder siloBuilder) =>
                  {
                      var orleansConfig = serviceInfo.OrleansClusterConfig;
                      var siloName = serviceInfo.ServiceName;
                      //ISiloHostBuilder siloBuilder = new SiloHostBuilder();

                      siloBuilder = isDevelopmentEnv
                          ? siloBuilder.GetDevelopmentSiloHostBuilder(serviceInfo)
                          : serviceInfo.RegisterProdOrleansProviders(
                              siloBuilder.GetProductionSiloHostBuilder(orleansConfig));

                      siloBuilder = siloBuilder
                          .Configure<ClusterOptions>(options =>
                          {
                              options.ClusterId = orleansConfig.ClusterId;
                              options.ServiceId = orleansConfig.ServiceId;
                          })
                          .Configure<SiloOptions>(options => options.SiloName = serviceInfo.ServiceName)
                          .ConfigureApplicationParts(parts =>
                            serviceInfo
                                .GrainPartsAssemblies
                                .ToList()
                                .ForEach(assembly =>
                                    parts
                                      .AddApplicationPart(assembly)
                                      .WithReferences()))
                          .UseServiceProviderFactory(
                              new AutofacServiceProviderFactory(builder2 =>
                              {
                                  serviceInfo.RegisterGrains(builder2);
                                  serviceInfo.RegisterConfig(builder2);
                                  serviceInfo.RegisterClients(builder2);
                                  serviceInfo.RegisterServices(builder2);

                                  _ = builder2.Register(_ =>
                                      new LoggingCallContextRestoreFilter(serviceInfo.NLogContextKeysToCapture));

                                  _ = builder2.RegisterType<GrainActivator>()
                                      .As<IGrainActivator>()
                                      .SingleInstance();
                              }))
                          .AddIncomingGrainCallFilter<LoggingCallContextRestoreFilter>()
                          .AddIncomingGrainCallFilter<LoggingFilter>()
                          // .AddOutgoingGrainCallFilter<LoggingCallContextCaptureFilter>()
                          ;

                      /*if (!isDevelopmentEnv)
                      {
                          siloBuilder = siloBuilder
                              .UseDashboard(options =>
                              {
                                  options.Username = "USERNAME";
                                  options.Password = "PASSWORD";
                                  options.Host = "*";
                                  options.Port = orleansConfig.DashboardPort + orleansConfig.PortOffset;
                                  options.HostSelf = true;
                                  options.CounterUpdateIntervalMs = 5000;
                              });
                      }*/

                      //return siloBuilder;
                  };
              });

            return builder;
        }

        private static ISiloHostBuilder GetDevelopmentSiloHostBuilder(
            this ISiloHostBuilder silo,
            IServiceInfo serviceInfo)
        {
            var userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            var userSID = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
            var port = 20;

            return serviceInfo.RegisterExtraDevOrleansProvider(
                silo
                    .UseLocalhostClustering(
                        20000 + port,
                        30000 + port,
                        null,
                        userName,
                        userName)
                    .UseInMemoryReminderService()
                    .AddMemoryGrainStorageAsDefault()
                    .AddSimpleMessageStreamProvider("SMSProvider")
                    .AddMemoryGrainStorage("PubSubStore"))
                    .ConfigureLogging(logging => logging.AddConsole());
        }

        private static ISiloHostBuilder GetProductionSiloHostBuilder(
            this ISiloHostBuilder silo,
            OrleansClusterConfig orleansConfig)
            => silo
                .Configure<EndpointOptions>(options =>
                    {
                        options.AdvertisedIPAddress = IPAddress.Parse(orleansConfig.Endpoint ?? "127.0.0.1");
                        options.GatewayPort = orleansConfig.GatewayPort + orleansConfig.PortOffset;
                        options.SiloPort = orleansConfig.ServicePort + orleansConfig.PortOffset;
                    })
                .ConfigureLogging(logging => logging.AddNLog())
                .UseAzureStorageClustering(options => options.ConnectionString = orleansConfig.AzureTableConnectionString);
    }
}
