using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.Core
{
    public class ComparerOptions
    {
        public string StatisticalTestThreshold { get; set; }
        public string NoiseThreshold { get; set; }
        public bool FullId { get; set; }
        public int? TopCount { get; set; }
        public IEnumerable<string> Filters { get; set; }
    }
}
