//---------------------------------------------------------------------
// <copyright file="QueryNodeVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;

    /// <summary>
    /// Visitor interface for walking the Semantic Tree.
    /// </summary>
    /// <typeparam name="T">Generic type produced by the visitor.</typeparam>
    public abstract class QueryNodeVisitor<T>
    {
        /// <summary>
        /// Visit an AllNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(AllNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an AnyNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(AnyNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a BinaryOperatorNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(BinaryOperatorNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a CountNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CountNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a CollectionNavigationNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionNavigationNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a CollectionPropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionPropertyAccessNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a CollectionOpenPropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionOpenPropertyAccessNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a ConstantNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(ConstantNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a ConvertNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(ConvertNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an CollectionResourceCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionResourceCastNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an ResourceRangeVariableReferenceNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(ResourceRangeVariableReferenceNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a NonEntityRangeVariableNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(NonResourceRangeVariableReferenceNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleResourceCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleResourceCastNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleNavigationNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleNavigationNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleResourceFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleResourceFunctionCallNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleValueFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleValueFunctionCallNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a CollectionResourceFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionResourceFunctionCallNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a CollectionFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionFunctionCallNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleValueOpenPropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleValueOpenPropertyAccessNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleValuePropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleValuePropertyAccessNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a UnaryOperatorNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(UnaryOperatorNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a NamedFunctionParameterNode.
        /// </summary>
        /// <param name="nodeIn">The node to visit.</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(NamedFunctionParameterNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a ParameterAliasNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public virtual T Visit(ParameterAliasNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SearchTermNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public virtual T Visit(SearchTermNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleComplexNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleComplexNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a CollectionComplexNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionComplexNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleValueCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleValueCastNode nodeIn)
        {
            throw new NotImplementedException();
        }
    }
}