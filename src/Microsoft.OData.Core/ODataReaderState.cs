//---------------------------------------------------------------------
// <copyright file="ODataReaderState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Enumeration of all possible states of an <see cref="ODataReader" />.
    /// </summary>
    public enum ODataReaderState
    {
        /// <summary>The reader is at the start; nothing has been read yet.</summary>
        /// <remarks>In this state the Item property of the <see cref="ODataReader"/> returns null.</remarks>
        Start,

        /// <summary>The start of a resource set has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataResourceSet"/> but no properties may be filled in until the ResourceSetEnd state is reached.
        /// </remarks>
        ResourceSetStart,

        /// <summary>The end of a resource set has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataResourceSet"/> with all properties filled in.
        /// </remarks>
        ResourceSetEnd,

        /// <summary>The start of a resource has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataResource"/> but no properties may be filled in until the EntryEnd state is reached.
        /// </remarks>
        ResourceStart,

        /// <summary>The end of a resource has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataResource"/> with all properties filled in.
        /// </remarks>
        ResourceEnd,

        /// <summary>The start of a nested resource info has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataNestedResourceInfo"/> but no properties may be filled in until the LinkEnd state is reached.
        /// </remarks>
        NestedResourceInfoStart,

        /// <summary>The end of a nested resource info has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataNestedResourceInfo"/> with all properties filled in.
        /// </remarks>
        NestedResourceInfoEnd,

        /// <summary>An entity reference link was read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataEntityReferenceLink"/> which is fully populated.
        /// Note that there's no End state for this item.
        /// </remarks>
        EntityReferenceLink,

        /// <summary>The reader has thrown an exception; nothing can be read from the reader anymore.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns null.
        /// </remarks>
        Exception,

        /// <summary>The reader has completed; nothing can be read anymore.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns null.
        /// </remarks>
        Completed,

        /// <summary>The reader is positioned on a non-null primivite value within an untyped collection.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataPrimitiveValue"/>.
        /// </remarks>
        Primitive,

        /// <summary>The start of a delta resource set has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataDeltaResourceSet"/>.
        /// </remarks>
        DeltaResourceSetStart,

        /// <summary>The end of a delta resource set has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataDeltaResourceSet"/>.
        /// </remarks>
        DeltaResourceSetEnd,

        /// <summary>The start of a deleted resource has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataDeletedResource"/>.
        /// </remarks>
        DeletedResourceStart,

        /// <summary>The end of a deleted resource has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataDeletedResource"/>.
        /// </remarks>
        DeletedResourceEnd,

        /// <summary>The reder is positioned on a delta link.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataDeltaLink"/>.
        /// </remarks>
        DeltaLink,

        /// <summary>The reader is positioned on a delta deleted link.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataDeltaDeletedLink"/>.
        /// </remarks>
        DeltaDeletedLink,
    }
}
