//---------------------------------------------------------------------
// <copyright file="ODataCollectionWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
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

        /// <summary>Starts writing a resource.</summary>
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
