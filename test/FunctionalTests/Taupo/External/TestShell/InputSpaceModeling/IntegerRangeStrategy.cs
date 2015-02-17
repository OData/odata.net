//---------------------------------------------------------------------
// <copyright file="IntegerRangeStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A strategy that returns all the values from within a given range.
	/// </summary>
	public class IntegerRangeStrategy : ExplorationStrategy<int>
	{
		private readonly int _inclusiveLowerLimit;
		private readonly int _exclusiveUpperLimit;

		/// <summary>
		/// Gets the inclusive lower limit.
		/// </summary>
		/// <value>The inclusive lower limit.</value>
		public int InclusiveLowerLimit
		{
			get { return _inclusiveLowerLimit; }
		}

		/// <summary>
		/// Gets the exclusive upper limit.
		/// </summary>
		/// <value>The exclusive upper limit.</value>
		public int ExclusiveUpperLimit
		{
			get { return _exclusiveUpperLimit; }
		} 

		/// <summary>
		/// Initializes a new instance of the <see cref="IntegerRangeStrategy"/> class.
		/// </summary>
		/// <param name="inclusiveLowerLimit">The inclusive lower limit.</param>
		/// <param name="exclusiveUpperLimit">The exclusive upper limit.</param>
		public IntegerRangeStrategy(int inclusiveLowerLimit, int exclusiveUpperLimit)
		{
			if (exclusiveUpperLimit <= inclusiveLowerLimit)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The upper limit given {0} should've been greater than the lower limit {1}.",
					exclusiveUpperLimit, inclusiveLowerLimit));
			}
			_inclusiveLowerLimit = inclusiveLowerLimit;
			_exclusiveUpperLimit = exclusiveUpperLimit;
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
		public override IEnumerable<int> Explore()
		{
			for (int i = _inclusiveLowerLimit; i < _exclusiveUpperLimit; i++)
			{
				yield return i;
			}
		}
	}
}
