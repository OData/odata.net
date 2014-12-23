//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core
{
    #region Namespaces
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Base class for OData delta writer.
    /// </summary>
    public abstract class ODataDeltaWriter
    {
        /// <summary>
        /// Start writing a delta feed.
        /// </summary>
        /// <param name="deltaFeed">Delta feed/collection to write.</param>
        public abstract void WriteStart(ODataDeltaFeed deltaFeed);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a delta feed.
        /// </summary>
        /// <param name="deltaFeed">Delta feed/collection to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataDeltaFeed deltaFeed);
#endif

        /// <summary>
        /// Finish writing a delta feed.
        /// </summary>
        public abstract void WriteEnd();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously finish writing a delta feed.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteEndAsync();
#endif

        /// <summary>
        /// Start writing a delta entry.
        /// </summary>
        /// <param name="deltaEntry">The delta entry to write.</param>
        public abstract void WriteStart(ODataEntry deltaEntry);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a delta entry.
        /// </summary>
        /// <param name="deltaEntry">The delta entry to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataEntry deltaEntry);
#endif

        /// <summary>
        /// Writing a delta deleted entry.
        /// </summary>
        /// <param name="deltaDeletedEntry">The delta deleted entry to write.</param>
        public abstract void WriteDeltaDeletedEntry(ODataDeltaDeletedEntry deltaDeletedEntry);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writing a delta deleted entry.
        /// </summary>
        /// <param name="deltaDeletedEntry">The delta deleted entry to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteDeltaDeletedEntryAsync(ODataDeltaDeletedEntry deltaDeletedEntry);
#endif

        /// <summary>
        /// Writing a delta link.
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        public abstract void WriteDeltaLink(ODataDeltaLink deltaLink);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writing a delta link.
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteDeltaLinkAsync(ODataDeltaLink deltaLink);
#endif

        /// <summary>
        /// Writing a delta deleted link.
        /// </summary>
        /// <param name="deltaDeletedLink">The delta deleted link to write.</param>
        public abstract void WriteDeltaDeletedLink(ODataDeltaDeletedLink deltaDeletedLink);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writing a delta deleted link.
        /// </summary>
        /// <param name="deltaDeletedLink">The delta deleted link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteDeltaDeletedLinkAsync(ODataDeltaDeletedLink deltaDeletedLink);
#endif

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public abstract void Flush();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public abstract Task FlushAsync();
#endif
    }
}
