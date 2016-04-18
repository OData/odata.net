//---------------------------------------------------------------------
// <copyright file="ODataWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
    using Microsoft.OData.Core.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for OData writers.
    /// </summary>
    public abstract class ODataWriter
    {
        /// <summary>Starts the writing of a feed.</summary>
        /// <param name="resourceSet">The feed or collection to write.</param>
        public abstract void WriteStart(ODataResourceSet resourceSet);

#if ODATALIB_ASYNC
        /// <summary> Asynchronously start writing a feed. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="resourceSet">The feed or collection to write.</param>
        public abstract Task WriteStartAsync(ODataResourceSet resourceSet);
#endif

        /// <summary>Starts the writing of a resource.</summary>
        /// <param name="resource">The resource or item to write.</param>
        public abstract void WriteStart(ODataResource resource);

#if ODATALIB_ASYNC
        /// <summary> Asynchronously start writing a resource. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="resource">The resource or item to write.</param>
        public abstract Task WriteStartAsync(ODataResource resource);
#endif

        /// <summary>Starts the writing of a navigation link.</summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        public abstract void WriteStart(ODataNestedResourceInfo navigationLink);

#if ODATALIB_ASYNC
        /// <summary> Asynchronously start writing a navigation link. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="navigationLink">The navigation link to writer.</param>
        public abstract Task WriteStartAsync(ODataNestedResourceInfo navigationLink);
#endif

        /// <summary>Finishes the writing of a feed, a resource, or a navigation link.</summary>
        public abstract void WriteEnd();

#if ODATALIB_ASYNC
        /// <summary> Asynchronously finish writing a feed, resource, or navigation link. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteEndAsync();
#endif

        /// <summary> Writes an entity reference link, which is used to represent binding to an existing resource in a request payload. </summary>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        /// <remarks>
        /// This method can only be called for writing request messages. The entity reference link must be surrounded
        /// by a navigation link written through WriteStart/WriteEnd.
        /// The <see cref="ODataNestedResourceInfo.Url"/> will be ignored in that case and the Uri from the <see cref="ODataEntityReferenceLink.Url"/> will be used
        /// as the binding URL to be written.
        /// </remarks>
        public abstract void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink);

#if ODATALIB_ASYNC
        /// <summary> Asynchronously writes an entity reference link, which is used to represent binding to an existing resource in a request payload. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        /// <remarks>
        /// This method can only be called for writing request messages. The entity reference link must be surrounded
        /// by a navigation link written through WriteStart/WriteEnd.
        /// The <see cref="ODataNestedResourceInfo.Url"/> will be ignored in that case and the Uri from the <see cref="ODataEntityReferenceLink.Url"/> will be used
        /// as the binding URL to be written.
        /// </remarks>
        public abstract Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink entityReferenceLink);
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
