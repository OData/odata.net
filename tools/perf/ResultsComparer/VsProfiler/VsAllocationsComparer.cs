//---------------------------------------------------------------------
// <copyright file="CommandLineOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using ResultsComparer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ResultsComparer.VsProfiler
{
    public class VsAllocationsComparer : IResultsComparer
    {
        public bool CanReadFile(string path)
        {
            using var reader = new StreamReader(path);
            string firstLine = reader.ReadLine();
            return firstLine != null && firstLine.Contains("Type");
        }

        public ComparerResults CompareResults(string basePath, string diffPath, ComparerOptions options)
        {
            using IReader<VsProfilerAllocations> baseReader = CreateReader(basePath);
            using IReader<VsProfilerAllocations> diffReader = CreateReader(diffPath);

            // since entries in the base and diff may be ordered differently,
            // let's store them in a dictionaries
            Dictionary<string, VsProfilerAllocations> baseResults = new();
            Dictionary<string, VsProfilerAllocations> diffResults = new();

            foreach (VsProfilerAllocations allocation in baseReader)
            {
                baseResults[allocation.Type] = allocation;
            }

            foreach (VsProfilerAllocations allocation in diffReader)
            {
                diffResults[allocation.Type] = allocation;
            }

            string metric = string.IsNullOrEmpty(options.Metric) ? "Allocations" : options.Metric;

            ComparerResults results = new() {
                MetricName = GetMetricName(metric),
                Results = GetResults(baseResults, diffResults, metric).ToArray()
            };

            return results;
        }

        private IEnumerable<ComparerResult> GetResults(Dictionary<string, VsProfilerAllocations> baseResults, Dictionary<string, VsProfilerAllocations> diffResults, string metric)
        {
            foreach ((string id, VsProfilerAllocations baseAlloc) in baseResults)
            {
                diffResults.TryGetValue(id, out VsProfilerAllocations diffAlloc);

                long baseResult = GetMetricValue(baseAlloc, metric);

                if (diffAlloc == null)
                {
                    yield return new ComparerResult()
                    {
                        Id = id,
                        BaseResult = new MeasurementResult { Result = baseResult },
                        Conclusion = ComparisonConclusion.Missing
                    };

                    continue;
                }

                ComparisonConclusion conclusion;

                // since allocations are exact values and consistent, and since we're dealing with single values
                // we compute the difference directly instead of using statistics helper
                // and we ignore the threshold for now
                // TODO: test using statistical helper and see whether there's a difference
                long diffResult = GetMetricValue(diffAlloc, metric);
                conclusion = diffResult > baseResult ? ComparisonConclusion.Worse :
                    diffResult < baseResult ? ComparisonConclusion.Better :
                    ComparisonConclusion.Same;
          
                // skip same results
                if (conclusion == ComparisonConclusion.Same)
                {
                    continue;
                }

                yield return new ComparerResult
                {
                    Id = id,
                    BaseResult = new MeasurementResult { Result = baseResult },
                    DiffResult = new MeasurementResult { Result = diffResult },
                    Conclusion = conclusion
                };
            }

            // find new entries in diff that are not in the base results
            foreach ((string id, VsProfilerAllocations diffAllocs) in diffResults)
            {
                if (baseResults.ContainsKey(id))
                {
                    continue;
                }

                long diffResult = GetMetricValue(diffAllocs, metric);

                yield return new ComparerResult
                {
                    Id = id,
                    DiffResult = new MeasurementResult { Result = diffResult },
                    Conclusion = ComparisonConclusion.New
                };
            }
        }

        static IReader<VsProfilerAllocations> CreateReader(string path)
        {
            var textReader = new StreamReader(File.OpenRead(path));
            return new VsProfilerReader<VsProfilerAllocations>(textReader);
        }

        private static long GetMetricValue(VsProfilerAllocations allocations, string metric)
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
        
        private static string GetMetricName(string metric)
        {
            if (metric.Equals("Allocations", StringComparison.OrdinalIgnoreCase))
            {
                return "Allocations";
            }
            else if (metric.Equals("Size", StringComparison.OrdinalIgnoreCase) || metric.Equals("Bytes", StringComparison.OrdinalIgnoreCase))
            {
                return "Size (bytes)";
            }

            return metric;
        }
    }
}
