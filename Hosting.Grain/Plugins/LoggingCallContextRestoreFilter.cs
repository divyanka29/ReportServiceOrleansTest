//-----------------------------------------------------------------------
// <copyright file="LoggingCallContextRestoreFilter.cs" company="MS">
// Copyright (c) MS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Hosting.Grain.Plugins
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Orleans;
    using Orleans.Runtime;

    /// <summary>
    /// Definition for LoggingCallContextRestoreFilter.
    /// </summary>
    public class LoggingCallContextRestoreFilter
        : IIncomingGrainCallFilter
    {
        private readonly string[] captureKeys;

        public LoggingCallContextRestoreFilter(string[] captureKeys)
        {
            this.captureKeys = captureKeys;
        }

        public static void RestoreLogContext(
            IList<string> contextVariables,
            string requestIdStr)
        {
            foreach (var variable in contextVariables)
            {
                NLog.MappedDiagnosticsLogicalContext.Set(
                    variable,
                    (string)RequestContext.Get(variable));
            }

            if (!string.IsNullOrWhiteSpace(requestIdStr))
            {
                var utcNow = System.DateTime.UtcNow;
                var requestId = (string)RequestContext.Get(requestIdStr);
                requestId += ":" + utcNow.ToBinary().ToString("X");

                NLog.MappedDiagnosticsLogicalContext.Set(
                    requestIdStr,
                    requestId);
            }
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            if (context.InterfaceMethod.DeclaringType.FullName.StartsWith("Orleans."))
            {
                await context.Invoke();
                return;
            }

            RestoreLogContext(
                this.captureKeys,
                null);

            await context.Invoke();
        }
    }
}
