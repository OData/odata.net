//   OData .NET Libraries ver. 6.9
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
    using Microsoft.OData.Edm.Library;
    #endregion Namespaces

    /// <summary>
    /// Node representing a type segment that casts a single value node.
    /// </summary>
    public sealed class SingleValueCastNode : SingleValueNode
    {
        /// <summary>
        /// The single value node that we're casting to a different type.
        /// </summary>
        private readonly SingleValueNode source;

        /// <summary>
        /// The target type that the source is cast to.
        /// </summary>
        private readonly IEdmComplexTypeReference typeReference;

        /// <summary>
        /// Created a SingleValueCastNode with the given source node and the given type to cast to.
        /// </summary>
        /// <param name="source"> Source <see cref="SingleValueNode"/> that is being cast.</param>
        /// <param name="complexType">Type to cast to.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input complexType is null.</exception>
        public SingleValueCastNode(SingleValueNode source, IEdmComplexType complexType)
        {
            ExceptionUtils.CheckArgumentNotNull(complexType, "complexType");
            this.source = source;
            this.typeReference = new EdmComplexTypeReference(complexType, false);
        }

        /// <summary>
        /// Gets the single value node that we're casting to a different type.
        /// </summary>
        public SingleValueNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the target type that the source is cast to.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.typeReference; }
        }

        /// <summary>
        /// Gets the kind of this query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.SingleValueCast;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
