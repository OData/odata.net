//---------------------------------------------------------------------
// <copyright file="NamedFunctionParameterNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Node representing a semantically parsed parameter to a function.
    /// </summary>
    public class NamedFunctionParameterNode : QueryNode
    {
        /// <summary>
        /// The name of this parameter
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The semantically parsed value of this parameter
        /// </summary>
        private readonly QueryNode value;

        /// <summary>
        /// Creates a NamedFunctionParameterNode to represent a semantically parsed parameter to a function.
        /// </summary>
        /// <param name="name">the name of this function</param>
        /// <param name="value">the already semantically parsed value of this parameter.</param>
        public NamedFunctionParameterNode(string name, QueryNode value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets the name of this parameter
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the semantically parsed value of this parameter.
        /// </summary>
        public QueryNode Value
        {
            get { return value; }
        }

        /// <summary>
        /// Gets the kind of this node
        /// </summary>
        public override QueryNodeKind Kind
        {
            get { return (QueryNodeKind)this.InternalKind; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.NamedFunctionParameter;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input visitor is null</exception>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}