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

namespace Microsoft.Data.OData.Query.SemanticAst
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
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an AnyNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(AnyNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a BinaryOperatorNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(BinaryOperatorNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a CollectionNavigationNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionNavigationNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a CollectionPropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionPropertyAccessNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a ConstantNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(ConstantNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a ConvertNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(ConvertNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an EntityCollectionCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(EntityCollectionCastNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an EntityRangeVariableReferenceNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(EntityRangeVariableReferenceNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a NonEntityRangeVariableNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(NonentityRangeVariableReferenceNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleEntityCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleEntityCastNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleNavigationNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleNavigationNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleEntityFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleEntityFunctionCallNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleValueFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleValueFunctionCallNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a EntityCollectionFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(EntityCollectionFunctionCallNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a CollectionFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(CollectionFunctionCallNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleValueOpenPropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleValueOpenPropertyAccessNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleValuePropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(SingleValuePropertyAccessNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a UnaryOperatorNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(UnaryOperatorNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a NamedFunctionParameterNode.
        /// </summary>
        /// <param name="nodeIn">The node to visit.</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Visit(NamedFunctionParameterNode nodeIn)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new NotImplementedException();
        }
    }
}
