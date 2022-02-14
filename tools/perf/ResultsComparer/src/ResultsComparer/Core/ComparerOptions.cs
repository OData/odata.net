//---------------------------------------------------------------------
// <copyright file="ComparerOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace ResultsComparer.Core
{
    /// <summary>
    /// Options used by a <see cref="IResultsComparer"/> when
    /// comparing files.
    /// </summary>
    public class ComparerOptions
    {
        /// <summary>
        /// The metric to compare. If this is not set,
        /// the comparer will pick a default metric for that report type.
        /// </summary>
        public string Metric { get; set; }

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
