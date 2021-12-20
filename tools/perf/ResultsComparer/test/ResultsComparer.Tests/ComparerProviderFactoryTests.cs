using ResultsComparer.Bdn;
using ResultsComparer.Core;
using ResultsComparer.VsProfiler;
using System;
using System.IO;
using Xunit;

namespace ResultsComparer.Tests
{
    public class ComparerProviderFactoryTests
    {
        [Theory]
        [InlineData("bdn", typeof(BdnComparer))]
        [InlineData("vsAllocs", typeof(VsTypeAllocationsComparer))]
        [InlineData("vsFuncAllocs", typeof(VsFunctionAllocationsComparer))]
        public void CreateDefaultPovider_GetById_ReturnsComparerWithSpecifiedId(string comparerId, Type comparerType)
        {
            IResultsComparerProvider provider = ComparerProviderFactory.CreateDefaultProvider();
            IResultsComparer comparer = provider.GetById(comparerId);
            Assert.IsType(comparerType, comparer);
        }

        [Fact]
        public void DefaultProvider_GetById_ThrowsForUnknownId()
        {
            IResultsComparerProvider provider = ComparerProviderFactory.CreateDefaultProvider();
            Assert.Throws<Exception>(() => provider.GetById("unknown"));
        }

        [Theory]
        [InlineData(".json", "{}", typeof(BdnComparer))]
        [InlineData(".txt", "Type\tAllocations\tBytes", typeof(VsTypeAllocationsComparer))]
        [InlineData(".txt", "Name\tTotal (Allocations)\tSelf (Allocations)\tSelf Size Bytes)", typeof(VsFunctionAllocationsComparer))]

        public void CreateDefaultProvider_GetForFile_ReturnsSupportedComparerForFile(string extension, string contents, Type comparerType)
        {
            string path = Path.GetTempFileName();
            File.Delete(path);
            path = $"path{extension}";
            File.WriteAllText(path, contents);
            IResultsComparerProvider provider = ComparerProviderFactory.CreateDefaultProvider();

            IResultsComparer comparer = provider.GetForFile(path);
            Assert.IsType(comparerType, comparer);
            File.Delete(path);
        }

        [Theory]
        [InlineData(".txt", "{}")]
        [InlineData(".txt", "")]
        public void DefaultProvider_GetForFile_ThrowsForUnsupportedFile(string extension, string contents)
        {
            string path = Path.GetTempFileName();
            File.Delete(path);
            path = $"path{extension}";
            File.WriteAllText(path, contents);
            IResultsComparerProvider provider = ComparerProviderFactory.CreateDefaultProvider();

            Assert.Throws<Exception>(() =>
            {
                provider.GetForFile(path);
            });
        }

    }
}
