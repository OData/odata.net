//---------------------------------------------------------------------
// <copyright file="RunStatistics.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;

    /// <summary>
    /// Encapsulates statistics for a particular test run.
    /// </summary>
    [Serializable]
    public class RunStatistics
    {
        /// <summary>
        /// Gets or sets the number of variations that failed.
        /// </summary>
        public int Failures { get; set; }

        /// <summary>
        /// Gets or sets the number of variations that passed.
        /// </summary>
        public int Passed { get; set; }

        /// <summary>
        /// Gets or sets the number of variations that were skipped.
        /// </summary>
        public int Skipped { get; set; }

        /// <summary>
        /// Gets or sets the number of variations that timed out.
        /// </summary>
        public int Timeouts { get; set; }

        /// <summary>
        /// Gets or sets the number of variations that had warnings.
        /// </summary>
        public int Warnings { get; set; }
    }
}
