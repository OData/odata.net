using ResultsComparer.VsProfiler.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.VsProfiler
{
    public class VsFunctionAllocationsComparer : VsAllocationsComparer<VsProfilerFunctionAllocations>
    {
        protected override IDictionary<string, string> MetricNameMap => new Dictionary<string, string>()
        {
            { "Total", "Total Allocations" },
            { "Self", "Self Allocations" },
            { "Size", "Self Size (bytes)" },
            { "Bytes", "Self Size (bytes)" }
        };

        protected override string DefaultMetric => "Self";

        public override bool CanReadFile(string filePath)
        {
            using var reader = new StreamReader(filePath);
            string firstLine = reader.ReadLine();
            return firstLine != null
                && firstLine.Contains("Name")
                && (firstLine.Contains("Allocations") || firstLine.Contains("Bytes"));
        }

        protected override string GetItemId(VsProfilerFunctionAllocations item)
        {
            return item.Name;
        }

        protected override long GetMetricValue(VsProfilerFunctionAllocations item, string metric)
        {
            if (metric.Equals("Total", StringComparison.OrdinalIgnoreCase))
            {
                return item.TotalAllocations;
            }
            else if (metric.Equals("Self", StringComparison.OrdinalIgnoreCase))
            {
                return item.SelfAllocations;
            }
            else if (metric.Equals("Size", StringComparison.OrdinalIgnoreCase) || metric.Equals("Bytes", StringComparison.OrdinalIgnoreCase))
            {
                return item.Size;
            }

            throw new Exception($"Unsupported metric {metric} for VS Profiler Allocations Comparer");
        }
    }
}
