using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportData
{
    public class GEReport : ReportBase
    {
        public GEReport(string type, string hr) : base(type)
        {
            this.HrVersion = hr;
        }

        public string HrVersion { get; set; }

        public override string ToString()
        {
            return base.ToString() + "\nHrVersion : " + this.HrVersion;
        }
    }
}
