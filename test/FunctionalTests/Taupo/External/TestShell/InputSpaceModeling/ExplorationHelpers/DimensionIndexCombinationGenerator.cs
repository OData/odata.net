//---------------------------------------------------------------------
// <copyright file="DimensionIndexCombinationGenerator.cs" company="Microsoft">
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
	/// A helper for pairwise generation: generates the combination of indexes of dimensions that need to be covered.
	/// </summary>
	internal abstract class DimensionIndexCombinationGenerator
	{
		/// <summary>
		/// Creates a dimension-index combination generator to be used in the pairwise exploration of a Matrix.
		/// </summary>
		/// <param name="numberOfDimensions">Total number of dimensions in the matrix explored.</param>
		/// <param name="order">The order of overall exploration (2 for pairwise).</param>
		/// <param name="extendedCoverageDimensionIndexSubsets">
		/// Sets of dimension indexes with associated order that should be covered at a higher order. For example,
		/// a tuple like ({1, 3, 6, 7}, 3) means that all order 3 combinations of the second, fourth, seventh and eighth dimensions should be returned.
		/// </param>
		/// <param name="noCoverageGuaranteeDimensionIndexes">
		/// The set of dimension indexes that should not be covered - this means that no combinations returned will contain
		/// any of these indexes.
		/// </param>
		/// <returns>
		/// A combination generator that returns all the combinations of indexes that should be covered in this exploration.
		/// </returns>
		public static DimensionIndexCombinationGenerator Create(int numberOfDimensions, int order,
			IEnumerable<Tuple<ReadOnlyCollection<int>, int>> extendedCoverageDimensionIndexSubsets,
			IEnumerable<int> noCoverageGuaranteeDimensionIndexes)
		{
			DimensionIndexCombinationGenerator basic = new PrimitiveDimensionIndexCombinationGenerator(numberOfDimensions, order);
			IEnumerable<DimensionIndexCombinationGenerator> underlyingGenerators = Enumerable.Repeat(basic, 1);
			if (extendedCoverageDimensionIndexSubsets != null)
			{
				// Add more combinations to cover the extended-coverage subset. Done for each subset { a0, ..., am }
				// by generating the combinations for { 0, ..., m } using the PrimitiveDimensionIndexCombinationGenerator, then
				// translating them into combinations for { a0, ..., am } using the TranslatedDimensionIndexCombinationGenerator.
                underlyingGenerators = underlyingGenerators.Concat(
					extendedCoverageDimensionIndexSubsets.Select(s =>
						new TranslatedDimensionIndexCombinationGenerator(new PrimitiveDimensionIndexCombinationGenerator(s.Item1.Count, s.Item2), s.Item1)
                        as DimensionIndexCombinationGenerator
					)
				);
			}
			DimensionIndexCombinationGenerator ret = new CompositeDimensionIndexCombinationGenerator(underlyingGenerators);
			if (noCoverageGuaranteeDimensionIndexes != null)
			{
				// Filter out any combinations that contain any of the indexes of dimensions we don't want to cover.
				ret = new FilteredDimensionIndexCombinationGenerator(ret,
					seq => !seq.Intersect(noCoverageGuaranteeDimensionIndexes).Any());
			}
			return ret;
		}

		/// <summary>
		/// Generates the combinations.
		/// </summary>
		/// <returns>All the combinations generated.</returns>
		public abstract IEnumerable<ReadOnlyCollection<int>> Generate();
	}
}
