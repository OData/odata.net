//---------------------------------------------------------------------
// <copyright file="ComparerResults.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace ResultsComparer.Core
{
    /// <summary>
    /// Results of a comparision between two files.
    /// </summary>
    public class ComparerResults
    {
        /// <summary>
        /// The name of the metric that was compared. This
        /// will be used in the comparison report. E.g: Median (ns), Size (bytes), etc.
        /// </summary>
        public string MetricName { get; set; }

        /// <summary>
        /// The list of results from the comparison. Each item
        /// represent the result for a single test.
        /// </summary>
        public IEnumerable<ComparerResult> Results {get;set;}

    }
}
