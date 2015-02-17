//---------------------------------------------------------------------
// <copyright file="RandomIntegerRangeStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// An exploration strategy for integers that just randomly select an integer from within a range every time.
	/// </summary>
	public class RandomIntegerRangeStrategy : RandomStrategyFromCategory<int>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RandomIntegerRangeStrategy"/> class.
		/// </summary>
		/// <param name="lowerLimit">The inclusive lower limit.</param>
		/// <param name="upperLimit">The exclusive upper limit.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="upperLimit"/> is lower than or equal to <paramref name="lowerLimit"/>.</exception>
		public RandomIntegerRangeStrategy(int lowerLimit, int upperLimit, Func<int, int, int> nextIntInRange)
            : this(lowerLimit, upperLimit, 1, nextIntInRange)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomIntegerRangeStrategy"/> class.
		/// </summary>
		/// <param name="lowerLimit">The inclusive lower limit.</param>
		/// <param name="upperLimit">The exclusive upper limit.</param>
		/// <param name="numberOfValuesToReturn">The fixed number of random values to return each time (deafult is 1).</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="upperLimit"/> is lower than or equal to <paramref name="lowerLimit"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfValuesToReturn"/> is less than 1.</exception>
		public RandomIntegerRangeStrategy(int lowerLimit, int upperLimit, int numberOfValuesToReturn, Func<int, int, int> nextIntInRange)
            : base(new RangeCategory<int>("Wrapped", lowerLimit, upperLimit, nextIntInRange), numberOfValuesToReturn)
		{
			if (upperLimit <= lowerLimit)
			{
				throw new ArgumentOutOfRangeException("upperLimit", "The requested range [" + lowerLimit + ", " + upperLimit + "[ is empty.");
			}
		}

		/// <summary>
		/// Gets the inclusive lower limit of the range.
		/// </summary>
		/// <value>The lower limit.</value>
		public int LowerLimit
		{
			get { return ((RangeCategory<int>)WrappedCategory).LowerLimit; }
		}

		/// <summary>
		/// Gets the exclusive upper limit of the range.
		/// </summary>
		/// <value>The upper limit.</value>
		public int UpperLimit
		{
			get { return ((RangeCategory<int>)WrappedCategory).UpperLimit; }
		}
	}
}
