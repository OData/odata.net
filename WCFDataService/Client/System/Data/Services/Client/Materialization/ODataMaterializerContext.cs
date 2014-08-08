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
    /// Contains state and methods required to materialize odata collection, complex and primitive values
    /// </summary>
    internal class ODataMaterializerContext : IODataMaterializerContext
    {
        /// <summary>
        /// Initializes a materializer context
        /// </summary>
        /// <param name="responseInfo">Response information used to initialize with the materializer</param>
        internal ODataMaterializerContext(ResponseInfo responseInfo)
        {
            this.ResponseInfo = responseInfo;
        }

        /// <summary>
        /// Gets a value indicating whether to ignore missing properties when materializing values
        /// </summary>
        public bool IgnoreMissingProperties
        {
            get { return this.ResponseInfo.IgnoreMissingProperties; }
        }

        /// <summary>
        /// Gets a Client Edm model used to materialize values
        /// </summary>
        public ClientEdmModel Model
        {
            get { return this.ResponseInfo.Model; }
        }

        /// <summary>
        /// Gets the materialization Events
        /// </summary>
        public DataServiceClientResponsePipelineConfiguration ResponsePipeline
        {
            get { return this.ResponseInfo.ResponsePipeline; }
        }

        /// <summary>
        /// Gets the Response information that backs the information on the context
        /// </summary>
        protected ResponseInfo ResponseInfo { get; private set; }

        /// <summary>
        /// Resolved the given edm type to clr type.
        /// </summary>
        /// <param name="expectedType">Expected Clr type.</param>
        /// <param name="wireTypeName">Edm name of the type returned by the resolver.</param>
        /// <returns>an instance of ClientTypeAnnotation with the given name.</returns>
        public ClientTypeAnnotation ResolveTypeForMaterialization(Type expectedType, string wireTypeName)
        {
            return this.ResponseInfo.TypeResolver.ResolveTypeForMaterialization(expectedType, wireTypeName);
        }

        /// <summary>
        /// Resolves the EDM type for the given CLR type.
        /// </summary>
        /// <param name="expectedType">The client side CLR type.</param>
        /// <returns>The resolved EDM type.</returns>
        public IEdmType ResolveExpectedTypeForReading(Type expectedType)
        {
            return this.ResponseInfo.TypeResolver.ResolveExpectedTypeForReading(expectedType);
        }
    }
}
