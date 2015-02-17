//---------------------------------------------------------------------
// <copyright file="FilterStrategy.cs" company="Microsoft">
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
	/// A strategy that filters values from an underlying wrapped strategy using a given predicate.
	/// </summary>
	/// <typeparam name="T">The type of values.</typeparam>
	public class FilterStrategy<T> : ExplorationStrategy<T>
	{
		private readonly Func<T, bool> _predicate;

		/// <summary>
		/// Gets the predicate.
		/// </summary>
		/// <value>The predicate.</value>
		public Func<T, bool> Predicate
		{
			get { return _predicate; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="predicate">The predicate.</param>
		public FilterStrategy(ExplorationStrategy<T> wrappedStrategy, Func<T, bool> predicate)
			: base(wrappedStrategy)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			_predicate = predicate;
		}

		/// <summary>
		/// Explores the input (sub-)space. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vectors/values from the (sub-)space.
		/// </returns>
		/// <remarks>
		/// Either <see cref="ExplorationStrategy&lt;T&gt;.Explore"/> or <see cref="ExplorationStrategy&lt;T&gt;.DynamicExplore"/> should be overriden.
		/// </remarks>
		public override IEnumerable<T> Explore()
		{
			return WrappedStrategies[0].Explore().Where(_predicate);
		}
	}
}
