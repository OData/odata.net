//---------------------------------------------------------------------
// <copyright file="BdnComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core
{
    public static class Helpers
    {
        /// <summary>
        /// Whether the conclusion is indicates that the diff is better or worse than the
        /// base result.
        /// </summary>
        /// <param name="conclusion"></param>
        /// <returns></returns>
        public static bool IsBetterOrWorse(this ComparisonConclusion conclusion) =>
            conclusion == ComparisonConclusion.Better || conclusion == ComparisonConclusion.Worse;

        /// <summary>
        /// Whether the conclusion indicates that the diff result is different from the base result.
        /// </summary>
        /// <param name="conclusion"></param>
        /// <returns></returns>
        public static bool IsDifferent(this ComparisonConclusion conclusion) =>
            conclusion != ComparisonConclusion.Same && conclusion != ComparisonConclusion.Unkown;

        /// <summary>
        /// Gets the diff/base or base/diff ratio from the <see cref="ComparerResult"/>
        /// depending on which is better.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static double GetRatio(ComparerResult item) => GetRatio(item.Conclusion, item.BaseResult, item.DiffResult);

        /// <summary>
        /// Gets the diff/base or base/diff ratio depending on which is better.
        /// </summary>
        /// <param name="conclusion"></param>
        /// <param name="baseResult"></param>
        /// <param name="diffResult"></param>
        /// <returns></returns>
        public static double GetRatio(ComparisonConclusion conclusion, MeasurementResult baseResult, MeasurementResult diffResult)
            => conclusion == ComparisonConclusion.Better
                ? baseResult.Result / diffResult.Result
                : diffResult.Result / baseResult.Result;
    }
}
