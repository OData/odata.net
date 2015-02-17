//---------------------------------------------------------------------
// <copyright file="QueryRecordValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Result of a query evaluation which is a data record (with indexed members).
    /// </summary>
    public class QueryRecordValue : QueryValue
    {
        private bool isNull;
        private QueryValue[] memberValues;

        /// <summary>
        /// Initializes a new instance of the QueryRecordValue class.
        /// </summary>
        /// <param name="type">The type of the value.</param>
        /// <param name="isNull">If set to <c>true</c> the record value is null.</param>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryRecordValue(QueryRecordType type, bool isNull, QueryError evaluationError, IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationError, evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            this.Type = type;
            this.isNull = isNull;

            this.SetupInitialMemberValues();
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Must be the same as the base class.")]
        public new QueryRecordType Type { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        /// <value><c>true</c> if this instance is null; otherwise, <c>false</c>.</value>
        public override bool IsNull
        {
            get { return this.isNull; }
        }

        /// <summary>
        /// Gets the member value with a specified name
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        /// <returns>The member value. If no member is found, or there are multiple members with a given name, exception is thrown.</returns>
        public QueryValue GetMemberValueByName(string memberName)
        {
            var index = this.FindPropertyIndexByName(memberName);

            return this.GetMemberValue(index);
        }

        /// <summary>
        /// Gets the member value at the specified index
        /// </summary>
        /// <param name="index">The index of the member.</param>
        /// <returns>The member value.</returns>
        public QueryValue GetMemberValue(int index)
        {
            ExceptionUtilities.Assert(index >= 0 && index < this.Type.Properties.Count, "Index is out of range for GetValue.");

            var memberType = this.Type.Properties[index].PropertyType;

            if (this.EvaluationError != null)
            {
                return memberType.CreateErrorValue(this.EvaluationError);
            }

            // Note: same behavior as for QueryStructuralValue
            // We rely on this in QueryCollectionValue.Select(Func<QueryValue, QueryValue> selector) where 
            // we compute the result type by applying predicate to a NULL value => hence projecting on a NULL value cannot throw.
            if (this.IsNull)
            {
                return memberType.NullValue;
            }

            return this.memberValues[index];
        }

        /// <summary>
        /// Sets the member value at the specified index
        /// </summary>
        /// <param name="index">The specified index</param>
        /// <param name="value">The value to set to</param>
        public void SetMemberValue(int index, QueryValue value)
        {
            ExceptionUtilities.Assert(index >= 0 && index < this.Type.Properties.Count, "Index is out of range for GetValue.");

            this.memberValues[index] = value;
        }

        /// <summary>
        /// Casts a <see cref="QueryValue"/> to a <see cref="QueryType"/>. The cast will return the value type cast to the new type.
        /// </summary>
        /// <param name="type">The type for the cast operation.</param>
        /// <returns><see cref="QueryValue"/> which is cast to the appropriate type</returns>
        public override QueryValue Cast(QueryType type)
        {
            return type.CreateErrorValue(new QueryError("Cannot perform Cast on a record value"));
        }

        /// <summary>
        /// Checks if a <see cref="QueryValue"/> is of a particular <see cref="QueryType"/>. This operation will return a true if the value is of the specified type.
        /// </summary>
        /// <param name="type">The type for the IsOf operation.</param>
        /// <param name="performExactMatch">Determines if an exact match needs to be performed.</param>
        /// <returns>A <see cref="QueryValue"/> containing true or false depending on whether the value is of the specified type or not.</returns>
        public override QueryValue IsOf(QueryType type, bool performExactMatch)
        {
            return type.CreateErrorValue(new QueryError("Cannot perform IsOf on a record value"));
        }

        /// <summary>
        /// Converts the <see cref="QueryValue"/> to a particular <see cref="QueryType"/>.
        /// </summary>
        /// <param name="type">The type for the As operation.</param>
        /// <returns>The <see cref="QueryValue"/> converted to the specified type if successful. Returns null if this operation fails.</returns>
        public override QueryValue TreatAs(QueryType type)
        {
            return type.CreateErrorValue(new QueryError("Cannot perform TreatAs on a record value"));
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
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.EvaluationError != null)
            {
                return "Record Value Error=" + this.EvaluationError + ", Type=" + this.Type.StringRepresentation;
            }
            else if (this.IsNull)
            {
                return "Null Record, Type=" + this.Type.StringRepresentation;
            }
            else
            {
                return "Non-null Record Value";
            }
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <returns>Type of the value.</returns>
        protected override QueryType GetTypeInternal()
        {
            return this.Type;
        }

        private void SetupInitialMemberValues()
        {
            int memberCount = this.Type.Properties.Count;
            this.memberValues = new QueryValue[memberCount];
            for (int index = 0; index < memberCount; index++)
            {
                this.memberValues[index] = this.Type.Properties[index].PropertyType.NullValue;
            }
        }

        private int FindPropertyIndexByName(string memberName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(memberName, "memberName");

            var membersWithName = this.Type.Properties.Where(m => m.Name == memberName);

            ExceptionUtilities.Assert(membersWithName.Count() != 0, "Could not find member with a given name: '" + memberName + "'.");
            ExceptionUtilities.Assert(membersWithName.Count() == 1, "Ambigous match found. There are more than one properties with the given name: '" + memberName + "'.");

            var member = membersWithName.Single();
            var index = this.Type.Properties.IndexOf(member);

            return index;
        }
    }
}
