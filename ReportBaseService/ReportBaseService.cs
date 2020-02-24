using Orleans;
using ReportCommon;
using ReportData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportBaseService
{
    public class ReportBaseService : Grain, IReportBaseService
    {
        private IReportPersistence ReportPersistence { get; set; }

        private IReportFactory ReportFactory { get; set; }

        public ReportBaseService(IReportPersistence reportPersistence, IReportFactory reportFactory)
        {
            this.ReportPersistence = reportPersistence;
            this.ReportFactory = reportFactory;
        }
        public Task AddReport<TReport>(TReport report) where TReport : ReportBase
        {
            return this.ReportPersistence.AddReport(report);
        }

        public Task Execute(ReportBase reportBase)
        {
            return this.ReportFactory.GetReportExecutorBase(reportBase).Execute(reportBase);
        }
    }
}
