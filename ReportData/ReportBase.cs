using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportData
{
    public abstract class ReportBase
    {
        public string ReportType { get; set; }

        public ReportBase(string type)
        {
            this.ReportType = type;
        }

        public override string ToString()
        {
            return "\nReportType : " + this.ReportType;
        }
    }
}
