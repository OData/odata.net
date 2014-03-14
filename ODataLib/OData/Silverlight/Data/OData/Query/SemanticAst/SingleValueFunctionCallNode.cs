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
    using System.Diagnostics;
    using System.Linq;
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Query.SemanticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Node representing a function call which returns a single value.
    /// </summary>
    public sealed class SingleValueFunctionCallNode : SingleValueNode
    {
        /// <summary>
        /// the name of this function.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The list of function imports
        /// </summary>
        private readonly ReadOnlyCollection<IEdmFunctionImport> functionImports;

        /// <summary>
        /// List of arguments to this function call.
        /// </summary>
        private readonly IEnumerable<QueryNode> arguments;

        /// <summary>
        /// The type of value returned by this function.
        /// </summary>
        private readonly IEdmTypeReference typeReference;

        /// <summary>
        /// The semantically bound parent of this function
        /// </summary>
        private readonly QueryNode source;

        /// <summary>
        /// Create a SingleValueFunctionCallNode
        /// </summary>
        /// <param name="name">The name of the function to call</param>
        /// <param name="arguments">List of arguments to this function call.</param>
        /// <param name="typeReference">The type of value returned by this function.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name is null.</exception>
        public SingleValueFunctionCallNode(string name, IEnumerable<QueryNode> arguments, IEdmTypeReference typeReference)
            : this(name, null, arguments, typeReference, null)
        {
        }

        /// <summary>
        /// Create a SingleValueFunctionCallNode
        /// </summary>
        /// <param name="name">The name of the function to call</param>
        /// <param name="functionImports">the list of functions to call</param>
        /// <param name="arguments">the list of arguments to this function</param>
        /// <param name="typeReference">the type of the value returned by this function.</param>
        /// <param name="source">The semantically bound parent of this function.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input functionImports is null.</exception>
        public SingleValueFunctionCallNode(string name, IEnumerable<IEdmFunctionImport> functionImports, IEnumerable<QueryNode> arguments, IEdmTypeReference typeReference, QueryNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            this.name = name;
            this.functionImports = new ReadOnlyCollection<IEdmFunctionImport>(functionImports != null ? functionImports.ToList() : new List<IEdmFunctionImport>());

            // TODO: SQLBU TFS Defect 1235273: SingleValueFunctionCallNode constructor should not allow arguments to be null, or should replace null with an empty collection
            this.arguments = arguments;

            // TODO: SQLBU TFS Defect 1235278: SingleValueFunctionCallNode constructor should not allow the type of the node to be a collection type
            Debug.Assert(typeReference == null || !typeReference.IsCollection(), "Type of a single value node cannot be a collection type");
            this.typeReference = typeReference;
            this.source = source;
        }

        /// <summary>
        /// Gets the name of the function to call.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the list of function imports.
        /// </summary>
        public IEnumerable<IEdmFunctionImport> FunctionImports
        {
            get
            {
                return this.functionImports;
            }
        }

        /// <summary>
        /// Gets the list of arguments to this function call.
        /// </summary>
        public IEnumerable<QueryNode> Arguments
        {
            get { return this.arguments; }
        }

        /// <summary>
        /// Gets The type of value returned by this function.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.typeReference; }
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
                return InternalQueryNodeKind.SingleValueFunctionCall;
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
