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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Node to represent a function call that returns a collection of entities.
    /// </summary>
    public sealed class EntityCollectionFunctionCallNode : EntityCollectionNode
    {
        /// <summary>
        /// the name of this function.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// list of functions that this node represents.
        /// </summary>
        private readonly ReadOnlyCollection<IEdmFunction> functions;

        /// <summary>
        /// list of parameters provided to this function
        /// </summary>
        private readonly ReadOnlyCollection<QueryNode> parameters; 

        /// <summary>
        /// the type a single entity returned by this function
        /// </summary>
        private readonly IEdmEntityTypeReference entityTypeReference;

        /// <summary>
        /// the type of the collection returned by this function
        /// </summary>
        private readonly IEdmCollectionTypeReference returnedCollectionTypeReference;

        /// <summary>
        /// the set containing the entities returned by this function.
        /// </summary>
        private readonly IEdmEntitySetBase navigationSource;

        /// <summary>
        /// The semantically bound parent of this EntityCollectionFunctionCallNode.
        /// </summary>
        private readonly QueryNode source;

        /// <summary>
        /// Creates an EntityCollectionFunctionCallNode to represent a operation call that returns a collection of entities.
        /// </summary>
        /// <param name="name">The name of this operation.</param>
        /// <param name="functions">the list of functions that this node should represent.</param>
        /// <param name="parameters">the list of parameters to this operation</param>
        /// <param name="returnedCollectionTypeReference">the type the entity collection returned by this operation. The element type must be an entity type.</param>
        /// <param name="navigationSource">the set containing entities returned by this operation</param>
        /// <param name="source">the semantically bound parent of this EntityCollectionFunctionCallNode.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the provided name is null.</exception>
        /// <exception cref="System.ArgumentNullException">Throws if the provided collection type reference is null.</exception>
        /// <exception cref="System.ArgumentException">Throws if the element type of the provided collection type reference is not an entity type.</exception>
        /// <exception cref="System.ArgumentNullException">Throws if the input operation imports is null</exception>
        public EntityCollectionFunctionCallNode(string name, IEnumerable<IEdmFunction> functions, IEnumerable<QueryNode> parameters, IEdmCollectionTypeReference returnedCollectionTypeReference, IEdmEntitySetBase navigationSource, QueryNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            ExceptionUtils.CheckArgumentNotNull(returnedCollectionTypeReference, "returnedCollectionTypeReference");
            this.name = name;
            this.functions = new ReadOnlyCollection<IEdmFunction>(functions == null ? new List<IEdmFunction>() : functions.ToList());
            this.parameters = new ReadOnlyCollection<QueryNode>(parameters == null ? new List<QueryNode>() : parameters.ToList());
            this.returnedCollectionTypeReference = returnedCollectionTypeReference;
            this.navigationSource = navigationSource;

            this.entityTypeReference = returnedCollectionTypeReference.ElementType().AsEntityOrNull();
            if (this.entityTypeReference == null)
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity);
            }

            this.source = source;
        }

        /// <summary>
        /// Gets the name of this function
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the list of operation imports that this node represents.
        /// </summary>
        public IEnumerable<IEdmFunction> Functions
        {
            get { return this.functions; }
        }

        /// <summary>
        /// Gets the list of parameters provided to this function.
        /// </summary>
        public IEnumerable<QueryNode> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Gets the individual item type returned by this function.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        public override IEdmCollectionTypeReference CollectionType
        {
            get { return this.returnedCollectionTypeReference; }
        }

        /// <summary>
        /// Gets the individual entity type returned by this function.
        /// </summary>
        public override IEdmEntityTypeReference EntityItemType
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the navigation source contaiing the entities returned by this function.
        /// </summary>
        public override IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Gets the semantically bound parent of this function.
        /// </summary>
        public QueryNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.EntityCollectionFunctionCall;
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
