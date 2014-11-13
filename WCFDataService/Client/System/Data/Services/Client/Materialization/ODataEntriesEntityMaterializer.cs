//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Client.Materialization
{
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using Microsoft.Data.OData;
    using DSClient = System.Data.Services.Client;

    /// <summary>
    /// Materializes entities from a sequence of ODataEntry objects
    /// </summary>
    internal sealed class ODataEntriesEntityMaterializer : ODataEntityMaterializer
    {
        /// <summary>The format of the response being materialized.</summary>
        private readonly ODataFormat format;

        /// <summary>The entries enumerator</summary>
        private IEnumerator<ODataEntry> feedEntries;

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
            IEnumerable<ODataEntry> entries,
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
        internal override ODataFeed CurrentFeed
        {
            get { return null; }
        }

        /// <summary>
        /// Entry being materialized; possibly null.
        /// </summary>
        internal override ODataEntry CurrentEntry
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
