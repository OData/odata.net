//   OData .NET Libraries ver. 6.8.1
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
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Atom;
    using Microsoft.OData.Core.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for OData collection writers.
    /// </summary>
    public abstract class ODataCollectionWriter
    {
        /// <summary>Start writing a collection.</summary>
        /// <param name="collectionStart">The <see cref="T:Microsoft.OData.Core.ODataCollectionStart" /> representing the collection.</param>
        public abstract void WriteStart(ODataCollectionStart collectionStart);

#if ODATALIB_ASYNC
        /// <summary>Asynchronously start writing a collection.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="collectionStart">The <see cref="T:Microsoft.OData.Core.ODataCollectionStart" /> representing the collection.</param>
        public abstract Task WriteStartAsync(ODataCollectionStart collectionStart);
#endif

        /// <summary>Starts writing an entry.</summary>
        /// <param name="item">The collection item to write.</param>
        public abstract void WriteItem(object item);

#if ODATALIB_ASYNC
        /// <summary>Asynchronously start writing a collection item.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="item">The collection item to write.</param>
        public abstract Task WriteItemAsync(object item);
#endif

        /// <summary>Finishes writing a collection.</summary>
        public abstract void WriteEnd();

#if ODATALIB_ASYNC
        /// <summary>Asynchronously finish writing a collection.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteEndAsync();
#endif

        /// <summary>Flushes the write buffer to the underlying stream.</summary>
        public abstract void Flush();

#if ODATALIB_ASYNC
        /// <summary>Flushes the write buffer to the underlying stream asynchronously.</summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public abstract Task FlushAsync();
#endif
    }
}
