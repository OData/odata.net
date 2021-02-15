//---------------------------------------------------------------------
// <copyright file="ODataReaderFeedTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System.IO;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using BenchmarkDotNet.Attributes;

    /// <summary>
    /// Measures the performance and memory usage of ODataReader
    /// when reading json payloads into a memory stream.
    /// </summary>
    [MemoryDiagnoser]
    public class ODataReaderFeedTests
    {
        private static readonly IEdmModel Model = TestUtils.GetAdventureWorksModel();
        private static readonly IEdmEntitySet TestEntitySet = Model.EntityContainer.FindEntitySet("Product");
        private static readonly IEdmEntityType TestEntityType = Model.FindDeclaredType("PerformanceServices.Edm.AdventureWorks.Product") as IEdmEntityType;

        [Params(true, false)]
        public bool _isFullValidation;

        public Stream _stream;

        [IterationCleanup]
        public void Cleanup()
        {
            _stream.Dispose();
        }

        [IterationSetup(Target = nameof(ReadFeed))]
        public void SetupForReadFeed()
        {
            SetupDataStream("Entry.json", 1000);
        }

        [Benchmark]
        public void ReadFeed()
        {
            RunReadFeedTest(_isFullValidation);
        }

        [IterationSetup(Target = nameof(ReadFeedIncludeSpatial))]
        public void SetupForReadFeedIncludeSpatial()
        {
            SetupDataStream("EntryIncludeSpatial.json", 1000);
        }

        [Benchmark]
        public void ReadFeedIncludeSpatial()
        {
            RunReadFeedTest(_isFullValidation);
        }

        [IterationSetup(Target = nameof(ReadFeedWithExpansions))]
        public void SetupForReadFeedWithExpansions()
        {
            SetupDataStream("EntryWithExpansions.json", 100);
        }

        [Benchmark]
        public void ReadFeedWithExpansions()
        {
            RunReadFeedTest(_isFullValidation);
        }

        [IterationSetup(Target = nameof(ReadFeedIncludeSpatialWithExpansions))]
        public void SetupForReadFeedIncludeSpatialWithExpansions()
        {
            SetupDataStream("EntryIncludeSpatialWithExpansions.json", 100);
        }

        [Benchmark]
        public void ReadFeedIncludeSpatialWithExpansions()
        {
            RunReadFeedTest(_isFullValidation);
        }
        
        
        public void SetupDataStream(string templateFile, int entryCount)
        {
            _stream = new MemoryStream(PayloadGenerator.GenerateFeed(templateFile, entryCount));
        }

        public void RunReadFeedTest(bool isFullValidation)
        {
            using (var messageReader = ODataMessageHelper.CreateMessageReader(_stream, Model, ODataMessageKind.Response, isFullValidation))
            {
                ODataReader feedReader = messageReader.CreateODataResourceSetReader(TestEntitySet, TestEntityType);
                while (feedReader.Read()) { }
            }
        }
    }
}
