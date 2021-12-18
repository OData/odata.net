using ResultsComparer.Core;
using ResultsComparer.VsProfiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ResultsComparer.Tests.VsProfiler
{
    public class VsAllocationsComparerTests
    {
        [Theory]
        [InlineData("Type  Allocations Bytes", true)]
        [InlineData("Type   Allocations", true)]
        [InlineData("Bytes  Type    Allocations", true)]
        [InlineData("", false)]
        [InlineData("Allocations    Bytes", false)]
        [InlineData("sfdsfd1432-osfg=", false)]
        public void CanReadFile_SupportsTextFileThatContainsTypeColumn(string contents, bool isSupported)
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path, contents);

            VsAllocationsComparer comparer = new();
            Assert.Equal(isSupported, comparer.CanReadFile(path));

            File.Delete(path);
        }


        [Fact]
        public void ComparesObjectAllocationsReportFiles()
        {
            string basePath = "Samples/VsProfiler/VsProfilerObjectAllocations.txt";
            string diffPath = "Samples/VsProfiler/VsProfilerObjectAllocations2.txt";

            VsAllocationsComparer comparer = new();
            ComparerOptions options = new();

            ComparerResults results = comparer.CompareResults(basePath, diffPath, options);

            Assert.False(results.NoDiff);
            Dictionary<string, ComparerResult> comparisons = results.Results.ToDictionary(r => r.Id);

            Assert.Equal(6, comparisons.Count);
            // entries with same results are excluded
            Assert.False(comparisons.ContainsKey("System.Collections.Generic.List<>.Enumerator"));
            Assert.False(comparisons.ContainsKey("System.Linq.Enumerable.EnumerablePartition<>"));

            // diffs with smaller allocation values are considered better
            var listComparison = comparisons["System.Collections.Generic.List<>"];
            Assert.Equal(237933, listComparison.BaseResult.Result);
            Assert.Equal(237000, listComparison.DiffResult.Result);
            Assert.Equal(ComparisonConslusion.Better, listComparison.Conclusion);

            var stringComparison = comparisons["System.String"];
            Assert.Equal(128696, stringComparison.BaseResult.Result);
            Assert.Equal(125696, stringComparison.DiffResult.Result);
            Assert.Equal(ComparisonConslusion.Better, stringComparison.Conclusion);

            // diffs with larger allocation values are considered worse
            var intComparison = comparisons["System.Int32"];
            Assert.Equal(115446, intComparison.BaseResult.Result);
            Assert.Equal(125446, intComparison.DiffResult.Result);
            Assert.Equal(ComparisonConslusion.Worse, intComparison.Conclusion);

            // entries that exist in the base but not in the diff are considered missing
            var collectionComparison = comparisons["System.Collections.ObjectModel.Collection<>"];
            Assert.Equal(125001, collectionComparison.BaseResult.Result);
            Assert.Null(collectionComparison.DiffResult);
            Assert.Equal(ComparisonConslusion.Missing, collectionComparison.Conclusion);

            // entries that exist in the diff but not the base are considered new
            var odataPropertyComparison = comparisons["Microsoft.OData.ODataProperty"];
            Assert.Null(odataPropertyComparison.BaseResult);
            Assert.Equal(45000, odataPropertyComparison.DiffResult.Result);
            Assert.Equal(ComparisonConslusion.New, odataPropertyComparison.Conclusion);

            var arrayEnumeratorComparison = comparisons["System.SZGenericArrayEnumerator<>"];
            Assert.Null(arrayEnumeratorComparison.BaseResult);
            Assert.Equal(50147, arrayEnumeratorComparison.DiffResult.Result);
            Assert.Equal(ComparisonConslusion.New, arrayEnumeratorComparison.Conclusion);
        }
    }
}
