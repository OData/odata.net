using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.Core.Reporting
{
    public interface IReporter
    {
        void GenerateReport(ComparerResults results, Stream destination, ComparerOptions options);
    }
}
