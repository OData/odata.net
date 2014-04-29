//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
