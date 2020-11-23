//---------------------------------------------------------------------
// <copyright file="ApplyQueryOptionExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
	using System;
	using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
	using Microsoft.OData.UriParser.Aggregation;

	/// <summary>
	/// A resource specific expression representing an apply query option.
	/// </summary>
	internal class ApplyQueryOptionExpression : QueryOptionExpression
	{
		/// <summary>
		/// The filter expressions that make the filter predicate
		/// </summary>
		private readonly List<Expression> filterExpressions;

		/// <summary>
		/// Creates an <see cref="ApplyQueryOptionExpression"/> expression.
		/// </summary>
		/// <param name="type">the return type of the expression.</param>
		internal ApplyQueryOptionExpression(Type type)
			: base(type)
		{
			this.Aggregations = new List<Aggregation>();
			this.filterExpressions = new List<Expression>();
		}

		/// <summary>
		/// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
		public override ExpressionType NodeType
		{
			get { return (ExpressionType)ResourceExpressionType.ApplyQueryOption; }
		}

		/// <summary>
		/// Gets the aggregations in the $apply expression
		/// </summary>
		internal List<Aggregation> Aggregations { get; private set; }

		/// <summary>
		/// Adds the conjuncts to the filter expressions
		/// </summary>
		internal void AddPredicateConjuncts(IEnumerable<Expression> predicates)
		{
			this.filterExpressions.AddRange(predicates);
		}

		internal ReadOnlyCollection<Expression> PredicateConjuncts
		{
			get
			{
				return new ReadOnlyCollection<Expression>(this.filterExpressions);
			}
		}

		/// <summary>
		/// Gets filter transformation predicate.
		/// </summary>
		/// <returns>A predicate with all conjuncts AND'd</returns>
		internal Expression GetPredicate()
		{
			Expression combinedPredicate = null;
			bool isFirst = true;

			foreach (Expression expr in this.filterExpressions)
			{
				if (isFirst)
				{
					combinedPredicate = expr;
					isFirst = false;
				}
				else
				{
					combinedPredicate = Expression.And(combinedPredicate, expr);
				}
			}

			return combinedPredicate;
		}

		/// <summary>
		/// Structure for an aggregation. Holds lambda expression plus enum indicating aggregation method
		/// </summary>
		internal struct Aggregation
		{
			/// <summary>
			/// Lambda expression for aggregation selector.
			/// </summary>
			internal readonly Expression Expression;

			/// <summary>
			/// Enum indicating aggregation method.
			/// </summary>
			internal readonly AggregationMethod AggregationMethod;

			/// <summary>
			/// Aggregation alias.
			/// </summary>
			internal readonly string AggregationAlias;

			/// <summary>
			/// Creates an aggregation.
			/// </summary>
			/// <param name="exp">Lambda expression for aggregation selector.</param>
			/// <param name="aggregationMethod">Enum indicating aggregation method.</param>
			internal Aggregation(Expression exp, AggregationMethod aggregationMethod)
			{
				this.Expression = exp;
				this.AggregationMethod = aggregationMethod;
				this.AggregationAlias = string.Empty;
			}

			/// <summary>
			/// Creates an aggregation.
			/// </summary>
			/// <param name="exp">Lambda expression for aggregation selector.</param>
			/// <param name="aggregationMethod">Enum indicating aggregation method.</param>
			/// <param name="aggregationAlias">Aggregation alias.</param>
			internal Aggregation(Expression exp, AggregationMethod aggregationMethod, string aggregationAlias)
			{
				this.Expression = exp;
				this.AggregationMethod = aggregationMethod;
				this.AggregationAlias = aggregationAlias;
			}
		}
	}
}
