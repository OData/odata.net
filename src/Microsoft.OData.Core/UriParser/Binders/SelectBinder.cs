//---------------------------------------------------------------------
// <copyright file="SelectBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Linq;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Use a Select syntactic tree to populate the correct values for Selection in an already parsed
    /// Expand Semantic Tree.
    /// </summary>
    internal sealed class SelectBinder
    {
        /// <summary>
        /// Visitor object to walk the select tree
        /// </summary>
        private readonly SelectPropertyVisitor visitor;


        /// <summary>
        /// Constructs a new SelectBinder.
        /// </summary>
        /// <param name="model">The model used for binding.</param>
        /// <param name="edmType">The entity type that the $select is being applied to.</param>
        /// <param name="maxDepth">the maximum recursive depth.</param>
        /// <param name="expandClauseToDecorate">The already built expand clause to decorate</param>
       /// <param name="resolver">Resolver for uri parser.</param>
        public SelectBinder(IEdmModel model, IEdmStructuredType edmType, int maxDepth, SelectExpandClause expandClauseToDecorate, ODataUriResolver resolver)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "tokenIn");
            ExceptionUtils.CheckArgumentNotNull(edmType, "entityType");
            ExceptionUtils.CheckArgumentNotNull(resolver, "resolver");

            this.visitor = new SelectPropertyVisitor(model, edmType, maxDepth, expandClauseToDecorate, resolver);
        }

        /// <summary>
        /// Visits the top level select token
        /// </summary>
        /// <param name="tokenIn">the select token to visit</param>
        /// <returns>A new SelectExpandClause decorated with the information from the selectToken</returns>
        public SelectExpandClause Bind(SelectToken tokenIn)
        {
            if (tokenIn == null || !tokenIn.Properties.Any())
            {
                // if there are no properties selected for this level, then by default we select
                // all properties (including nav prop links, functions, actions, and structural properties)
                this.visitor.DecoratedExpandClause.SetAllSelected(true);
            }
            else
            {
                // if there are properties selected for this level, then we return only
                // those specific properties in the payload, so clear the all selected flag
                // for this level.
                this.visitor.DecoratedExpandClause.SetAllSelected(false);
                foreach (PathSegmentToken property in tokenIn.Properties)
                {
                    property.Accept(this.visitor);
                }
            }

            return this.visitor.DecoratedExpandClause;
        }
    }
}