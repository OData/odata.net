//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Class that knows how to bind a LambdaToken.
    /// </summary>
    internal sealed class LambdaBinder
    {
        /// <summary>
        /// Method used to bind a parent token.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor bindMethod;

        /// <summary>
        /// Constructs a LambdaBinder.
        /// </summary>
        /// <param name="bindMethod">Method used to bind a parent token.</param>
        internal LambdaBinder(MetadataBinder.QueryTokenVisitor bindMethod)
        {
            ExceptionUtils.CheckArgumentNotNull(bindMethod, "bindMethod");
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Binds a LambdaToken to metadata.
        /// </summary>
        /// <param name="lambdaToken">Token to bind.</param>
        /// <param name="state">Object to hold the state of binding.</param>
        /// <returns>A metadata bound any or all node.</returns>
        internal LambdaNode BindLambdaToken(LambdaToken lambdaToken, BindingState state)
        {
            ExceptionUtils.CheckArgumentNotNull(lambdaToken, "LambdaToken");
            ExceptionUtils.CheckArgumentNotNull(state, "state");

            // Start by binding the parent token
            CollectionNode parent = this.BindParentToken(lambdaToken.Parent);
            RangeVariable rangeVariable = null;

            // Add the lambda variable to the stack
            if (lambdaToken.Parameter != null)
            {
                rangeVariable = NodeFactory.CreateParameterNode(lambdaToken.Parameter, parent);
                state.RangeVariables.Push(rangeVariable);
            }

            // Bind the expression
            SingleValueNode expression = this.BindExpressionToken(lambdaToken.Expression);
            
            // Create the node
            LambdaNode lambdaNode = NodeFactory.CreateLambdaNode(state, parent, expression, rangeVariable, lambdaToken.Kind);

            // Remove the lambda variable as it is now out of scope
            if (rangeVariable != null)
            {
                state.RangeVariables.Pop();
            }

            return lambdaNode;
        }
        
        /// <summary>
        /// Bind the parent of the LambdaToken
        /// </summary>
        /// <param name="queryToken">the parent token</param>
        /// <returns>the bound parent node</returns>
        private CollectionNode BindParentToken(QueryToken queryToken)
        {
            QueryNode parentNode = this.bindMethod(queryToken);
            CollectionNode parentCollectionNode = parentNode as CollectionNode;
            if (parentCollectionNode == null)
            {
                SingleValueOpenPropertyAccessNode parentOpenPropertyNode =
                    parentNode as SingleValueOpenPropertyAccessNode;

                if (parentOpenPropertyNode == null)
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_LambdaParentMustBeCollection);
                }

                // support open collection properties 
                return new CollectionOpenPropertyAccessNode(parentOpenPropertyNode.Source, parentOpenPropertyNode.Name);
            }

            return parentCollectionNode;
        }

        /// <summary>
        /// Bind the expression of the LambdaToken
        /// </summary>
        /// <param name="queryToken">the expression token</param>
        /// <returns>the bound expression node</returns>
        private SingleValueNode BindExpressionToken(QueryToken queryToken)
        {
            SingleValueNode expression = this.bindMethod(queryToken) as SingleValueNode;
            if (expression == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_AnyAllExpressionNotSingleValue);
            }

            // type reference is allowed to be null for open properties.
            IEdmTypeReference expressionTypeReference = expression.GetEdmTypeReference();
            if (expressionTypeReference != null && !expressionTypeReference.AsPrimitive().IsBoolean())
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_AnyAllExpressionNotSingleValue);
            }

            return expression;
        }
    }
}
