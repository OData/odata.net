//---------------------------------------------------------------------
// <copyright file="IODataMaterializerContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Context for materialization of OData values
    /// </summary>
    internal interface IODataMaterializerContext
    {
        /// <summary>
        /// The DataServiceContext associated with this materializer context
        /// </summary>
        DataServiceContext Context { get; }

        /// <summary>
        /// Gets a value indicating whether to support untyped properties is set or not
        /// </summary>
        UndeclaredPropertyBehavior UndeclaredPropertyBehavior { get; }

        /// <summary>
        /// Gets a value indicated the Client Edm Model
        /// </summary>
        ClientEdmModel Model { get; }

        /// <summary>
        /// Gets the materialization Events
        /// </summary>
        DataServiceClientResponsePipelineConfiguration ResponsePipeline { get; }

        /// <summary>
        /// Resolves the client type that should be used for materialization.
        /// </summary>
        /// <param name="expectedType">Expected client clr type based on the API called.</param>
        /// <param name="readerTypeName">
        /// The name surfaced by the ODataLib reader. 
        /// If we have a server model, this will be a server type name that needs to be resolved. 
        /// If not, then this will already be a client type name.</param>
        /// <returns>The resolved annotation for the client type to materialize into.</returns>
        ClientTypeAnnotation ResolveTypeForMaterialization(Type expectedType, string readerTypeName);

        /// <summary>
        /// Resolves the expected EDM type to give to the ODataLib reader based on a client CLR type.
        /// </summary>
        /// <param name="clientClrType">The client side CLR type.</param>
        /// <returns>The resolved EDM type to provide to ODataLib.</returns>
        IEdmType ResolveExpectedTypeForReading(Type clientClrType);
    }
}
