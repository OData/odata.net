//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;

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
