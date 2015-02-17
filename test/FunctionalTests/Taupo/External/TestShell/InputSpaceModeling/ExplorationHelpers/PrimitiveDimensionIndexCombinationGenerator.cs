//---------------------------------------------------------------------
// <copyright file="PrimitiveDimensionIndexCombinationGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// Helper class to generate r order combinations of n objects - used in pairwise exploration.
	/// </summary>
	internal class PrimitiveDimensionIndexCombinationGenerator : DimensionIndexCombinationGenerator
	{
		private readonly int _numberOfDimensions;
		private readonly int _order;

		/// <param name="numberOfDimensions">The total number of TargetMatrix.Dimensions</param>
		/// <param name="order">The order of combinations to be generated</param>
		public PrimitiveDimensionIndexCombinationGenerator(int numberOfDimensions, int order)
		{
			if (order > numberOfDimensions)
			{
				throw new ArgumentException("order > numberOfDimensions", "order");
			}

			if (numberOfDimensions < 1)
			{
				throw new ArgumentOutOfRangeException("numberOfDimensions");
			}

			this._numberOfDimensions = numberOfDimensions;
			this._order = order;
		}

		public override IEnumerable<ReadOnlyCollection<int>> Generate()
		{
			int[] workingArray = new int[_order];

			for (int i = 0; i < workingArray.Length; i++)
			{
				workingArray[i] = i;
			}

			yield return workingArray.ToList().AsReadOnly();

			int total = Choose(_numberOfDimensions, _order);
			for (int current = 1; current < total; current++)
			{

				int i = _order - 1;
				while (workingArray[i] == _numberOfDimensions - _order + i)
				{
					i--;
				}

				workingArray[i] += 1;

				for (int j = i + 1; j < _order; j++)
				{
					workingArray[j] = workingArray[i] + j - i;
				}

				yield return workingArray.ToList().AsReadOnly();
			}
		}

		/// <summary>
		/// Computes the binomial coefficient nCk
		/// </summary>
		/// <param name="n">The n.</param>
		/// <param name="k">The k.</param>
		/// <returns>The binomial coefficient</returns>
		private static int Choose(int n, int k)
		{
			// DEVNOTE: Copied the code almost verbatim from wikipedia http://en.wikipedia.org/wiki/Binomial_coefficient
			if (k > n)
			{
				return 0;
			}

			if (k > n / 2)
			{
				k = n - k; // faster
			}

			int accum = 1;
			for (int i = 0; i++ < k; )
			{
				accum = accum * (n - k + i) / i;
			}

			return accum;
		}
	}
}
