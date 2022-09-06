//---------------------------------------------------------------------
// <copyright file="RunStatisticsUpdatedEventArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Event arguments that encapsulate run statistics.
    /// </summary>
    [Serializable]
    public class RunStatisticsUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunStatisticsUpdatedEventArgs"/> class.
        /// </summary>
        /// <param name="statistics">The statistics.</param>
        public RunStatisticsUpdatedEventArgs(RunStatistics statistics)
        {
            ExceptionUtilities.CheckArgumentNotNull(statistics, "statistics");

            this.Failures = statistics.Failures;
            this.Passed = statistics.Passed;
            this.Skipped = statistics.Skipped;
            this.Timeouts = statistics.Timeouts;
            this.Warnings = statistics.Warnings;
        }

        /// <summary>
        /// Gets the number of variations that failed.
        /// </summary>
        public int Failures { get; private set; }

        /// <summary>
        /// Gets the number of variations that passed.
        /// </summary>
        public int Passed { get; private set; }

        /// <summary>
        /// Gets the number of variations that were skipped.
        /// </summary>
        public int Skipped { get; private set; }

        /// <summary>
        /// Gets the number of variations that timed out.
        /// </summary>
        public int Timeouts { get; private set; }

        /// <summary>
        /// Gets the number of variations that had warnings.
        /// </summary>
        public int Warnings { get; private set; }
    }
}
