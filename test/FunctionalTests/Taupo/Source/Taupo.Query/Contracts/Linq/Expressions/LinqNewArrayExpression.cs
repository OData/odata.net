//---------------------------------------------------------------------
// <copyright file="LinqNewArrayExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Expression node representing the LinqNewArrayExpression expression in a Linq query.
    /// </summary>
    public class LinqNewArrayExpression : QueryExpression
    {
        internal LinqNewArrayExpression(QueryType type, IEnumerable<QueryExpression> expressions)
            : base(type)
        {
            ExceptionUtilities.CheckArgumentNotNull(expressions, "expressions");
            this.Expressions = new ReadOnlyCollection<QueryExpression>(expressions.ToList());
        }

        /// <summary>
        /// Gets a enumerable of Expressions
        /// </summary>
        public ReadOnlyCollection<QueryExpression> Expressions { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            string innerResults = string.Join(",", this.Expressions.Select(s => s.ToString()));
            var collectionType = this.ExpressionType as QueryCollectionType;
            ExceptionUtilities.Assert(collectionType != null, "Type should be a collection.");

            return string.Format(CultureInfo.InvariantCulture, "NewArray<{0}>({1})", collectionType.ElementType, innerResults);
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
