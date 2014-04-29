//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;

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
        public SelectBinder(IEdmModel model, IEdmStructuredType edmType, int maxDepth, SelectExpandClause expandClauseToDecorate)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "tokenIn");
            ExceptionUtils.CheckArgumentNotNull(edmType, "entityType");

            this.visitor = new SelectPropertyVisitor(model, edmType, maxDepth, expandClauseToDecorate);
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
