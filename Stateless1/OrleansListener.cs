using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Hosting.Grain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Newtonsoft.Json;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;

namespace ReportServiceApp
{
    /// <summary>
    /// Service Fabric communication listener which hosts an Orleans silo.
    /// </summary>
    public class OrleansListener : ICommunicationListener
    {
        private readonly IServiceInfo serviceInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrleansCommunicationListener" /> class.
        /// </summary>
        public OrleansListener(IServiceInfo serviceInfo)
        {
            this.serviceInfo = serviceInfo ?? throw new ArgumentNullException(nameof(serviceInfo));
        }

        /// <summary>
        /// Gets or sets the underlying <see cref="ISiloHost"/>.
        /// </summary>
        /// <remarks>Only valid after <see cref="OpenAsync"/> has been invoked. Exposed for testability.</remarks>
        public ISiloHost Host { get; private set; }

        /// <inheritdoc />
        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            try
            {
                var builder = new ContainerBuilder().RegisterSilo(true);
                var siloHostBuilder = builder.Build().Resolve<Func<IServiceInfo, ISiloHostBuilder>>()(serviceInfo);

                this.Host = siloHostBuilder.Build();
                await this.Host.StartAsync(cancellationToken);
            }
            catch
            {
                this.Abort();
                throw;
            }

            var endpoint = this.serviceInfo.OrleansClusterConfig;
            return JsonConvert.SerializeObject(endpoint);
        }

        /// <inheritdoc />
        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            var siloHost = this.Host;
            if (siloHost != null)
            {
                await siloHost.StopAsync(cancellationToken);
            }

            this.Host = null;
        }

        /// <inheritdoc />
        public void Abort()
        {
            var host = this.Host;
            if (host == null) return;

            var cancellation = new CancellationTokenSource();
            cancellation.Cancel(false);

            try
            {
                host.StopAsync(cancellation.Token).GetAwaiter().GetResult();
            }
            catch
            {
                // Ignore.
            }
            finally
            {
                this.Host = null;
            }
        }
    }
}