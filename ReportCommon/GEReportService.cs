using NLog;
using Orleans;
using ReportCommon;
using ReportData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenericService
{
    public class GEReportService : ReportServiceBase<GEReport>
    {
        public GEReportService(IReportPersistence reportPersistence, ILogger logger) : base(reportPersistence, logger)
        {
        }
    }
}
