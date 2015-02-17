//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Enumeration of all concrete payload element types for use in switch statements.
    /// </summary>
    public enum ODataPayloadElementType
    {
        /// <summary>
        /// Default value of the enum. No other option can be assumed.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Represents a completely empty payload, typically associated with 204 (No Content) responses
        /// </summary>
        EmptyPayload,

        /// <summary>
        /// Represents a property containing multiple complex type instances
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        ComplexMultiValueProperty,

        /// <summary>
        /// Represents an instance of a complex type
        /// </summary>
        ComplexInstance,

        /// <summary>
        /// Represents a collection of complex type instances
        /// </summary>
        ComplexInstanceCollection,
        
        /// <summary>
        /// Represents the value of a complex multi-value property
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        ComplexMultiValue,

        /// <summary>
        /// Represents a property that contains a complex type instance
        /// </summary>
        ComplexProperty,

        /// <summary>
        /// Represents a link that has not been expanded and is just a URI
        /// </summary>
        DeferredLink,
        
        /// <summary>
        /// Represents an instance of an entity type
        /// </summary>
        EntityInstance,

        /// <summary>
        /// Represents an instance of an entity set
        /// </summary>
        EntitySetInstance,
        
        /// <summary>
        /// Represents an Error payload, typically associated with BadRequests or instream errors
        /// </summary>
        ODataErrorPayload,

        /// <summary>
        /// Represents an internal exception inside an error payload
        /// </summary>
        ODataInternalExceptionPayload,

        /// <summary>
        /// Represents a link that has been expanded and contains another element
        /// </summary>
        ExpandedLink,

        /// <summary>
        /// Represents an HtmlError payload, typically associated with BadRequests or instream errors
        /// </summary>
        HtmlErrorPayload,

        /// <summary>
        /// Represents a collection of links
        /// </summary>
        LinkCollection,

        /// <summary>
        /// Represents a named stream instance
        /// </summary>
        NamedStreamInstance,

        /// <summary>
        /// Represents a navigation property, which contains another element
        /// </summary>
        NavigationPropertyInstance,

        /// <summary>
        /// Represents a collection of primitive values
        /// </summary>
        PrimitiveCollection,

        /// <summary>
        /// Represents the value of a primitive multi-value property
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        PrimitiveMultiValue,

        /// <summary>
        /// Represents a property containing multiple primitive values
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        PrimitiveMultiValueProperty,

        /// <summary>
        /// Represents a property with a primitive value
        /// </summary>
        PrimitiveProperty,

        /// <summary>
        /// Represents a primitive value
        /// </summary>
        PrimitiveValue,

        /// <summary>
        /// Represents a payload that is identifiable as an empty collection, but cannot be further differentiated
        /// </summary>
        EmptyUntypedCollection,

        /// <summary>
        /// Represents a payload that is a property whose value is an empty collection.
        /// </summary>
        EmptyCollectionProperty,

        /// <summary>
        /// Represents a payload that is identifiable as a null property, but cannot be further differentiated
        /// </summary>
        NullPropertyInstance,

        /// <summary>
        /// Represents a payload that is retrieved from $metadata endpoint
        /// </summary>
        MetadataPayloadElement,

        /// <summary>
        /// Represents a payload that is sent in a batch request
        /// </summary>
        BatchRequestPayload,

        /// <summary>
        /// Represents a payload that is received from a batch response
        /// </summary>
        BatchResponsePayload,

        /// <summary>
        /// Represents a payload this is received from the service document endpoint.
        /// </summary>
        ServiceDocumentInstance,

        /// <summary>
        /// Represents a workspace in a service document.
        /// </summary>
        WorkspaceInstance,

        /// <summary>
        /// Represents a resource collection in a workspace.
        /// </summary>
        ResourceCollectionInstance,

        /// <summary>
        /// Represents an service operation descriptor.
        /// </summary>
        ServiceOperationDescriptor,
    }
}
