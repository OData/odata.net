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
