//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaExpandExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;
   
    /// <summary>
    /// Expression node representing the Expand expression in a Linq query.
    /// </summary>
    public class LinqToAstoriaExpandExpression : LinqQueryMethodExpression
    {
        internal LinqToAstoriaExpandExpression(QueryExpression source, string expandString, QueryType type, bool isImplicit)
            : base(source, type)
        {
            this.ExpandString = expandString;
            this.IsImplicit = isImplicit;
        }

        /// <summary>
        /// Gets the Expand string
        /// </summary>
        public string ExpandString { get; private set; }

        /// <summary>
        /// Gets a value indicating whether it is Implicit
        /// </summary>
        public bool IsImplicit { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.Expand({1})", this.Source, this.ExpandString);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(ICommonExpressionVisitor<TResult> visitor)
        {
            return ((ILinqToAstoriaExpressionVisitor<TResult>)visitor).Visit(this);
        }
    }
}
