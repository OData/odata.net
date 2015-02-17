//---------------------------------------------------------------------
// <copyright file="IReaderMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Contract for additional metadata that can be passed to the ODataLib Reader to 
    /// aid deserialization.
    /// </summary>
    public interface IReaderMetadata
    {
        /// <summary>
        /// The type to use when reading payloads to specify as the expected type for the reader.
        /// </summary>
        IEdmTypeReference ExpectedType { get; }

        /// <summary>
        /// The entity set to use when reading entry or feed payloads.
        /// </summary>
        IEdmEntitySet EntitySet { get; }

        /// <summary>
        /// The structural property to use when reading property payloads.
        /// </summary>
        IEdmStructuralProperty StructuralProperty { get; }

        /// <summary>
        /// The navigation property to use when reading entity reference link payloads.
        /// </summary>
        IEdmNavigationProperty NavigationProperty { get; }

        /// <summary>
        /// The function import metadata used when reading a collection or parameter payload, 
        /// or when reading a property payload produced by an operation.
        /// </summary>
        IEdmOperationImport FunctionImport { get; }
    }
}
