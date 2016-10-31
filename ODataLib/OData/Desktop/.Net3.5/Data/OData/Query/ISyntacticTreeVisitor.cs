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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using Microsoft.Data.OData.Query.SemanticAst;

    /// <summary>
    /// Visitor interface for walking the Syntactic Tree.
    /// </summary>
    /// <typeparam name="T">Return type for the visitor methods on this visitor.</typeparam>
    internal interface ISyntacticTreeVisitor<T>
    {
        /// <summary>
        /// Visit an AllToken
        /// </summary>
        /// <param name="tokenIn">The All token to visit</param>
        /// <returns>An AllNode bound to this token</returns>
         T Visit(AllToken tokenIn);

        /// <summary>
        /// Visits an AnyToken
        /// </summary>
        /// <param name="tokenIn">The Any token to visit</param>
        /// <returns>An AnyNode that's bound to this token</returns>
        T Visit(AnyToken tokenIn);

        /// <summary>
        /// Visits a BinaryOperatorToken
        /// </summary>
        /// <param name="tokenIn">The Binary operator token to visit.</param>
        /// <returns>A BinaryOperatorNode thats bound to this token</returns>
        T Visit(BinaryOperatorToken tokenIn);

        /// <summary>
        /// Visits a DottedIdentifierToken
        /// </summary>
        /// <param name="tokenIn">The DottedIdentifierToken to visit</param>
        /// <returns>Either a SingleEntityCastNode, or EntityCollectionCastNode bound to this DottedIdentifierToken</returns>
        T Visit(DottedIdentifierToken tokenIn);

        /// <summary>
        /// Visits an ExpandToken
        /// </summary>
        /// <param name="tokenIn">The ExpandToken to visit</param>
        /// <returns>A QueryNode bound to this ExpandToken</returns>
        T Visit(ExpandToken tokenIn);

        /// <summary>
        /// Visits an ExpandTermToken
        /// </summary>
        /// <param name="tokenIn">The ExpandTermToken to visit</param>
        /// <returns>A QueryNode bound to this ExpandTermToken</returns>
        T Visit(ExpandTermToken tokenIn);

        /// <summary>
        /// Visits a FunctionCallToken
        /// </summary>
        /// <param name="tokenIn">The FunctionCallToken to visit</param>
        /// <returns>A SingleValueFunctionCallNode bound to this FunctionCallToken</returns>
        T Visit(FunctionCallToken tokenIn);

        /// <summary>
        /// Visits a LambdaToken
        /// </summary>
        /// <param name="tokenIn">The LambdaToken to visit</param>
        /// <returns>A LambdaNode bound to this LambdaToken</returns>
        T Visit(LambdaToken tokenIn);

        /// <summary>
        /// Visits a LiteralToken
        /// </summary>
        /// <param name="tokenIn">LiteralToken to visit</param>
        /// <returns>A ConstantNode bound to this LiteralToken</returns>
        T Visit(LiteralToken tokenIn);

        /// <summary>
        /// Visits a InnerPathToken
        /// </summary>
        /// <param name="tokenIn">The InnerPathToken to bind</param>
        /// <returns>A SingleValueNode or SingleEntityNode bound to this InnerPathToken</returns>
        T Visit(InnerPathToken tokenIn);

        /// <summary>
        /// Visits an OrderByToken
        /// </summary>
        /// <param name="tokenIn">The OrderByToken to bind</param>
        /// <returns>An OrderByClause bound to this OrderByToken</returns>
        T Visit(OrderByToken tokenIn);

        /// <summary>
        /// Visits a EndPathToken
        /// </summary>
        /// <param name="tokenIn">The EndPathToken to bind</param>
        /// <returns>A PropertyAccessNode bound to this EndPathToken</returns>
        T Visit(EndPathToken tokenIn);

        /// <summary>
        /// Visits a CustomQueryOptionToken
        /// </summary>
        /// <param name="tokenIn">The CustomQueryOptionToken to bind</param>
        /// <returns>A CustomQueryOptionNode bound to this CustomQueryOptionToken</returns>
        T Visit(CustomQueryOptionToken tokenIn);

        /// <summary>
        /// Visits a RangeVariableToken
        /// </summary>
        /// <param name="tokenIn">The RangeVariableToken to bind</param>
        /// <returns>An Entity or NonEntity RangeVariableReferenceNode bound to this RangeVariableToken</returns>
        T Visit(RangeVariableToken tokenIn);

        /// <summary>
        /// Visits a SelectToken
        /// </summary>
        /// <param name="tokenIn">The SelectToken to bind</param>
        /// <returns>A QueryNode bound to this SelectToken</returns>
        T Visit(SelectToken tokenIn);
        
        /// <summary>
        /// Visits a StarToken
        /// </summary>
        /// <param name="tokenIn">The StarToken to bind</param>
        /// <returns>A QueryNode bound to this StarToken</returns>
        T Visit(StarToken tokenIn);

        /// <summary>
        /// Visits a UnaryOperatorToken
        /// </summary>
        /// <param name="tokenIn">The UnaryOperatorToken to bind</param>
        /// <returns>A UnaryOperatorNode bound to this UnaryOperatorToken</returns>
        T Visit(UnaryOperatorToken tokenIn);

        /// <summary>
        /// Visits a FunctionParameterToken
        /// </summary>
        /// <param name="tokenIn">The FunctionParameterTokenb to bind</param>
        /// <returns>A FunctionParametertoken bound to this UnaryOperatorToken</returns>
        T Visit(FunctionParameterToken tokenIn);
    }
}
