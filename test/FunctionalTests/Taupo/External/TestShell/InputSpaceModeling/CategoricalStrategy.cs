//---------------------------------------------------------------------
// <copyright file="CategoricalStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// (Internal usage) Non-generic interface for CategoricalStrategy.
	/// </summary>
	/// <remarks>
	/// This exists mainly for internal usage when we are exploring and manipulating dimensions for which we don't know the type.
	/// Client code should only use the generic version.
	/// </remarks>
	public interface ICategoricalStrategy : IExplorationStrategy
	{
		/// <summary>
		/// All the categories in there.
		/// </summary>
		IEnumerable<Category> BaseCategories
		{
			get;
		}
	}

	/// <summary>
	/// Exploration strategy on a dimension that partitions the dimension into a number of categories (or equivalence classes),
	/// then picks one value from each category.
	/// </summary>
	/// <typeparam name="T">The type of values in the dimension.</typeparam>
	/// <example><code><![CDATA[
	///CategoricalStrategy<int> tableSizeStrategy = new CategoricalStrategy<int>(
	///  new PointCategory<int>("Empty", 0),
	///  new PointCategory<int>("OneRow", 1),
	///  new IntegerRangeCategory("Small", 2, 100),
	///  new IntegerRangeCategory("Medium", 1000, 9000),
	///  new IntegerRangeCategory("Large", 100000, 200000));
	///CategoricalStrategy<SqlDataType> dataTypeStrategy = new CategoricalStrategy<SqlDataType>(
	///  new PointCategory<SqlDataType>("Integral", SqlDataType.TinyInt, SqlDataType.SmallInt, SqlDataType.Int, SqlDataType.BigInt),
	///  new PointCategory<SqlDataType>("String", SqlDataType.Char, SqlDataType.NChar, SqlDataType.VarChar, SqlDataType.NVarChar));
	/// ]]></code></example>
	public sealed class CategoricalStrategy<T> : ExplorationStrategy<T>, ICategoricalStrategy
	{
		private List<Category<T>> _allCategories;

		/// <summary>
		/// Gets the categories.
		/// </summary>
		/// <value>All categories.</value>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = @"fxCop (correctly) thinks that the syntax for nesting is clunky,
but I can't think of any other way to expose this property (and most clients don't really have to use it)")]
		public Collection<Category<T>> AllCategories
		{
			get
			{
				return new Collection<Category<T>>(_allCategories);
			}
		}

		/// <summary>
		/// Retrieves the categories as non-generic objects (for internal usage).
		/// </summary>
		IEnumerable<Category> ICategoricalStrategy.BaseCategories
		{
			get
			{
				foreach (Category<T> category in _allCategories)
					yield return category;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoricalStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="allCategories">All categories.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="allCategories"/> is null.</exception>
		public CategoricalStrategy(params Category<T>[] allCategories)
			: this((IEnumerable<Category<T>>) allCategories)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoricalStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="allCategories">All categories.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="allCategories"/> is null.</exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public CategoricalStrategy(IEnumerable<Category<T>> allCategories)
		{
			if (allCategories == null)
				throw new ArgumentNullException("allCategories");
			foreach (Category<T> current in allCategories)
			{
				if (current == null)
					throw new ArgumentNullException("allCategories");
			}
			_allCategories = new List<Category<T>>(allCategories);
		}

		/// <summary>
		/// Explores the input (sub-)space to give (potentially dynamic) value factories. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vector/value-factories from the (sub-)space.
		/// </returns>
		public override IEnumerable<IValueFactory<T>> DynamicExplore()
		{
			foreach (Category<T> category in AllCategories)
				yield return category;
		}
	}
}
