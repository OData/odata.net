//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Query.SemanticAst;

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
        /// <param name="entityType">The entity type that the $select is being applied to.</param>
        /// <param name="maxDepth">the maximum recursive depth.</param>
        /// <param name="expandClauseToDecorate">The already built expand clause to decorate</param>
        public SelectBinder(IEdmModel model, IEdmEntityType entityType, int maxDepth, SelectExpandClause expandClauseToDecorate)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "tokenIn");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");

            this.visitor = new SelectPropertyVisitor(model, entityType, maxDepth, expandClauseToDecorate);
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
                this.visitor.DecoratedExpandClause.SetAllSelectionRecursively();
            }
            else
            {
                foreach (PathSegmentToken property in tokenIn.Properties)
                {
                    property.Accept(this.visitor);
                }
            }

            return this.visitor.DecoratedExpandClause;
        }
    }
}
