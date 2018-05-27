//---------------------------------------------------------------------
// <copyright file="IODataJsonLightWriterResourceState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Interface representing a state of the JSON writer for resource.
    /// </summary>
    internal interface IODataJsonLightWriterResourceState
    {
        /// <summary>
        /// The resource being written.
        /// </summary>
        ODataResourceBase Resource { get; }

        /// <summary>
        /// The structured type for the resource (if available)
        /// </summary>
        IEdmStructuredType ResourceType { get; }

        /// <summary>
        /// The structured type which was derived from the model (may be either the same as structured type or its base type).
        /// </summary>
        IEdmStructuredType ResourceTypeFromMetadata { get; }

        /// <summary>
        /// The serialization info for the current resource.
        /// </summary>
        ODataResourceSerializationInfo SerializationInfo { get; }

        /// <summary>
        /// The current resource is for undeclared property or not.
        /// </summary>
        bool IsUndeclared { get; }

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
        /// Gets or creates the type context to answer basic questions regarding the type info of the resource.
        /// </summary>
        /// <param name="writingResponse">True if writing a response payload, false otherwise.</param>
        /// <returns>The type context to answer basic questions regarding the type info of the resource.</returns>
        ODataResourceTypeContext GetOrCreateTypeContext(bool writingResponse);
    }
}
