//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

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
        /// list of function imports that this node represents.
        /// </summary>
        private readonly ReadOnlyCollection<IEdmFunctionImport> functionImports;

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
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// The semantically bound parent of this EntityCollectionFunctionCallNode.
        /// </summary>
        private readonly QueryNode source;

        /// <summary>
        /// Creates an EntityCollecitonFunctionCallNode to represent a function call that returns a collection of entities.
        /// </summary>
        /// <param name="name">The name of this function.</param>
        /// <param name="functionImports">the list of function imports that this node represents.</param>
        /// <param name="parameters">the list of parameters to this function</param>
        /// <param name="returnedCollectionTypeReference">the type the entity collection returned by this function. The element type must be an entity type.</param>
        /// <param name="entitySet">the set containing entities returned by this function</param>
        /// <param name="source">the semantically bound parent of this EntityCollectionFunctionCallNode.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the provided name is null.</exception>
        /// <exception cref="System.ArgumentNullException">Throws if the provided collection type reference is null.</exception>
        /// <exception cref="System.ArgumentException">Throws if the element type of the provided collection type reference is not an entity type.</exception>
        /// <exception cref="System.ArgumentNullException">Throws if the input function imports is null</exception>
        public EntityCollectionFunctionCallNode(string name, IEnumerable<IEdmFunctionImport> functionImports, IEnumerable<QueryNode> parameters, IEdmCollectionTypeReference returnedCollectionTypeReference, IEdmEntitySet entitySet, QueryNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            ExceptionUtils.CheckArgumentNotNull(returnedCollectionTypeReference, "returnedCollectionTypeReference");
            this.name = name;
            this.functionImports = new ReadOnlyCollection<IEdmFunctionImport>(functionImports == null ? new List<IEdmFunctionImport>() : functionImports.ToList());
            this.parameters = new ReadOnlyCollection<QueryNode>(parameters == null ? new List<QueryNode>() : parameters.ToList());
            this.returnedCollectionTypeReference = returnedCollectionTypeReference;
            this.entitySet = entitySet;

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
        /// Gets the list of function imports that this node represents.
        /// </summary>
        public IEnumerable<IEdmFunctionImport> FunctionImports
        {
            get { return this.functionImports; }
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
        /// Gets the entity set contaiing the entities returned by this function.
        /// </summary>
        public override IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
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
                DebugUtils.CheckNoExternalCallers();
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
