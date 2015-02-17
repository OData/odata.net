//---------------------------------------------------------------------
// <copyright file="QueryStructuralValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Result of a query evaluation which is structural object (with named members).
    /// </summary>
    public class QueryStructuralValue : QueryValue
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private Dictionary<string, QueryValue> membersDictionary = new Dictionary<string, QueryValue>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool inToString = false;

        private bool isNull;

        /// <summary>
        /// Initializes a new instance of the QueryStructuralValue class.
        /// </summary>
        /// <param name="type">The type of the value.</param>
        /// <param name="isNull">If set to <c>true</c> the structural value is null.</param>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryStructuralValue(QueryStructuralType type, bool isNull, QueryError evaluationError, IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationError, evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            this.Type = type;
            this.isNull = isNull;
        }

        /// <summary>
        /// Gets the structural type of the value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Must be the same as the base class.")]
        public new QueryStructuralType Type { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        /// <value><c>true</c> if this instance is null; otherwise, <c>false</c>.</value>
        public override bool IsNull
        {
            get { return this.isNull; }
        }

        /// <summary>
        /// Gets the names of this instance's member properties
        /// </summary>
        public virtual IEnumerable<string> MemberNames
        {
            get
            {
                return this.membersDictionary.Keys.AsEnumerable();
            }
        }

        /// <summary>
        /// Gets a <see cref="QueryStructuralValue"/> value indicating whether two values are equal.
        /// </summary>
        /// <param name="otherValue">The second value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of comparison.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011", Justification = "For a cleaner API, we compare objects of the same static types.")]
        public QueryScalarValue EqualTo(QueryStructuralValue otherValue)
        {
            if ((this.IsNull && otherValue.IsNull) || object.ReferenceEquals(this, otherValue))
            {
                return new QueryScalarValue(EvaluationStrategy.BooleanType, true, this.EvaluationError, this.EvaluationStrategy);
            }
            else
            {
                return new QueryScalarValue(EvaluationStrategy.BooleanType, false, this.EvaluationError, this.EvaluationStrategy);
            }
        }

        /// <summary>
        /// Gets the scalar value of a member.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>Value of a member.</returns>
        public QueryScalarValue GetScalarValue(string memberName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(memberName, "memberName");
            this.AssertPropertyType<QueryScalarType>(memberName);

            return (QueryScalarValue)this.GetValue(memberName);
        }

        /// <summary>
        /// Gets the value of a reference member.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>Value of the member.</returns>
        public QueryStructuralValue GetStructuralValue(string memberName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(memberName, "memberName");
            this.AssertPropertyType<QueryStructuralType>(memberName);

            return (QueryStructuralValue)this.GetValue(memberName);
        }

        /// <summary>
        /// Gets the value of a collection member.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>Value of a member.</returns>
        public QueryCollectionValue GetCollectionValue(string memberName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(memberName, "memberName");
            this.AssertPropertyType<QueryCollectionType>(memberName);

            return (QueryCollectionValue)this.GetValue(memberName);
        }

        /// <summary>
        /// Gets the untyped value of a StructuralType member.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>Value of a member.</returns>
        public QueryValue GetValue(string memberName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(memberName, "memberName");

            var property = this.EnsurePropertyExists(memberName);
            var propertyType = property.PropertyType;

            QueryValue value;
            if (this.IsNull)
            {
                value = propertyType.NullValue;
            }
            else if (!this.membersDictionary.TryGetValue(memberName, out value))
            {
                value = propertyType.DefaultValue;
            }

            if (this.EvaluationError != null)
            {
                value.EvaluationError = this.EvaluationError;
            }

            return value;
        }

        /// <summary>
        /// Gets a <see cref="QueryStructuralValue"/> value indicating whether two values are not equal.
        /// </summary>
        /// <param name="otherValue">The second value.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> which represents the result of comparison.
        /// </returns>
        public QueryScalarValue NotEqualTo(QueryStructuralValue otherValue)
        {
            bool areEqual = (bool)this.EqualTo(otherValue).Value;
            return new QueryScalarValue(EvaluationStrategy.BooleanType, !areEqual, this.EvaluationError, this.EvaluationStrategy);
        }

        /// <summary>
        /// Sets the value of a member.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="value">The value.</param>
        public void SetValue(string memberName, QueryValue value)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(memberName, "memberName");
            ExceptionUtilities.CheckArgumentNotNull(value, "value");

            var property = this.Type.Properties.SingleOrDefault(m => m.Name == memberName);
            if (property == null)
            {
                throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Could not find property '{0}'.", memberName));
            }

            if (!property.PropertyType.IsAssignableFrom(value.Type))
            {
                throw new TaupoInvalidOperationException("Value '" + value + "' does not match the property type: '" + property.PropertyType.StringRepresentation + "'.");
            }

            this.membersDictionary[memberName] = value;
        }

        /// <summary>
        /// Sets the value of a primitive member.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="value">The value.</param>
        public void SetPrimitiveValue(string memberName, object value)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(memberName, "memberName");
            ExceptionUtilities.Assert(!(value is QueryValue), "value must not be QueryValue");

            var type = AssertPropertyType<QueryScalarType>(memberName);
            this.membersDictionary[memberName] = type.CreateValue(value);
        }

        /// <summary>
        /// Removes the member property with the given name
        /// </summary>
        /// <param name="memberName">The name of the member to remove</param>
        public void RemoveMember(string memberName)
        {
            this.membersDictionary.Remove(memberName);
        }

        /// <summary>
        /// Checks if a <see cref="QueryValue"/> is of a particular <see cref="QueryType"/>. This operation will return a true if the value is of the specified type.
        /// </summary>
        /// <param name="type">The type for the IsOf operation.</param>
        /// <param name="performExactMatch">Determines if an exact match needs to be performed.</param>
        /// <returns>A <see cref="QueryValue"/> containing true or false depending on whether the value is of the specified type or not.</returns>
        public override QueryValue IsOf(QueryType type, bool performExactMatch)
        {
            if (this.Type == type)
            {
                return this.Type.EvaluationStrategy.BooleanType.CreateValue(true);
            }
            else if (!performExactMatch && ((QueryStructuralType)type).DerivedTypes.Contains(this.Type))
            {
                return this.Type.EvaluationStrategy.BooleanType.CreateValue(true);
            }

            return this.Type.EvaluationStrategy.BooleanType.CreateValue(false);
        }

        /// <summary>
        /// Converts the <see cref="QueryValue"/> to a particular <see cref="QueryType"/>.
        /// </summary>
        /// <param name="type">The type for the As operation.</param>
        /// <returns>The <see cref="QueryValue"/> converted to the specified type if successful. Returns null if this operation fails.</returns>
        public override QueryValue TreatAs(QueryType type)
        {
            QueryStructuralType structuralType = type as QueryStructuralType;
            if (structuralType == null)
            {
                return this.Type.CreateErrorValue(new QueryError("Invalid 'As' Operation : Cannot perform an 'As' of a structural type to a non-structural type"));
            }

            return this.TreatAs(structuralType);
        }

        /// <summary>
        /// Casts a <see cref="QueryValue"/> to a <see cref="QueryType"/>. The cast will return the value type cast to the new type.
        /// </summary>
        /// <param name="type">The type for the cast operation.</param>
        /// <returns><see cref="QueryValue"/> which is cast to the appropriate type</returns>
        public override QueryValue Cast(QueryType type)
        {
            QueryStructuralType structuralType = type as QueryStructuralType;
            if (structuralType == null)
            {
                return this.Type.CreateErrorValue(new QueryError("Invalid Cast : Cannot perform a cast of a structural type to a non-structural type"));
            }

            QueryValue result = this.TreatAs(structuralType);

            if (result.IsNull)
            {
                return this.Type.CreateErrorValue(new QueryError("Invalid Cast : Cannot perform Cast to the specified type"));
            }

            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.inToString)
            {
                return "Non-Null value, Type=" + this.Type.StringRepresentation + ", Properties={ ... }";
            }

            if (this.EvaluationError != null)
            {
                return "Error=" + this.EvaluationError + ", Type=" + this.Type.StringRepresentation;
            }
            else if (this.IsNull)
            {
                return "Null value, Type=" + this.Type.StringRepresentation;
            }
            else
            {
                this.inToString = true;

                try
                {
                    return "Non-Null value, Type=" + this.Type.StringRepresentation + ", Properties={\r\n    " + string.Join("\r\n    ", this.membersDictionary.Select(c => "'" + c.Key + "' = " + c.Value.ToString().Replace("\r\n", "\r\n    ")).ToArray()) + "\r\n}";
                }
                finally
                {
                    this.inToString = false;
                }
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
        /// Asserts the type of the property.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property type.</typeparam>
        /// <param name="propertyName">The name of property.</param>
        /// <returns>The property type.</returns>
        public TPropertyType AssertPropertyType<TPropertyType>(string propertyName)
           where TPropertyType : QueryType
        {
            var property = this.EnsurePropertyExists(propertyName);

            var propertyType = property.PropertyType as TPropertyType;
            if (propertyType == null)
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Property '{0}' is not of the given type '{1}' (was '{2}').", propertyName, typeof(TPropertyType).Name, property.PropertyType));
            }

            return propertyType;
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <returns>Type of the value.</returns>
        protected override QueryType GetTypeInternal()
        {
            return this.Type;
        }

        private QueryValue TreatAs(QueryStructuralType structuralType)
        {
            if (structuralType.IsAssignableFrom(this.Type))
            {
                return this;
            }

            return structuralType.NullValue;
        }

        private QueryProperty EnsurePropertyExists(string propertyName)
        {
            var property = this.Type.Properties.SingleOrDefault(m => m.Name == propertyName);
            if (property == null)
            {
                throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Could not find property '{0}'.", propertyName));
            }

            return property;
        }
    }
}
