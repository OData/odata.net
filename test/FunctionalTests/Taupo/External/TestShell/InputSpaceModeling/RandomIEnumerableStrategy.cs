//---------------------------------------------------------------------
// <copyright file="RandomIEnumerableStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// An exploration strategy for IEnumerable types which that returns one (or a fixed number of) random value(s) from the collection each time.
	/// </summary>
	/// <typeparam name="T">the type of the items of the collection.</typeparam>
	public class RandomIEnumerableStrategy<T> : RandomStrategyFromCategory<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RandomIEnumerableStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <exception cref="ArgumentNullException"><paramref name="items"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="items"/> is empty.</exception>
		public RandomIEnumerableStrategy(Func<int, int> nextInt, params T[] items)
			: this(nextInt, (IEnumerable<T>)items, 1)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomIEnumerableStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <exception cref="ArgumentNullException"><paramref name="items"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="items"/> is empty.</exception>
		public RandomIEnumerableStrategy(Func<int, int> nextInt, IEnumerable<T> items)
            : this(nextInt, items, 1)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomIEnumerableStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <param name="numberOfValuesToReturn">The fixed number of random values to return each time (deafult is 1).</param>
		/// <exception cref="ArgumentNullException"><paramref name="items"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="items"/> is empty.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfValuesToReturn"/> is less than 1.</exception>
        public RandomIEnumerableStrategy(Func<int, int> nextInt, IEnumerable<T> items, int numberOfValuesToReturn)
            : base(new PointCategory<T>("Wrapped", nextInt, items), numberOfValuesToReturn)
		{
			if (new List<T>(items).Count == 0)
			{
				throw new ArgumentOutOfRangeException("items", "The collection must have at least one value.");
			}
		}
	}
}