﻿using ResultsComparer.Core;
using ResultsComparer.Core.Reporting;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ResultsComparer.Tests
{
    public class ScenarioTests
    {

        public static IEnumerable<object[]> ComparisonReportData =>
            new List<object[]>
            {
                new object[] {
                    "vsAllocs",
                    new ComparerOptions(),
                    "Samples/VsProfiler/VsProfilerObjectAllocations.txt",
                    "Samples/VsProfiler/VsProfilerObjectAllocations2.txt",
                    "Samples/VsProfiler/ObjectAllocationsComparisonReport.md"
                },
                new object[] {
                    "vsAllocs",
                    new ComparerOptions()
                    {
                        Metric = "Bytes"
                    },
                    "Samples/VsProfiler/VsProfilerObjectAllocations.txt",
                    "Samples/VsProfiler/VsProfilerObjectAllocations2.txt",
                    "Samples/VsProfiler/ObjectAllocationsSizeComparisonReport.md"
                },
                new object[] {
                    "vsFuncAllocs",
                    new ComparerOptions(),
                    "Samples/VsProfiler/VsProfilerFunctionAllocations.txt",
                    "Samples/VsProfiler/VsProfilerFunctionAllocations2.txt",
                    "Samples/VsProfiler/FunctionAllocationsComparisonReport.md"
                },
                new object[]
                {
                    "bdn",
                    new ComparerOptions()
                    {
                        StatisticalTestThreshold = "1%",
                        NoiseThreshold = "0.3ns"
                    },
                    "Samples/Bdn/ODataSerialization.json",
                    "Samples/Bdn/ODataSerialization2.json",
                    "Samples/Bdn/ODataSerializationCpuMarkdownReport.md"
                }
            };

        [Theory]
        [MemberData(nameof(ComparisonReportData))]
        public void ComparerReportFilesAndGenerateMarkdownReport(string comparerId, ComparerOptions options, string basePath, string diffPath, string reportPath)
        {
            IResultsComparerProvider comparerProvider = ComparerProviderFactory.CreateDefaultProvider();
            IResultsComparer comparer = comparerProvider.GetById(comparerId);

            ComparerResults results = comparer.CompareResults(basePath, diffPath, options);
            MarkdownReporter reporter = new();
            using MemoryStream output = new();
            reporter.GenerateReport(results, output, options);

            output.Seek(0, SeekOrigin.Begin);
            string expectedReport = File.ReadAllText(reportPath);
            string actualReport = new StreamReader(output).ReadToEnd();
            Assert.Equal(expectedReport, actualReport);
        }
    }
}