// <copyright file="IServiceInfo.cs" company="MS">
// Copyright (c) MS. All rights reserved.
// </copyright>

namespace Hosting.Grain
{
    using System.Reflection;
    using Autofac;
    using Orleans.Hosting;

    public interface IServiceInfo
    {
        string ServiceName
        { get; }

        int ServiceId
        { get; }

        OrleansClusterConfig OrleansClusterConfig
        { get; }

        Assembly[] GrainPartsAssemblies
        { get; }

        string[] NLogContextKeysToCapture
        { get; }

        ISiloHostBuilder RegisterProdOrleansProviders(ISiloHostBuilder siloBuilder);

        ISiloHostBuilder RegisterExtraDevOrleansProvider(ISiloHostBuilder siloBuilder);

        void RegisterConfig(ContainerBuilder containerBuilder);

        void RegisterGrains(ContainerBuilder containerBuilder);

        void RegisterClients(ContainerBuilder containerBuilder);

        void RegisterServices(ContainerBuilder containerBuilder);
    }
}
