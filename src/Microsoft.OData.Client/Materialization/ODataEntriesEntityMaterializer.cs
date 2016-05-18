//---------------------------------------------------------------------
// <copyright file="ODataEntriesEntityMaterializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Materializes entities from a sequence of ODataResource objects
    /// </summary>
    internal sealed class ODataEntriesEntityMaterializer : ODataEntityMaterializer
    {
        /// <summary>The format of the response being materialized.</summary>
        private readonly ODataFormat format;

        /// <summary>The entries enumerator</summary>
        private IEnumerator<ODataResource> feedEntries;

        /// <summary>Is the enumerator finished.</summary>
        private bool isFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataEntriesEntityMaterializer" /> class.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="entityTrackingAdapter">The entity tracking adapter.</param>
        /// <param name="queryComponents">The query components.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="materializeEntryPlan">The materialize entry plan.</param>
        /// <param name="format">The format.</param>
        public ODataEntriesEntityMaterializer(
            IEnumerable<ODataResource> entries,
            IODataMaterializerContext materializerContext,
            EntityTrackingAdapter entityTrackingAdapter,
            QueryComponents queryComponents,
            Type expectedType,
            ProjectionPlan materializeEntryPlan,
            ODataFormat format)
            : base(materializerContext, entityTrackingAdapter, queryComponents, expectedType, materializeEntryPlan)
        {
            this.format = format;
            this.feedEntries = entries.GetEnumerator();
        }

        /// <summary>
        /// Feed being materialized; possibly null.
        /// </summary>
        internal override ODataResourceSet CurrentFeed
        {
            get { return null; }
        }

        /// <summary>
        /// Entry being materialized; possibly null.
        /// </summary>
        internal override ODataResource CurrentEntry
        {
            get
            {
                this.VerifyNotDisposed();
                return this.feedEntries.Current;
            }
        }

        /// <summary>
        /// The count tag's value, if requested
        /// </summary>
        /// <returns>The count value returned from the server</returns>
        internal override long CountValue
        {
            get
            {
                throw new InvalidOperationException(DSClient.Strings.MaterializeFromAtom_CountNotPresent);
            }
        }

        /// <summary>
        /// Returns true if the underlying object used for counting is available
        /// </summary>
        internal override bool IsCountable
        {
            get { return false; }
        }

        /// <summary>
        /// Whether we have finished processing the current data stream.
        /// </summary>
        internal override bool IsEndOfStream
        {
            get { return this.isFinished; }
        }

        /// <summary>
        /// Returns true if the materializer has been disposed
        /// </summary>
        protected override bool IsDisposed
        {
            get { return this.feedEntries == null; }
        }

        /// <summary>
        /// The format of the response being materialized.
        /// </summary>
        protected override ODataFormat Format
        {
            get { return this.format; }
        }

        /// <summary>
        /// Reads the next feed or entry.
        /// </summary>
        /// <returns>
        /// True if an entry was read, otherwise false
        /// </returns>
        protected override bool ReadNextFeedOrEntry()
        {
            if (!this.isFinished)
            {
                if (!this.feedEntries.MoveNext())
                {
                    this.isFinished = true;
                }
            }

            return !this.isFinished;
        }

        /// <summary>
        /// Called when IDisposable.Dispose is called.
        /// </summary>
        protected override void OnDispose()
        {
            if (this.feedEntries != null)
            {
                this.feedEntries.Dispose();
                this.feedEntries = null;
            }
        }
    }
}
