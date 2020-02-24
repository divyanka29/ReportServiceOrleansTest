using Orleans;
using ReportData;
using System.Threading.Tasks;

namespace ReportGenericService
{
    public interface IReportGenericService<TReport> : IGrainWithIntegerKey where TReport : ReportBase
    {
        Task Execute(TReport report);

        Task AddReport(TReport report);
    }
}
