using NLog;
using ReportData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportCommon
{
    public class ReportPersistence : IReportPersistence
    {
        public ReportPersistence(ILogger logger)
        {
            this.logger = logger;
        }

        public Task AddReport<TReport>(TReport report) where TReport : ReportBase
        {
            logger.Info("Adding Report " + typeof(TReport).Name + " " + report.ToString());
            return Task.CompletedTask;
        }

        private ILogger logger { get; }
    }
}
