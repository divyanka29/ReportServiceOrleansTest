//-----------------------------------------------------------------------
// <copyright file="LoggingFilter.cs" company="MS">
// Copyright (c) MS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Hosting.Grain.Plugins
{
    using System;
    using System.Threading.Tasks;
    using NLog;
    using Orleans;

    /// <summary>
    /// Definition for LoggingFilter.
    /// </summary>
    public class LoggingFilter
        : IIncomingGrainCallFilter
    {
        private static readonly ILogger Logger = LogManager.GetLogger("LoggingFilter");

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            var startTime = DateTime.UtcNow;
            if (context.InterfaceMethod.DeclaringType.FullName.StartsWith("Orleans.")
                || context.InterfaceMethod.DeclaringType.FullName.StartsWith("OrleansDashboard.Client"))
            {
                await context.Invoke();
                return;
            }

            var logEventInfo = new LogEventInfo(
                LogLevel.Info,
                Logger.Name,
                string.Format(
                    "message {0}",
                    context.InterfaceMethod.DeclaringType.FullName + ":" + context.InterfaceMethod.Name));

            await context.Invoke();

            Log(logEventInfo, DateTime.UtcNow - startTime);
        }

        private static void Log(
            LogEventInfo logEventInfo,
            TimeSpan requestDuration)
        {
            if (requestDuration.TotalMilliseconds > 200
                || logEventInfo.Level == LogLevel.Error)
            {
                if (logEventInfo.Level != LogLevel.Error
                    && logEventInfo.Level != LogLevel.Fatal)
                {
                    logEventInfo.Level = LogLevel.Warn;
                }
            }

            Logger.Log(logEventInfo);
        }
    }
}
