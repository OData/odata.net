//---------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core
{
    /// <summary>
    /// Contains static helper methods for working with comparison results.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Whether the conclusion is indicates that the diff is better or worse than the
        /// base result.
        /// </summary>
        /// <param name="conclusion">The conclusion to test.</param>
        /// <returns>Whether the conclusion indicates that the the diff is either better or worse than the base.</returns>
        public static bool IsBetterOrWorse(this ComparisonConclusion conclusion) =>
            conclusion == ComparisonConclusion.Better || conclusion == ComparisonConclusion.Worse;

        /// <summary>
        /// Whether the conclusion indicates that the diff result is different from the base result.
        /// </summary>
        /// <param name="conclusion">The conclusion to test.</param>
        /// <returns>Whether the conclusion indicates that the diff is neither better or worse than the base (could be same or unknown).</returns>
        public static bool IsDifferent(this ComparisonConclusion conclusion) =>
            conclusion != ComparisonConclusion.Same && conclusion != ComparisonConclusion.Unknown;

        /// <summary>
        /// Gets the diff/base or base/diff ratio from the <see cref="ComparerResult"/>
        /// depending on which is better.
        /// </summary>
        /// <param name="item">The result item for which to compute the ratio.</param>
        /// <returns>The ratio between worse result and the better result.</returns>
        public static double GetRatio(ComparerResult item) => GetRatio(item.Conclusion, item.BaseResult, item.DiffResult);

        /// <summary>
        /// Gets the diff/base or base/diff ratio depending on which is better.
        /// </summary>
        /// <param name="conclusion">The conclusion of the result.</param>
        /// <param name="baseResult">The measured result of the base report.</param>
        /// <param name="diffResult">The measured result of the diff report.</param>
        /// <returns>The ratio between the worse result and the better result.</returns>
        public static double GetRatio(ComparisonConclusion conclusion, MeasurementResult baseResult, MeasurementResult diffResult)
            => conclusion == ComparisonConclusion.Better
                ? baseResult.Result / diffResult.Result
                : diffResult.Result / baseResult.Result;
    }
}
