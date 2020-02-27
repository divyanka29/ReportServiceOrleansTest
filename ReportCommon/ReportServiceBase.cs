using NLog;
using Orleans;
using Orleans.CodeGeneration;
using ReportCommon;
using ReportData;
using ReportGenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenericService
{
    public abstract class ReportServiceBase<TReport> : Grain, IReportGenericService<TReport> where TReport : ReportBase
    {
        public ReportServiceBase(IReportPersistence reportPersistence, ILogger logger)
        {
            this.ReportPersistence = reportPersistence;
            this.logger = logger;
        }

        public virtual Task AddReport(TReport report)
        {
            return this.ReportPersistence.AddReport(report);
        }

        public virtual Task Execute(TReport report)
        {
            this.Prepare(report);
            logger.Info("Executing in report base report " + typeof(TReport).Name);
            return Task.CompletedTask;
        }

        internal virtual Task Prepare(TReport report)
        {
            logger.Info("Preparing in report base report " + typeof(TReport).Name);
            return Task.CompletedTask;
        }

        internal IReportPersistence ReportPersistence { get; set; }

        internal ILogger logger { get; }
    }
}
