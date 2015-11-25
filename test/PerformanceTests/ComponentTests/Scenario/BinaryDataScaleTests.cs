//---------------------------------------------------------------------
// <copyright file="BinaryDataScaleTests.cs" company="Microsoft">
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
    using Microsoft.Xunit.Performance;

    public class BinaryDataScaleTests : WriteReadFeedTestBase
    {
        private static IEdmModel exchangeAttachmentModel = TestUtils.GetExchangeAttachmentModel();
        private static IEdmEntitySet testEntitySet = exchangeAttachmentModel.EntityContainer.FindEntitySet("Item");
        private static IEdmEntityType testEntityType = exchangeAttachmentModel.FindDeclaredType("PerformanceServices.Edm.ExchangeAttachment.Item") as IEdmEntityType;
        private const int NumberOfEntries = 10;
        private static Stream WriteStream = new MemoryStream();

        [Benchmark]
        public void WriteFeedBinaryData_4MB()
        {
            var entry = CreateEntry(4 * 1024);

            foreach (var iteration in Benchmark.Iterations)
            {
                WriteStream.SetLength(0);

                using (iteration.StartMeasurement())
                {
                    WriteFeed(WriteStream, exchangeAttachmentModel, NumberOfEntries, entry, testEntitySet);
                }
            }
        }

        [Benchmark]
        public void ReadFeedBinaryData_4MB()
        {
            var entry = CreateEntry(4 * 1024);

            foreach (var iteration in Benchmark.Iterations)
            {
                WriteStream.SetLength(0);
                WriteFeed(WriteStream, exchangeAttachmentModel, NumberOfEntries, entry, testEntitySet);

                using (iteration.StartMeasurement())
                {
                    ReadFeed(WriteStream, exchangeAttachmentModel, testEntitySet, testEntityType);
                }
            }
        }

        private ODataEntry CreateEntry(int dataSizeKb)
        {
            var entry = new ODataEntry
            {
                Id = new Uri("http://www.odata.org/Perf.svc/Item(1)"),
                EditLink = new Uri("Item(1)", UriKind.Relative),
                ReadLink = new Uri("Item(1)", UriKind.Relative),
                TypeName = "PerformanceServices.Edm.ExchangeAttachment.Item",
                Properties = new[]
                    {
                        new ODataProperty{ Name = "HasAttachments", Value = false},
                        new ODataProperty{ Name = "Attachments", Value = new ODataCollectionValue
                            {
                                TypeName = "Collection(PerformanceServices.Edm.ExchangeAttachment.Attachment)",
                                Items = dataSizeKb == 0 ? new ODataComplexValue[0]: 
                                Enumerable.Range(0, 1).Select(n => new ODataComplexValue
                                {
                                    TypeName = "PerformanceServices.Edm.ExchangeAttachment.Attachment",
                                    Properties = new[]
                                    {
                                        new ODataProperty { Name = "Name", Value = "attachment" },
                                        new ODataProperty { Name = "IsInline", Value = false },
                                        new ODataProperty { Name = "LastModifiedTime", Value = new DateTimeOffset(1987, 6, 5, 4, 3, 21, 0, new TimeSpan(0, 0, 3, 0)) },
                                        new ODataProperty { Name = "Content", Value = new byte[dataSizeKb * 1024]}, 
                                    }
                                })
                            }}
                    }
            };

            return entry;
        }
    }
}
