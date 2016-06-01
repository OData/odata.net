//---------------------------------------------------------------------
// <copyright file="ODataDeltaReaderState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Enumeration of all possible states of an <see cref="ODataDeltaReader" />.
    /// </summary>
    public enum ODataDeltaReaderState
    {
        /// <summary>The reader is at the start; nothing has been read yet.</summary>
        /// <remarks>In this state, the Item property of the <see cref="ODataDeltaReader"/> returns null.</remarks>
        Start,

        /// <summary>The start of a delta resource set has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns
        /// an <see cref="ODataDeltaResourceSet"/> but no properties may be filled in until the DeltaResourceSetEnd state is reached.
        /// </remarks>
        DeltaResourceSetStart,

        /// <summary>The end of a delta resource set has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns
        /// an <see cref="ODataDeltaResourceSet"/> with all properties filled in.
        /// </remarks>
        DeltaResourceSetEnd,

        /// <summary>The start of a delta resource has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns
        /// an <see cref="ODataResource"/> but no properties may be filled in until the EntryEnd state is reached.
        /// </remarks>
        DeltaResourceStart,

        /// <summary>The end of a delta resource has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns
        /// an <see cref="ODataResource"/> with all properties filled in.
        /// </remarks>
        DeltaResourceEnd,

        /// <summary>An delta deleted resource was read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns
        /// an <see cref="ODataDeltaDeletedEntry"/> which is fully populated.
        /// Note that there's no End state for this item.
        /// </remarks>
        DeltaDeletedEntry,

        /// <summary>An delta link was read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns
        /// an <see cref="ODataDeltaLink"/> which is fully populated.
        /// Note that there's no End state for this item.
        /// </remarks>
        DeltaLink,

        /// <summary>An delta deleted link was read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns
        /// an <see cref="ODataDeltaDeletedLink"/> which is fully populated.
        /// Note that there's no End state for this item.
        /// </remarks>
        DeltaDeletedLink,

        /// <summary>The reader has thrown an exception; nothing can be read from the reader anymore.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns null.
        /// </remarks>
        Exception,

        /// <summary>The reader has completed; nothing can be read anymore.</summary>
        /// <remarks>
        /// In this state, the Item property of the <see cref="ODataDeltaReader"/> returns null.
        /// </remarks>
        Completed,

        /// <summary>A nested resource info was read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns
        /// the current item of the underlying nested resource reader.
        /// </remarks>
        NestedResource,
    }
}
