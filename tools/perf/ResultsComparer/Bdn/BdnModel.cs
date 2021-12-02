//---------------------------------------------------------------------
// <copyright file="BdnModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace ResultsComparer.Bdn
{
    public class Statistics
    {
        public int N { get; set; }
        public double Median { get; set; }
        public List<double> OriginalValues { get; set; }
    }

    public class Measurement
    {
        public string IterationStage { get; set; }
        public long Operations { get; set; }
        public double Nanoseconds { get; set; }
    }

    public class Benchmark
    {
        public string FullName { get; set; }
        public Statistics Statistics { get; set; }
        public List<Measurement> Measurements { get; set; }
        internal double[] GetOriginalValues()
        {
            if (Measurements != null)
            {
                return Measurements
                .Where(measurement => measurement.IterationStage == "Result")
                .Select(measurement => measurement.Nanoseconds / measurement.Operations)
                .ToArray();
            }
            else
            {
                return Statistics.OriginalValues.ToArray();
            }
        }
    }

    public class BdnResult
    {
        public List<Benchmark> Benchmarks { get; set; }
    }
}
