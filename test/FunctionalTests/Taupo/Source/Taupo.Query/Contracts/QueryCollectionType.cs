//---------------------------------------------------------------------
// <copyright file="QueryCollectionType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a collection type in a QueryType hierarchy. 
    /// </summary>
    public abstract class QueryCollectionType : QueryType
    {
        /// <summary>
        /// Initializes a new instance of the QueryCollectionType class.
        /// </summary>
        /// <param name="elementType">Type of a single element in the collection.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        protected QueryCollectionType(QueryType elementType, IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
            this.ElementType = elementType;
        }

        /// <summary>
        /// Gets the type of a single element in the collection.
        /// </summary>
        public QueryType ElementType { get; private set; }

        /// <summary>
        /// Gets the null value for a given type.
        /// </summary>
        public new QueryCollectionValue NullValue
        {
            get { return this.CreateCollectionWithValues(null); }
        }

        /// <summary>
        /// Creates a generic collection of elements of this type.
        /// </summary>
        /// <typeparam name="TElement">Type argument for the collection.</typeparam>
        /// <param name="element">Type of the single element of the collection.</param>
        /// <returns>Collection of the given type.</returns>
        public static QueryCollectionType<TElement> Create<TElement>(TElement element) 
            where TElement : QueryType
        {
            ExceptionUtilities.CheckArgumentNotNull(element, "element");

            return new QueryCollectionType<TElement>(element, element.EvaluationStrategy);
        }

        /// <summary>
        /// Creates a collection with values the specified values.
        /// </summary>
        /// <param name="elements">The collection elements. If null is passed, the collection will be a null
        /// collection.</param>
        /// <returns>Newly created collection value.</returns>
        public virtual QueryCollectionValue CreateCollectionWithValues(IEnumerable<QueryValue> elements)
        {
            return new QueryCollectionValue(this, this.EvaluationStrategy, QueryError.GetErrorFromValues(elements), elements);
        }

        /// <summary>
        /// Determines whether the type can be assigned from another.
        /// </summary>
        /// <param name="queryType">Type to assign from.</param>
        /// <returns>True if assignment is possible, false otherwise.</returns>
        public override bool IsAssignableFrom(QueryType queryType)
        {
            if (object.ReferenceEquals(this, queryType))
            {
                return true;
            }

            // if it is not a collection type, then it is not assignable
            var collectionType = queryType as QueryCollectionType;
            if (collectionType == null)
            {
                return false;
            }
            else
            {
                // otherwise, it is assignable if its element type is
                return this.ElementType.IsAssignableFrom(collectionType.ElementType);
            }
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query type.</param>
        /// <returns>The result of visiting this query type.</returns>
        public override TResult Accept<TResult>(IQueryTypeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// The default value of a collection is an empty collection.
        /// </summary>
        /// <returns>An empty collection.</returns>
        protected override QueryValue GetDefaultValueInternal()
        {
            return this.CreateCollectionWithValues(Enumerable.Empty<QueryValue>());
        }
    }
}
