// <copyright file="OrleansClusterConfig.cs" company="MS">
// Copyright (c) MS. All rights reserved.
// </copyright>

namespace Hosting.Grain
{
    /// <summary>
    /// Definition for OrleansClusterConfig.
    /// </summary>
    public abstract class OrleansClusterConfig
    {
        public string ClusterId { get; set; }

        public string CollectionPrefix { get; set; }

        public string ServiceId { get; set; }

        public string Endpoint { get; set; }

        public int GatewayPort { get; set; }

        public int ServicePort { get; set; }

        public int DashboardPort { get; set; } = 8010;

        public int PortOffset { get; set; }

        public string AzureTableConnectionString { get; set; }
    }
}
