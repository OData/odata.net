//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamBufferState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    #endregion Namespaces

    /// <summary>
    /// Class modeling the state of the ODataBatchReaderStreamBuffer
    /// </summary>
    public class BatchReaderStreamBufferState
    {
        /// <summary>The expected read position in the buffer; 
        /// or null to ignore during verification.</summary>
        public int? ReadPositionInBuffer { get; set; }

        /// <summary>The expected number of bytes in the buffer; 
        /// or null to ignore during verification.</summary>
        public int? NumberOfBytesInBuffer { get; set; }

        /// <summary>A list of expected bytes at specified positions in the buffer.</summary>
        public IEnumerable<KeyValuePair<int, byte>> ExpectedBytesInBuffer { get; set; }

        /// <summary>
        /// Verifies that the resulting stream buffer is in the expected state.
        /// </summary>
        /// <param name="assert">The assertion handler.</param>
        /// <param name="streamBuffer">The stream buffer whose state to verify.</param>
        /// <param name="testCaseDebugDescription">A textual description of the test case to make debugging easier.</param>
        public virtual void VerifyResult(AssertionHandler assert, BatchReaderStreamBufferWrapper streamBuffer, string testCaseDebugDescription)
        {
            Debug.Assert(assert != null, "assert != null");
            Debug.Assert(streamBuffer != null, "streamBuffer != null");

            if (this.ReadPositionInBuffer.HasValue)
            {
                assert.AreEqual(
                    this.ReadPositionInBuffer.Value, streamBuffer.CurrentReadPosition,
                    string.Format("\r\n{0}:\r\nCurrent read position in the buffer '{1}' does not match expected read position '{2}'", testCaseDebugDescription, streamBuffer.CurrentReadPosition, this.ReadPositionInBuffer.Value));
            }

            if (this.NumberOfBytesInBuffer.HasValue)
            {
                assert.AreEqual(
                    this.NumberOfBytesInBuffer.Value, streamBuffer.NumberOfBytesInBuffer,
                    string.Format("\r\n{0}:\r\nNumber of bytes in the buffer '{1}' does not match expected number '{2}'", testCaseDebugDescription, streamBuffer.NumberOfBytesInBuffer, this.NumberOfBytesInBuffer.Value));
            }

            if (this.ExpectedBytesInBuffer != null)
            {
                foreach (KeyValuePair<int, byte> kvp in this.ExpectedBytesInBuffer)
                {
                    assert.AreEqual(
                        kvp.Value, streamBuffer.Bytes[kvp.Key],
                        string.Format("\r\n{0}:\r\nExpected value '{1}' at position '{2}' in the buffer but found '{3}'", testCaseDebugDescription, kvp.Value, kvp.Key, streamBuffer.Bytes[kvp.Key]));
                }
            }
        }

    }
}
