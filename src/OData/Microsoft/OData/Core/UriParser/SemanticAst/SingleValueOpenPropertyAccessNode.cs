//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;

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
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
