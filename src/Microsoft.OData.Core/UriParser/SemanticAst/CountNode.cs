//---------------------------------------------------------------------
// <copyright file="CountNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Node representing count of related entities or items within a collection-valued property.
    /// </summary>
    public sealed class CountNode : SingleValueNode
    {
        /// <summary>
        /// The collection to be counted, could be any type of collection includes primitive type, enum type, complex type or entity type collection.
        /// </summary>
        private readonly CollectionNode source;

        /// <summary>
        /// The filter clause to be evaluated first before count is applied.
        /// </summary>
        private readonly FilterClause filterClause;

        /// <summary>
        /// The search clause to be evaluated first before count is applied.
        /// </summary>
        private readonly SearchClause searchClause;

        /// <summary>
        /// Constructs a new <see cref="CountNode"/>.
        /// </summary>
        /// <param name="source">The value containing the property.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source is null.</exception>
        public CountNode(CollectionNode source)
            :this(source, null, null)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");

            this.source = source;
        }

        /// <summary>
        /// Constructs a new <see cref="CountNode"/>.
        /// </summary>
        /// <param name="source">The value containing the property.</param>
        /// <param name="filterClause">The <see cref="Microsoft.OData.UriParser.FilterClause"/>in the count node.</param>
        /// <param name="searchClause">The <see cref="Microsoft.OData.UriParser.SearchClause"/>in the count node.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source is null.</exception>
        public CountNode(CollectionNode source, FilterClause filterClause, SearchClause searchClause)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");

            this.source = source;
            this.filterClause = filterClause;
            this.searchClause = searchClause;
        }

        /// <summary>
        /// Gets the collection property node to be counted.
        /// </summary>
        public CollectionNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the filter node.
        /// </summary>
        public FilterClause FilterClause
        {
            get { return this.filterClause; }
        }

        /// <summary>
        /// Gets the search node.
        /// </summary>
        public SearchClause SearchClause
        {
            get { return this.searchClause; }
        }

        /// <summary>
        /// Gets the value type this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            // The value type is same type as the type returned by IQueryable LongCount method
            get { return EdmCoreModel.Instance.GetInt64(false); }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.Count;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> to walk a tree of <see cref="QueryNode"/>s.
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