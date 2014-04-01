//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Bind a select and expand AST with metadata from an IEdmModel.
    /// </summary>
    //// TODO 1466134 We don't need this layer once V4 is working and always used.
    internal interface ISelectExpandSemanticBinder
    {
        /// <summary>
        /// Add semantic meaning to a Select or Expand AST
        /// </summary>
        /// <param name="elementType">the top level entity type.</param>
        /// <param name="entitySet">the top level entity set</param>
        /// <param name="expandToken">the syntactically parsed expand token</param>
        /// <param name="selectToken">the syntactically parsed select token</param>
        /// <param name="configuration">The configuration to use for parsing.</param>
        /// <returns>A select expand clause bound to metadata.</returns>
        SelectExpandClause Bind(
            IEdmStructuredType elementType, 
            IEdmEntitySet entitySet, 
            ExpandToken expandToken, 
            SelectToken selectToken, 
            ODataUriParserConfiguration configuration);
    }
}
