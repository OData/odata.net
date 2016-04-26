//---------------------------------------------------------------------
// <copyright file="ODataReaderEntityMaterializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.OData.Edm;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Materializes feeds and entities from an ODataReader
    /// </summary>
    internal class ODataReaderEntityMaterializer : ODataEntityMaterializer
    {
        /// <summary>The enty or feed reader.</summary>
        private FeedAndEntryMaterializerAdapter feedEntryAdapter;

        /// <summary>The message reader.</summary>
        private ODataMessageReader messageReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataReaderEntityMaterializer" /> class.
        /// </summary>
        /// <param name="odataMessageReader">The odata message reader.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="entityTrackingAdapter">The entity tracking adapter.</param>
        /// <param name="queryComponents">The query components.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="materializeEntryPlan">The materialize entry plan.</param>
        public ODataReaderEntityMaterializer(
            ODataMessageReader odataMessageReader,
            ODataReaderWrapper reader,
            IODataMaterializerContext materializerContext,
            EntityTrackingAdapter entityTrackingAdapter,
            QueryComponents queryComponents,
            Type expectedType,
            ProjectionPlan materializeEntryPlan)
            : base(materializerContext, entityTrackingAdapter, queryComponents, expectedType, materializeEntryPlan)
        {
            this.messageReader = odataMessageReader;
            this.feedEntryAdapter = new FeedAndEntryMaterializerAdapter(odataMessageReader, reader, materializerContext.Model, entityTrackingAdapter.MergeOption);
        }

        /// <summary>
        /// Feed being materialized; possibly null.
        /// </summary>
        internal override ODataResourceSet CurrentFeed
        {
            get { return this.feedEntryAdapter.CurrentFeed; }
        }

        /// <summary>
        /// Entry being materialized; possibly null.
        /// </summary>
        internal override ODataResource CurrentEntry
        {
            get { return this.feedEntryAdapter.CurrentEntry; }
        }

        /// <summary>
        /// Whether we have finished processing the current data stream.
        /// </summary>
        internal override bool IsEndOfStream
        {
            get { return this.IsDisposed || this.feedEntryAdapter.IsEndOfStream; }
        }

        /// <summary>
        /// The count tag's value, if requested
        /// </summary>
        /// <returns>The count value returned from the server</returns>
        internal override long CountValue
        {
            get
            {
                return this.feedEntryAdapter.GetCountValue(!this.IsDisposed);
            }
        }

        /// <summary>
        /// Returns true if the underlying object used for counting is available
        /// </summary>
        internal override bool IsCountable
        {
            get { return true; }
        }

        /// <summary>
        /// Returns true if the materializer has been disposed
        /// </summary>
        protected override bool IsDisposed
        {
            get { return this.messageReader == null; }
        }

        /// <summary>
        /// The format of the response being materialized.
        /// </summary>
        protected override ODataFormat Format
        {
            get { return ODataUtils.GetReadFormat(this.messageReader); }
        }

        /// <summary>
        /// This method is for parsing CUD operation payloads which should contain
        /// 1 a single entry
        /// 2 An Error
        /// </summary>
        /// <param name="message">the message for the payload</param>
        /// <param name="responseInfo">The current ResponseInfo object</param>
        /// <param name="expectedType">The expected type</param>
        /// <returns>the MaterializerEntry that was read</returns>
        internal static MaterializerEntry ParseSingleEntityPayload(IODataResponseMessage message, ResponseInfo responseInfo, Type expectedType)
        {
            ODataPayloadKind messageType = ODataPayloadKind.Resource;
            using (ODataMessageReader messageReader = CreateODataMessageReader(message, responseInfo, ref messageType))
            {
                IEdmType edmType = responseInfo.TypeResolver.ResolveExpectedTypeForReading(expectedType);
                ODataReaderWrapper reader = ODataReaderWrapper.Create(messageReader, messageType, edmType, responseInfo.ResponsePipeline);

                FeedAndEntryMaterializerAdapter parser = new FeedAndEntryMaterializerAdapter(messageReader, reader, responseInfo.Model, responseInfo.MergeOption);

                ODataResource entry = null;
                bool readFeed = false;
                while (parser.Read())
                {
                    readFeed |= parser.CurrentFeed != null;
                    if (parser.CurrentEntry != null)
                    {
                        if (entry != null)
                        {
                            throw new InvalidOperationException(DSClient.Strings.AtomParser_SingleEntry_MultipleFound);
                        }

                        entry = parser.CurrentEntry;
                    }
                }

                if (entry == null)
                {
                    if (readFeed)
                    {
                        throw new InvalidOperationException(DSClient.Strings.AtomParser_SingleEntry_NoneFound);
                    }
                    else
                    {
                        throw new InvalidOperationException(DSClient.Strings.AtomParser_SingleEntry_ExpectedFeedOrEntry);
                    }
                }

                return MaterializerEntry.GetEntry(entry);
            }
        }

        /// <summary>
        /// Called when IDisposable.Dispose is called.
        /// </summary>
        protected override void OnDispose()
        {
            if (this.messageReader != null)
            {
                this.messageReader.Dispose();
                this.messageReader = null;
            }

            this.feedEntryAdapter.Dispose();
        }

        /// <summary>
        /// Reads the next feed or entry.
        /// </summary>
        /// <returns>
        /// True if an entry was read, otherwise false
        /// </returns>
        protected override bool ReadNextFeedOrEntry()
        {
            return this.feedEntryAdapter.Read();
        }
    }
}
