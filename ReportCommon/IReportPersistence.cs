using ReportData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportCommon
{
    public interface IReportPersistence
    {
        Task AddReport<TReport>(TReport report) where TReport : ReportBase;
    }
}
