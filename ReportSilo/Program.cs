using Orleans.Hosting;
using System;
using System.Threading.Tasks;
using Hosting.Grain;
using Autofac;
using ReportGenericService;
using Orleans.Configuration;
using Orleans;
using ReportCommon;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Autofac.Extensions.DependencyInjection;
using Hosting.Grain.Plugins;
using Orleans.Runtime;

namespace ReportSilo
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var builder = new ContainerBuilder().RegisterSilo(true);

            var siloHostBuilder = new SiloHostBuilder();
            builder.Build().Resolve<Action<IServiceInfo, ISiloHostBuilder>>()(new ReportServiceInfo(), siloHostBuilder);

            var host = siloHostBuilder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
