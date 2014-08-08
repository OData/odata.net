//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Client.Materialization
{
    using System.Data.Services.Client.Metadata;
    using Microsoft.Data.Edm;

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
