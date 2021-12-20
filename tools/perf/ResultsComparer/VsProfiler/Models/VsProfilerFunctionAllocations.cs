using ResultsComparer.Core;

namespace ResultsComparer.VsProfiler.Models
{
    public class VsProfilerFunctionAllocations
    {
        [FieldName("Name")]
        public string Name { get; set; }

        [FieldName("Total (Allocations)")]
        public long TotalAllocations { get; set; }

        [FieldName("Self (Allocations)")]
        public long SelfAllocations { get; set; }

        [FieldName("Self Size (Bytes)")]
        public long Size { get; set; }
    }
}
