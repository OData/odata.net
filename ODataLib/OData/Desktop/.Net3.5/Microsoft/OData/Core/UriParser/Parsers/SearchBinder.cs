//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Class responsible for binding a syntactic filter expression into a bound tree of semantic nodes.
    /// </summary>
    internal sealed class SearchBinder
    {
        /// <summary>
        /// Method to use to visit the token tree and bind the tokens recursively.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor bindMethod;

        /// <summary>
        /// Creates a SearchBinder.
        /// </summary>
        /// <param name="bindMethod">Method to use to visit the token tree and bind the tokens recursively.</param>
        internal SearchBinder(MetadataBinder.QueryTokenVisitor bindMethod)
        {
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Binds the given filter token.
        /// </summary>
        /// <param name="search">The search token to bind.</param>
        /// <returns>A SearchClause with for given Token.</returns>
        internal SearchClause BindSearch(QueryToken search)
        {
            ExceptionUtils.CheckArgumentNotNull(search, "filter");

            QueryNode expressionNode = this.bindMethod(search);

            SingleValueNode expressionResultNode = expressionNode as SingleValueNode;

            SearchClause searchClause = new SearchClause(expressionResultNode);

            return searchClause;
        }
    }
}
