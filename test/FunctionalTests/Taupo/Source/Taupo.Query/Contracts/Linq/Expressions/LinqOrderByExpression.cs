//---------------------------------------------------------------------
// <copyright file="LinqOrderByExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Expression node representing the OrderBy expression in a Linq query.
    /// </summary>
    public class LinqOrderByExpression : LinqQueryMethodExpression
    {
        internal LinqOrderByExpression(QueryExpression source, IEnumerable<LinqLambdaExpression> keySelectors, IEnumerable<bool> areDescending, QueryType type)
            : base(source, type)
        {
            this.KeySelectors = keySelectors.ToList().AsReadOnly();
            this.AreDescending = areDescending.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the collection of key selectors.
        /// </summary>
        public ReadOnlyCollection<LinqLambdaExpression> KeySelectors { get; private set; }

        /// <summary>
        /// Gets the collection of Ascending/Descending information.
        /// </summary>
        public ReadOnlyCollection<bool> AreDescending { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < this.KeySelectors.Count; i++)
            {
                string methodName = i == 0 ? "OrderBy" : "ThenBy";

                if (this.AreDescending[i])
                {
                    methodName += "Descending";
                }

                builder.Append(string.Format(CultureInfo.InvariantCulture, ".{0}({1})", methodName, this.KeySelectors[i].ToString()));
            }

            return this.Source.ToString() + builder.ToString();
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
