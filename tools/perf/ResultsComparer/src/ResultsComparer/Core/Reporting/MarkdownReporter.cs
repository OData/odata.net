//---------------------------------------------------------------------
// <copyright file="MarkdownReporter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownLog;

namespace ResultsComparer.Core.Reporting
{
    /// <summary>
    /// Generates comparison reports in markdown format.
    /// </summary>
    public class MarkdownReporter : IReporter
    {
        /// <inheritdoc/>
        public void GenerateReport(ComparerResults results, Stream destination, ComparerOptions options, bool leaveStreamOpen = false)
        {
            ComparerResult[] resultsArray = results.Results.ToArray();

            if (resultsArray.Length == 0)
            {
                return;
            }

            using StreamWriter writer = new(destination, leaveOpen: leaveStreamOpen);
            PrintSummary(resultsArray, writer);
            PrintTable(resultsArray, results.MetricName, ComparisonConclusion.Worse, writer, options);
            PrintTable(resultsArray, results.MetricName, ComparisonConclusion.Better, writer, options);
            PrintTable(resultsArray, results.MetricName, ComparisonConclusion.New, writer, options);
            PrintTable(resultsArray, results.MetricName, ComparisonConclusion.Missing, writer, options);
            writer.Flush();
        }

        private static void PrintSummary(ComparerResult[] notSame, StreamWriter writer)

        {
            IEnumerable<ComparerResult> better = notSame.Where(result =>
                result.Conclusion == ComparisonConclusion.Better).ToList();
            IEnumerable<ComparerResult> worse = notSame.Where(result =>
                result.Conclusion == ComparisonConclusion.Worse).ToList();
            int betterCount = better.Count();
            int worseCount = worse.Count();
            int missingCount = notSame.Where(result => result.Conclusion == ComparisonConclusion.Missing).Count();
            int newCount = notSame.Where(result => result.Conclusion == ComparisonConclusion.New).Count();
            int totalCount = betterCount + worseCount + missingCount + newCount;

            // If the baseline doesn't have the same set of tests, you wind up with Infinity in the list of diffs.
            // Exclude them for purposes of geomean.
            worse = worse.Where(x => Helpers.GetRatio(x) != double.PositiveInfinity).ToList();
            better = better.Where(x => Helpers.GetRatio(x) != double.PositiveInfinity).ToList();

            writer.WriteLine("summary:");

            if (betterCount > 0)
            {
                var betterGeoMean = Math.Pow(10,
                    better.Skip(1)
                    .Aggregate(Math.Log10(Helpers.GetRatio(better.First())), (x, y) => x + Math.Log10(Helpers.GetRatio(y))) / better.Count());
                writer.WriteLine($"better: {betterCount}, geomean: {betterGeoMean:F3}");
            }

            if (worseCount > 0)
            {
                var worseGeoMean = Math.Pow(10, worse.Skip(1).Aggregate(Math.Log10(Helpers.GetRatio(worse.First())), (x, y) => x + Math.Log10(Helpers.GetRatio(y))) / worse.Count());
                writer.WriteLine($"worse: {worseCount}, geomean: {worseGeoMean:F3}");
            }

            if (newCount > 0)
            {
                writer.WriteLine($"new (results in the diff that are not in the base): {newCount}");
            }

            if (missingCount > 0)
            {
                writer.WriteLine($"missing (results in the base that are not in the diff): {missingCount}");
            }    

            writer.WriteLine($"total diff: {totalCount}");
            writer.WriteLine();
        }

        private static void PrintTable(ComparerResult[] notSame, string metricName, ComparisonConclusion conclusion, StreamWriter writer, ComparerOptions args)
        {
            var data = notSame
                .Where(result => result.Conclusion == conclusion)
                .OrderByDescending(result => conclusion.IsBetterOrWorse() ?
                    Helpers.GetRatio(conclusion, result.BaseResult, result.DiffResult) : 0)
                .Take(args.TopCount ?? int.MaxValue)
                .Select(result => new
                {
                    Id = (result.Id.Length <= 80 || args.FullId) ? result.Id : result.Id.Substring(0, 80),
                    DisplayValue = conclusion.IsBetterOrWorse() ? Helpers.GetRatio(conclusion, result.BaseResult, result.DiffResult).ToString() : "N/A",
                    BaseMedian = result.BaseResult?.Result,
                    DiffMedian = result.DiffResult?.Result,
                    Modality = conclusion.IsBetterOrWorse() ? GetModalInfo(result.BaseResult) ?? GetModalInfo(result.DiffResult) : "N/A"
                })
                .ToArray();

            if (!data.Any())
            {
                writer.WriteLine($"No {conclusion} results for the provided threshold = {args.StatisticalTestThreshold} and noise filter = {args.NoiseThreshold}.");
                writer.WriteLine();
                return;
            }

            Table table = data
                .ToMarkdownTable()
                .WithHeaders(
                    conclusion.ToString(),
                    conclusion == ComparisonConclusion.Better ? "base/diff" : "diff/base",
                    $"Base {metricName}",
                    $"Diff {metricName}",
                    "Modality");

            foreach (string line in table.ToMarkdown().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            {
                writer.WriteLine($"| {line.TrimStart()}|"); // the table starts with \t and does not end with '|' and it looks bad so we fix it
            }

            writer.WriteLine();
        }

        private static string GetModalInfo(MeasurementResult benchmark)
        {
            if (benchmark == null || benchmark.Modality == Modality.Unknown) // not enough data to tell
            {
                return null;
            }

            if (benchmark.Modality == Modality.Multimodal)
            {
                return "multimodal";
            }

            if (benchmark.Modality == Modality.Bimodal)
            {
                return "bimodal";
            }

            if (benchmark.Modality == Modality.Several)
            {
                return "several?";
            }

            return null;
        }
    }
}
