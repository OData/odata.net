//---------------------------------------------------------------------
// <copyright file="QueryOptionExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    /// <summary>
    /// An resource specific expression representing a query option.
    /// </summary>
    internal abstract class QueryOptionExpression : Expression
    {
        /// <summary>The CLR type this node will evaluate into.</summary>
        private Type type;

        /// <summary>
        /// Creates a QueryOptionExpression expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        internal QueryOptionExpression(Type type)
        {
            Debug.Assert(type != null, "type != null");
            this.type = type;
        }

        /// <summary>
        /// The <see cref="Type"/> of the value represented by this <see cref="Expression"/>.
        /// </summary>
        public override Type Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Composes the <paramref name="previous"/> expression with this one when it's specified multiple times.
        /// </summary>
        /// <param name="previous"><see cref="QueryOptionExpression"/> to compose.</param>
        /// <returns>
        /// The expression that results from composing the <paramref name="previous"/> expression with this one.
        /// </returns>
        internal virtual QueryOptionExpression ComposeMultipleSpecification(QueryOptionExpression previous)
        {
            Debug.Assert(previous != null, "other != null");
            Debug.Assert(previous.GetType() == this.GetType(), "other.GetType == this.GetType() -- otherwise it's not the same specification");
            return this;
        }
    }
}
