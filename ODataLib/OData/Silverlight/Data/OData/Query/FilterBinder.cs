//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData.Query
{
    using System;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

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
            DebugUtils.CheckNoExternalCallers(); 
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
            DebugUtils.CheckNoExternalCallers(); 
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
