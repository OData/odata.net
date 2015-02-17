//---------------------------------------------------------------------
// <copyright file="QueryCollectionType`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents a generic collection type in a QueryType hierarchy. 
    /// </summary>
    /// <typeparam name="TElement">Type of the single element in the collection.</typeparam>
    public class QueryCollectionType<TElement> : QueryCollectionType
         where TElement : QueryType
    {
        private object collectionType;

        /// <summary>
        /// Initializes a new instance of the QueryCollectionType class.
        /// </summary>
        /// <param name="elementType">Type of a single element in the collection.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        internal QueryCollectionType(TElement elementType, IQueryEvaluationStrategy evaluationStrategy)
            : base(elementType, evaluationStrategy)
        {
        }

        /// <summary>
        /// Gets the type of a single element in the collection.
        /// </summary>
        public new TElement ElementType
        {
            get { return (TElement)base.ElementType; }
        }

        /// <summary>
        /// Gets the null value for a given type.
        /// </summary>
        /// <value>The null value.</value>
        public new QueryCollectionValue NullValue
        {
            get { return this.CreateCollectionWithValues(null); }
        }

        /// <summary>
        /// Gets the string representation of a given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "Collection({0})", this.ElementType.StringRepresentation);
            }
        }

        /// <summary>
        /// Creates the typed collection where element type is the current primitive type.
        /// </summary>
        /// <returns>Collection of the given type.</returns>
        public new QueryCollectionType<QueryCollectionType<TElement>> CreateCollectionType()
        {
            if (this.collectionType == null)
            {
                this.collectionType = Create(this);
            }

            return (QueryCollectionType<QueryCollectionType<TElement>>)this.collectionType;
        }

        /// <summary>
        /// Gets the non-strongly typed collection type for this type.
        /// </summary>
        /// <returns>
        /// Instance of <see cref="QueryCollectionType"/> which is a collection of this type.
        /// </returns>
        protected override QueryCollectionType CreateCollectionTypeInternal()
        {
            return this.CreateCollectionType();
        }

        /// <summary>
        /// Gets the non-strongly typed null value.
        /// </summary>
        /// <returns>Null value.</returns>
        protected override QueryValue GetNullValueInternal()
        {
            return this.NullValue;
        }

        /// <summary>
        /// Creates the non-strongly typed error value.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <returns>Created error value.</returns>
        protected override QueryValue CreateErrorValueInternal(QueryError evaluationError)
        {
            return new QueryCollectionValue(this, this.EvaluationStrategy, evaluationError, null);
        }
    }
}
