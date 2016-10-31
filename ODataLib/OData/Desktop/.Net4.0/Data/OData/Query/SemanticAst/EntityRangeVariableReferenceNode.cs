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
    using System;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;

    #endregion Namespaces

    /// <summary>
    /// Node to represent a range variable in an Any or All clause that referrs to an entity.
    /// </summary>
    public sealed class EntityRangeVariableReferenceNode : SingleEntityNode
    {
        /// <summary>
        ///  The name of the associated range variable (null if none)
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The entity type of the associated range variable.
        /// </summary>
        private readonly IEdmEntityTypeReference entityTypeReference;

        /// <summary>
        /// The range variable that the node represents.
        /// </summary>
        private readonly EntityRangeVariable rangeVariable;

        /// <summary>
        /// The entity set containing the collection that this range variable iterates over.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// Creates an <see cref="EntityRangeVariableReferenceNode"/>.
        /// </summary>
        /// <param name="name"> The name of the associated range variable (null if none)</param>
        /// <param name="rangeVariable">The actual range variable on the bind stack that this refers to</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name or rangeVariable is null.</exception>
        public EntityRangeVariableReferenceNode(string name, EntityRangeVariable rangeVariable)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            ExceptionUtils.CheckArgumentNotNull(rangeVariable, "rangeVariable");
            this.name = name;
            this.entitySet = rangeVariable.EntitySet;
            this.entityTypeReference = rangeVariable.EntityTypeReference;
            this.rangeVariable = rangeVariable;
        }

        /// <summary>
        /// Gets the name of the associated rangevariable (null if none)
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the entity type of the associated range variable.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the entity type of the associated range variable.
        /// </summary>
        public override IEdmEntityTypeReference EntityTypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets a reference to the range variable that this node represents.
        /// </summary>
        public EntityRangeVariable RangeVariable
        {
            get { return this.rangeVariable; }
        }

        /// <summary>
        /// Gets the entity set containing the collection that this range variable iterates over.
        /// </summary>
        public override IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return InternalQueryNodeKind.EntityRangeVariableReference;
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
