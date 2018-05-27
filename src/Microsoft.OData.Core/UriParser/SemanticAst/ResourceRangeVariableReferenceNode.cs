//---------------------------------------------------------------------
// <copyright file="ResourceRangeVariableReferenceNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Node to represent a range variable in an Any or All clause that refers to an entity or a complex.
    /// </summary>
    public sealed class ResourceRangeVariableReferenceNode : SingleResourceNode
    {
        /// <summary>
        ///  The name of the associated range variable (null if none)
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The structured type of the associated range variable.
        /// </summary>
        private readonly IEdmStructuredTypeReference structuredTypeReference;

        /// <summary>
        /// The range variable that the node represents.
        /// </summary>
        private readonly ResourceRangeVariable rangeVariable;

        /// <summary>
        /// The navigation source containing the collection that this range variable iterates over.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Creates an <see cref="ResourceRangeVariableReferenceNode"/>.
        /// </summary>
        /// <param name="name"> The name of the associated range variable (null if none)</param>
        /// <param name="rangeVariable">The actual range variable on the bind stack that this refers to</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name or rangeVariable is null.</exception>
        public ResourceRangeVariableReferenceNode(string name, ResourceRangeVariable rangeVariable)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            ExceptionUtils.CheckArgumentNotNull(rangeVariable, "rangeVariable");
            this.name = name;
            this.navigationSource = rangeVariable.NavigationSource;
            this.structuredTypeReference = rangeVariable.StructuredTypeReference;
            this.rangeVariable = rangeVariable;
        }

        /// <summary>
        /// Gets the name of the associated range variable (null if none)
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
            get { return this.structuredTypeReference; }
        }

        /// <summary>
        /// Gets a reference to the range variable that this node represents.
        /// </summary>
        public ResourceRangeVariable RangeVariable
        {
            get { return this.rangeVariable; }
        }

        /// <summary>
        /// Gets the navigation source containing the collection that this range variable iterates over.
        /// </summary>
        public override IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Gets the structured type of the associated range variable.
        /// </summary>
        public override IEdmStructuredTypeReference StructuredTypeReference
        {
            get { return this.structuredTypeReference; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.ResourceRangeVariableReference;
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