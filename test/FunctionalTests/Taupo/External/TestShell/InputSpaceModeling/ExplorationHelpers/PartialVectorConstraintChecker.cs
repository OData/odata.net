//---------------------------------------------------------------------
// <copyright file="PartialVectorConstraintChecker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// Manages constraints for pairwise exploration so that we can partially check constraints as we're assigning values to dimensions.
	/// </summary>
	internal class PartialVectorConstraintChecker
	{
		private readonly ReadOnlyCollection<DimensionWithValues> _dimensionValues;
		private readonly ReadOnlyCollection<PossiblyExtendedConstraint> _extendedConstraints;
		private const int _maxDimensionCountToExtend = 3;

		/// <summary>
		/// Constructs a partial constraint checker for the given 
		/// </summary>
		/// <param name="dimensionValues"></param>
		/// <param name="constraints"></param>
		public PartialVectorConstraintChecker(ReadOnlyCollection<DimensionWithValues> dimensionValues, IEnumerable<IConstraint> constraints)
		{
			_dimensionValues = dimensionValues;
			List<PossiblyExtendedConstraint> extendedConstraints = new List<PossiblyExtendedConstraint>();
			foreach (IConstraint constraint in constraints)
			{
				PossiblyExtendedConstraint extendedConstraint;
				if (constraint.RequiredDimensions.SelectMany(d => ExpandOuterMatrixDimensionIfNeeded(d, dimensionValues)).Count() > _maxDimensionCountToExtend)
				{
					extendedConstraint = new NonExtendedConstraint(GetDimensionIndexes(constraint, dimensionValues), constraint, dimensionValues);
				}
				else
				{
					extendedConstraint = ExtendConstraint(constraint, dimensionValues);
				}
				extendedConstraints.Add(extendedConstraint);
			}
			// Possible optimization: Discover implied constraints among the actually extended constraints and add them.
			_extendedConstraints = extendedConstraints.AsReadOnly();
		}

		/// <summary>
		/// Gets the allowed values for a given target dimension, given the values that have been set so far for other dimensions.
		/// </summary>
		/// <param name="targetDimension">The index of the target dimension (index is relative to the dimensionValues collection passed in at construction).</param>
		/// <param name="setDimensionIndexes">The indexes of the dimensions where we have chosen values.</param>
		/// <param name="currentCombination">The chosen values for the dimensions.</param>
		/// <returns>A list of indexes refering to values from the dimensionValues collection passed in at construction that are valid according to constraints.</returns>
		public ReadOnlyCollection<int> GetAllowedValueIndexesForDimension(int targetDimension, IEnumerable<int> setDimensionIndexes,
			DimensionValueIndexCombination currentCombination)
		{
			List<int> ret = new List<int>(Enumerable.Range(0, _dimensionValues[targetDimension].Values.Count));
			foreach (PossiblyExtendedConstraint constraint in _extendedConstraints.Where(c => c.IsRelevantConstraint(targetDimension, setDimensionIndexes)))
			{
				ret.RemoveAll(vi => !constraint.IsValid(currentCombination, targetDimension, vi));
			}
			return ret.AsReadOnly();
		}

		/// <summary>
		/// Checks if the vector so far is valid according to constraints.
		/// </summary>
		/// <param name="combination">The combination with the values chosen so far.</param>
		/// <param name="setDimensionIndexes">The indexes of the dimensions where we have chosen values.</param>
		/// <returns><c>true</c></returns>
		public bool IsValidForApplicableConstraints(DimensionValueIndexCombination combination, IEnumerable<int> setDimensionIndexes)
		{
			return _extendedConstraints.Where(c => c.IsRelevantConstraint(setDimensionIndexes)).All(c => c.IsValid(combination));
		}

		private static int GetIndexOfConstraintDimension(IConstraint constraint, ReadOnlyCollection<DimensionWithValues> dimensionValues,
			QualifiedDimension dimension)
		{
			int ret = 0;
			foreach (DimensionWithValues dv in dimensionValues)
			{
				if (IsSubset(dimension, dv.Dimension))
				{
					return ret;
				}
				ret++;
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
				"Dimension {0} from constraint {1} was not found in the matrix (all dimensions: {2}).",
				dimension.FullyQualifiedName, constraint.GetType().Name, string.Join(",", dimensionValues.Select(dv => dv.Dimension.FullyQualifiedName))));
		}

		private static ExtendedConstraint ExtendConstraint(IConstraint constraint, ReadOnlyCollection<DimensionWithValues> dimensionValues)
		{
			List<int> dimensionIndexes = GetDimensionIndexes(constraint, dimensionValues).ToList();
			if (dimensionIndexes.Count == 0)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
					"Constraint {0} doesn't have any required dimensions.", constraint.GetType().Name));
			}
			ReadOnlyCollection<DimensionWithValues> dimensionValuesSubset = dimensionIndexes.Select(di => dimensionValues[di]).ToList().AsReadOnly();
			ExtendedConstraint ret = new ExtendedConstraint(dimensionIndexes, dimensionValues);
			foreach (ReadOnlyCollection<int> indexes in CombinatorialUtilities.GetAllIndexCombinations(dimensionValuesSubset.Select(dv => dv.Values.Count)))
			{
				Vector v = CombinatorialUtilities.ConstructVector(dimensionValuesSubset, indexes);
				if (constraint.IsValid(v))
				{
					ret.AddNewValueCombinations(indexes);
				}
			}
			return ret;
		}

		private static bool IsSubset(QualifiedDimension superset, QualifiedDimension subset)
		{
			return superset.Path.Count >= subset.Path.Count &&
				superset.FullPath.Take(subset.FullPath.Count()).SequenceEqual(subset.FullPath);
		}

		private static IEnumerable<QualifiedDimension> ExpandOuterMatrixDimensionIfNeeded(QualifiedDimension toExpand, ReadOnlyCollection<DimensionWithValues> dimensionValues)
		{
			IEnumerable<QualifiedDimension> ret = dimensionValues.Select(dv => dv.Dimension).Where(d => IsSubset(d, toExpand));
			if (toExpand.BaseDimension is Matrix && ret.Any())
			{
				return ret;
			}
			else
			{
				return Enumerable.Repeat(toExpand, 1);
			}
		}

		private static IEnumerable<int> GetDimensionIndexes(IConstraint constraint, ReadOnlyCollection<DimensionWithValues> dimensionValues)
		{
			return constraint.RequiredDimensions
				.SelectMany(d => ExpandOuterMatrixDimensionIfNeeded(d, dimensionValues))
				.Select(d => GetIndexOfConstraintDimension(constraint, dimensionValues, d))
				.Distinct();
		}

		private abstract class PossiblyExtendedConstraint
		{
			protected readonly ReadOnlyCollection<int> _dimensionIndexes;

			protected PossiblyExtendedConstraint(IEnumerable<int> dimensionIndexes)
			{
				_dimensionIndexes = new List<int>(dimensionIndexes).AsReadOnly();
			}

			public bool IsRelevantConstraint(int targetDimensionIndex, IEnumerable<int> remainingDimensionIndexes)
			{
				return _dimensionIndexes.Contains(targetDimensionIndex) &&
					_dimensionIndexes.Intersect(remainingDimensionIndexes).Count() == _dimensionIndexes.Count - 1;
			}

			public bool IsRelevantConstraint(IEnumerable<int> dimensionIndexesSetSoFar)
			{
				return _dimensionIndexes.Intersect(dimensionIndexesSetSoFar).Count() == _dimensionIndexes.Count;
			}

			private int GetValueIndex(int dimensionIndexIndex, DimensionValueIndexCombination combination, int targetDimensionIndex, int targetDimensionIndexValue)
			{
				return _dimensionIndexes[dimensionIndexIndex] == targetDimensionIndex ? targetDimensionIndexValue : combination[_dimensionIndexes[dimensionIndexIndex]];
			}

			private IEnumerable<int> GetValueIndexes(DimensionValueIndexCombination combination, int targetDimensionIndex, int targetDimensionIndexValue)
			{
				return Enumerable.Range(0, _dimensionIndexes.Count).Select(i => GetValueIndex(i, combination, targetDimensionIndex, targetDimensionIndexValue));
			}

			private IEnumerable<int> GetValueIndexes(DimensionValueIndexCombination combination)
			{
				return _dimensionIndexes.Select(d => combination[d]);
			}

			public bool IsValid(DimensionValueIndexCombination combination, int targetDimensionIndex, int targetDimensionIndexValue)
			{
				return IsValid(GetValueIndexes(combination, targetDimensionIndex, targetDimensionIndexValue));
			}

			public bool IsValid(DimensionValueIndexCombination combination)
			{
				return IsValid(GetValueIndexes(combination));
			}

			protected abstract bool IsValid(IEnumerable<int> combination);
		}

		private class NonExtendedConstraint : PossiblyExtendedConstraint
		{
			private readonly IConstraint _constraint;
			private readonly ReadOnlyCollection<DimensionWithValues> _dimensionValuesSubset;

			public NonExtendedConstraint(IEnumerable<int> dimensionIndexes, IConstraint constraint, ReadOnlyCollection<DimensionWithValues> dimensionValues)
				: base(dimensionIndexes)
			{
				_constraint = constraint;
				_dimensionValuesSubset = _dimensionIndexes.Select(di => dimensionValues[di]).ToList().AsReadOnly();
			}

			protected override bool IsValid(IEnumerable<int> combination)
			{
				Vector v = CombinatorialUtilities.ConstructVector(_dimensionValuesSubset, combination.ToList().AsReadOnly());
				return _constraint.IsValid(v);
			}
		}

		private class ExtendedConstraint : PossiblyExtendedConstraint
		{
			private readonly ReadOnlyCollection<int> _dimensionSizeMultipliers;
			private readonly HashSet<int> _combinations = new HashSet<int>();

			public ExtendedConstraint(IEnumerable<int> dimensionIndexes, ReadOnlyCollection<DimensionWithValues> dimensionValues)
				: base(dimensionIndexes)
			{
				List<int> dimensionSizeMultipliers = new List<int>(_dimensionIndexes.Count);
				int currentProduct = 1, i = _dimensionIndexes.Count - 1;
				do
				{
					dimensionSizeMultipliers.Insert(0, currentProduct);
					currentProduct *= dimensionValues[_dimensionIndexes[i--]].Values.Count;
				} while (i >= 0);
				_dimensionSizeMultipliers = dimensionSizeMultipliers.AsReadOnly();
			}

			private int Encode(IEnumerable<int> combination)
			{
				return _dimensionSizeMultipliers.Zip(combination, (a, b) => a * b).Sum();
			}

			protected override bool IsValid(IEnumerable<int> combination)
			{
				return _combinations.Contains(Encode(combination));
			}

			public void AddNewValueCombinations(IEnumerable<int> valueCombination)
			{
				_combinations.Add(Encode(valueCombination));
			}
		}
	}
}
