//---------------------------------------------------------------------
// <copyright file="ShuffleStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A wrapper strategy that shuffles the values from the underlying strategy.
	/// </summary>
	/// <typeparam name="T">The type of values from the strategy.</typeparam>
	public class ShuffleStrategy<T> : ExplorationStrategy<T>
	{
        private Func<int, int> _nextInt;
        
        /// <summary>
		/// Initializes a new instance of the <see cref="ShuffleStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		public ShuffleStrategy(ExplorationStrategy<T> wrappedStrategy, Func<int, int> nextInt)
			: base(wrappedStrategy)
		{
            if (nextInt == null)
            {
                throw new ArgumentNullException("nextInt");
            }

            _nextInt = nextInt;
		}

		/// <summary>
		/// Explores the input (sub-)space to give (potentially dynamic) value factories. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vector/value-factories from the (sub-)space.
		/// </returns>
		public override IEnumerable<IValueFactory<T>> DynamicExplore()
		{
			List<IValueFactory<T>> originalValues = new List<IValueFactory<T>>(WrappedStrategies[0].DynamicExplore());
			RandomUtilities.ShuffleList(originalValues, _nextInt);
			return originalValues;
		}

		// DEVNOTE: This is currently implemented as a stop and go operator with all the values buffered,
		// this of course won't scale for huge strategies, so if needed we could relax this to not buffer
		// everything (at the expense of randomness).
		// Implementation in that case (from Rajneesh)
		//int batchSize = 10000;
		//List<T> batch = new List<T>();

		//foreach (T inputElement in WrappedStrategy.Explore())
		//{
		//  batch.Add(inputElement);
		//  if (batch.Count >= batchSize)
		//  {
		//    // if batch size has been reached, shuffle and yield its elements
		//    CommonRandom.ShuffleList(batch);
		//    foreach (T batchElement in batch)
		//      yield return batchElement;
		//    batch = new List<T>();
		//  }
		//}
		//// shuffle and yield the remaining elements
		//CommonRandom.ShuffleList(batch);
		//foreach (T batchElement in batch)
		//  yield return batchElement;
	}
}
