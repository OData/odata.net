using ResultsComparer.VsProfiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                }
            };

            List<VsProfilerAllocations> actualValues = new();

            Assert.True(reader.ReadNext(out var value1));
            value1.Should().BeEquivalentTo(new VsProfilerAllocations()
            {
                Type = "System.Collections.Generic.List<>",
                Allocations = 237933,
                Bytes = 7613856
            });

            Assert.True(reader.ReadNext(out var value2));
            value2.Should().BeEquivalentTo(new VsProfilerAllocations()
            {
                Type = "System.String",
                Allocations = 128696,
                Bytes = 5339698
            });

            Assert.True(reader.ReadNext(out var value3));
            value3.Should().BeEquivalentTo(new VsProfilerAllocations()
            {
                Type = "System.Collections.ObjectModel.Collection<>",
                Allocations = 125001,
                Bytes = 3000024
            });

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
                }
            };

            List<VsProfilerAllocations> actualValues = reader.ToList();
            actualValues.Should().BeEquivalentTo(expectedValues);
        }
    }
}
