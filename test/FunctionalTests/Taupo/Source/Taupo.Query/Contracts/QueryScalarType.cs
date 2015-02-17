//---------------------------------------------------------------------
// <copyright file="QueryScalarType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a scalar type in a QueryType hierarchy.
    /// </summary>
    public abstract class QueryScalarType : QueryType
    {
        private object queryCollectionType;

        /// <summary>
        /// Initializes a new instance of the QueryScalarType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        protected QueryScalarType(IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this type supports Order Comparison
        /// </summary>
        /// <returns>Value <c>true</c> if Order Comparison operation can be performed; otherwise, <c>false</c>.</returns>
        public bool SupportsOrderComparison
        {
            get
            {
                return this.EvaluationStrategy.SupportsOrderComparison(this);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this type supports Equality Comparison
        /// </summary>
        /// <returns>Value <c>true</c> if Equality Comparison operation can be performed; otherwise, <c>false</c>.</returns>
        public bool SupportsEqualityComparison
        {
            get
            {
                return this.EvaluationStrategy.Supports(QueryBinaryOperation.EqualTo, this, this);
            }
        }

        /// <summary>
        /// Gets the null value for the type.
        /// </summary>
        /// <value>The null value.</value>
        public new QueryScalarValue NullValue
        {
            get { return (QueryScalarValue)this.GetNullValueInternal(); }
        }

        /// <summary>
        /// Creates the typed collection where element type is the current scalar type.
        /// </summary>
        /// <returns>Collection of the given type.</returns>
        public new QueryCollectionType<QueryScalarType> CreateCollectionType()
        {
            return (QueryCollectionType<QueryScalarType>)this.CreateCollectionTypeInternal();
        }

        /// <summary>
        /// Creates the scalar value for this type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Newly created value.</returns>
        public virtual QueryScalarValue CreateValue(object value)
        {
            return new QueryScalarValue(this, value, null, this.EvaluationStrategy);
        }

        /// <summary>
        /// Creates the null value with error information attached.
        /// </summary>
        /// <param name="error">The error information.</param>
        /// <returns>Newly created value.</returns>
        public new QueryScalarValue CreateErrorValue(QueryError error)
        {
            return (QueryScalarValue)this.CreateErrorValueInternal(error);
        }

        /// <summary>
        /// Determines whether data of this type can do certain operation
        /// </summary>
        /// <param name="operation">the operation</param>
        /// <returns>Value <c>true</c> if operation can be performed; otherwise, <c>false</c>.</returns>
        public bool Supports(QueryUnaryOperation operation)
        {
            return this.EvaluationStrategy.Supports(operation, this);
        }

        /// <summary>
        /// Determines whether data of this type can do certain operation with data of another type
        /// </summary>
        /// <param name="operation">the operation</param>
        /// <param name="otherScalarType">the other data type</param>
        /// <returns>Value <c>true</c> if operation can be performed; otherwise, <c>false</c>.</returns>
        public bool Supports(QueryBinaryOperation operation, QueryScalarType otherScalarType)
        {
            ExceptionUtilities.CheckArgumentNotNull(otherScalarType, "otherScalarType");

            return this.EvaluationStrategy.Supports(operation, this, otherScalarType);
        }

        /// <summary>
        /// Determines whether an operation is applicable for this type
        /// </summary>
        /// <param name="operation">the operation</param>
        /// <returns>Value <c>true</c> if operation can be performed; otherwise, <c>false</c>.</returns>
        public bool Supports(QueryBinaryOperation operation)
        {
            return this.EvaluationStrategy.Supports(operation, this, this);
        }

        /// <summary>
        /// Gets the non-strongly typed collection type for this type.
        /// </summary>
        /// <returns>
        /// Instance of <see cref="QueryCollectionType"/> which is a collection of this type.
        /// </returns>
        protected override QueryCollectionType CreateCollectionTypeInternal()
        {
            if (this.queryCollectionType == null)
            {
                this.queryCollectionType = QueryCollectionType.Create(this);
            }

            return (QueryCollectionType)this.queryCollectionType;
        }

        /// <summary>
        /// Gets the non-strongly typed null value.
        /// </summary>
        /// <returns>Null value.</returns>
        protected override QueryValue GetNullValueInternal()
        {
            return this.CreateValue(null);
        }

        /// <summary>
        /// Creates the non-strongly typed error value.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <returns>Created error value.</returns>
        protected override QueryValue CreateErrorValueInternal(QueryError evaluationError)
        {
            return new QueryScalarValue(this, null, evaluationError, this.EvaluationStrategy);
        }
    }
}
