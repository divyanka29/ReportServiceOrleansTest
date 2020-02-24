//-----------------------------------------------------------------------
// <copyright file="GrainActivator.cs" company="MS">
// Copyright (c) MS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Hosting.Grain.Plugins
{
    using System;
    using Autofac;
    using Autofac.Core.Lifetime;
    using Microsoft.Extensions.DependencyInjection;
    using Orleans.Runtime;

    /// <summary>
    /// Definition for GrainActivator.
    /// </summary>
    public class GrainActivator : DefaultGrainActivator
    {
        private const string GrainScopeStr = "grainScope";

        public GrainActivator(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        /// <inheritdoc />
        public override object Create(IGrainActivationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var grainType = context.GrainType;

            if (grainType == null)
            {
                throw new ArgumentException(
                    string.Format(
                        "The '{0}' property of '{1}' must not be null.",
                        nameof(context.GrainType),
                        nameof(IGrainActivationContext)));
            }

            var lifetimeScope = context.ActivationServices.GetService<ILifetimeScope>()
                as LifetimeScope;

            var grainScope = lifetimeScope.BeginLifetimeScope(
                (builder) => builder.RegisterInstance(context.GrainIdentity));

            context.Items[GrainScopeStr] = grainScope;

            var serviceProvider = context.ActivationServices;
            var grain = grainScope.ResolveOptional(grainType);

            return grain
                ?? base.Create(context);
        }

        /// <inheritdoc />
        public override void Release(IGrainActivationContext context, object grain)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (grain == null)
            {
                throw new ArgumentNullException(nameof(grain));
            }

            if (grain is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if (context.Items[GrainScopeStr] is LifetimeScope grainScope)
            {
                grainScope.Dispose();
            }
        }
    }
}
