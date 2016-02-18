//---------------------------------------------------------------------
// <copyright file="QueryNodeVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Visitors
{
    using System;
    using Microsoft.OData.Core.UriParser.Semantic;

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
        /// Visit a CollectionCountNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionCountNode nodeIn)
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
        /// Visit an CollectionOpenPropertyAccessNode
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
        /// Visit an EntityCollectionCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(EntityCollectionCastNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an EntityRangeVariableReferenceNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(EntityRangeVariableReferenceNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a NonEntityRangeVariableNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(NonentityRangeVariableReferenceNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleEntityCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleEntityCastNode nodeIn)
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
        /// Visit a SingleEntityFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleEntityFunctionCallNode nodeIn)
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
        /// Visit a EntityCollectionFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(EntityCollectionFunctionCallNode nodeIn)
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
        /// Visit an CollectionPropertyCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionPropertyCastNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an SingleValueCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleValueCastNode nodeIn)
        {
            throw new NotImplementedException();
        }
    }
}