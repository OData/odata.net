//---------------------------------------------------------------------
// <copyright file="PairwiseStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A combinatorial exploration strategy that ensures that all pair-wise combinations for each two dimensions are picked.
	/// In general, this strategy can provide n-wise coverage, that is, ensure that all n-tuples
	/// (instead of just pairs) are covered.
	/// </summary>
    public class PairwiseStrategy : CombinatorialStrategy
	{
		// The overall order of the strategy (2 for pairwise)
		private int _order = 2;
		// Max times the algorithm will try to select random values for
		// dimensions that are not in the currently selected pairwise combination
		private const int MAXTRIES = 5;
		// Do I use the partial constraint checker optimization?
		private bool _usePartialConstraintChecker = true;
		// Sets of extended coverage sub-sets of dimensions
		private readonly List<Tuple<ReadOnlyCollection<QualifiedDimension>, int>> _extendedCoverageDimensionSubsets =
			new List<Tuple<ReadOnlyCollection<QualifiedDimension>, int>>();
		// No coverage dimensions
		private readonly Collection<QualifiedDimension> _noCoverageGuaranteeDimensions = new Collection<QualifiedDimension>();

        private Func<int, int> _nextInt;

		/// <summary>
		/// Use the partial constraint checker optimization (very useful in heavily constrained spaces).
		/// </summary>
		/// <remarks>
		/// Default is <c>true</c>. I only expose it as internal for benchmarking purposes.
		/// </remarks>
		public bool UsePartialConstraintChecker
		{
			get { return _usePartialConstraintChecker; }
			set { _usePartialConstraintChecker = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PairwiseStrategy"/> class.
		/// </summary>
		/// <param name="entireMatrix">The entire matrix.</param>
		/// <param name="constraints">The constraints.</param>
		/// <exception cref="ArgumentNullException">Any of the TargetMatrix.Dimensions is null.</exception>
		public PairwiseStrategy(Matrix entireMatrix, Func<int, int> nextInt, IEnumerable<IConstraint> constraints)
			: base(entireMatrix, constraints)
		{
            if (nextInt == null)
                throw new ArgumentNullException("nextInt");
            _nextInt = nextInt;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PairwiseStrategy"/> class.
		/// </summary>
		/// <param name="entireMatrix">The entire matrix.</param>
		/// <param name="constraints">The constraints.</param>
		/// <exception cref="ArgumentNullException">Any of the TargetMatrix.Dimensions is null.</exception>
		public PairwiseStrategy(Matrix entireMatrix, Func<int, int> nextInt, params IConstraint[] constraints)
			: this(entireMatrix, nextInt, (IEnumerable<IConstraint>)constraints)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PairwiseStrategy"/> class.
		/// </summary>
		/// <param name="sourceStrategy">The source strategy.</param>
		/// <exception cref="ArgumentNullException"><paramref name="sourceStrategy"/> is null.</exception>
		public PairwiseStrategy(CombinatorialStrategy sourceStrategy)
			: base(sourceStrategy)
		{
		}

		/// <summary>
		/// The order n of the strategy. The strategy provides n-wise coverage, that is, ensure that all n-tuples
		/// are covered. The default value is 2 (giving pairwise coverage).
		/// </summary>
		/// <value>The order.</value>
		public int Order
		{
			get { return _order; }
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("value", "The value provided for the 'Order' property must be greater than or equal to 1. Value: " + value);
				}
				_order = value;
			}
		}

		/// <summary>
		/// The dimensions for which no coverage guarantees are given (a random valid value will be chosen for these dimensions for each vector produced).
		/// </summary>
		public Collection<QualifiedDimension> NoCoverageGuaranteeDimensions
		{
			get { return _noCoverageGuaranteeDimensions; }
		}

		/// <summary>
		/// Specifies that the given subset of dimensions should be covered at the extended specified order.
		/// </summary>
		/// <param name="order">The order - should be greater than <see cref="Order"/>.</param>
		/// <param name="dimensionSubset">The subset of dimensions.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="order"/> is less than or equal <see cref="Order"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="order"/> is greater than the number of dimensions in <paramref name="dimensionSubset"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="dimensionSubset"/> is <c>null</c>.</exception>
		public void AddExtendedCoverageDimensionSubset(int order, IEnumerable<QualifiedDimension> dimensionSubset)
		{
			if (dimensionSubset == null)
			{
				throw new ArgumentNullException("dimensionSubset");
			}
			if (order <= Order)
			{
				throw new ArgumentOutOfRangeException("order", "The given order " + order + " is less than or equal to the overall order " + Order);
			}
			ReadOnlyCollection<QualifiedDimension> subsetCollection = dimensionSubset.ToList().AsReadOnly();
			if (order > subsetCollection.Count)
			{
				throw new ArgumentOutOfRangeException("order", "The given order " + order + " is greater than the number of dimensions in the subset " + subsetCollection.Count);
			}
			_extendedCoverageDimensionSubsets.Add(new Tuple<ReadOnlyCollection<QualifiedDimension>, int>(subsetCollection, order));
		}

		/// <summary>
		/// Specifies that the given subset of dimensions should be covered at the extended specified order.
		/// </summary>
		/// <param name="order">The order - should be greater than <see cref="Order"/>.</param>
		/// <param name="dimensionSubset">The subset of dimensions.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="order"/> is less than or equal <see cref="Order"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="order"/> is greater than the number of dimensions in <paramref name="dimensionSubset"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="dimensionSubset"/> is <c>null</c>.</exception>
		public void AddExtendedCoverageDimensionSubset(int order, params QualifiedDimension[] dimensionSubset)
		{
			AddExtendedCoverageDimensionSubset(order, (IEnumerable<QualifiedDimension>)dimensionSubset);
		}

		/// <summary>
		/// Specifies that the whole matrix should be covered at <see cref="Order"/> coverage with no special subsets
		/// (doesn't affect <see cref="NoCoverageGuaranteeDimensions"/> though).
		/// </summary>
		public void ClearExtendedCoverageDimensionSubsets()
		{
			_extendedCoverageDimensionSubsets.Clear();
		}

		/// <summary>
		/// The subsets of dimensions that are to be covered at a higher order, with the associated order of coverage.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "This is an advanced property that won't normally be used.")]
		public IEnumerable<Tuple<ReadOnlyCollection<QualifiedDimension>, int>> ExtendedCoverageDimensionSubsets
		{
			get { return _extendedCoverageDimensionSubsets; }
		}

		/// <summary>
		/// Explores the input (sub-)space. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vectors/values from the (sub-)space.
		/// </returns>
		public override IEnumerable<Vector> Explore()
		{
			if (TargetMatrix.Dimensions.Count == 0)
			{
				// No dimensions to explore, then yield one empty vector.
				yield return new Vector();
				yield break;
			}

			ReadOnlyCollection<DimensionWithValues> dimensionValues = GetAllDimensionValues();

			if (dimensionValues.Any(dim => dim.Values.Count == 0))
			{
				yield break; // One of the dimensions is empty
			}

			if (dimensionValues.Count < _order)
			{
				// This is really non-sensical to ask for an n-wise strategy for a matrix with less than n dimensions
				// but I'd rather return a good default than throw. So I'll just cheat and do an exhaustive strategy instead
				ExhaustiveCombinatorialStrategy exhaustiveStrategy = new ExhaustiveCombinatorialStrategy(this);
				foreach (Vector v in exhaustiveStrategy.Explore())
					yield return v;
				yield break;
			}

			CoveredPairwiseCombinations coveredCombinations = new CoveredPairwiseCombinations();

			DimensionValueIndexCombination currentCombination = new DimensionValueIndexCombination(dimensionValues);
			DimensionIndexCombinationGenerator dimensionIndexGenerator = CreateDimensionIndexCombinationGenerator(dimensionValues);
			Vector currentVector = currentCombination.GenerateVector();
			if (IsValidVector(currentVector))
			{
				coveredCombinations.AddCoveredPairwiseCombinations(currentCombination, dimensionIndexGenerator);
				yield return currentVector;
			}

			PartialVectorConstraintChecker partialConstraintChecker = UsePartialConstraintChecker ?
				new PartialVectorConstraintChecker(dimensionValues, Constraints) : null;

			foreach (ReadOnlyCollection<int> currentDimensionsToCover in dimensionIndexGenerator.Generate())
			{
				// Initialize the selection of values in the newly selected pairwise combination
				for (int i = 0; i < currentDimensionsToCover.Count; i++)
				{
					currentCombination[currentDimensionsToCover[i]] = 0;
				}

				while (true)
				{
					bool selectionSucceeded = SelectNextValuesForCurrentDimensionCombination(currentCombination, currentDimensionsToCover);

					if (!selectionSucceeded)
					{
						// We exhausted all combinations of values for the current pairwise combination
						// select the next pairwise combination
						break;
					}

					if (coveredCombinations.IsCombinationCovered(currentDimensionsToCover, currentCombination))
					{
						continue;
					}

					if (partialConstraintChecker != null &&
						!partialConstraintChecker.IsValidForApplicableConstraints(currentCombination, currentDimensionsToCover))
					{
						// This combination is forbidden by constraints - carry on
						continue;
					}

					// Randomize other values, try at most MAXTRIES since we might hit invalid combinations
					for (int i = 0; i < MAXTRIES; i++)
					{
						List<int> remainingDimensionIndexes = Enumerable.Range(0, dimensionValues.Count)
							.Where(di => !currentDimensionsToCover.Contains(di)) // Don't randomize the pair currently worked on
							.ToList();
                        RandomUtilities.ShuffleList(remainingDimensionIndexes, _nextInt);
						List<int> setDimensionIndexes = new List<int>(currentDimensionsToCover);
						foreach (int dimensionIndex in remainingDimensionIndexes)
						{
							ReadOnlyCollection<int> allowedValueIndexes = UsePartialConstraintChecker ?
								partialConstraintChecker.GetAllowedValueIndexesForDimension(dimensionIndex, setDimensionIndexes, currentCombination) :
								Enumerable.Range(0, currentCombination.GetDimensionSize(dimensionIndex)).ToList().AsReadOnly();
							if (allowedValueIndexes.Count == 0)
							{
								break;
							}
							currentCombination[dimensionIndex] = allowedValueIndexes[_nextInt(allowedValueIndexes.Count)];
							setDimensionIndexes.Add(dimensionIndex);
						}
						if (setDimensionIndexes.Count < dimensionValues.Count)
						{
							// Couldn't set all dimension values, try again (or quit)
							continue;
						}

						currentVector = currentCombination.GenerateVector();
						if (partialConstraintChecker != null || IsValidVector(currentVector))
						{
							coveredCombinations.AddCoveredPairwiseCombinations(currentCombination, dimensionIndexGenerator);
							yield return currentVector;
							break; // Out of the MAXTRIES loop
						}
					}
				}
			}
		}

		private DimensionIndexCombinationGenerator CreateDimensionIndexCombinationGenerator(ReadOnlyCollection<DimensionWithValues> dimensionValues)
		{
			return DimensionIndexCombinationGenerator.Create(dimensionValues.Count, _order,
				_extendedCoverageDimensionSubsets.Select(s =>
					Tuple.Create(
						s.Item1.Select(d => GetIndexOfDimension(d, dimensionValues, "ExtendedCoverageDimensionSubsets")).ToList().AsReadOnly(),
						s.Item2
					)
				),
				_noCoverageGuaranteeDimensions.Select(d =>
					GetIndexOfDimension(d, dimensionValues, "NoCoverageGuaranteeDimensions")
				)
			);
		}

		private static int GetIndexOfDimension(QualifiedDimension targetDimension, ReadOnlyCollection<DimensionWithValues> dimensionValues,
			string origin)
		{
			int ret = 0;
			foreach (DimensionWithValues dv in dimensionValues)
			{
				if (targetDimension.Equals(dv.Dimension))
				{
					return ret;
				}
				ret++;
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
				"Dimension {0} from {1} was not found in the explored matrix (all dimensions: {2}).",
				targetDimension.FullyQualifiedName, origin, string.Join(",", dimensionValues.Select(dv => dv.Dimension.FullyQualifiedName))));
		}

		private bool SelectNextValuesForCurrentDimensionCombination(DimensionValueIndexCombination currentCombination,
			ReadOnlyCollection<int> currentDimensionsToCover)
		{
			int currentIndex = currentDimensionsToCover.Count - 1;
			bool selectNext = true;

			// Continue enumerating pairwise indices until a value is selected (!selectNext)
			// or we exhaust all possibilities for the current pairwise combination
			// (selectNext) && (currentIndex == -1)
			while ((selectNext) && (currentIndex >= 0))
			{
				Debug.Assert(currentCombination[currentDimensionsToCover[currentIndex]] < currentCombination.GetDimensionSize(currentDimensionsToCover[currentIndex]));
				if (currentCombination[currentDimensionsToCover[currentIndex]] == currentCombination.GetDimensionSize(currentDimensionsToCover[currentIndex]) - 1)
				{
					currentCombination[currentDimensionsToCover[currentIndex]] = 0;
					selectNext = true;
					currentIndex--;
				}
				else
				{
					currentCombination[currentDimensionsToCover[currentIndex]]++;
					selectNext = false;
				}
			}

			return !selectNext || (currentIndex != -1);
		}

		#region Helper classes
		/// <summary>
		/// Helper class to store which pairwise combinations are covered so far
		/// </summary>
		private class CoveredPairwiseCombinations
		{
			// All the covered combinations.
			private List<List<int>> _coveredCombinations = new List<List<int>>();

			/// <summary>
			/// Determines whether the given combination has already been covered.
			/// </summary>
			/// <param name="dimensionsOfInterest">The dimensions of interest.</param>
			/// <param name="combination">The combination.</param>
			/// <returns>
			/// 	<c>true</c> if the given combination is already covered; otherwise, <c>false</c>.
			/// </returns>
			public bool IsCombinationCovered(ICollection<int> dimensionsOfInterest, DimensionValueIndexCombination combination)
			{
				List<int> encodedCombination = EncodeCombination(dimensionsOfInterest, combination);
				foreach (List<int> covered in _coveredCombinations)
				{
					if (AreEqual(covered, encodedCombination))
						return true;
				}
				return false;
			}

			/// <summary>
			/// Adds all the combinations of the given order as covered.
			/// </summary>
			/// <param name="combination">The combination to cover.</param>
			/// <param name="dimensionIndexGenerator">The dimension index generator to use for getting all dimension index combinations we care about.</param>
			public void AddCoveredPairwiseCombinations(DimensionValueIndexCombination combination, DimensionIndexCombinationGenerator dimensionIndexGenerator)
			{
				foreach (ReadOnlyCollection<int> dimensionsOfInterest in dimensionIndexGenerator.Generate())
				{
					if (!IsCombinationCovered(dimensionsOfInterest, combination))
						_coveredCombinations.Add(EncodeCombination(dimensionsOfInterest, combination));
				}
			}

			/// <summary>
			/// Checks if the given int lists are equal.
			/// </summary>
			private static bool AreEqual(List<int> a, List<int> b)
			{
				if (a.Count != b.Count)
					return false;
				for (int i = 0; i < a.Count; i++)
					if (a[i] != b[i])
						return false;
				return true;
			}

			/// <summary>
			/// Encodes the given combination as a list of integers
			/// </summary>
			/// <param name="dimensionsOfInterest"></param>
			/// <param name="combination"></param>
			/// <returns></returns>
			/// <remarks>
			/// I'll just encode a covered combination like: {D4:2,D1:10} as {1,10,4,2} (sort on dimension index first).
			/// </remarks>
			private static List<int> EncodeCombination(ICollection<int> dimensionsOfInterest, DimensionValueIndexCombination combination)
			{
				List<int> sortedDimensions = new List<int>(dimensionsOfInterest);
				sortedDimensions.Sort();
				List<int> ret = new List<int>(dimensionsOfInterest.Count * 2);
				foreach (int dimension in sortedDimensions)
				{
					ret.Add(dimension);
					ret.Add(combination[dimension]);
				}
				return ret;
			}
		}
		#endregion
	}
}
