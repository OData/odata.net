//---------------------------------------------------------------------
// <copyright file="MakeDeterministicStrategy.cs" company="Microsoft">
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
	/// Wraps a possibly non-deterministic strategy (e.g. random and categorical) to make it deterministic.
	/// </summary>
	/// <typeparam name="T">The type of values in the strategy.</typeparam>
	public class MakeDeterministicStrategy<T> : ExplorationStrategy<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MakeDeterministicStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		public MakeDeterministicStrategy(ExplorationStrategy<T> wrappedStrategy)
			: base(wrappedStrategy)
		{
		}

		/// <summary>
		/// Explores the input (sub-)space. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vectors/values from the (sub-)space.
		/// </returns>
		public override IEnumerable<T> Explore()
		{
			// All I really need to do is override Explore() instead of DynamicExplore(), thus making it deterministic.
			return WrappedStrategies[0].Explore();
		}
	}
}
