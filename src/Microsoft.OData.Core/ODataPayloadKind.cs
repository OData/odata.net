//---------------------------------------------------------------------
// <copyright file="ODataPayloadKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Enumeration representing the different kinds of payloads ODatLib can write.
    /// </summary>
    public enum ODataPayloadKind
    {
        /// <summary>Payload kind for a resource set.</summary>
        ResourceSet = 0,

        /// <summary>Payload kind for a resource.</summary>
        Resource = 1,

        /// <summary>Payload kind for a property.</summary>
        Property = 2,

        /// <summary>Payload kind for an entity reference link.</summary>
        EntityReferenceLink = 3,

        /// <summary>Payload kind for entity reference links.</summary>
        EntityReferenceLinks = 4,

        /// <summary>Payload kind for a raw value.</summary>
        Value = 5,

        /// <summary>Payload kind for a binary value.</summary>
        BinaryValue = 6,

        /// <summary>Payload kind for a collection.</summary>
        Collection = 7,

        /// <summary>Payload kind for a service document.</summary>
        ServiceDocument = 8,

        /// <summary>Payload kind for a metadata document.</summary>
        MetadataDocument = 9,

        /// <summary>Payload kind for an error.</summary>
        Error = 10,

        /// <summary>Payload kind for a batch.</summary>
        Batch = 11,

        /// <summary>Payload kind for parameters for a service action.</summary>
        Parameter = 12,

        /// <summary>Payload kind for individual property in an entity.</summary>
        IndividualProperty = 13,

        /// <summary>Payload kind for delta.</summary>
        Delta = 14,

        /// <summary>Payload kind for async.</summary>
        Asynchronous = 15,

        /// <summary>Unknown format</summary>
        Unsupported = int.MaxValue,
    }
}
