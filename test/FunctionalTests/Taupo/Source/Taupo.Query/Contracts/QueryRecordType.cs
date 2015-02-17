//---------------------------------------------------------------------
// <copyright file="QueryRecordType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a record type in a QueryType hierarchy.
    /// It has members, but member should be accessed by index only (names could have duplicates) 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class QueryRecordType : QueryStructuralType
    {
        /// <summary>
        /// Initializes a new instance of the QueryRecordType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryRecordType(IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
        }

        /// <summary>
        /// Gets the string representation of a given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "RecordType(" + string.Join(", ", this.Properties.Select(c => c.Name + " : " + c.PropertyType.StringRepresentation).ToArray()) + ")";
            }
        }

        /// <summary>
        /// Gets the null value for a given type.
        /// </summary>
        public new QueryRecordValue NullValue
        {
            get { return new QueryRecordValue(this, true, null, this.EvaluationStrategy); }
        }

        /// <summary>
        /// Creates the new instance of the record type.
        /// </summary>
        /// <returns>Instance of newly created <see cref="QueryRecordValue"/> with all member values uninitialized (null)</returns>
        public new QueryRecordValue CreateNewInstance()
        {
            return new QueryRecordValue(this, false, null, this.EvaluationStrategy);
        }

        /// <summary>
        /// Creates the error value of this type.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <returns>Created error value.</returns>
        public new QueryRecordValue CreateErrorValue(QueryError evaluationError)
        {
            return new QueryRecordValue(this, true, evaluationError, this.EvaluationStrategy);
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

            var rowType = queryType as QueryRecordType;
            if (rowType == null)
            {
                return false;
            }

            // if the number of member properties does not match, then it is not assignable
            if (this.Properties.Count != rowType.Properties.Count)
            {
                return false;
            }

            // if the member property types do not match, then it is not assignable
            for (int i = 0; i < this.Properties.Count; i++)
            {
                if (!this.Properties[i].PropertyType.IsAssignableFrom(rowType.Properties[i].PropertyType))
                {
                    return false;
                }
            }

            return true;
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
