//---------------------------------------------------------------------
// <copyright file="ODataReaderWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Edm;
    using Microsoft.OData;

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
                    case ODataReaderState.ResourceStart:
                        this.responsePipeline.ExecuteOnEntryStartActions((ODataResource)this.reader.Item);
                        break;
                    case ODataReaderState.ResourceEnd:
                        this.responsePipeline.ExecuteOnEntryEndActions((ODataResource)this.reader.Item);
                        break;
                    case ODataReaderState.ResourceSetStart:
                        this.responsePipeline.ExecuteOnFeedStartActions((ODataResourceSet)this.reader.Item);
                        break;
                    case ODataReaderState.ResourceSetEnd:
                        this.responsePipeline.ExecuteOnFeedEndActions((ODataResourceSet)this.reader.Item);
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        this.responsePipeline.ExecuteOnNavigationStartActions((ODataNestedResourceInfo)this.reader.Item);
                        break;
                    case ODataReaderState.NestedResourceInfoEnd:
                        this.responsePipeline.ExecuteOnNavigationEndActions((ODataNestedResourceInfo)this.reader.Item);
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
            IEdmStructuredType entityType = expectedType as IEdmStructuredType;
            if (messageType == ODataPayloadKind.Resource)
            {
                return new ODataReaderWrapper(messageReader.CreateODataResourceReader(entityType), responsePipeline);
            }

            return new ODataReaderWrapper(messageReader.CreateODataResourceSetReader(entityType), responsePipeline);
        }

        /// <summary>
        /// Wraps an ODataReader
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="responsePipeline">The data service response pipeling configuration object.</param>
        /// <returns>A reader.</returns>
        internal static ODataReaderWrapper CreateForTest(ODataReader reader, DataServiceClientResponsePipelineConfiguration responsePipeline)
        {
            return new ODataReaderWrapper(reader, responsePipeline);
        }
    }
}
