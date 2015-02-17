//---------------------------------------------------------------------
// <copyright file="RepeatStrategy.cs" company="Microsoft">
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
	/// An exploration strategy that just repeats the values returned by the underlying strategy a set number of times.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RepeatStrategy<T> : ExplorationStrategy<T>
	{
		private int _numberOfTimesToRepeat;

		/// <summary>
		/// Initializes a new instance of the <see cref="RepeatStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="numberOfTimesToRepeat">The number of times to repeat.</param>
		public RepeatStrategy(ExplorationStrategy<T> wrappedStrategy, int numberOfTimesToRepeat)
			: base(wrappedStrategy)
		{
			if (numberOfTimesToRepeat <= 0)
			{
                throw new ArgumentOutOfRangeException("numberOfTimesToRepeat", "Must be a positive integer. Actual: " + numberOfTimesToRepeat);
			}
			_numberOfTimesToRepeat = numberOfTimesToRepeat;
		}

		/// <summary>
		/// Explores the input (sub-)space to give (potentially dynamic) value factories. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vector/value-factories from the (sub-)space.
		/// </returns>
		public override IEnumerable<IValueFactory<T>> DynamicExplore()
		{
			for (int i = 0; i < _numberOfTimesToRepeat; i++)
			{
				foreach (IValueFactory<T> factory in WrappedStrategies[0].DynamicExplore())
				{
					yield return factory;
				}
			}
		}
	}
}
