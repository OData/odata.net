//---------------------------------------------------------------------
// <copyright file="ConcatStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// Returns an exploration strategy that concatenates two underlying strategies.
	/// </summary>
	/// <typeparam name="T">The type of values in the strategy.</typeparam>
	public class ConcatStrategy<T> : ExplorationStrategy<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConcatStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="firstStrategy">The first strategy.</param>
		/// <param name="secondStrategy">The second strategy.</param>
		public ConcatStrategy(ExplorationStrategy<T> firstStrategy, ExplorationStrategy<T> secondStrategy)
			: base(firstStrategy, secondStrategy)
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
			return WrappedStrategies[0].DynamicExplore().Concat(WrappedStrategies[1].DynamicExplore());
		}
	}
}
