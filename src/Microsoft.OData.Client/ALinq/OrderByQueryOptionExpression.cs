//---------------------------------------------------------------------
// <copyright file="OrderByQueryOptionExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// An resource specific expression representing an OrderBy query option.
    /// </summary>
    internal class OrderByQueryOptionExpression : QueryOptionExpression
    {
        /// <summary> selectors for OrderBy query option</summary>
        private List<Selector> selectors;

        /// <summary>
        /// Creates an OrderByQueryOptionExpression expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        /// <param name="selectors">selectors for orderby expression</param>
        internal OrderByQueryOptionExpression(Type type, List<Selector> selectors)
            : base(type)
        {
            this.selectors = selectors;
        }

        /// <summary>
        /// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
        /// </summary>
        public override ExpressionType NodeType
        {
            get { return (ExpressionType)ResourceExpressionType.OrderByQueryOption; }
        }

        /// <summary>
        /// Selectors for OrderBy expression
        /// </summary>
        internal List<Selector> Selectors
        {
            get
            {
                return this.selectors;
            }
        }

        /// <summary>
        /// Structure for selectors.  Holds lambda expression + flag indicating desc.
        /// </summary>
        internal struct Selector
        {
            /// <summary>
            /// lambda expression for selector
            /// </summary>
            internal readonly Expression Expression;

            /// <summary>
            /// flag indicating if descending
            /// </summary>
            internal readonly bool Descending;

            /// <summary>
            /// Creates a Selector
            /// </summary>
            /// <param name="e">lambda expression for selector</param>
            /// <param name="descending">flag indicating if descending</param>
            internal Selector(Expression e, bool descending)
            {
                this.Expression = e;
                this.Descending = descending;
            }
        }
    }
}
