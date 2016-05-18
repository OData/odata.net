//---------------------------------------------------------------------
// <copyright file="FilterBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Class responsible for binding a syntactic filter expression into a bound tree of semantic nodes.
    /// </summary>
    internal sealed class FilterBinder
    {
        /// <summary>
        /// Method to use to visit the token tree and bind the tokens recursively.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor bindMethod;

        /// <summary>
        /// State to use for binding.
        /// </summary>
        private readonly BindingState state;

        /// <summary>
        /// Creates a FilterBinder.
        /// </summary>
        /// <param name="bindMethod">Method to use to visit the token tree and bind the tokens recursively.</param>
        /// <param name="state">State to use for binding.</param>
        internal FilterBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
        {
            this.bindMethod = bindMethod;
            this.state = state;
        }

        /// <summary>
        /// Binds the given filter token.
        /// </summary>
        /// <param name="filter">The filter token to bind.</param>
        /// <returns>A FilterNode with the given path linked to it (if provided).</returns>
        internal FilterClause BindFilter(QueryToken filter)
        {
            ExceptionUtils.CheckArgumentNotNull(filter, "filter");

            QueryNode expressionNode = this.bindMethod(filter);

            SingleValueNode expressionResultNode = expressionNode as SingleValueNode;
            if (expressionResultNode == null ||
                (expressionResultNode.TypeReference != null && !expressionResultNode.TypeReference.IsODataPrimitiveTypeKind()))
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue);
            }

            // The type may be null here if the query statically represents the null literal or an open property.
            IEdmTypeReference expressionResultType = expressionResultNode.TypeReference;
            if (expressionResultType != null)
            {
                IEdmPrimitiveTypeReference primitiveExpressionResultType = expressionResultType.AsPrimitiveOrNull();
                if (primitiveExpressionResultType == null ||
                    primitiveExpressionResultType.PrimitiveKind() != EdmPrimitiveTypeKind.Boolean)
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue);
                }
            }

            FilterClause filterNode = new FilterClause(expressionResultNode, this.state.ImplicitRangeVariable);

            return filterNode;
        }
    }
}