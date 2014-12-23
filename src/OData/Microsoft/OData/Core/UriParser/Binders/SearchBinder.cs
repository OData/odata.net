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
