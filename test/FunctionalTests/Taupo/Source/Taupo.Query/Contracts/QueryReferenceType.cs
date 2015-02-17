//---------------------------------------------------------------------
// <copyright file="QueryReferenceType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Query reference type which references an entity type
    /// </summary>
    public class QueryReferenceType : QueryType
    {
        private object collectionType;

        /// <summary>
        /// Initializes a new instance of the QueryReferenceType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy</param>
        /// <param name="queryEntityType">The referenced query entity type</param>
        internal QueryReferenceType(IQueryEvaluationStrategy evaluationStrategy, QueryEntityType queryEntityType)
            : base(evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryEntityType, "queryEntityType");
            this.QueryEntityType = queryEntityType;
        }

        /// <summary>
        /// Gets the query entity type of this reference type
        /// </summary>
        public QueryEntityType QueryEntityType { get; private set; }

        /// <summary>
        /// Gets the null value for a given type.
        /// </summary>
        public new QueryReferenceValue NullValue
        {
            get { return new QueryReferenceValue(this, null, this.EvaluationStrategy); }
        }

        /// <summary>
        /// Gets the string representation of a given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "ReferenceType(" + this.QueryEntityType.EntityType.FullName + ")";
            }
        }

        /// <summary>
        /// Creates the typed collection where element type is the current primitive type.
        /// </summary>
        /// <returns>Collection of the given type.</returns>
        public new QueryCollectionType<QueryReferenceType> CreateCollectionType()
        {
            if (this.collectionType == null)
            {
                this.collectionType = QueryCollectionType.Create(this);
            }

            return (QueryCollectionType<QueryReferenceType>)this.collectionType;
        }

        /// <summary>
        /// Creates the reference value for the given eneity value
        /// </summary>
        /// <param name="entityValue">the entity value</param>
        /// <returns>the reference value</returns>
        public QueryReferenceValue CreateReferenceValue(QueryStructuralValue entityValue)
        {
            ExceptionUtilities.Assert(entityValue.Type is QueryEntityType, "Create reference value needs a value of query entity type, not {0}.", entityValue.Type);

            var referenceValue = new QueryReferenceValue(this, null, this.EvaluationStrategy);
            referenceValue.SetReferenceValue(entityValue);
            return referenceValue;
        }

        /// <summary>
        /// Creates the reference value for the given entity set name and key value
        /// </summary>
        /// <param name="setFullName">the entity set full name</param>
        /// <param name="keyValue">the key value</param>
        /// <returns>the refernce value</returns>
        /// <remarks>
        /// This will create a "dangling" reference, which Dereference will evaluate to query null value. It's only intended to be used:
        /// 1. when it's really a "dangling reference
        /// 2. dumping product output for comparision (in which case, Dereference evaluation is not valid anyway)
        /// </remarks>
        public QueryReferenceValue CreateReferenceValue(string setFullName, QueryRecordValue keyValue)
        {
            ExceptionUtilities.Assert(keyValue.Type is QueryRecordType, "Create reference value using key needs a value of query record type, not {0}.", keyValue.Type);

            var referenceValue = new QueryReferenceValue(this, null, this.EvaluationStrategy);
            referenceValue.SetReferenceValue(setFullName, keyValue);
            return referenceValue;
        }

        /// <summary>
        /// Creates the error value of this type.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <returns>Created error value.</returns>
        public new QueryReferenceValue CreateErrorValue(QueryError evaluationError)
        {
            return new QueryReferenceValue(this, evaluationError, this.EvaluationStrategy);
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

            var other = queryType as QueryReferenceType;
            if (other == null)
            {
                return false;
            }

            return this.QueryEntityType.IsAssignableFrom(other.QueryEntityType);
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
        /// Gets the non-strongly typed collection type for this type.
        /// </summary>
        /// <returns>Instance of <see cref="QueryCollectionType"/> which is a collection of this type.</returns>
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
            return this.CreateErrorValue(evaluationError);
        }
    }
}
