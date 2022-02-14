//---------------------------------------------------------------------
// <copyright file="ComparisonConclusion.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core
{
    /// <summary>
    /// Represents the conclusion of a comparison between the base and diff reports
    /// for a single data point.
    /// </summary>
    public enum ComparisonConclusion
    {
        /// <summary>
        /// The base and diff reports have the same result or within the configured threshold
        /// for the measured metric.
        /// </summary>
        Same,

        /// <summary>
        /// The diff measurement is better than the base.
        /// </summary>
        Better,

        /// <summary>
        /// The diff measurement is worse than the base.
        /// </summary>
        Worse,

        /// <summary>
        /// The measured data point is new in the diff (does not exist in the base).
        /// </summary>
        New,

        /// <summary>
        /// The measured data point is missing in the diff (exists in the base but not the diff).
        /// </summary>
        Missing,

        /// <summary>
        /// Unknown conclusion.
        /// </summary>
        Unknown
    }
}
