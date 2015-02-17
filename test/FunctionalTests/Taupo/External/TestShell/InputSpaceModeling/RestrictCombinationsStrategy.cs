//---------------------------------------------------------------------
// <copyright file="RestrictCombinationsStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// Restricts the number of combinations of values for a given set of dimensions from a combinatorial strategy.
	/// </summary>
	public class RestrictCombinationsStrategy : ExplorationStrategy<Vector>
	{
		private readonly ReadOnlyCollection<Dimension> _targetDimensions;
		private readonly int _numberOfCombinationsAllowed;
		private readonly ReadOnlyCollection<IConstraint> _constraints;
        private readonly Func<int, int> _nextInt;

		/// <summary>
		/// Initializes a new instance of the <see cref="RestrictCombinationsStrategy"/> class.
		/// </summary>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="constraints">The constraints.</param>
		/// <param name="numberOfCombinationsAllowed">The number of combinations allowed.</param>
		/// <param name="targetDimensions">The target dimensions.</param>
		public RestrictCombinationsStrategy(ExplorationStrategy<Vector> wrappedStrategy, IEnumerable<IConstraint> constraints,
			int numberOfCombinationsAllowed, Func<int, int> nextInt, params Dimension[] targetDimensions)
			: base(wrappedStrategy)
		{
			_targetDimensions = new List<Dimension>(targetDimensions).AsReadOnly();
			_numberOfCombinationsAllowed = numberOfCombinationsAllowed;
			_constraints = new List<IConstraint>(constraints).AsReadOnly();
            _nextInt = nextInt;
		}

		/// <summary>
		/// Explores the input (sub-)space. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vectors/values from the (sub-)space.
		/// </returns>
		public override IEnumerable<Vector> Explore()
		{
			List<Vector> encounteredCombinations = new List<Vector>();
			foreach (Vector current in WrappedStrategies[0].Explore())
			{
				Vector currentCombination = new Vector();
				foreach (Dimension dimension in _targetDimensions)
				{
					currentCombination.SetBaseValue(dimension, current.GetBaseValue(dimension), current.GetBaseCategory(dimension));
				}
				if (!encounteredCombinations.Any(v => v.IsEquivalent(currentCombination)))
				{
					if (encounteredCombinations.Count < _numberOfCombinationsAllowed)
					{
						encounteredCombinations.Add(currentCombination);
					}
					else
					{
						// Exceeded allowed number of combinations - coerce the values for these dimensions to be one of encountered combinations
						// I try to change as few of them as possible by getting a combination that already has most of the dimension values in there.
						Vector chosenCombination = GetCombinationWithMaximalSatisfiedDimensions(current, encounteredCombinations);
						foreach (Dimension dimension in _targetDimensions)
						{
							current.SetBaseValue(dimension, chosenCombination.GetBaseValue(dimension), chosenCombination.GetBaseCategory(dimension));
						}
						if (!_constraints.All(c => c.IsValid(current)))
						{
							// Can't satisfy the constraints, just sacrifice this vector
							continue;
						}
					}
				}
				yield return current;
			}
		}

		private Vector GetCombinationWithMaximalSatisfiedDimensions(Vector toModify, IEnumerable<Vector> combinations)
		{
			List<Vector> possibleCombinations = new List<Vector>();
			int currentMax = 0;
			foreach (Vector currentCombination in combinations.Skip(1))
			{
				int currentCount = CountSatisfiedDimensions(toModify, currentCombination);
				if (currentCount > currentMax)
				{
					possibleCombinations = new List<Vector>() { currentCombination };
					currentMax = currentCount;
				}
				else if (currentCount == currentMax)
				{
					possibleCombinations.Add(currentCombination);
				}
			}
            return possibleCombinations[_nextInt(possibleCombinations.Count)];
		}

		private int CountSatisfiedDimensions(Vector toModify, Vector combination)
		{
			int ret = 0;
			foreach (Dimension dimension in _targetDimensions)
			{
				if ((toModify.GetBaseCategory(dimension) != null && combination.GetBaseCategory(dimension) != null &&
					toModify.GetBaseCategory(dimension).Equals(combination.GetBaseCategory(dimension))) ||
					Object.Equals(toModify.GetBaseValue(dimension), combination.GetBaseValue(dimension)))
				{
					ret++;
				}
			}
			return ret;
		}
	}
}
