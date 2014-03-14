//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Query.SemanticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Node representing a function call which returns a single entity.
    /// </summary>
    public sealed class SingleEntityFunctionCallNode : SingleEntityNode
    {
        /// <summary>
        /// the name of this function 
        /// </summary>
        private readonly string name;
        
        /// <summary>
        /// the list of funcitonImports represented by this node.
        /// </summary>
        private readonly ReadOnlyCollection<IEdmFunctionImport> functionImports; 
        
        /// <summary>
        /// List of arguments provided to the function.
        /// </summary>
        private readonly IEnumerable<QueryNode> arguments;

        /// <summary>
        /// The return type of this function.
        /// </summary>
        private readonly IEdmEntityTypeReference entityTypeReference;

        /// <summary>
        /// The EntitySet containing the single entity that this function returns.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// The semantically bound parent of this function.
        /// </summary>
        private readonly QueryNode source;

        /// <summary>
        /// Create a SingleEntityFunctionCallNode
        /// </summary>
        /// <param name="name">The name of the function to call</param>
        /// <param name="arguments">List of arguments provided to the function. Can be null.</param>
        /// <param name="entityTypeReference">The return type of this function.</param>
        /// <param name="entitySet">The EntitySet containing the single entity that this function returns.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name, returnedEntityTypeReference, or entitySet is null.</exception>
        public SingleEntityFunctionCallNode(string name, IEnumerable<QueryNode> arguments, IEdmEntityTypeReference entityTypeReference, IEdmEntitySet entitySet)
        : this(name, null, arguments, entityTypeReference, entitySet, null)
        {
        }

        /// <summary>
        /// Create a SingleEntityFunctionCallNode
        /// </summary>
        /// <param name="name">The name of the function to call</param>
        /// <param name="functionImports">the list of function imports this node represents.</param>
        /// <param name="arguments">List of arguments provided to the function. Can be null.</param>
        /// <param name="entityTypeReference">The return type of this function.</param>
        /// <param name="entitySet">The EntitySet containing the single entity that this function returns.</param>
        /// <param name="source">The semantically bound parent of this function.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name, returnedEntityTypeReference, or entitySet is null.</exception>
        public SingleEntityFunctionCallNode(string name, IEnumerable<IEdmFunctionImport> functionImports, IEnumerable<QueryNode> arguments, IEdmEntityTypeReference entityTypeReference, IEdmEntitySet entitySet, QueryNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            ExceptionUtils.CheckArgumentNotNull(entityTypeReference, "typeReference");

            this.name = name;
            this.functionImports = new ReadOnlyCollection<IEdmFunctionImport>(functionImports != null ? functionImports.ToList() : new List<IEdmFunctionImport>());
            this.arguments = arguments;
            this.entityTypeReference = entityTypeReference;
            this.entitySet = entitySet;
            this.source = source;
        }

        /// <summary>
        /// Gets the name of the function to call
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the list of function imports that this node represents
        /// </summary>
        public IEnumerable<IEdmFunctionImport> FunctionImports
        {
            get { return this.functionImports; }
        }

        /// <summary>
        /// Gets the list of arguments provided to the function.
        /// </summary>
        public IEnumerable<QueryNode> Arguments
        {
            get { return this.arguments; }
        }

        /// <summary>
        /// Gets the return type of this function.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the EntitySet containing the single entity that this function returns.
        /// </summary>
        public override IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the return type of this function.
        /// </summary>
        public override IEdmEntityTypeReference EntityTypeReference
        {
            get { return this.entityTypeReference; }
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
                return InternalQueryNodeKind.SingleEntityFunctionCall;
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
