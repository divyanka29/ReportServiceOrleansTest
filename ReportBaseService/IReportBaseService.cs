using Orleans;
using ReportData;
using System.Threading.Tasks;

namespace ReportBaseService
{
    public interface IReportBaseService : IGrainWithIntegerKey
    {
        Task AddReport<TReport>(TReport report) where TReport : ReportBase;

        Task Execute(ReportBase reportBase);
    }
}