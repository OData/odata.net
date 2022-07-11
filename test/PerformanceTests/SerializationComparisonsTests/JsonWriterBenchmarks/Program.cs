//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace JsonWriterBenchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(
                args,
                DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(100)));
        }
    }
}
