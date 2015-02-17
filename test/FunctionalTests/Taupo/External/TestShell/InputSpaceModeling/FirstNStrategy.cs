//---------------------------------------------------------------------
// <copyright file="FirstNStrategy.cs" company="Microsoft">
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
	/// A strategy that takes the first fixed number of elements from a wrapped strategy and just returns those.
	/// </summary>
	/// <typeparam name="T">The type of element.</typeparam>
	public class FirstNStrategy<T> : ExplorationStrategy<T>
	{
		private readonly int _numberToTake;

		/// <summary>
		/// Gets the number to take.
		/// </summary>
		/// <value>The number to take.</value>
		public int NumberToTake
		{
			get { return _numberToTake; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FirstNStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="numberToTake">The number to take.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="numberToTake"/> is less than or equal to 0.</exception>
		public FirstNStrategy(ExplorationStrategy<T> wrappedStrategy, int numberToTake)
			: base(wrappedStrategy)
		{
			if (numberToTake <= 0)
			{
				throw new ArgumentOutOfRangeException("numberToTake", "The number to take must be a strictly positive number");
			}
			_numberToTake = numberToTake;
		}

		/// <summary>
		/// Explores the input (sub-)space to give (potentially dynamic) value factories. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vector/value-factories from the (sub-)space.
		/// </returns>
		public override IEnumerable<IValueFactory<T>> DynamicExplore()
		{
			int currentTaken = 0;
			foreach (IValueFactory<T> current in WrappedStrategies[0].DynamicExplore())
			{
				yield return current;
				currentTaken++;
				if (currentTaken >= NumberToTake)
				{
					break;
				}
			}
		}
	}
}
