using ResultsComparer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.VsProfiler
{
    public abstract class VsAllocationsComparer<T> : IResultsComparer where T: new()
    {
        protected abstract IDictionary<string, string> MetricNameMap { get; }
        protected abstract string DefaultMetric { get; } 

        protected abstract string GetItemId(T item);
        protected abstract long GetMetricValue(T item, string metric);
        public abstract bool CanReadFile(string filePath);

        public ComparerResults CompareResults(string basePath, string diffPath, ComparerOptions options)
        {
            using IReader<T> baseReader = CreateReader(basePath);
            using IReader<T> diffReader = CreateReader(diffPath);

            // since entries in the base and diff may be ordered differently,
            // let's store them in a dictionaries
            Dictionary<string, T> baseResults = new();
            Dictionary<string, T> diffResults = new();

            foreach (T allocation in baseReader)
            {
                baseResults[GetItemId(allocation)] = allocation;
            }

            foreach (T allocation in diffReader)
            {
                diffResults[GetItemId(allocation)] = allocation;
            }

            string metric = string.IsNullOrEmpty(options.Metric) ? DefaultMetric : options.Metric;

            ComparerResults results = new()
            {
                MetricName = GetMetricName(metric),
                Results = GetResults(baseResults, diffResults, metric).ToArray()
            };

            return results;
        }

        private IEnumerable<ComparerResult> GetResults(Dictionary<string, T> baseResults, Dictionary<string, T> diffResults, string metric)
        {
            foreach ((string id, T baseAlloc) in baseResults)
            {
                diffResults.TryGetValue(id, out T diffAlloc);

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
            foreach ((string id, T diffAllocs) in diffResults)
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

        static IReader<T> CreateReader(string path)
        {
            var textReader = new StreamReader(File.OpenRead(path));
            return new VsProfilerReader<T>(textReader);
        }

        private string GetMetricName(string metric)
        {
            if (MetricNameMap.TryGetValue(metric, out string displayName))
            {
                return displayName;
            }

            if (MetricNameMap.TryGetValue(metric.ToLower(), out displayName))
            {
                return displayName;
            }

            return metric;
        }
    }
}
