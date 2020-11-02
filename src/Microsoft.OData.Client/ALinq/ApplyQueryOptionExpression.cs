//---------------------------------------------------------------------
// <copyright file="ApplyQueryOptionExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Microsoft.OData.UriParser.Aggregation;

	/// <summary>
	/// A resource specific expression representing an apply query option.
	/// </summary>
	internal class ApplyQueryOptionExpression : QueryOptionExpression
	{

		/// <summary>
		/// Creates an <see cref="ApplyQueryOptionExpression"/> expression.
		/// </summary>
		/// <param name="type">the return type of the expression.</param>
		internal ApplyQueryOptionExpression(Type type)
			: base(type)
		{
			this.Aggregations = new List<Aggregation>();
			this.GroupingExpressions = new List<Expression>();
			this.GroupingExpressionsMap = new Dictionary<string, Expression>(StringComparer.Ordinal);
		}

		/// <summary>
		/// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
		public override ExpressionType NodeType
		{
			get { return (ExpressionType)ResourceExpressionType.ApplyQueryOption; }
		}

		/// <summary>
		/// Aggregations in the $apply expression
		/// </summary>
		internal List<Aggregation> Aggregations { get; private set; }

		/// <summary>
		/// The individual expressions that make up the GroupBy selector.
		/// </summary>
		internal List<Expression> GroupingExpressions { get; private set; }

		/// <summary>
		/// Mapping of grouping expression and member name
		/// </summary>
		internal IDictionary<string, Expression> GroupingExpressionsMap { get; private set; }

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
