//---------------------------------------------------------------------
// <copyright file="ExhaustiveIEnumerableStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// An exploration strategy for IEnumerable types which simply returns all values in the list
	/// </summary>
	/// <typeparam name="TItemType">the type of the items of the collection</typeparam>
	public class ExhaustiveIEnumerableStrategy<TItemType> : ExplorationStrategy<TItemType>
	{

		private List<TItemType> _items;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExhaustiveIEnumerableStrategy&lt;TItemType&gt;"/> class.
		/// </summary>
		/// <param name="items">The items.</param>
		public ExhaustiveIEnumerableStrategy(params TItemType[] items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			_items = new List<TItemType>(items);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExhaustiveIEnumerableStrategy&lt;TItemType&gt;"/> class.
		/// </summary>
		/// <param name="items">The items.</param>
		public ExhaustiveIEnumerableStrategy(IEnumerable<TItemType> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			_items = new List<TItemType>(items);
		}

		/// <summary>
		/// Explores the input (sub-)space.
		/// </summary>
		/// <returns>
		/// Enumeration over all the values in the list
		/// </returns>
		public override IEnumerable<TItemType> Explore()
		{
			foreach (TItemType v in _items)
				yield return v;
		}
	}

}
