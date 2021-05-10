//---------------------------------------------------------------------
// <copyright file="ISyntacticTreeVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    using Microsoft.OData.UriParser.Aggregation;

    /// <summary>
    /// Visitor interface for walking the Syntactic Tree.
    /// </summary>
    /// <typeparam name="T">Return type for the visitor methods on this visitor.</typeparam>
    public interface ISyntacticTreeVisitor<T>
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
        /// <returns>A BinaryOperatorNode that's bound to this token</returns>
        T Visit(BinaryOperatorToken tokenIn);

        /// <summary>
        /// Visits a CountSegmentToken
        /// </summary>
        /// <param name="tokenIn">The Count segment token to visit.</param>
        /// <returns>A CountNode that's bound to this token</returns>
        T Visit(CountSegmentToken tokenIn);

        /// <summary>
        /// Visits an InToken
        /// </summary>
        /// <param name="tokenIn">The In token to visit.</param>
        /// <returns>An InNode that's bound to this token</returns>
        T Visit(InToken tokenIn);

        /// <summary>
        /// Visits a DottedIdentifierToken
        /// </summary>
        /// <param name="tokenIn">The DottedIdentifierToken to visit</param>
        /// <returns>Either a SingleResourceCastNode, or CollectionResourceCastNode bound to this DottedIdentifierToken</returns>
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
        /// <returns>A Resource or NonResource RangeVariableReferenceNode bound to this RangeVariableToken</returns>
        T Visit(RangeVariableToken tokenIn);

        /// <summary>
        /// Visits a SelectToken
        /// </summary>
        /// <param name="tokenIn">The SelectToken to bind</param>
        /// <returns>A QueryNode bound to this SelectToken</returns>
        T Visit(SelectToken tokenIn);

        /// <summary>
        /// Visits an SelectTermToken
        /// </summary>
        /// <param name="tokenIn">The SelectTermToken to visit</param>
        /// <returns>A QueryNode bound to this SelectTermToken</returns>
        T Visit(SelectTermToken tokenIn);

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

        /// <summary>
        /// Visits a AggregateToken
        /// </summary>
        /// <param name="tokenIn">The AggregateToken to bind</param>
        /// <returns>A T node bound to this AggregateToken</returns>
        T Visit(AggregateToken tokenIn);

        /// <summary>
        /// Visits a AggregateExpressionToken
        /// </summary>
        /// <param name="tokenIn">The AggregateExpressionToken to bind</param>
        /// <returns>A T node bound to this AggregateExpressionToken</returns>
        T Visit(AggregateExpressionToken tokenIn);

        /// <summary>
        /// Visits a EntitySetAggregateToken
        /// </summary>
        /// <param name="tokenIn">The EntitySetAggregateToken to bind</param>
        /// <returns>A T node bound to this EntitySetAggregateToken</returns>
        T Visit(EntitySetAggregateToken tokenIn);

        /// <summary>
        /// Visits a GroupByToken
        /// </summary>
        /// <param name="tokenIn">The GroupByToken to bind</param>
        /// <returns>A T node bound to this GroupByToken</returns>
        T Visit(GroupByToken tokenIn);
    }
}