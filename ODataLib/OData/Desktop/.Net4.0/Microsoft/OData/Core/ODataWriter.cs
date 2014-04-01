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
    /// Base class for OData writers.
    /// </summary>
    public abstract class ODataWriter
    {
        /// <summary>Starts the writing of a feed.</summary>
        /// <param name="feed">The feed or collection to write.</param>
        public abstract void WriteStart(ODataFeed feed);

#if ODATALIB_ASYNC
        /// <summary> Asynchronously start writing a feed. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="feed">The feed or collection to write.</param>
        public abstract Task WriteStartAsync(ODataFeed feed);
#endif

        /// <summary>Starts the writing of an entry.</summary>
        /// <param name="entry">The entry or item to write.</param>
        public abstract void WriteStart(ODataEntry entry);

#if ODATALIB_ASYNC
        /// <summary> Asynchronously start writing an entry. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="entry">The entry or item to write.</param>
        public abstract Task WriteStartAsync(ODataEntry entry);
#endif

        /// <summary>Starts the writing of a navigation link.</summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        public abstract void WriteStart(ODataNavigationLink navigationLink);

#if ODATALIB_ASYNC
        /// <summary> Asynchronously start writing a navigation link. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="navigationLink">The navigation link to writer.</param>
        public abstract Task WriteStartAsync(ODataNavigationLink navigationLink);
#endif

        /// <summary>Finishes the writing of a feed, an entry, or a navigation link.</summary>
        public abstract void WriteEnd();

#if ODATALIB_ASYNC
        /// <summary> Asynchronously finish writing a feed, entry, or navigation link. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteEndAsync();
#endif

        /// <summary> Writes an entity reference link, which is used to represent binding to an existing resource in a request payload. </summary>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        /// <remarks>
        /// This method can only be called for writing request messages. The entity reference link must be surrounded
        /// by a navigation link written through WriteStart/WriteEnd.
        /// The <see cref="ODataNavigationLink.Url"/> will be ignored in that case and the Uri from the <see cref="ODataEntityReferenceLink.Url"/> will be used
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
        /// The <see cref="ODataNavigationLink.Url"/> will be ignored in that case and the Uri from the <see cref="ODataEntityReferenceLink.Url"/> will be used
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
