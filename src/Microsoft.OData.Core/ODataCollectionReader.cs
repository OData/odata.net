//---------------------------------------------------------------------
// <copyright file="ODataCollectionReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Base class for OData collection readers.
    /// </summary>
    public abstract class ODataCollectionReader
    {
        /// <summary>Gets the current state of the reader.</summary>
        /// <returns>The current state of the reader.</returns>
        public abstract ODataCollectionReaderState State
        {
            get;
        }

        /// <summary>Gets the most recent item that has been read.</summary>
        /// <returns>The most recent item that has been read.</returns>
        /// <remarks>
        /// This property returns an <see cref="ODataCollectionStart"/> when in state ODataCollectionReaderState.CollectionStart
        /// or ODataCollectionReaderState.CollectionEnd. It returns either a primitive value or 'null' when
        /// in state ODataCollectionReaderState.Value and 'null' in all other states.
        /// </remarks>
        public abstract object Item
        {
            get;
        }

        /// <summary>Reads the next item from the message payload. </summary>
        /// <returns>True if more items were read; otherwise false.</returns>
        public abstract bool Read();

#if PORTABLELIB
        /// <summary>Asynchronously reads the next item from the message payload.</summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public abstract Task<bool> ReadAsync();
#endif
    }
}
