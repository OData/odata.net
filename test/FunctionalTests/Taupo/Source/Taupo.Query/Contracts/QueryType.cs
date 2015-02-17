//---------------------------------------------------------------------
// <copyright file="QueryType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents type of QueryExpression based trees.
    /// </summary>
    public abstract class QueryType : AnnotatedQueryItem
    {
        /// <summary>
        /// Initializes a new instance of the QueryType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        protected QueryType(IQueryEvaluationStrategy evaluationStrategy)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(evaluationStrategy, "evaluationStrategy");
            this.EvaluationStrategy = evaluationStrategy;
        }

        /// <summary>
        /// Gets instance of the QueryUnresolvedType.
        /// </summary>
        public static QueryType Unresolved
        {
            get
            {
                return QueryUnresolvedType.Instance;
            }
        }

        /// <summary>
        /// Gets instance of the QueryUnresolvedPrimitiveType.
        /// </summary>
        public static QueryScalarType UnresolvedPrimitive
        {
            get
            {
                return QueryUnresolvedScalarType.Instance;
            }
        }

        /// <summary>
        /// Gets instance of the QueryUnresolvedCollectionType.
        /// </summary>
        public static QueryCollectionType UnresolvedCollection
        {
            get
            {
                return QueryUnresolvedCollectionType.Instance;
            }
        }

        /// <summary>
        /// Gets the null value for the type.
        /// </summary>
        /// <value>The null value.</value>
        public QueryValue NullValue
        {
            get { return this.GetNullValueInternal(); }
        }

        /// <summary>
        /// Gets the default value for the type.
        /// </summary>
        /// <value>The default value.</value>
        public QueryValue DefaultValue
        {
            get { return this.GetDefaultValueInternal(); }
        }

        /// <summary>
        /// Gets a value indicating whether this type is unresolved.
        /// </summary>
        public virtual bool IsUnresolved
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the evaluation strategy.
        /// </summary>
        public IQueryEvaluationStrategy EvaluationStrategy { get; private set; }

        /// <summary>
        /// Gets string representation of the given query type.
        /// </summary>
        public abstract string StringRepresentation { get; }

        /// <summary>
        /// Creates the collection type with element type being this type.
        /// </summary>
        /// <returns>Created collection type.</returns>
        public QueryCollectionType CreateCollectionType()
        {
            return this.CreateCollectionTypeInternal();
        }

        /// <summary>
        /// Creates the error value of this type.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <returns>Created error value.</returns>
        public QueryValue CreateErrorValue(QueryError evaluationError)
        {
            return this.CreateErrorValueInternal(evaluationError);
        }

        /// <summary>
        /// Determines whether the type can be assigned from another.
        /// </summary>
        /// <param name="queryType">Type to assign from.</param>
        /// <returns>True if assignment is possible, false otherwise.</returns>
        public virtual bool IsAssignableFrom(QueryType queryType)
        {
            return object.ReferenceEquals(this, queryType);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query type.</param>
        /// <returns>The result of visiting this query type.</returns>
        public abstract TResult Accept<TResult>(IQueryTypeVisitor<TResult> visitor);

        /// <summary>
        /// Gets the non-strongly typed collection type for this type.
        /// </summary>
        /// <returns>Instance of <see cref="QueryCollectionType"/> which is a collection of this type.</returns>
        protected abstract QueryCollectionType CreateCollectionTypeInternal();

        /// <summary>
        /// Gets the non-strongly typed default value.
        /// </summary>
        /// <returns>default value.</returns>
        protected virtual QueryValue GetDefaultValueInternal()
        {
            return this.GetNullValueInternal();
        }

        /// <summary>
        /// Gets the non-strongly typed null value.
        /// </summary>
        /// <returns>Null value.</returns>
        protected abstract QueryValue GetNullValueInternal();

        /// <summary>
        /// Creates the non-strongly typed error value.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <returns>Created error value.</returns>
        protected abstract QueryValue CreateErrorValueInternal(QueryError evaluationError);
    }
}
