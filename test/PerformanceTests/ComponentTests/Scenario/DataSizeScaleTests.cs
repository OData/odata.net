//---------------------------------------------------------------------
// <copyright file="DataSizeScaleTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.IO;
    using System.Linq;
    using global::Xunit;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Xunit.Performance;

    public class DataSizeScaleTests : WriteReadFeedTestBase
    {
        private static readonly IEdmModel AdventureWorksModel = TestUtils.GetAdventureWorksModel();
        private static readonly IEdmEntitySet TestEntitySet = AdventureWorksModel.EntityContainer.FindEntitySet("PropertyTest");
        private static readonly IEdmEntityType TestEntityType = AdventureWorksModel.FindDeclaredType("PerformanceServices.Edm.AdventureWorks.PropertyTestType") as IEdmEntityType;
        private const int BaseNumberOfEntries = 5;
        private const int MaxStreamSize = 8 * 1024;
        private static Stream WriteStream = new MemoryStream(MaxStreamSize);

        [Benchmark]
        public void WriteFeedDataSize_4MB()
        {
            long numberOfEntries = 0;
            int dataSizeKb = 4 * 1024;

            WriteStream.SetLength(0);

            var entry = CreateEntry();
            if (dataSizeKb != 0)
            {
                long basePayLoadLength = WriteFeed(WriteStream, AdventureWorksModel, BaseNumberOfEntries, entry, TestEntitySet);
                numberOfEntries = (dataSizeKb * 1024) * BaseNumberOfEntries / basePayLoadLength;
            }

            foreach (var iteration in Benchmark.Iterations)
            {
                WriteStream.SetLength(0);
                using (iteration.StartMeasurement())
                {
                    WriteFeed(WriteStream, AdventureWorksModel, numberOfEntries, entry, TestEntitySet);
                }
            }
        }
        
        [Benchmark]
        public void WriteFeedDataSize_8MB()
        {
            long numberOfEntries = 0;
            int dataSizeKb = 8 * 1024;

            WriteStream.SetLength(0);

            var entry = CreateEntry();
            if (dataSizeKb != 0)
            {
                long basePayLoadLength = WriteFeed(WriteStream, AdventureWorksModel, BaseNumberOfEntries, entry, TestEntitySet);
                numberOfEntries = (dataSizeKb * 1024) * BaseNumberOfEntries / basePayLoadLength;
            }

            foreach (var iteration in Benchmark.Iterations)
            {
                WriteStream.SetLength(0);
                using (iteration.StartMeasurement())
                {
                    WriteFeed(WriteStream, AdventureWorksModel, numberOfEntries, entry, TestEntitySet);
                }
            }
        }

        [Benchmark]
        public void ReadFeedDataSize_4MB()
        {
            long numberOfEntries = 0;
            int dataSizeKb = 4 * 1024;

            WriteStream.SetLength(0);

            var entry = CreateEntry();
            if (dataSizeKb != 0)
            {
                long basePayLoadLength = WriteFeed(WriteStream, AdventureWorksModel, BaseNumberOfEntries, entry, TestEntitySet);
                numberOfEntries = (dataSizeKb * 1024) * BaseNumberOfEntries / basePayLoadLength;
            }

            WriteStream.SetLength(0);
            WriteFeed(WriteStream, AdventureWorksModel, numberOfEntries, entry, TestEntitySet);

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    ReadFeed(WriteStream, AdventureWorksModel, TestEntitySet, TestEntityType);
                }
            }
        }

        [Benchmark]
        public void ReadFeedDataSize_8MB()
        {
            long numberOfEntries = 0;
            int dataSizeKb = 8 * 1024;

            WriteStream.SetLength(0);

            var entry = CreateEntry();
            if (dataSizeKb != 0)
            {
                long basePayLoadLength = WriteFeed(WriteStream, AdventureWorksModel, BaseNumberOfEntries, entry, TestEntitySet);
                numberOfEntries = (dataSizeKb * 1024) * BaseNumberOfEntries / basePayLoadLength;
            }

            WriteStream.SetLength(0);
            WriteFeed(WriteStream, AdventureWorksModel, numberOfEntries, entry, TestEntitySet);

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    ReadFeed(WriteStream, AdventureWorksModel, TestEntitySet, TestEntityType);
                }
            }
        }

        private ODataEntry CreateEntry()
        {
            var entry = new ODataEntry
            {
                Id = new Uri("http://www.odata.org/Perf.svc/PropertyTest(1)"),
                EditLink = new Uri("PropertyTest(1)", UriKind.Relative),
                ReadLink = new Uri("PropertyTest(1)", UriKind.Relative),
                TypeName = "PerformanceServices.Edm.AdventureWorks.PropertyTestType",
                Properties = new[]
                                {
                                    new ODataProperty{ Name = "PropertyOne", Value = new ODataComplexValue
                                    {
                                        TypeName = "PerformanceServices.Edm.AdventureWorks.ManyPropertiedComplexType",
                                        Properties = new[]
                                        {
                                            new ODataProperty{ Name = "Property1", Value = new byte[] {0, 1, 2, 3, 4, 5, 6, 7}}, 
                                            new ODataProperty{ Name = "Property2", Value = true}, 
                                            new ODataProperty{ Name = "Property3", Value = (byte)1},
                                            new ODataProperty{ Name = "Property4", Value = new DateTimeOffset(1987, 6, 5, 4, 3, 21, 0, new TimeSpan(0, 0, 3, 0))},
                                            new ODataProperty{ Name = "Property5", Value = Decimal.MaxValue},
                                            new ODataProperty{ Name = "Property13", Value = new Guid("00005259-2341-5431-5432-234234234234")},
                                            new ODataProperty{ Name = "Property14", Value = Int16.MaxValue},
                                            new ODataProperty{ Name = "Property15", Value = Int32.MaxValue},
                                            new ODataProperty{ Name = "Property16", Value = Int64.MaxValue},
                                            new ODataProperty{ Name = "Property17", Value = (Single)5.4},
                                            new ODataProperty{ Name = "Property18", Value = "Hello World"},
                                            new ODataProperty{ Name = "Property19", Value = new TimeSpan(1987, 6, 54, 32)},
                                            new ODataProperty{ Name = "CollectionProperty1", Value = new ODataCollectionValue { TypeName = "Collection(Binary)", Items = Enumerable.Range(0, 10).Select(n => new byte[] {0, 1, 2, 3, 4, 5, 6, 7})}},
                                            new ODataProperty{ Name = "CollectionProperty2", Value = new ODataCollectionValue { TypeName = "Collection(Boolean)", Items = Enumerable.Range(0, 10).Select(n => true)}},
                                            new ODataProperty{ Name = "CollectionProperty3", Value = new ODataCollectionValue { TypeName = "Collection(Byte)", Items = Enumerable.Range(0, 10).Select(n => (byte)1)}},
                                            new ODataProperty{ Name = "CollectionProperty4", Value = new ODataCollectionValue { TypeName = "Collection(DateTimeOffset)", Items = Enumerable.Range(0, 10).Select(n => new DateTimeOffset(1987, 6, 5, 4, 3, 21, 0, new TimeSpan(0, 0, 3, 0)))}},
                                            new ODataProperty{ Name = "CollectionProperty5", Value = new ODataCollectionValue { TypeName = "Collection(Decimal)", Items = Enumerable.Range(0, 10).Select(n => Decimal.MaxValue)}},
                                            new ODataProperty{ Name = "CollectionProperty13", Value = new ODataCollectionValue { TypeName = "Collection(Guid)", Items = Enumerable.Range(0, 10).Select(n => new Guid("00005259-2341-5431-5432-234234234234"))}},
                                            new ODataProperty{ Name = "CollectionProperty14", Value = new ODataCollectionValue { TypeName = "Collection(Int16)", Items = Enumerable.Range(0, 10).Select(n => Int16.MaxValue)}},
                                            new ODataProperty{ Name = "CollectionProperty15", Value = new ODataCollectionValue { TypeName = "Collection(Int32)", Items = Enumerable.Range(0, 10).Select(n => Int32.MaxValue)}},
                                            new ODataProperty{ Name = "CollectionProperty16", Value = new ODataCollectionValue { TypeName = "Collection(Int64)", Items = Enumerable.Range(0, 10).Select(n => Int64.MaxValue)}},
                                            new ODataProperty{ Name = "CollectionProperty17", Value = new ODataCollectionValue { TypeName = "Collection(Single)", Items = Enumerable.Range(0, 10).Select(n => (Single)5.4)}},
                                            new ODataProperty{ Name = "CollectionProperty18", Value = new ODataCollectionValue { TypeName = "Collection(String)", Items = Enumerable.Range(0, 10).Select(n => "Hello World")}},
                                            new ODataProperty{ Name = "CollectionProperty19", Value = new ODataCollectionValue { TypeName = "Collection(Duration)", Items = Enumerable.Range(0, 10).Select(n => new TimeSpan(1987, 6, 54, 32))}},
                                            new ODataProperty{ Name = "ComplexProperty2", Value = new ODataComplexValue
                                            {
                                                TypeName = "PerformanceServices.Edm.AdventureWorks.TimeZone",
                                                Properties = new[]
                                                {
                                                    new ODataProperty { Name = "Region", Value = GeographyFactory.Polygon().Ring(33.1, -110).LineTo(1,2).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(0.03, -0.01).LineTo(45.23, 23.10).Build() },
                                                    new ODataProperty { Name = "Offset", Value = new DateTimeOffset(1987, 6, 5, 4, 3, 21, 0, new TimeSpan(0, 0, 3, 0)) },
                                                    new ODataProperty { Name = "StartTime", Value = TimeSpan.MaxValue },
                                                }
                                            }},
                                            new ODataProperty { Name = "CollectionComplexProperty2", Value = new ODataCollectionValue
                                            {
                                                TypeName = "Collection(PerformanceServices.Edm.AdventureWorks.TimeZone)",
                                                Items = Enumerable.Range(0, 10).Select(n => new ODataComplexValue
                                                {
                                                    TypeName = "PerformanceServices.Edm.AdventureWorks.TimeZone",
                                                    Properties = new[]
                                                    {
                                                        new ODataProperty { Name = "Region", Value = GeographyFactory.Polygon().Ring(33.1, -110).LineTo(1,2).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(0.03, -0.01).LineTo(45.23, 23.10).Build() },
                                                        new ODataProperty { Name = "Offset", Value = new DateTimeOffset(1987, 6, 5, 4, 3, 21, 0, new TimeSpan(0, 0, 3, 0)) },
                                                        new ODataProperty { Name = "StartTime", Value = TimeSpan.MaxValue },
                                                    }
                                                })
                                            }}
                                        }
                                    }}
                                },
            };

            return entry;
        }
    }
}
