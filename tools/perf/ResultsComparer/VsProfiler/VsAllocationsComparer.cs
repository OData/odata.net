using ResultsComparer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResultsComparer.VsProfiler
{
    class VsAllocationsComparer : IResultsComparer
    {
        public Task<bool> CanReadFile(string path)
        {
            throw new NotImplementedException();
        }

        public Task<ComparerResults> CompareResults(string basePath, string diffPath, ComparerOptions options)
        {
            using IReader<VsProfilerAllocation> baseReader = CreateReader(basePath);
            using IReader<VsProfilerAllocation> diffReader = CreateReader(diffPath);

            // since entries in the base and diff may be ordered differently,
            // let's store them in a dictionaries
            Dictionary<string, VsProfilerAllocation> baseResults = new();
            Dictionary<string, VsProfilerAllocation> diffResults = new();

            foreach (VsProfilerAllocation allocation in baseReader)
            {
                baseResults[allocation.Type] = allocation;
            }

            foreach (VsProfilerAllocation allocation in diffReader)
            {
                baseResults[allocation.Type] = allocation;
            }

            ComparerResults results = new();
            results.Results = GetResults(baseResults, diffResults).ToArray();
            results.NoDiff = results.Results.Any();
        }

        private IEnumerable<ComparerResult> GetResults(Dictionary<string, VsProfilerAllocation> baseResults, Dictionary<string, VsProfilerAllocation> diffResults)
        {
            foreach ((string id, VsProfilerAllocation baseAlloc) in baseResults)
            {
                VsProfilerAllocation diffAlloc = null;
                diffResults.TryGetValue(id, out diffAlloc);

                // TODO: now we're assuming "Allocations" are to be compared. We should allow user to configure
                // which measurement to compare (i.e. bytes, allocations, etc.)
                long baseResult = baseAlloc.Allocations;

                if (diffAlloc == null)
                {
                    yield return new ComparerResult()
                    {
                        Id = id,
                        BaseResult = new MeasurementResult { Result = baseResult }
                    };
                }

                ComparisonConslusion conclusion;

                // since allocations are exact values and consistent, and since we're dealing with single values
                // we compute the difference directly instead of using statistics helper
                // and we ignore the threshold for now
                // TODO: test using statistical helper and see whether there's a difference
                long diffResult = diffAlloc.Allocations;
                conclusion = diffResult > baseResult ? ComparisonConslusion.Worse :
                    diffResult < baseResult ? ComparisonConslusion.Better :
                    ComparisonConslusion.Same;
          
                // skip same results
                if (conclusion == ComparisonConslusion.Same)
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
            foreach ((string id, VsProfilerAllocation diffAllocs) in diffResults)
            {
                if (baseResults.ContainsKey(id))
                {
                    continue;
                }

                long diffResult = diffAllocs.Allocations;

                yield return new ComparerResult
                {
                    Id = id,
                    DiffResult = new MeasurementResult { Result = diffResult }
                };
            }
        }

        interface IReader<T>: IDisposable, IEnumerable<T>
        {
            T ReadNext();
        }

        IReader<VsProfilerAllocation> CreateReader(string path)
        {
            return default(IReader<VsProfilerAllocation>);
        }
    }
}
