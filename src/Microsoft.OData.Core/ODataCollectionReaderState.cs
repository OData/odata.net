//---------------------------------------------------------------------
// <copyright file="ODataCollectionReaderState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Enumeration of all possible states of an <see cref="ODataCollectionReader" />.
    /// </summary>
    public enum ODataCollectionReaderState
    {
        /// <summary>The reader is at the start; nothing has been read yet.</summary>
        /// <remarks>In this state, the Item property of the <see cref="ODataCollectionReader"/> returns null.</remarks>
        Start,

        /// <summary>
        /// The reader has started reading and is reading the start element of the collection wrapper (if any).
        /// No items have been read.
        /// </summary>
        /// <remarks>
        /// In this state, the Item property of the <see cref="ODataCollectionReader"/> returns
        /// an instance of <see cref="ODataCollectionStart"/>.
        /// </remarks>
        CollectionStart,

        /// <summary>
        /// The reader read an item from the collection.
        /// </summary>
        /// <remarks>In this state, the Item property of the <see cref="ODataCollectionReader"/> returns the read item (a primitive value or null).</remarks>
        Value,

        /// <summary>
        /// The reader has finished reading and is reading the end element of the collection wrapper (if any).
        /// All items have been read.
        /// </summary>
        /// <remarks>
        /// In this state, the Item property of the <see cref="ODataCollectionReader"/> returns the same
        /// instance of <see cref="ODataCollectionStart"/> as in state CollectionStart.
        /// </remarks>
        CollectionEnd,

        /// <summary>The reader has thrown an exception; nothing can be read from the reader anymore.</summary>
        /// <remarks>
        /// In this state, the Item property of the <see cref="ODataCollectionReader"/> returns null.
        /// </remarks>
        Exception,

        /// <summary>The reader has completed; nothing can be read anymore.</summary>
        /// <remarks>
        /// In this state, the Item property of the <see cref="ODataCollectionReader"/> returns null.
        /// </remarks>
        Completed,
    }
}
