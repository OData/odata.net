//---------------------------------------------------------------------
// <copyright file="QueryComplexType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Structural type which wraps a <see cref="ComplexType"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class QueryComplexType : QueryStructuralType, IQueryClrType
    {
        /// <summary>
        /// Initializes a new instance of the QueryComplexType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        /// <param name="complexType">Complex type to wrap.</param>
        /// <param name="clrType">The CLR corresponding to the complex type.</param>
        public QueryComplexType(IQueryEvaluationStrategy evaluationStrategy, ComplexType complexType, Type clrType)
            : base(evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(complexType, "complexType");

            this.ComplexType = complexType;
            this.ClrType = clrType;
        }

        /// <summary>
        /// Gets or sets the CLR type information.
        /// </summary>
        public Type ClrType { get; set; }

        /// <summary>
        /// Gets the complex type.
        /// </summary>
        public ComplexType ComplexType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is a value type.
        /// </summary>
        /// <value>
        /// Value <c>true</c> if this instance is value type; otherwise, <c>false</c>.
        /// </value>
        public override bool IsValueType
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the string representation of a given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "ComplexType(" + this.ComplexType.FullName + ")";
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

            // if it is not a complex type, then it is not assignable
            var complexType = queryType as QueryComplexType;
            if (complexType == null)
            {
                return false;
            }
            else
            {
                // otherwise, it is assignable if it has the same definition
                return object.ReferenceEquals(this.ComplexType, complexType.ComplexType);
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
