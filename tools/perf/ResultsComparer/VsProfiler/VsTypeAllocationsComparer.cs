//---------------------------------------------------------------------
// <copyright file="CommandLineOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace ResultsComparer.VsProfiler
{
    public class VsTypeAllocationsComparer : VsAllocationsComparer<VsProfilerTypeAllocations>
    {
        protected override IDictionary<string, string> MetricNameMap => new Dictionary<string, string>()
        {
            { "Allocations", "Allocations" },
            { "Size", "Size (bytes)" },
            { "Bytes", "Size (bytes)" }
        };

        protected override string DefaultMetric => "Allocations";

        public override bool CanReadFile(string path)
        {
            using var reader = new StreamReader(path);
            string firstLine = reader.ReadLine();
            return firstLine != null
                && firstLine.Contains("Type")
                && (firstLine.Contains("Allocations") || firstLine.Contains("Bytes"));
        }
        protected override string GetItemId(VsProfilerTypeAllocations item)
        {
            return item.Type;
        }

        protected override long GetMetricValue(VsProfilerTypeAllocations allocations, string metric)
        {
            if (metric.Equals("Allocations", StringComparison.OrdinalIgnoreCase))
            {
                return allocations.Allocations;
            }
            else if (metric.Equals("Size", StringComparison.OrdinalIgnoreCase) || metric.Equals("Bytes", StringComparison.OrdinalIgnoreCase))
            {
                return allocations.Bytes;
            }

            throw new Exception($"Unsupported metric {metric} for VS Profiler Allocations Comparer");
        }
    }
}
