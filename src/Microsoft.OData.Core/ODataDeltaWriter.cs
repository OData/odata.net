//---------------------------------------------------------------------
// <copyright file="ODataDeltaWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Threading.Tasks;

    #endregion Namespaces

    /// <summary>
    /// Base class for OData delta writer.
    /// </summary>
    public abstract class ODataDeltaWriter
    {
        /// <summary>
        /// Start writing a delta resource set.
        /// </summary>
        /// <param name="deltaResourceSet">Delta resource set/collection to write.</param>
        public abstract void WriteStart(ODataDeltaResourceSet deltaResourceSet);

        /// <summary>
        /// Asynchronously start writing a delta resource set.
        /// </summary>
        /// <param name="deltaResourceSet">Delta resource set/collection to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataDeltaResourceSet deltaResourceSet);

        /// <summary>
        /// Finish writing a delta resource set.
        /// </summary>
        public abstract void WriteEnd();

        /// <summary>
        /// Asynchronously finish writing a delta resource set.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteEndAsync();

        /// <summary>
        /// Start writing a nested resource info.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        public abstract void WriteStart(ODataNestedResourceInfo nestedResourceInfo);

        /// <summary>
        /// Asynchronously start writing a nested resource info.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataNestedResourceInfo nestedResourceInfo);

        /// <summary>
        /// Start writing an expanded resource set.
        /// </summary>
        /// <param name="expandedResourceSet">The expanded resource set to write.</param>
        public abstract void WriteStart(ODataResourceSet expandedResourceSet);

        /// <summary>
        /// Asynchronously start writing an expanded resource set.
        /// </summary>
        /// <param name="expandedResourceSet">The expanded resource set to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataResourceSet expandedResourceSet);

        /// <summary>
        /// Start writing a delta resource.
        /// </summary>
        /// <param name="deltaResource">The delta resource to write.</param>
        public abstract void WriteStart(ODataResource deltaResource);

        /// <summary>
        /// Asynchronously start writing a delta resource.
        /// </summary>
        /// <param name="deltaResource">The delta resource to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataResource deltaResource);


        /// <summary>
        /// Writing a delta deleted resource.
        /// </summary>
        /// <param name="deltaDeletedEntry">The delta deleted resource to write.</param>
        public abstract void WriteDeltaDeletedEntry(ODataDeltaDeletedEntry deltaDeletedEntry);

        /// <summary>
        /// Asynchronously writing a delta deleted resource.
        /// </summary>
        /// <param name="deltaDeletedEntry">The delta deleted resource to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteDeltaDeletedEntryAsync(ODataDeltaDeletedEntry deltaDeletedEntry);

        /// <summary>
        /// Writing a delta link.
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        public abstract void WriteDeltaLink(ODataDeltaLink deltaLink);

        /// <summary>
        /// Asynchronously writing a delta link.
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteDeltaLinkAsync(ODataDeltaLink deltaLink);

        /// <summary>
        /// Writing a delta deleted link.
        /// </summary>
        /// <param name="deltaDeletedLink">The delta deleted link to write.</param>
        public abstract void WriteDeltaDeletedLink(ODataDeltaDeletedLink deltaDeletedLink);

        /// <summary>
        /// Asynchronously writing a delta deleted link.
        /// </summary>
        /// <param name="deltaDeletedLink">The delta deleted link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteDeltaDeletedLinkAsync(ODataDeltaDeletedLink deltaDeletedLink);

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public abstract void Flush();

        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public abstract Task FlushAsync();
    }
}
