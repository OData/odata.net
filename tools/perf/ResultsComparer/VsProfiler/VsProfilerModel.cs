using ResultsComparer.Core;

namespace ResultsComparer.VsProfiler
{
    class VsProfilerAllocations
    {
        [FieldName("Type")]
        public string Type { get; set; }
        [FieldName("Allocations")]
        public long Allocations { get; set; }
        [FieldName("Bytes")]
        public long Bytes { get; set; }
    }
}
