using ResultsComparer.VsProfiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace ResultsComparer.Tests.VsProfiler
{
    public class VsProfilerReaderTests
    {
        [Fact]
        public void ReadNext_ReadsAllocationsData_AndReturnsFalse_AtEndOfFile()
        {
            string path = "Samples/VsProfiler/VsProfilerObjectAllocations.txt";
            using VsProfilerReader<VsProfilerAllocations> reader = new(new StreamReader(path));

            List<VsProfilerAllocations> expectedValues = new()
            {
                new VsProfilerAllocations()
                {
                    Type = "System.Collections.Generic.List<>",
                    Allocations = 237933,
                    Bytes = 7613856
                },
                new VsProfilerAllocations()
                {
                    Type = "System.String",
                    Allocations = 128696,
                    Bytes = 5339698
                },
                new VsProfilerAllocations()
                {
                    Type = "System.Collections.ObjectModel.Collection<>",
                    Allocations = 125001,
                    Bytes = 3000024
                },
                new VsProfilerAllocations()
                {
                    Type = "System.Collections.Generic.List<>.Enumerator",
                    Allocations = 124285,
                    Bytes = 4971400
                },
                new VsProfilerAllocations()
                {
                    Type = "System.Int32",
                    Allocations = 115446,
                    Bytes = 2770704
                },
                new VsProfilerAllocations()
                {
                    Type = "System.Linq.Enumerable.EnumerablePartition<>",
                    Allocations = 75002,
                    Bytes = 4200112
                }
            };

            List<VsProfilerAllocations> actualValues = new();

            foreach (var expectedValue in expectedValues)
            {
                Assert.True(reader.ReadNext(out var actualValue));
                actualValue.Should().BeEquivalentTo(expectedValue);
            }

            Assert.False(reader.ReadNext(out _));
        }

        [Fact]
        public void Enumerates_Entries_FromAllocationsData()
        {
            string path = "Samples/VsProfiler/VsProfilerObjectAllocations.txt";
            using VsProfilerReader<VsProfilerAllocations> reader = new(new StreamReader(path));

            List<VsProfilerAllocations> expectedValues = new()
            {
                new VsProfilerAllocations()
                {
                    Type = "System.Collections.Generic.List<>",
                    Allocations = 237933,
                    Bytes = 7613856
                },
                new VsProfilerAllocations()
                {
                    Type = "System.String",
                    Allocations = 128696,
                    Bytes = 5339698
                },
                new VsProfilerAllocations()
                {
                    Type = "System.Collections.ObjectModel.Collection<>",
                    Allocations = 125001,
                    Bytes = 3000024
                },
                new VsProfilerAllocations()
                {
                    Type = "System.Collections.Generic.List<>.Enumerator",
                    Allocations = 124285,
                    Bytes = 4971400
                },
                new VsProfilerAllocations()
                {
                    Type = "System.Int32",
                    Allocations = 115446,
                    Bytes = 2770704
                },
                new VsProfilerAllocations()
                {
                    Type = "System.Linq.Enumerable.EnumerablePartition<>",
                    Allocations = 75002,
                    Bytes = 4200112
                }
            };

            List<VsProfilerAllocations> actualValues = reader.ToList();
            actualValues.Should().BeEquivalentTo(expectedValues);
        }
    }
}
