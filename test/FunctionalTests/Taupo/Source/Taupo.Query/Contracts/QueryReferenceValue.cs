//---------------------------------------------------------------------
// <copyright file="QueryReferenceValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Result of a query evaluation which is a reference of entity
    /// </summary>
    public class QueryReferenceValue : QueryValue
    {
        internal QueryReferenceValue(QueryReferenceType type, QueryError evaluationError, IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationError, evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            this.Type = type;
        }

        /// <summary>
        /// Gets the reference type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Must be the same as the base class.")]
        public new QueryReferenceType Type { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        public override bool IsNull
        {
            get { return this.KeyValue == null; }
        }

        /// <summary>
        /// Gets the entity value (for dereference)
        /// </summary>
        /// <remarks>For dangling reference, this should be null value</remarks>
        public QueryStructuralValue EntityValue { get; private set; }

        /// <summary>
        /// Gets the entity set full name
        /// </summary>
        public string EntitySetFullName { get; private set; }

        /// <summary>
        /// Gets the key value
        /// </summary>
        public QueryRecordValue KeyValue { get; private set; }

        /// <summary>
        /// Casts a <see cref="QueryValue"/> to a <see cref="QueryType"/>. The cast will return the value type cast to the new type.
        /// </summary>
        /// <param name="type">The type for the cast operation.</param>
        /// <returns><see cref="QueryValue"/> which is cast to the appropriate type</returns>
        public override QueryValue Cast(QueryType type)
        {
            return type.CreateErrorValue(new QueryError("Cannot perform Cast on a reference value"));
        }

        /// <summary>
        /// Checks if a <see cref="QueryValue"/> is of a particular <see cref="QueryType"/>. This operation will return a true if the value is of the specified type.
        /// </summary>
        /// <param name="type">The type for the IsOf operation.</param>
        /// <param name="performExactMatch">Determines if an exact match needs to be performed.</param>
        /// <returns>A <see cref="QueryValue"/> containing true or false depending on whether the value is of the specified type or not.</returns>
        public override QueryValue IsOf(QueryType type, bool performExactMatch)
        {
            return type.CreateErrorValue(new QueryError("Cannot perform IsOf on a reference value"));
        }

        /// <summary>
        /// Converts the <see cref="QueryValue"/> to a particular <see cref="QueryType"/>.
        /// </summary>
        /// <param name="type">The type for the As operation.</param>
        /// <returns>The <see cref="QueryValue"/> converted to the specified type if successful. Returns null if this operation fails.</returns>
        public override QueryValue TreatAs(QueryType type)
        {
            return type.CreateErrorValue(new QueryError("Cannot perform TreatAs on a reference value"));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.EvaluationError != null)
            {
                return "Reference Value Error=" + this.EvaluationError + ", Type=" + this.Type.StringRepresentation;
            }
            else if (this.IsNull)
            {
                return "Null Reference, Type=" + this.Type.StringRepresentation;
            }
            else
            {
                return "Reference Value=" + this.EntitySetFullName + ", keyValue[" + this.KeyValue + "], Type=" + this.Type.StringRepresentation;
            }
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query value.</param>
        /// <returns>The result of visiting this query value.</returns>
        public override TResult Accept<TResult>(IQueryValueVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Gets a <see cref="QueryReferenceValue"/> value indicating whether two values are equal.
        /// </summary>
        /// <param name="otherValue">The second value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of comparison.
        /// </returns>
        public QueryScalarValue EqualTo(QueryReferenceValue otherValue)
        {
            if ((this.IsNull && otherValue.IsNull) || object.ReferenceEquals(this.EntityValue, otherValue.EntityValue))
            {
                return new QueryScalarValue(EvaluationStrategy.BooleanType, true, this.EvaluationError, this.EvaluationStrategy);
            }
            else
            {
                return new QueryScalarValue(EvaluationStrategy.BooleanType, false, this.EvaluationError, this.EvaluationStrategy);
            }
        }

        /// <summary>
        /// Gets a <see cref="QueryReferenceValue"/> value indicating whether two values are not equal.
        /// </summary>
        /// <param name="otherValue">The second value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of comparison.
        /// </returns>
        public QueryScalarValue NotEqualTo(QueryReferenceValue otherValue)
        {
            bool areEqual = (bool)this.EqualTo(otherValue).Value;
            return new QueryScalarValue(EvaluationStrategy.BooleanType, !areEqual, this.EvaluationError, this.EvaluationStrategy);
        }

        internal void SetReferenceValue(QueryStructuralValue entityValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityValue, "entityValue");
            this.EntityValue = entityValue;

            // compute key value
            QueryEntityType entityType = this.Type.QueryEntityType;
            var keyType = new QueryRecordType(this.EvaluationStrategy);
            keyType.AddProperties(entityType.Properties.Where(m => m.IsPrimaryKey));

            this.KeyValue = keyType.CreateNewInstance();

            for (int i = 0; i < keyType.Properties.Count; i++)
            {
                this.KeyValue.SetMemberValue(i, entityValue.GetValue(keyType.Properties[i].Name));
            }

            var set = entityType.EntitySet;
            this.EntitySetFullName = set.Container.Name + "." + set.Name;
        }

        // this is only heppening when reading from product or creating dangling reference
        internal void SetReferenceValue(string entitySetFullName, QueryRecordValue keyValue)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(entitySetFullName, "entitySetFullName");
            ExceptionUtilities.CheckArgumentNotNull(keyValue, "keyValue");

            this.EntitySetFullName = entitySetFullName;
            this.KeyValue = keyValue;
            this.EntityValue = this.Type.QueryEntityType.NullValue;
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <returns>Type of the value.</returns>
        protected override QueryType GetTypeInternal()
        {
            return this.Type;
        }
    }
}
