using Perfolizer.Mathematics.SignificanceTesting;
using ResultsComparer.Bdn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.Core
{
    /// <summary>
    /// Results of a comparision between two files.
    /// </summary>
    public class ComparerResults
    {
        /// <summary>
        /// Indicates that no statisically significant difference
        /// was detected between the two files.
        /// </summary>
        public bool NoDiff { get; set; }

        /// <summary>
        /// The list of results from the comparison. Each item
        /// represent the result for a single test.
        /// </summary>
        public IEnumerable<ComparerResult> Results {get;set;}

    }

    /// <summary>
    /// Comparison result for a single test.
    /// </summary>
    public class ComparerResult
    {
        /// <summary>
        /// The identifier of the test.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The reference measurements for the given test.
        /// </summary>
        public Benchmark BaseResult { get; set; }
        /// <summary>
        /// The diff measurements for the given test.
        /// </summary>
        public Benchmark DiffResult { get; set; }
        /// <summary>
        /// Conclusion specifying whether an improvement, regression, no difference, etc. was detected.
        /// </summary>
        public EquivalenceTestConclusion Conclusion { get; set; }
    }
}
