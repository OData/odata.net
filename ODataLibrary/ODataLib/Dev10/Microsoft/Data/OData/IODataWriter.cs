//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    #region Namespaces
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion

    // TODO: jli -
    // The ODataMessageWriter uses this interface to communicate state changes to the writers it created. This approach requires third party writers
    // to implement the abstract protected methods WriteErrorImplementation and DisposeWriterImplementation. Should remove this interface and consider
    // a different design that doesn't require third party writers to override those methods.

    /// <summary>
    /// An common interface that allows the <see cref="ODataMessageWriter"/> to communicate with different type of writers it creates.
    /// </summary>
    internal interface IODataWriter
    {
        /// <summary>
        /// Synchronously flushes the write buffer to the underlying stream.
        /// </summary>
        void FlushWriter();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        Task FlushWriterAsync();
#endif

        /// <summary>
        /// Write an OData error.
        /// </summary>
        /// <param name='errorInstance'>The error information to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        void WriteError(ODataError errorInstance, bool includeDebugInformation);

        /// <summary>
        /// This method will be called by ODataMessageWriter.Dispose() to dispose the object implementing this interface.
        /// </summary>
        void DisposeWriter();
    }
}
