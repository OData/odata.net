//---------------------------------------------------------------------
// <copyright file="BdnComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core
{
    public static class Helpers
    {
        public static bool IsBetterOrWorse(this ComparisonConclusion conclusion) =>
            conclusion == ComparisonConclusion.Better || conclusion == ComparisonConclusion.Worse;

        public static bool IsDifferent(this ComparisonConclusion conclusion) =>
            conclusion != ComparisonConclusion.Same && conclusion != ComparisonConclusion.Unkown;

        public static double GetRatio(ComparerResult item) => GetRatio(item.Conclusion, item.BaseResult, item.DiffResult);

        public static double GetRatio(ComparisonConclusion conclusion, MeasurementResult baseResult, MeasurementResult diffResult)
            => conclusion == ComparisonConclusion.Better
                ? baseResult.Result / diffResult.Result
                : diffResult.Result / baseResult.Result;
    }
}
