using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.VsProfiler
{
    class VsProfilerAllocation
    {
        public string Type { get; set; }
        public long Allocations { get; set; }
        public long Bytes { get; set; }
    }
}
