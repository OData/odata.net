//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
        /// Gets a value indicating whether Ignore missing properties is set or not
        /// </summary>
        bool IgnoreMissingProperties { get; }

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
