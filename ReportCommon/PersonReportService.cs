using NLog;
using Orleans;
using ReportCommon;
using ReportData;
using System;
using System.Threading.Tasks;

namespace ReportGenericService
{
    public class PersonReportService : ReportServiceBase<PersonReport>
    {
        public PersonReportService(IReportPersistence reportPersistence, ILogger logger) : base(reportPersistence, logger)
        {
        }

        public override Task Execute(PersonReport report)
        {
            this.Prepare(report);
            logger.Info("Executing in person report report " + typeof(PersonReport).Name);
            return Task.CompletedTask;
        }
    }
}
