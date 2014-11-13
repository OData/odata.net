//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Node representing a primitive constant value.
    /// </summary>
    public sealed class ConstantNode : SingleValueNode
    {
        /// <summary>
        /// The primitive constant value.
        /// </summary>
        private readonly object constantValue;

        /// <summary>
        /// Cache for the TypeReference after it has been calculated for the current state of the node.
        /// </summary>
        private readonly IEdmTypeReference typeReference;

        /// <summary>
        /// Create a ConstantNode
        /// </summary>
        /// <param name="constantValue">This node's primitive value.</param>
        /// <param name="literalText">The literal text for this node's value, formatted according to the OData URI literal formatting rules.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input literalText is null.</exception>
        public ConstantNode(object constantValue, string literalText)
            : this(constantValue)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalText, "literalText");
            this.LiteralText = literalText;
        }

        /// <summary>
        /// Create a ConstantNode
        /// </summary>
        /// <param name="constantValue">This node's primitive value.</param>
        /// <param name="literalText">The literal text for this node's value, formatted according to the OData URI literal formatting rules.</param>
        /// <param name="typeReference">The typeReference of this node's value.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input literalText is null.</exception>
        public ConstantNode(object constantValue, string literalText, IEdmTypeReference typeReference)
            : this(constantValue)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalText, "literalText");
            this.LiteralText = literalText;
            this.typeReference = typeReference;
        }

        /// <summary>
        /// Create a ConstantNode
        /// </summary>
        /// <param name="constantValue">This node's primitive value.</param>
        public ConstantNode(object constantValue)
        {
            this.constantValue = constantValue;
            this.typeReference = constantValue == null ? null : EdmLibraryExtensions.GetPrimitiveTypeReference(constantValue.GetType());
        }

        /// <summary>
        /// Gets the primitive constant value.
        /// </summary>
        public object Value
        {
            get
            {
                return this.constantValue;
            }
        }

        /// <summary>
        /// Get or Set the literal text for this node's value, formatted according to the OData URI literal formatting rules. May be null if the text was not provided at construction time.
        /// </summary>
        public string LiteralText { get; private set; }

        /// <summary>
        /// Gets the resouce type of the single value this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get
            {
                return this.typeReference;
            }
        }

        /// <summary>
        /// Gets the kind of the query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.Constant;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> to walk a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input visitor is null.</exception>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
