//---------------------------------------------------------------------
// <copyright file="UnionStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A UNION of two Vector strategies
	/// </summary>
	public class UnionStrategy : ExplorationStrategy<Vector>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnionStrategy"/> class.
		/// </summary>
		/// <param name="firstSet">The first set.</param>
		/// <param name="secondSet">The second set.</param>
		/// <remarks>
		/// Returns an exploration strategy that always returns the UNION (with duplicate removal) of the output of two exploration strategies,
		/// where two values are considered equal if they are in the same category or if they are identical.
		/// </remarks>
		public UnionStrategy(ExplorationStrategy<Vector> firstSet, ExplorationStrategy<Vector> secondSet)
			: base(firstSet, secondSet)
		{
		}

		/// <summary>
		/// Explores the input (sub-)space. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vectors/values from the (sub-)space.
		/// </returns>
		public override IEnumerable<Vector> Explore()
		{
			return Vector.Union(WrappedStrategies[0].Explore(), WrappedStrategies[1].Explore());
		}
	}

	/// <summary>
	/// A UNION (with duplicate removal) of two strategies.
	/// </summary>
	/// <typeparam name="T">The type of values.</typeparam>
	public class UnionStrategy<T> : ExplorationStrategy<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnionStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="firstSet">The first set.</param>
		/// <param name="secondSet">The second set.</param>
		public UnionStrategy(ExplorationStrategy<T> firstSet, ExplorationStrategy<T> secondSet)
			: base(firstSet, secondSet)
		{
		}

		/// <summary>
		/// Explores the input (sub-)space to give (potentially dynamic) value factories. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vector/value-factories from the (sub-)space.
		/// </returns>
		public override IEnumerable<IValueFactory<T>> DynamicExplore()
		{
			return WrappedStrategies[0].DynamicExplore().Union(WrappedStrategies[1].DynamicExplore());
		}
	}
}
