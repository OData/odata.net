//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using CommandLine;
using ResultsComparer.Core;
using System.IO;
using ResultsComparer.Core.Reporting;

namespace ResultsComparer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // we print a lot of numbers here and we want to make it always in invariant way
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            IResultsComparerProvider comparerProvider = ComparerProviderFactory.CreateDefaultProvider();
            IReporterProvider reporterProvider = new ReporterProvider();

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed((options) => Compare(options, comparerProvider, reporterProvider));
        }

        private static void Compare(CommandLineOptions args, IResultsComparerProvider comparerProvider, IReporterProvider reporterProvider)
        {
            try
            {
                IResultsComparer comparer = string.IsNullOrEmpty(args.Comparer) ?
                   comparerProvider.GetForFile(args.BasePath) : comparerProvider.GetById(args.Comparer);

                Console.WriteLine($"Comparer selected: {comparer.Name}\n");


                ComparerOptions options = new()
                {
                    StatisticalTestThreshold = args.StatisticalTestThreshold,
                    NoiseThreshold = args.NoiseThreshold,
                    FullId = args.FullId,
                    TopCount = args.TopCount,
                    Filters = args.Filters,
                    Metric = args.Metric
                };

                ComparerResults results = comparer.CompareResults(args.BasePath, args.DiffPath, options);

                IReporter reporter = reporterProvider.GetDefault();
                Stream output = Console.OpenStandardOutput();
                reporter.GenerateReport(results, output, options, leaveStreamOpen: true);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
