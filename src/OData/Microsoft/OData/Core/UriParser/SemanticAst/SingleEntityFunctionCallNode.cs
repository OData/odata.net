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

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.UriParser.Semantic;

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
        /// the list of functions represented by this node.
        /// </summary>
        private readonly ReadOnlyCollection<IEdmFunction> functions; 
        
        /// <summary>
        /// List of arguments provided to the function.
        /// </summary>
        private readonly IEnumerable<QueryNode> parameters;

        /// <summary>
        /// The return type of this function.
        /// </summary>
        private readonly IEdmEntityTypeReference returnedEntityTypeReference;

        /// <summary>
        /// The entity set or singleton containing the single entity that this function returns.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// The semantically bound parent of this function.
        /// </summary>
        private readonly QueryNode source;

        /// <summary>
        /// Create a SingleEntityFunctionCallNode
        /// </summary>
        /// <param name="name">The name of the function to call</param>
        /// <param name="parameters">List of arguments provided to the operation import. Can be null.</param>
        /// <param name="returnedEntityTypeReference">The return type of this function.</param>
        /// <param name="navigationSource">The entity set or singleton containing the single entity that this operation import returns.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name, returnedEntityTypeReference, or navigationSource is null.</exception>
        public SingleEntityFunctionCallNode(string name, IEnumerable<QueryNode> parameters, IEdmEntityTypeReference returnedEntityTypeReference, IEdmNavigationSource navigationSource)
            : this(name, null, parameters, returnedEntityTypeReference, navigationSource, null)
        {
        }

        /// <summary>
        /// Create a SingleEntityFunctionCallNode
        /// </summary>
        /// <param name="name">The name of the operation import to call</param>
        /// <param name="functions">the list of functions this node represents.</param>
        /// <param name="parameters">List of arguments provided to the function. Can be null.</param>
        /// <param name="returnedEntityTypeReference">The return type of this operation import.</param>
        /// <param name="navigationSource">The entity set or singleton containing the single entity that this operation import returns.</param>
        /// <param name="source">The semantically bound parent of this operation import.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name, returnedEntityTypeReference, or navigationSource is null.</exception>
        public SingleEntityFunctionCallNode(string name, IEnumerable<IEdmFunction> functions, IEnumerable<QueryNode> parameters, IEdmEntityTypeReference returnedEntityTypeReference, IEdmNavigationSource navigationSource, QueryNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            ExceptionUtils.CheckArgumentNotNull(returnedEntityTypeReference, "returnedEntityTypeReference");

            this.name = name;
            this.functions = new ReadOnlyCollection<IEdmFunction>(functions != null ? functions.ToList() : new List<IEdmFunction>());
            this.parameters = new ReadOnlyCollection<QueryNode>(parameters == null ? new List<QueryNode>() : parameters.ToList());
            this.returnedEntityTypeReference = returnedEntityTypeReference;
            this.navigationSource = navigationSource;
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
        /// Gets the list of functions that this node represents
        /// </summary>
        public IEnumerable<IEdmFunction> Functions
        {
            get { return this.functions; }
        }

        /// <summary>
        /// Gets the list of arguments provided to the function.
        /// </summary>
        public IEnumerable<QueryNode> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Gets the return type of this function.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.returnedEntityTypeReference; }
        }

        /// <summary>
        /// Gets the navigation source containing the single entity that this function returns.
        /// </summary>
        public override IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Gets the return type of this function.
        /// </summary>
        public override IEdmEntityTypeReference EntityTypeReference
        {
            get { return this.returnedEntityTypeReference; }
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
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
