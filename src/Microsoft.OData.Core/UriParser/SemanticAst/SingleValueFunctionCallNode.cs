//---------------------------------------------------------------------
// <copyright file="SingleValueFunctionCallNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

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
        /// The list of functions
        /// </summary>
        private readonly ReadOnlyCollection<IEdmFunction> functions;

        /// <summary>
        /// List of arguments to this function call.
        /// </summary>
        private readonly IEnumerable<QueryNode> parameters;

        /// <summary>
        /// The type of value returned by this function.
        /// </summary>
        private readonly IEdmTypeReference returnedTypeReference;

        /// <summary>
        /// The semantically bound parent of this function
        /// </summary>
        private readonly QueryNode source;

        /// <summary>
        /// Create a SingleValueFunctionCallNode
        /// </summary>
        /// <param name="name">The name of the function to call</param>
        /// <param name="parameters">List of arguments to this function call.</param>
        /// <param name="returnedTypeReference">The type of value returned by this function.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name is null.</exception>
        public SingleValueFunctionCallNode(string name, IEnumerable<QueryNode> parameters, IEdmTypeReference returnedTypeReference)
            : this(name, null, parameters, returnedTypeReference, null)
        {
        }

        /// <summary>
        /// Create a SingleValueFunctionCallNode
        /// </summary>
        /// <param name="name">The name of the function to call</param>
        /// <param name="functions">the list of functions that this node should represent.</param>
        /// <param name="parameters">the list of arguments to this function</param>
        /// <param name="returnedTypeReference">the type of the value returned by this function.</param>
        /// <param name="source">The semantically bound parent of this function.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input operationImports is null.</exception>
        public SingleValueFunctionCallNode(string name, IEnumerable<IEdmFunction> functions, IEnumerable<QueryNode> parameters, IEdmTypeReference returnedTypeReference, QueryNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");

            this.name = name;
            this.functions = new ReadOnlyCollection<IEdmFunction>(functions != null ? functions.ToList() : new List<IEdmFunction>());

            this.parameters = parameters ?? Enumerable.Empty<QueryNode>();

            if (returnedTypeReference != null)
            {
                if (returnedTypeReference.IsCollection()
                    || !(returnedTypeReference.IsComplex() || returnedTypeReference.IsPrimitive() || returnedTypeReference.IsEnum()))
                {
                    throw new ArgumentException(ODataErrorStrings.Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
                }
            }

            this.returnedTypeReference = returnedTypeReference;
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
        /// Gets the list of operation imports.
        /// </summary>
        public IEnumerable<IEdmFunction> Functions
        {
            get
            {
                return this.functions;
            }
        }

        /// <summary>
        /// Gets the list of arguments to this function call.
        /// </summary>
        public IEnumerable<QueryNode> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Gets The type of value returned by this function.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.returnedTypeReference; }
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