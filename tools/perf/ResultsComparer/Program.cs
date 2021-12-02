// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Perfolizer.Mathematics.Multimodality;
using Perfolizer.Mathematics.SignificanceTesting;
using Perfolizer.Mathematics.Thresholds;
using CommandLine;
using MarkdownLog;
using Newtonsoft.Json;
using ResultsComparer.Core;
using ResultsComparer.Bdn;
using System.Threading.Tasks;

namespace ResultsComparer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // we print a lot of numbers here and we want to make it always in invariant way
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed((options) => Compare(options).Wait());
        }

        private static async Task Compare(CommandLineOptions args)
        {
            IResultsComparer comparer = new BdnComparer();

            try
            {
                ComparerOptions options = new ComparerOptions
                {
                    StatisticalTestThreshold = args.StatisticalTestThreshold,
                    NoiseThreshold = args.NoiseThreshold,
                    FullId = args.FullId,
                    TopCount = args.TopCount,
                    Filters = args.Filters
                };

                var results = await comparer.CompareResults(args.BasePath, args.DiffPath, options);

                if (results.NoDiff)
                {
                    return;
                }

                var resultsArray = results.Results.ToArray();
                PrintSummary(resultsArray);

                PrintTable(resultsArray, EquivalenceTestConclusion.Slower, args);
                PrintTable(resultsArray, EquivalenceTestConclusion.Faster, args);

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }
        private static void PrintSummary(ComparerResult[] notSame)
        {
            var better = notSame.Where(result => result.Conclusion == EquivalenceTestConclusion.Faster);
            var worse = notSame.Where(result => result.Conclusion == EquivalenceTestConclusion.Slower);
            var betterCount = better.Count();
            var worseCount = worse.Count();

            // If the baseline doesn't have the same set of tests, you wind up with Infinity in the list of diffs.
            // Exclude them for purposes of geomean.
            worse = worse.Where(x => GetRatio(x) != double.PositiveInfinity);
            better = better.Where(x => GetRatio(x) != double.PositiveInfinity);

            Console.WriteLine("summary:");

            if (betterCount > 0)
            {
                var betterGeoMean = Math.Pow(10, better.Skip(1).Aggregate(Math.Log10(GetRatio(better.First())), (x, y) => x + Math.Log10(GetRatio(y))) / better.Count());
                Console.WriteLine($"better: {betterCount}, geomean: {betterGeoMean:F3}");
            }

            if (worseCount > 0)
            {
                var worseGeoMean = Math.Pow(10, worse.Skip(1).Aggregate(Math.Log10(GetRatio(worse.First())), (x, y) => x + Math.Log10(GetRatio(y))) / worse.Count());
                Console.WriteLine($"worse: {worseCount}, geomean: {worseGeoMean:F3}");
            }

            Console.WriteLine($"total diff: {notSame.Count()}");
            Console.WriteLine();
        }

        private static void PrintTable(ComparerResult[] notSame, EquivalenceTestConclusion conclusion, CommandLineOptions args)
        {
            var data = notSame
                .Where(result => result.Conclusion == conclusion)
                .OrderByDescending(result => GetRatio(conclusion, result.BaseResult, result.DiffResult))
                .Take(args.TopCount ?? int.MaxValue)
                .Select(result => new
                {
                    Id = (result.Id.Length <= 80 || args.FullId) ? result.Id : result.Id.Substring(0, 80),
                    DisplayValue = GetRatio(conclusion, result.BaseResult, result.DiffResult),
                    BaseMedian = result.BaseResult.Statistics.Median,
                    DiffMedian = result.DiffResult.Statistics.Median,
                    Modality = GetModalInfo(result.BaseResult) ?? GetModalInfo(result.DiffResult)
                })
                .ToArray();

            if (!data.Any())
            {
                Console.WriteLine($"No {conclusion} results for the provided threshold = {args.StatisticalTestThreshold} and noise filter = {args.NoiseThreshold}.");
                Console.WriteLine();
                return;
            }

            var table = data.ToMarkdownTable().WithHeaders(conclusion.ToString(), conclusion == EquivalenceTestConclusion.Faster ? "base/diff" : "diff/base", "Base Median (ns)", "Diff Median (ns)", "Modality");

            foreach (var line in table.ToMarkdown().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                Console.WriteLine($"| {line.TrimStart()}|"); // the table starts with \t and does not end with '|' and it looks bad so we fix it

            Console.WriteLine();
        }

        // code and magic values taken from BenchmarkDotNet.Analysers.MultimodalDistributionAnalyzer
        // See http://www.brendangregg.com/FrequencyTrails/modes.html
        private static string GetModalInfo(Benchmark benchmark)
        {
            if (benchmark.Statistics.N < 12) // not enough data to tell
                return null;

            double mValue = MValueCalculator.Calculate(benchmark.GetOriginalValues());
            if (mValue > 4.2)
                return "multimodal";
            else if (mValue > 3.2)
                return "bimodal";
            else if (mValue > 2.8)
                return "several?";

            return null;
        }

        private static double GetRatio(ComparerResult item) => GetRatio(item.Conclusion, item.BaseResult, item.DiffResult);

        private static double GetRatio(EquivalenceTestConclusion conclusion, Benchmark baseResult, Benchmark diffResult)
            => conclusion == EquivalenceTestConclusion.Faster
                ? baseResult.Statistics.Median / diffResult.Statistics.Median
                : diffResult.Statistics.Median / baseResult.Statistics.Median;
    }
}
