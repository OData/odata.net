//---------------------------------------------------------------------
// <copyright file="LinqFreeVariableExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Expression node representing the free variable expression in a Linq query.
    /// </summary>
    public class LinqFreeVariableExpression : QueryExpression
    {
        internal LinqFreeVariableExpression(string name, QueryType type, IEnumerable<QueryExpression> values)
            : base(type)
        {
            this.Name = name;
            this.Values = values.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets possible values of for the variable.
        /// </summary>
        public ReadOnlyCollection<QueryExpression> Values { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", this.Name);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(ICommonExpressionVisitor<TResult> visitor)
        {
            return ((ILinqExpressionVisitor<TResult>)visitor).Visit(this);
        }
    }
}
