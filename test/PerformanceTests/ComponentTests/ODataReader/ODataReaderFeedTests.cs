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
    using Microsoft.OData.Core;
    using Microsoft.Xunit.Performance;

    public class ODataReaderFeedTests
    {
        private static readonly IEdmModel Model = TestUtils.GetAdventureWorksModel();
        private static readonly IEdmEntitySet TestEntitySet = Model.EntityContainer.FindEntitySet("Product");
        private static readonly IEdmEntityType TestEntityType = Model.FindDeclaredType("PerformanceServices.Edm.AdventureWorks.Product") as IEdmEntityType;

        [Benchmark]
        public void ReadBasicEntry_1000()
        { 
            ReadFeedTestAndMeasure("BasicEntry.json", 1000, true);
        }

        [Benchmark]
        public void ReadEntryExcludeSpatial_1000()
        {
            ReadFeedTestAndMeasure("EntryExcludeSpatialData.json", 1000, true); 
        }

        [Benchmark]
        public void ReadEntryWithExpansions_100()
        {
            ReadFeedTestAndMeasure("EntryWithExpansions.json", 100, true);
        }

        [Benchmark]
        public void ReadEntryWithExpansionsExcludeSpatial_100()
        {
            ReadFeedTestAndMeasure("EntryWithExpansionsExcludeSpatial.json", 100, true);
        }

        [Benchmark]
        public void ReadBasicEntryNoValidation_1000()
        {
            ReadFeedTestAndMeasure("BasicEntry.json", 1000, false);
        }

        [Benchmark]
        public void ReadEntryExcludeSpatialDataNoValidation_1000()
        {
            ReadFeedTestAndMeasure("EntryExcludeSpatialData.json", 1000, false);
        }

        [Benchmark]
        public void ReadEntryWithExpansionsNoValidation_100()
        {
            ReadFeedTestAndMeasure("EntryWithExpansions.json", 100, false);
        }

        [Benchmark]
        public void ReadEntryWithExpansionsExcludeSpatialNoValidation_100()
        {
            ReadFeedTestAndMeasure("EntryWithExpansionsExcludeSpatial.json", 100, false);
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
                            ODataReader feedReader = messageReader.CreateODataFeedReader(TestEntitySet, TestEntityType);
                            while (feedReader.Read()) { }
                        }
                    }
                }
            }
        }
    }
}
