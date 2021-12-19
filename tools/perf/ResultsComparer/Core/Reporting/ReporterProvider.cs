using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.Core.Reporting
{
    public class ReporterProvider : IReporterProvider
    {
        private readonly IReporter _default = new MarkdownReporter();

        public IReporter GetDefault()
        {
            return _default;
        }
    }
}
