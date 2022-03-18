//---------------------------------------------------------------------
// <copyright file="CaseParameterNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Query node representing a binary operator.
    /// </summary>
    public sealed class CaseParameterNode : QueryNode
    {
        /// <summary>
        /// Create a CaseParameterNode
        /// </summary>
        /// <param name="condition">The condition operand.</param>
        /// <param name="value">The value operand.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the left or right inputs are null.</exception>
        /// <exception cref="ODataException">Throws if the two operands don't have the same type.</exception>
        public CaseParameterNode(SingleValueNode condition, QueryNode value)
        {
            ExceptionUtils.CheckArgumentNotNull(condition, "condition");
            ExceptionUtils.CheckArgumentNotNull(value, "value");

            Condition = condition;
            Value = value;

            if (condition.TypeReference == null || !condition.TypeReference.IsBoolean())
            {
                throw new ODataException("TODO:");
            }
        }

        /// <summary>
        /// Gets the Condition operand.
        /// </summary>
        public QueryNode Condition { get; }

        /// <summary>
        /// Gets the value operand.
        /// </summary>
        public QueryNode Value { get; }

        public override QueryNodeKind Kind => QueryNodeKind.CaseParameter;

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.CaseParameter;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        /// <exception cref="System.ArgumentNullException">throws if the input visitor is null.</exception>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}