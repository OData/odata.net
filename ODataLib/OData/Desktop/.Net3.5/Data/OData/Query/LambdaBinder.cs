//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData.Query
{
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

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
            DebugUtils.CheckNoExternalCallers(); 
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
            DebugUtils.CheckNoExternalCallers(); 
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

                // TODO: Add support for open collection properties here by replacing
                //      with something like an OpenCollectionNode.
                throw new ODataException(ODataErrorStrings.MetadataBinder_CollectionOpenPropertiesNotSupportedInThisRelease);
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
