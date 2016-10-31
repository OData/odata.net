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
    using Microsoft.Data.Edm;

    /// <summary>
    /// Semantic node that represents a single-value open property access, which is not bound to an EDM model.
    /// </summary>
    public sealed class SingleValueOpenPropertyAccessNode : SingleValueNode
    {
        /// <summary>
        /// The value containing this property.
        /// </summary>
        private readonly SingleValueNode source;

        /// <summary>
        /// The name of the open property to be bound outside the EDM model.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Constructs a <see cref="SingleValueOpenPropertyAccessNode"/>.
        /// </summary>
        /// <param name="source">The value containing this property.</param>
        /// <param name="openPropertyName">The name of the open property to be bound outside the EDM model.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source or openPropertyName is null.</exception>
        public SingleValueOpenPropertyAccessNode(SingleValueNode source, string openPropertyName)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(openPropertyName, "openPropertyName");

            this.name = openPropertyName;
            this.source = source;
        }

        /// <summary>
        ///  Gets the value containing this property.
        /// </summary>
        public SingleValueNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the name of the open property to be bound outside the EDM model.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the type of the single value this node represents.
        /// </summary>
        /// <remarks>
        /// The value of this property will always be null for open properties.
        /// </remarks>
        public override IEdmTypeReference TypeReference
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the kind of this query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return InternalQueryNodeKind.SingleValueOpenPropertyAccess;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input visitor is null.</exception>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
