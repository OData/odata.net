//---------------------------------------------------------------------
// <copyright file="QueryClrPrimitiveType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a primitive type in a QueryType hierarchy.
    /// </summary>
    public class QueryClrPrimitiveType : QueryScalarType, IQueryClrType
    {
        /// <summary>
        /// Initializes a new instance of the QueryClrPrimitiveType class.
        /// </summary>
        /// <param name="clrType">Wrapped CLR type.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryClrPrimitiveType(Type clrType, IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");

            this.ClrType = clrType;
        }

        /// <summary>
        /// Gets the CLR type information.
        /// </summary>
        /// <value></value>
        public Type ClrType { get; private set; }

        /// <summary>
        /// Gets the string representation of a given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "CLR Primitive({0})", this.ClrType.Name);
            }
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

            // if it is not a clr primitive type, then it is not assignable
            var primitiveClrType = queryType as QueryClrPrimitiveType;
            if (primitiveClrType == null)
            {
                return false;
            }
            else
            {
                // otherwise, fall back to CLR semantics for assignment
                return this.ClrType.IsAssignableFrom(primitiveClrType.ClrType);
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
        /// Gets the strongly typed default value.
        /// </summary>
        /// <returns>Default value.</returns>
        protected override QueryValue GetDefaultValueInternal()
        {
            if (this.GetClrTypeNulable())
            {
                return CreateValue(null);
            }
            else if (this.ClrType == typeof(DateTime))
            {
                return CreateValue(default(DateTime));
            }
            else if (this.ClrType == typeof(float))
            {
                return CreateValue(default(float));
            }
            else if (this.ClrType == typeof(decimal))
            {
                return CreateValue(default(decimal));
            }
            else if (this.ClrType == typeof(double))
            {
                return CreateValue(default(double));
            }
            else if (this.ClrType == typeof(TimeSpan))
            {
                return CreateValue(default(TimeSpan));
            }
            else if (this.ClrType == typeof(bool))
            {
                return CreateValue(default(bool));
            }
            else if (this.ClrType == typeof(int))
            {
                return CreateValue(default(int));
            }
            else if (this.ClrType == typeof(long))
            {
                return CreateValue(default(long));
            }
            else if (this.ClrType == typeof(short))
            {
                return CreateValue(default(short));
            }
            else if (this.ClrType == typeof(byte))
            {
                return CreateValue(default(byte));
            }
            else if (this.ClrType == typeof(DateTimeOffset))
            {
                return CreateValue(default(DateTimeOffset));
            }
            else if (this.ClrType == typeof(Guid))
            {
                return CreateValue(default(Guid));
            }
            else
            {
                return CreateValue(null);
            }
        }

        /// <summary>
        /// Returns a value indicating whether this Clr type of this query type is nullable
        /// </summary>
        /// <returns>A value indicating whether this Clr type of this query type is nullable</returns>
        private bool GetClrTypeNulable()
        {
            return this.ClrType.IsGenericType() && this.ClrType.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}