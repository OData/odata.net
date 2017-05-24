//---------------------------------------------------------------------
// <copyright file="ODataBatchJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Class for writing OData batch messages of JSON type.
    /// </summary>
    public sealed class ODataBatchJsonWriter : ODataBatchWriter
    {
        /// <summary>
        /// A flag to indicate the last response json data has been closed.
        /// Initial state is true since responses is empty.
        /// </summary>
        private bool latestResponseJsonClosed = true;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rawOutputContext">The output context to write to.</param>
        /// <param name="batchBoundary">The boundary string for the batch structure itself.</param>
        internal ODataBatchJsonWriter(ODataRawOutputContext rawOutputContext, string batchBoundary)
            : base(rawOutputContext, batchBoundary)
        {}

        /// <summary>
        /// Starts a new batch - implementation of the actual functionality.
        /// </summary>
        internal override void WriteStartBatchImplementation()
        {
            this.SetState(BatchWriterState.BatchStarted);
            ODataBatchWriterUtils.WriteJsonStart(this.rawOutputContext.TextWriter, /*firstBoundary*/true);
        }

        /// <summary>
        /// Ends a batch - implementation of the actual functionality.
        /// </summary>
        internal override void WriteEndBatchImplementation()
        {
            Debug.Assert(
                this.batchStartBoundaryWritten || this.CurrentOperationMessage == null,
                "If not batch boundary was written we must not have an active message.");

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            this.SetState(BatchWriterState.BatchCompleted);

            // write the end boundary for the sub-batch
            ODataBatchWriterUtils.WriteJsonEndBatch(this.rawOutputContext.TextWriter);

            // Write a newline after the end batch boundary.
        }

        /// <summary>
        /// Starts a new changeset - implementation of the actual functionality.
        /// </summary>
        internal override void WriteStartChangesetImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // important to do this first since it will set up the change set boundary.
            this.SetState(BatchWriterState.ChangeSetStarted);
            Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");

            // reset the size of the current changeset and increase the size of the batch
            this.ResetChangeSetSize();
            this.InterceptException(this.IncreaseBatchSize);

            // Fixed value 202 is seems to be more appropriate (ODataV4 spec says 200, but RDS seems to be more reasonable with 202).
            int subBatchStatusCode = 202; // HttpStatusCode.Accepted
           
            // TODO: biaol --- For top level property name of subBatch, need to use subBatchId, e.g. "batch1", from request.
            // Using changeSet boundary for now as the Id of the sub-batch.
            ODataBatchWriterUtils.WriteSubBatchJsonStartBoundary(
                this.rawOutputContext.TextWriter,
                this.changeSetBoundary,
                subBatchStatusCode,
                !this.batchStartBoundaryWritten);
            this.batchStartBoundaryWritten = true;
            this.changesetStartBoundaryWritten = false;
        }

        /// <summary>
        /// Ends an active changeset - implementation of the actual functionality.
        /// </summary>
        internal override void WriteEndChangesetImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // change the state first so we validate the change set boundary before attempting to write it.
            this.SetState(BatchWriterState.ChangeSetCompleted);

            // In the case of an empty changeset the start changeset boundary has not been written yet
            // we will leave it like that, since we want the empty changeset to be represented only as
            // the end changeset boundary.
            // Due to WCF DS V2 compatiblity we must not write the start boundary in this case
            // otherwise WCF DS V2 won't be able to read it (it fails on the start-end boundary empty changeset).

            // write the end boundary for the change set
            if (!this.latestResponseJsonClosed)
            {
                ODataBatchWriterUtils.WriteJsonEndResponse(this.rawOutputContext.TextWriter);
                this.latestResponseJsonClosed = true;
            }
            ODataBatchWriterUtils.WriteJsonEndSubBatch(this.rawOutputContext.TextWriter);

            // Reset the cache of content IDs here. As per spec, content IDs are only unique inside a change set.
            this.urlResolver.Reset();
            this.currentOperationContentId = null;
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response - implementation of the actual functionality.
        /// </summary>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <returns>The message that can be used to write the response operation.</returns>
        internal override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation(string contentId)
        {
            this.WritePendingMessageData(true);

            // In responses we don't need to use our batch URL resolver, since there are no cross referencing URLs
            // so use the URL resolver from the batch message instead.
            this.CurrentOperationResponseMessage = ODataBatchOperationResponseMessage.CreateWriteMessage(
                this.rawOutputContext.OutputStream,
                /*operationListener*/ this,
                this.urlResolver.BatchMessageUrlResolver,
                contentId);
            this.SetState(BatchWriterState.OperationCreated);

            Debug.Assert(this.currentOperationContentId == null, "The Content-ID header is only supported in request messages.");

            // write the sub-batch operation's start Json object
            this.WriteStartBoundaryForOperation();

            return this.CurrentOperationResponseMessage;
        }

        /// <summary>
        /// Write any pending headers for the current operation message (if any).
        /// </summary>
        /// <param name="reportMessageCompleted">
        /// A flag to control whether after writing the pending data we report writing the message to be completed or not.
        /// </param>
        internal override void WritePendingMessageData(bool reportMessageCompleted)
        {
            if (this.CurrentOperationMessage != null)
            {
                Debug.Assert(this.rawOutputContext.TextWriter != null, "Must have a batch writer if pending data exists.");

                if (this.CurrentOperationResponseMessage != null)
                {
                    Debug.Assert(this.rawOutputContext.WritingResponse, "If the response message is available we must be writing response.");

                    this.rawOutputContext.TextWriter.WriteLine("\"requestId\": \"{0}\", ", CurrentOperationResponseMessage.ContentId);
                    this.rawOutputContext.TextWriter.WriteLine("\"status\": \"{0}\", ", this.CurrentOperationResponseMessage.StatusCode);
                }

                IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationMessage.Headers;
                if (headers != null && !headers.IsEmptyReadOnlyEnumerable())
                {
                    this.rawOutputContext.TextWriter.WriteLine("\"headers\": {");
                    foreach (KeyValuePair<string, string> headerPair in headers)
                    {
                        string headerName = headerPair.Key;
                        string headerValue = headerPair.Value;
                        //TODO: biaol --- this will convert primitive value such as 1 and true --> "1" and "true", might not be correct.
                        this.rawOutputContext.TextWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\": \"{1}\", ", headerName, headerValue));
                    }
                    this.rawOutputContext.TextWriter.WriteLine("}, ");
                }

                this.latestResponseJsonClosed = false;

                if (reportMessageCompleted)
                {
                    this.CurrentOperationMessage.PartHeaderProcessingCompleted();
                    this.CurrentOperationRequestMessage = null;
                    this.CurrentOperationResponseMessage = null;
                }
            }
        }

        /// <summary>
        /// Writes the start boundary for an operation. This is either the batch or the changeset boundary.
        /// </summary>
        internal override void WriteStartBoundaryForOperation()
        {
            // TODO: biaol --- How does this affect request serilization?
            // Response serialization logic.
            if (this.changeSetBoundary == null)
            {
                ODataBatchWriterUtils.WriteStartBoundary(this.rawOutputContext.TextWriter, this.batchBoundary, !this.batchStartBoundaryWritten);
                this.batchStartBoundaryWritten = true;
            }
            else
            {
                ODataBatchWriterUtils.WriteResponseJsonStartBoundary(this.rawOutputContext.TextWriter, !this.changesetStartBoundaryWritten);
                this.changesetStartBoundaryWritten = true;
                this.latestResponseJsonClosed = false;
            }
        }
    }
}
