using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.Core
{
    /// <summary>
    /// Options used by a <see cref="IResultsComparer"/> when
    /// comparing files.
    /// </summary>
    public class ComparerOptions
    {
        /// <summary>
        /// Threshold for Statistical Test. Examples: 5%, 10ms, 100ns, 1s.
        /// </summary>
        public string StatisticalTestThreshold { get; set; }
        /// <summary>
        /// Noise threshold for Statistical Test. The difference for 1.0ns and 1.1ns is 10%, but it's just a noise. Examples: 0.5ns 1ns.
        /// </summary>
        public string NoiseThreshold { get; set; }
        /// <summary>
        /// Display the full benchmark name id. Optional.
        /// </summary>
        public bool FullId { get; set; }
        /// <summary>
        /// Filter the diff to top/bottom N results. Optional.
        /// </summary>
        public int? TopCount { get; set; }
        /// <summary>
        /// Filter the benchmarks by name using glob pattern(s). Optional.
        /// </summary>
        public IEnumerable<string> Filters { get; set; }
    }
}
