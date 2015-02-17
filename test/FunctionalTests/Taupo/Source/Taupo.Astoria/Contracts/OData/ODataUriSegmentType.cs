//---------------------------------------------------------------------
// <copyright file="ODataUriSegmentType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Enumeration of segment types
    /// </summary>
    public enum ODataUriSegmentType
    {
        /// <summary>
        /// Represents a segment that could not be interpreted or was missing from the metadata
        /// </summary>
        Unrecognized = 0,

        /// <summary>
        /// Represents a primitive property segment
        /// </summary>
        PrimitiveProperty,

        /// <summary>
        /// Represents a complex property segment
        /// </summary>
        ComplexProperty,

        /// <summary>
        /// Represents a multi-value property segment
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        MultiValueProperty,

        /// <summary>
        /// Represents a navigation property segment
        /// </summary>
        NavigationProperty,

        /// <summary>
        /// Represents a top-level entity set segment
        /// </summary>
        EntitySet,

        /// <summary>
        /// Represents a key expression segment
        /// </summary>
        Key,

        /// <summary>
        /// Represents a parameters segment
        /// </summary>
        Parameters,

        /// <summary>
        /// Represents the system '$value' segment
        /// </summary>
        Value,

        /// <summary>
        /// Represents the system '$ref' segment
        /// </summary>
        EntityReferenceLinks,

        /// <summary>
        /// Represents the system '$count' segment
        /// </summary>
        Count,

        /// <summary>
        /// Represents the system '$batch' segment
        /// </summary>
        Batch,

        /// <summary>
        /// Represents the system '$metadata' segment
        /// </summary>
        Metadata,
        
        /// <summary>
        /// Represents the root segment of the service
        /// </summary>
        ServiceRoot,

        /// <summary>
        /// Represents the '*' segment used to select all non-navigation properties on a type
        /// </summary>
        SelectAll,

        /// <summary>
        /// Represents the named stream endpoint for an entities media resource
        /// </summary>
        NamedStream,

        /// <summary>
        /// Represents an EntityType segment
        /// </summary>
        EntityType,

        /// <summary>
        /// Represents a function segment
        /// </summary>
        Function,
    }
}
