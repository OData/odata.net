//---------------------------------------------------------------------
// <copyright file="BdnComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ResultsComparer.Core;
using Perfolizer.Mathematics.SignificanceTesting;
using Perfolizer.Mathematics.Thresholds;

namespace ResultsComparer.Bdn
{
    /// <summary>
    /// Results comparer for BenchmarkDotNet full JSON reports.
    /// </summary>
    internal class BdnComparer : IResultsComparer
    {
        private const string FullBdnJsonFileExtension = "full.json";

        public Task<bool> CanReadFile(string path)
        {
            bool isJson = path.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
            return Task.FromResult(isJson);
        }

        public Task<ComparerResults> CompareResults(string basePath, string diffPath, ComparerOptions options)
        {
            if (!Threshold.TryParse(options.StatisticalTestThreshold, out var testThreshold))
            {
                throw new Exception($"Invalid statistical test threshold {options.StatisticalTestThreshold}. Examples: 5%, 10ms, 100ns, 1s.");
            }

            if (!Threshold.TryParse(options.NoiseThreshold, out var noiseThreshold))
            {
                throw new Exception($"Invalid noise threshold {options.NoiseThreshold}. Examples: 0.3ns 1ns.");
            }

            IEnumerable<(string id, Benchmark baseResult, Benchmark diffResult, EquivalenceTestConclusion conclusion)> notSame =
                GetNotSameResults(basePath, diffPath, options, testThreshold, noiseThreshold).ToArray();

            ComparerResults results = new();

            if (!notSame.Any())
            {
                results.NoDiff = true;
                return Task.FromResult(results);
            }

            results.Results = notSame.Select(r =>
                new ComparerResult { Id = r.id, BaseResult = r.baseResult, DiffResult = r.diffResult, Conclusion = r.conclusion });

            return Task.FromResult(results);
        }

        private static IEnumerable<(string id, Benchmark baseResult, Benchmark diffResult, EquivalenceTestConclusion conclusion)> GetNotSameResults(string basePath, string diffPath, ComparerOptions options, Threshold testThreshold, Threshold noiseThreshold)
        {
            foreach ((string id, Benchmark baseResult, Benchmark diffResult) in ReadResults(basePath, diffPath, options)
                .Where(result => result.baseResult.Statistics != null && result.diffResult.Statistics != null)) // failures
            {
                double[] baseValues = baseResult.GetOriginalValues();
                double[] diffValues = diffResult.GetOriginalValues();

                TostResult<MannWhitneyResult> userThresholdResult = StatisticalTestHelper.CalculateTost(MannWhitneyTest.Instance, baseValues, diffValues, testThreshold);
                if (userThresholdResult.Conclusion == EquivalenceTestConclusion.Same)
                    continue;

                TostResult<MannWhitneyResult> noiseResult = StatisticalTestHelper.CalculateTost(MannWhitneyTest.Instance, baseValues, diffValues, noiseThreshold);
                if (noiseResult.Conclusion == EquivalenceTestConclusion.Same)
                    continue;

                yield return (id, baseResult, diffResult, userThresholdResult.Conclusion);
            }
        }

        private static IEnumerable<(string id, Benchmark baseResult, Benchmark diffResult)> ReadResults(string basePath, string diffPath, ComparerOptions options)
        {
            string[] baseFiles = GetFilesToParse(basePath);
            string[] diffFiles = GetFilesToParse(diffPath);

            if (!baseFiles.Any() || !diffFiles.Any())
            {
                throw new ArgumentException($"Provided paths contained no {FullBdnJsonFileExtension} files.");
            }

            IEnumerable<BdnResult> baseResults = baseFiles.Select(ReadFromFile);
            IEnumerable<BdnResult> diffResults = diffFiles.Select(ReadFromFile);

            Regex[] filters = options.Filters.Select(pattern => new Regex(WildcardToRegex(pattern), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).ToArray();

            Dictionary<string, Benchmark> benchmarkIdToDiffResults = diffResults
                .SelectMany(result => result.Benchmarks)
                .Where(benchmarkResult => !filters.Any() || filters.Any(filter => filter.IsMatch(benchmarkResult.FullName)))
                .ToDictionary(benchmarkResult => benchmarkResult.FullName, benchmarkResult => benchmarkResult);

            return baseResults
                .SelectMany(result => result.Benchmarks)
                .ToDictionary(benchmarkResult => benchmarkResult.FullName, benchmarkResult => benchmarkResult) // we use ToDictionary to make sure the results have unique IDs
                .Where(baseResult => benchmarkIdToDiffResults.ContainsKey(baseResult.Key))
                .Select(baseResult => (baseResult.Key, baseResult.Value, benchmarkIdToDiffResults[baseResult.Key]));
        }

        private static string[] GetFilesToParse(string path)
        {
            if (Directory.Exists(path))
                return Directory.GetFiles(path, $"*{FullBdnJsonFileExtension}", SearchOption.AllDirectories);
            else if (File.Exists(path) || !path.EndsWith(FullBdnJsonFileExtension))
                return new[] { path };
            else
                throw new FileNotFoundException($"Provided path does NOT exist or is not a {path} file", path);
        }

        private static BdnResult ReadFromFile(string resultFilePath)
        {
            try
            {
                return JsonSerializer.Deserialize<BdnResult>(File.ReadAllText(resultFilePath));
            }
            catch (Exception)
            {
                Console.WriteLine($"Exception while reading the {resultFilePath} file.");

                throw;
            }
        }

        // https://stackoverflow.com/a/6907849/5852046 not perfect but should work for all we need
        private static string WildcardToRegex(string pattern) => $"^{Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".")}$";
    }
}
