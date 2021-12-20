//---------------------------------------------------------------------
// <copyright file="BdnComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;

namespace ResultsComparer.Core.Reporting
{
    /// <summary>
    /// Generates reports from comparison results.
    /// </summary>
    public interface IReporter
    {
        /// <summary>
        /// Generate a report from the results of comparing two files
        /// and outputs the report to the specified <paramref name="destination"/>.
        /// </summary>
        /// <param name="results">The comparison results for which to generate a report.</param>
        /// <param name="destination">Where the report should be written. This stream should not be closed by the reporter.</param>
        /// <param name="options">The options used when making the comparison.</param>
        void GenerateReport(ComparerResults results, Stream destination, ComparerOptions options);
    }
}
