//---------------------------------------------------------------------
// <copyright file="ODataReaderFeedTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.IO;
    using global::Xunit;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Xunit.Performance;

    public class ODataReaderFeedTests
    {
        private static readonly IEdmModel Model = TestUtils.GetAdventureWorksModel();
        private static readonly IEdmEntitySet TestEntitySet = Model.EntityContainer.FindEntitySet("Product");
        private static readonly IEdmEntityType TestEntityType = Model.FindDeclaredType("PerformanceServices.Edm.AdventureWorks.Product") as IEdmEntityType;

        [Benchmark]
        [MeasureGCAllocations]
        public void ReadFeed()
        { 
            ReadFeedTestAndMeasure("Entry.json", 1000, true);
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void ReadFeedIncludeSpatial()
        {
            ReadFeedTestAndMeasure("EntryIncludeSpatial.json", 1000, true); 
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void ReadFeedWithExpansions()
        {
            ReadFeedTestAndMeasure("EntryWithExpansions.json", 100, true);
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void ReadFeedIncludeSpatialWithExpansions()
        {
            ReadFeedTestAndMeasure("EntryIncludeSpatialWithExpansions.json", 100, true);
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void ReadFeed_NoValidation()
        {
            ReadFeedTestAndMeasure("Entry.json", 1000, false);
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void ReadFeedIncludeSpatial_NoValidation()
        {
            ReadFeedTestAndMeasure("EntryIncludeSpatial.json", 1000, false);
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void ReadFeedWithExpansions_NoValidation()
        {
            ReadFeedTestAndMeasure("EntryWithExpansions.json", 100, false);
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void ReadFeedIncludeSpatialWithExpansions_NoValidation()
        {
            ReadFeedTestAndMeasure("EntryIncludeSpatialWithExpansions.json", 100, false);
        }

        private void ReadFeedTestAndMeasure(string templateFile, int entryCount, bool isFullValidation)
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (var stream = new MemoryStream(PayloadGenerator.GenerateFeed(templateFile, entryCount)))
                {
                    using (iteration.StartMeasurement())
                    {
                        using (var messageReader = ODataMessageHelper.CreateMessageReader(stream, Model, ODataMessageKind.Response, isFullValidation))
                        {
                            ODataReader feedReader = messageReader.CreateODataResourceSetReader(TestEntitySet, TestEntityType);
                            while (feedReader.Read()) { }
                        }
                    }
                }
            }
        }
    }
}
