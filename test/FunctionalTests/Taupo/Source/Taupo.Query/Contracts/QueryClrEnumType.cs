//---------------------------------------------------------------------
// <copyright file="QueryClrEnumType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Represents a enum type in a QueryType hierarchy.
    /// </summary>
    public class QueryClrEnumType : QueryScalarType, IQueryClrType
    {
        /// <summary>
        /// Initializes a new instance of the QueryClrEnumType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        /// <param name="enumType">The wrapped enum type.</param>
        /// <param name="clrType">Wrapped CLR type.</param>
        public QueryClrEnumType(IQueryEvaluationStrategy evaluationStrategy, EnumType enumType, Type clrType)
            : base(evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(enumType, "enumType");

            this.EnumType = enumType;
            this.ClrType = clrType;
        }

        /// <summary>
        /// Gets or sets the CLR type information.
        /// </summary>
        /// <value></value>
        public Type ClrType { get; set; }

        /// <summary>
        /// Gets the enum type.
        /// </summary>
        public EnumType EnumType { get; private set; }

        /// <summary>
        /// Gets the string representation of the type.
        /// </summary>
        public override string StringRepresentation
        {
            get 
            {
                return string.Format(CultureInfo.InvariantCulture, "CLR Enum({0})", this.EnumType.Name);
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
    }
}