//---------------------------------------------------------------------
// <copyright file="FilterQueryOptionExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    /// <summary>
    /// An resource specific expression representing a filter query option.
    /// </summary>
    internal class FilterQueryOptionExpression : QueryOptionExpression
    {
        /// <summary>
        /// The individual expressions that makes the filter predicate
        /// </summary>
        private readonly List<Expression> individualExpressions;

         /// <summary>
        /// Creates a FilterQueryOptionExpression expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        internal FilterQueryOptionExpression(Type type)
            : base(type)
        {
            this.individualExpressions = new List<Expression>();
        }

        /// <summary>
        /// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
        /// </summary>
        public override ExpressionType NodeType
        {
            get { return (ExpressionType)ResourceExpressionType.FilterQueryOption; }
        }

        /// <summary>
        /// Gets the list of individual conjucts which are separated by AND for the predicate
        /// i.e. if the filter statement is id1=1 and id2="var1" and id3=datetimeoffset'31'
        /// then this list will have 3 entries, id1=1, id2="var1" and id3=datetimeoffset'xxxxxxxxx'
        /// </summary>
        internal ReadOnlyCollection<Expression> PredicateConjuncts
        {
            get
            {
                return new ReadOnlyCollection<Expression>(this.individualExpressions);
            }
        }

        /// <summary>
        /// Adds the conjuncts to individualExpressions
        /// </summary>
        /// <param name="predicates">The predicates.</param>
        public void AddPredicateConjuncts(IEnumerable<Expression> predicates)
        {
            this.individualExpressions.AddRange(predicates);
        }

        /// <summary>
        /// Gets the query option value.
        /// </summary>
        /// <returns>A predicate with all Conjuncts AND'ed</returns>
        public Expression GetPredicate()
        {
            Expression combinedPredicate = null;
            bool isFirst = true;

            foreach (Expression individual in this.individualExpressions)
            {
                if (isFirst)
                {
                    combinedPredicate = individual;
                    isFirst = false;
                }
                else
                {
                    combinedPredicate = Expression.And(combinedPredicate, individual);
                }
            }

            return combinedPredicate;
        }
    }
}
