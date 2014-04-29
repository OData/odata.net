//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client.Materialization
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;

    /// <summary>
    /// Contains an odata reader that is wrapped
    /// </summary>
    internal class ODataReaderWrapper
    {
        /// <summary> The reader. </summary>
        private readonly ODataReader reader;

        /// <summary> The payload reading events. </summary>
        private readonly DataServiceClientResponsePipelineConfiguration responsePipeline;

        /// <summary>
        /// Prevents a default instance of the <see cref="ODataReaderWrapper" /> class from being created.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="responsePipeline">The data service response pipeling configuration object.</param>
        private ODataReaderWrapper(ODataReader reader, DataServiceClientResponsePipelineConfiguration responsePipeline)
        {
            Debug.Assert(reader != null, "reader!=null");
            this.reader = reader;
            this.responsePipeline = responsePipeline;
        }

        /// <summary>
        /// The current state of the reader.
        /// </summary>
        public ODataReaderState State
        {
            get
            {
                return this.reader.State;
            }
        }

        /// <summary>
        /// The most recent <see cref="ODataItem"/> that has been read.
        /// </summary>
        public ODataItem Item
        {
            get
            {
                return this.reader.Item;
            }
        }

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public bool Read()
        {
            bool read = this.reader.Read();

            if (read && this.responsePipeline.HasConfigurations)
            {
                // ODataReaderState.EntityReferenceLink is not in the switch as entity reference link
                // is only in an update payload so it will never occur
                switch (this.reader.State)
                {
                    case ODataReaderState.EntryStart:
                        this.responsePipeline.ExecuteOnEntryStartActions((ODataEntry)this.reader.Item);
                        break;
                    case ODataReaderState.EntryEnd:
                        this.responsePipeline.ExecuteOnEntryEndActions((ODataEntry)this.reader.Item);
                        break;
                    case ODataReaderState.FeedStart:
                        this.responsePipeline.ExecuteOnFeedStartActions((ODataFeed)this.reader.Item);
                        break;
                    case ODataReaderState.FeedEnd:
                        this.responsePipeline.ExecuteOnFeedEndActions((ODataFeed)this.reader.Item);
                        break;
                    case ODataReaderState.NavigationLinkStart:
                        this.responsePipeline.ExecuteOnNavigationStartActions((ODataNavigationLink)this.reader.Item);
                        break;
                    case ODataReaderState.NavigationLinkEnd:
                        this.responsePipeline.ExecuteOnNavigationEndActions((ODataNavigationLink)this.reader.Item);
                        break;
                }
            }

            return read;
        }

        /// <summary>
        /// Creates and Wraps an ODataReader for feeds or entries.
        /// </summary>
        /// <param name="messageReader">The message reader.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="expectedType">The expected EDM type.</param>
        /// <param name="responsePipeline">The data service response pipeling configuration object.</param>
        /// <returns>A reader.</returns>
        internal static ODataReaderWrapper Create(ODataMessageReader messageReader, ODataPayloadKind messageType, IEdmType expectedType, DataServiceClientResponsePipelineConfiguration responsePipeline)
        {
            IEdmEntityType entityType = expectedType as IEdmEntityType;
            if (messageType == ODataPayloadKind.Entry)
            {
                return new ODataReaderWrapper(messageReader.CreateODataEntryReader(entityType), responsePipeline);
            }

            return new ODataReaderWrapper(messageReader.CreateODataFeedReader(entityType), responsePipeline);
        }

        /// <summary>
        /// Wraps an ODataReader
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="responsePipeline">The data service response pipeling configuration object.</param>
        /// <returns>A reader.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:'ODataReaderWrapper.CreateForTest(ODataReader)' appears to have no upstream public or protected callers.", Justification = "Method is only used for testing purposes.")]
        internal static ODataReaderWrapper CreateForTest(ODataReader reader, DataServiceClientResponsePipelineConfiguration responsePipeline)
        {
            return new ODataReaderWrapper(reader, responsePipeline);
        }
    }
}
