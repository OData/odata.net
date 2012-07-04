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
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Atom;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for OData collection writers.
    /// </summary>
    public abstract class ODataCollectionWriter
    {
        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        public abstract void WriteStart(ODataCollectionStart collectionStart);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a collection.
        /// </summary>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataCollectionStart collectionStart);
#endif

        /// <summary>
        /// Start writing a collection item.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        public abstract void WriteItem(object item);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a collection item.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteItemAsync(object item);
#endif

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        public abstract void WriteEnd();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously finish writing a collection.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteEndAsync();
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
