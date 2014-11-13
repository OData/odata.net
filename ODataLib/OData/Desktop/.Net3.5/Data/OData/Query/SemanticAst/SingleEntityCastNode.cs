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
    #region Namespaces

    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.Edm.Library;

    #endregion Namespaces

    /// <summary>
    /// Node representing a type segment that casts a single entity parent node.
    /// </summary>
    public sealed class SingleEntityCastNode : SingleEntityNode
    {
        /// <summary>
        /// The entity that we're casting to a different type.
        /// </summary>
        private readonly SingleEntityNode source;

        /// <summary>
        /// The target type that the source is cast to.
        /// </summary>
        private readonly IEdmEntityTypeReference entityTypeReference;

        /// <summary>
        /// The EntitySet containing the source entity.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// Created a SingleEntityCastNode with the given source node and the given type to cast to.
        /// </summary>
        /// <param name="source"> Source <see cref="SingleValueNode"/> that is being cast.</param>
        /// <param name="entityType">Type to cast to.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input entityType is null.</exception>
        public SingleEntityCastNode(SingleEntityNode source, IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            this.source = source;
            this.entitySet = source != null ? source.EntitySet : null;
            this.entityTypeReference = new EdmEntityTypeReference(entityType, false);
        }

        /// <summary>
        /// Gets the entity that we're casting to a different type.
        /// </summary>
        public SingleEntityNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the target type that the source is cast to.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the target type that the source is cast to.
        /// </summary>
        public override IEdmEntityTypeReference EntityTypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the EntitySet containing the source entity..
        /// </summary>
        public override IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the kind of this query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return InternalQueryNodeKind.SingleEntityCast;
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
