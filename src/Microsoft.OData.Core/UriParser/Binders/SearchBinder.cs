//---------------------------------------------------------------------
// <copyright file="SearchBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using ODataErrorStrings = Microsoft.OData.Strings;

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