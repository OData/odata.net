using Perfolizer.Mathematics.SignificanceTesting;
using ResultsComparer.Bdn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsComparer.Core
{
    public class ComparerResults
    {
        public bool NoDiff { get; set; }

        public IEnumerable<ComparerResult> Results {get;set;}

    }

    public class ComparerResult
    {
        public string Id { get; set; }
        public Benchmark BaseResult { get; set; }
        public Benchmark DiffResult { get; set; }
        public EquivalenceTestConclusion Conclusion { get; set; }
    }
}
