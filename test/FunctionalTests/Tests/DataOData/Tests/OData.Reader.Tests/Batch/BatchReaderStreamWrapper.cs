//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Wrapper of ODataMultipartMixedBatchReaderStream to expose the internal API.
    /// </summary>
    public sealed class BatchReaderStreamWrapper
    {
        /// <summary>
        /// The type of the stream buffer class in the product code.
        /// </summary>
        private static readonly Type batchStreamType = typeof(ODataBatchReader).Assembly.GetType("Microsoft.OData.ODataMultipartMixedBatchReaderStream");

        /// <summary>
        /// The stream buffer instance from the product code.
        /// </summary>
        private readonly object batchStream;

        /// <summary>
        /// The batch stream buffer used by the batch stream.
        /// </summary>
        private readonly BatchReaderStreamBufferWrapper batchStreamBuffer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchReader">The batch reader to get the batch stream from.</param>
        public BatchReaderStreamWrapper(ODataBatchReader batchReader)
        {
            this.batchStream = ReflectionUtils.GetField(batchReader, "batchStream");
            this.batchStreamBuffer = new BatchReaderStreamBufferWrapper(this.batchStream);
        }

        /// <summary>
        /// The boundary string for the batch structure itself.
        /// </summary>
        public string BatchBoundary
        {
            get
            {
                return (string)ReflectionUtils.GetProperty(this.batchStream, "BatchBoundary");
            }
        }

        /// <summary>
        /// The boundary string for the current change set (only set when reading a change set
        /// or an operation in a change set).
        /// </summary>
        /// <remarks>When not reading a change set (or operation in a change set) this field is null.</remarks>
        public string ChangeSetBoundary
        {
            get
            {
                return (string)ReflectionUtils.GetProperty(this.batchStream, "ChangeSetBoundary");
            }
        }

        /// <summary>
        /// Set the private 'batchEncoding' field of the batch stream to the specified value.
        /// </summary>
        /// <param name="encoding">The encoding to set.</param>
        public void SetBatchEncoding(Encoding encoding)
        {
            ReflectionUtils.SetField(this.batchStream, "batchEncoding", encoding);
        }

        /// <summary>
        /// Skips all the data in the stream until a boundary is found.
        /// </summary>
        /// <param name="requireLineFeedAtStart">true if we expect the boundary to start with a line feed; otherwise false.</param>
        /// <param name="isEndBoundary">true if the boundary that was found is an end boundary; otherwise false.</param>
        /// <returns>true if a boundary was found; otherwise false.</returns>
        public bool SkipToBoundary(out bool isEndBoundary, out bool isParentBoundary)
        {
            object[] args = new object[2];
            bool result = (bool)ReflectionUtils.InvokeMethod(this.batchStream, "SkipToBoundary", args);
            isEndBoundary = (bool)args[0];
            isParentBoundary = (bool)args[1];
            return result;
        }

        /// <summary>
        /// Reads from the batch stream while ensuring that we stop reading at each boundary.
        /// </summary>
        /// <param name="userBuffer">The byte array to read bytes into.</param>
        /// <param name="userBufferOffset">The offset in the buffer where to start reading bytes into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        public int ReadWithDelimiter(byte[] userBuffer, int userBufferOffset, int count)
        {
            return (int)ReflectionUtils.InvokeMethod(this.batchStream, "ReadWithDelimiter", userBuffer, userBufferOffset, count);
        }

        /// <summary>
        /// Reads from the batch stream without checking for a boundary delimiter since we
        /// know the length of the stream.
        /// </summary>
        /// <param name="userBuffer">The byte array to read bytes into.</param>
        /// <param name="userBufferOffset">The offset in the buffer where to start reading bytes into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        public int ReadWithLength(byte[] userBuffer, int userBufferOffset, int count)
        {
            return (int)ReflectionUtils.InvokeMethod(this.batchStream, "ReadWithLength", userBuffer, userBufferOffset, count);
        }

        /// <summary>
        /// Reads the start of a part including the request/response line and the headers.
        /// </summary>
        /// <returns>true if the start of a change set part was detected; otherwise false.</returns>
        /// <remarks>
        /// This method caches what it has read from the stream in a request or response
        /// operation message that will be returned to the client later when CreateRequestOperationMessage
        /// or CreateResponseOperationMessage is called.
        /// </remarks>
        public bool ProcessPartHeader()
        {
            return (bool)ReflectionUtils.InvokeMethod(this.batchStream, "ProcessPartHeader", /*Content-ID*/ "1");
        }

        /// <summary>
        /// Reads and validates the headers of a batch part.
        /// </summary>
        /// <param name="isChangeSetPart">true if the headers indicate a change set part; otherwise false.</param>
        /// <returns>A batch operation headers; never null.</returns>
        public BatchOperationHeadersWrapper ReadPartHeaders(out bool isChangeSetPart)
        {
            object[] args = new object[1];
            object headers = ReflectionUtils.InvokeMethod(this.batchStream, "ReadPartHeaders", args);
            isChangeSetPart = (bool)args[0];
            return new BatchOperationHeadersWrapper(headers);
        }

        /// <summary>
        /// Reads a line (all bytes until a line feed) from the underlying stream.
        /// </summary>
        /// <returns>Returns the string that was read from the underlying stream (not including a terminating line feed).</returns>
        public string ReadLine()
        {
            return (string)ReflectionUtils.InvokeMethod(this.batchStream, "ReadLine");
        }

        /// <summary>The batch buffer underlying the batch reader stream.</summary>
        public BatchReaderStreamBufferWrapper BatchBuffer
        {
            get { return this.batchStreamBuffer; }
        }
    }
}
