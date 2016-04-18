//---------------------------------------------------------------------
// <copyright file="ODataDeltaWriter.cs" company="Microsoft">
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
    /// Base class for OData delta writer.
    /// </summary>
    public abstract class ODataDeltaWriter
    {
        /// <summary>
        /// Start writing a delta feed.
        /// </summary>
        /// <param name="deltaResourceSet">Delta feed/collection to write.</param>
        public abstract void WriteStart(ODataDeltaResourceSet deltaResourceSet);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a delta feed.
        /// </summary>
        /// <param name="deltaResourceSet">Delta feed/collection to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataDeltaResourceSet deltaResourceSet);
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
        /// Start writing a navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        public abstract void WriteStart(ODataNestedResourceInfo navigationLink);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataNestedResourceInfo navigationLink);
#endif

        /// <summary>
        /// Start writing an expanded feed.
        /// </summary>
        /// <param name="expandedFeed">The expanded feed to write.</param>
        public abstract void WriteStart(ODataResourceSet expandedFeed);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing an expanded feed.
        /// </summary>
        /// <param name="expandedFeed">The expanded feed to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataResourceSet expandedFeed);
#endif

        /// <summary>
        /// Start writing a delta resource.
        /// </summary>
        /// <param name="deltaResource">The delta resource to write.</param>
        public abstract void WriteStart(ODataResource deltaResource);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a delta resource.
        /// </summary>
        /// <param name="deltaResource">The delta resource to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataResource deltaResource);
#endif

        /// <summary>
        /// Writing a delta deleted resource.
        /// </summary>
        /// <param name="deltaDeletedEntry">The delta deleted resource to write.</param>
        public abstract void WriteDeltaDeletedEntry(ODataDeltaDeletedEntry deltaDeletedEntry);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writing a delta deleted resource.
        /// </summary>
        /// <param name="deltaDeletedEntry">The delta deleted resource to write.</param>
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
