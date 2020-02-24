using Autofac;
using Hosting.Grain;
using NLog;
using Orleans.Hosting;
using ReportCommon;
using ReportGenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReportSilo
{
    public class ReportServiceInfo : IServiceInfo
    {
        public string ServiceName => "Stateless1";

        public int ServiceId => 50;

        public OrleansClusterConfig OrleansClusterConfig => new ReportClusterConfig();

        public Assembly[] GrainPartsAssemblies => new Assembly[] { typeof(IReportGenericService<>).Assembly, typeof(ReportServiceBase<>).Assembly };

        public string[] NLogContextKeysToCapture => new string[] { };

        public void RegisterClients(ContainerBuilder containerBuilder)
        {
        }

        public void RegisterConfig(ContainerBuilder containerBuilder)
        {
        }

        public ISiloHostBuilder RegisterExtraDevOrleansProvider(ISiloHostBuilder siloBuilder)
        {
            return siloBuilder;
        }

        public void RegisterGrains(ContainerBuilder containerBuilder)
        {
        }

        public ISiloHostBuilder RegisterProdOrleansProviders(ISiloHostBuilder siloBuilder)
        {
            return siloBuilder;
        }

        public void RegisterServices(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(icc => {
                var config = new NLog.Config.LoggingConfiguration();
                var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };

                config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);

                // Apply config           
                NLog.LogManager.Configuration = config;
                return LogManager.GetCurrentClassLogger();
            })
                .SingleInstance()
                .As<ILogger>();

            containerBuilder.Register(icc => new ReportPersistence(icc.Resolve<ILogger>()))
                .SingleInstance()
                .As<IReportPersistence>();
        }
    }
}
