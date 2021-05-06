//---------------------------------------------------------------------
// <copyright file="CountSegmentBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Class that knows how to bind a Count segment token.
    /// </summary>
    internal sealed class CountSegmentBinder
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
        /// Constructs a CountSegmentBinder object.
        /// </summary>
        /// <param name="bindMethod">Method to use to visit the token tree and bind the tokens recursively.</param>
        /// <param name="state">State of the metadata binding.</param>
        internal CountSegmentBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
        {
            this.bindMethod = bindMethod;
            this.state = state;
        }

        /// <summary>
        /// Binds an Count segment token.
        /// </summary>
        /// <param name="countSegmentToken">The Count segment token to bind.</param>
        /// <param name="state">State of the metadata binding.</param>
        /// <returns>The bound Count segment token.</returns>
        internal QueryNode BindCountSegment(CountSegmentToken countSegmentToken)
        {
            ExceptionUtils.CheckArgumentNotNull(countSegmentToken, "countSegmentToken");

            QueryNode source = this.bindMethod(countSegmentToken.NextToken);
            CollectionNode node = source as CollectionNode;

            if(node == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_CountSegmentNextTokenNotCollectionValue());
            }

            FilterClause filterClause = null;
            SearchClause searchClause = null;

            BindingState innerBindingState = new BindingState(state.Configuration);            
            innerBindingState.ImplicitRangeVariable = NodeFactory.CreateParameterNode(ExpressionConstants.It, node);
            MetadataBinder binder = new MetadataBinder(innerBindingState);

            if (countSegmentToken.FilterOption != null)
            {
                FilterBinder filterBinder = new FilterBinder(binder.Bind, innerBindingState);
                filterClause = filterBinder.BindFilter(countSegmentToken.FilterOption);
            }

            if(countSegmentToken.SearchOption != null)
            {
                SearchBinder searchBinder = new SearchBinder(binder.Bind);
                searchClause = searchBinder.BindSearch(countSegmentToken.SearchOption);
            }

            return new CountNode(node, filterClause, searchClause);
        }
    }
}