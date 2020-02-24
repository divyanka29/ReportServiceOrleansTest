using Hosting.Grain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportSilo
{
    public class ReportClusterConfig : OrleansClusterConfig
    {
        public ReportClusterConfig()
        {
            this.ClusterId = "dev";
            this.ServiceId = "Stateless1";
            this.ServicePort = 30000;
            this.PortOffset = 0;
            this.AzureTableConnectionString = "connectionString";
        }
    }
}
