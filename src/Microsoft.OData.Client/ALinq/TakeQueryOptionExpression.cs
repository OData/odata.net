//---------------------------------------------------------------------
// <copyright file="TakeQueryOptionExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    /// <summary>
    /// An resource specific expression representing a take query option.
    /// </summary>
    [DebuggerDisplay("TakeQueryOptionExpression {TakeAmount}")]
    internal class TakeQueryOptionExpression : QueryOptionExpression
    {
        /// <summary>amount to skip</summary>
        private ConstantExpression takeAmount;

        /// <summary>
        /// Creates a TakeQueryOption expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        /// <param name="takeAmount">the query option value</param>
        internal TakeQueryOptionExpression(Type type, ConstantExpression takeAmount)
            : base(type)
        {
            this.takeAmount = takeAmount;
        }

        /// <summary>
        /// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
        /// </summary>
        public override ExpressionType NodeType
        {
            get { return (ExpressionType)ResourceExpressionType.TakeQueryOption; }
        }

        /// <summary>
        /// query option value
        /// </summary>
        internal ConstantExpression TakeAmount
        {
            get
            {
                return this.takeAmount;
            }
        }

        /// <summary>
        /// Composes the <paramref name="previous"/> expression with this one when it's specified multiple times.
        /// </summary>
        /// <param name="previous"><see cref="QueryOptionExpression"/> to compose.</param>
        /// <returns>
        /// The expression that results from composing the <paramref name="previous"/> expression with this one.
        /// </returns>
        internal override QueryOptionExpression ComposeMultipleSpecification(QueryOptionExpression previous)
        {
            Debug.Assert(previous != null, "other != null");
            Debug.Assert(previous.GetType() == this.GetType(), "other.GetType == this.GetType() -- otherwise it's not the same specification");
            Debug.Assert(this.takeAmount != null, "this.takeAmount != null");
            Debug.Assert(
                this.takeAmount.Type == typeof(int),
                "this.takeAmount.Type == typeof(int) -- otherwise it wouldn't have matched the Enumerable.Take(source, int count) signature");
            int thisValue = (int)this.takeAmount.Value;
            int previousValue = (int)((TakeQueryOptionExpression)previous).takeAmount.Value;
            return (thisValue < previousValue) ? this : previous;
        }
    }
}
