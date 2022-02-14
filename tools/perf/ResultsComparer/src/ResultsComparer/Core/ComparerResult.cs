//---------------------------------------------------------------------
// <copyright file="ComparerResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core
{
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
        public MeasurementResult BaseResult { get; set; }

        /// <summary>
        /// The diff measurements for the given test.
        /// </summary>
        public MeasurementResult DiffResult { get; set; }

        /// <summary>
        /// Conclusion specifying whether an improvement, regression, no difference, etc. was detected.
        /// </summary>
        public ComparisonConclusion Conclusion { get; set; }
    }
}
