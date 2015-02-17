//---------------------------------------------------------------------
// <copyright file="IODataJsonLightWriterEntryState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Interface representing a state of the JSON writer for entry.
    /// </summary>
    internal interface IODataJsonLightWriterEntryState
    {
        /// <summary>
        /// The entry being written.
        /// </summary>
        ODataEntry Entry { get; }

        /// <summary>
        /// The entity type for the entry (if available)
        /// </summary>
        IEdmEntityType EntityType { get; }

        /// <summary>
        /// The entity type which was derived from the model (may be either the same as entity type or its base type.
        /// </summary>
        IEdmEntityType EntityTypeFromMetadata { get; }

        /// <summary>
        /// The serialization info for the current entry.
        /// </summary>
        ODataFeedAndEntrySerializationInfo SerializationInfo { get; }

        /// <summary>
        /// Flag which indicates that the odata.editLink metadata property has been written.
        /// </summary>
        bool EditLinkWritten { get; set; }

        /// <summary>
        /// Flag which indicates that the odata.readLink metadata property has been written.
        /// </summary>
        bool ReadLinkWritten { get; set; }

        /// <summary>
        /// Flag which indicates that the odata.mediaEditLink metadata property has been written.
        /// </summary>
        bool MediaEditLinkWritten { get; set; }
        
        /// <summary>
        /// Flag which indicates that the odata.mediaReadLink metadata property has been written.
        /// </summary>
        bool MediaReadLinkWritten { get; set; }

        /// <summary>
        /// Flag which indicates that the odata.mediaContentType metadata property has been written.
        /// </summary>
        bool MediaContentTypeWritten { get; set; }

        /// <summary>
        /// Flag which indicates that the odata.mediaEtag metadata property has been written.
        /// </summary>
        bool MediaETagWritten { get; set; }

        /// <summary>
        /// Gets or creates the type context to answer basic questions regarding the type info of the entry.
        /// </summary>
        /// <param name="model">The Edm model to use.</param>
        /// <param name="writingResponse">True if writing a response payload, false otherwise.</param>
        /// <returns>The type context to answer basic questions regarding the type info of the entry.</returns>
        ODataFeedAndEntryTypeContext GetOrCreateTypeContext(IEdmModel model, bool writingResponse);
    }
}
