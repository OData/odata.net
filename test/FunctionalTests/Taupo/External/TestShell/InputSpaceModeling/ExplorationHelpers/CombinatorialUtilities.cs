//---------------------------------------------------------------------
// <copyright file="CombinatorialUtilities.cs" company="Microsoft">
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
	/// Utility methods for combinatorial exploraiton.
	/// </summary>
	internal static class CombinatorialUtilities
	{
		/// <summary>
		/// Enumerates all index combinations in lexicographical order.
		/// </summary>
		/// <param name="sizes">The sizes of the dimensions to enumerate through.</param>
		/// <returns>All the combinations in lexicographcial order.</returns>
		/// <remarks>
		/// Given sizes {2,3,2}, this method would return:
		/// {0,0,0}
		/// {0,0,1}
		/// {0,1,0}
		/// {0,1,1}
		/// {0,2,0}
		/// {0,2,1}
		/// {1,0,0}
		/// {1,0,1}
		/// {1,1,0}
		/// {1,1,1}
		/// {1,2,0}
		/// {1,2,1}
		/// </remarks>
		internal static IEnumerable<ReadOnlyCollection<int>> GetAllIndexCombinations(IEnumerable<int> sizes)
		{
			// The algorithm iterates through all the combinations in lexicographical order
			// DEVNOTE: I'm having a hard time explaining the algorithm, I suggest stepping through the unit test
			// ExhaustiveCombinatorialStrategyTest.ExploreTest() to grasp it.

			// I can't use the Stack class because I need to peek at all the values,
			// but it really is a stack in terms of pushing and popping operations
			List<int> sizesList = sizes.ToList();
			List<int> workingStack = new List<int>(sizesList.Count);
			workingStack.Add(0);
			do
			{
				List<int> currentVector = new List<int>(sizesList.Count);
				for (int counter = 0; counter < workingStack.Count; counter++)
				{
					if (sizesList[counter] <= workingStack[counter])
					{
						// We exhausted all combinations for the dimensions at indexes [counter, sizesList.Count[
						// Pop out all the values above the current one from the stack
						while (workingStack.Count > counter)
						{
							workingStack.RemoveAt(workingStack.Count - 1);
						}
						if (counter == 0)
						{
							break; // We exhausted the values for the first dimension, we're done.
						}
						// Increment the top of the stack
						workingStack[counter - 1]++;
						// Start a fresh vector and reset the loop
						currentVector = new List<int>(sizesList.Count);
						counter = -1;
					}
					else
					{
						currentVector.Add(workingStack[counter]);
					}
				}
				if (workingStack.Count == 0)
				{
					break; // That's it, we're done
				}
				// Reset the rest of the dimensions
				for (int counter = workingStack.Count; counter < sizesList.Count; counter++)
				{
					if (sizesList[counter] == 0)
					{
						// One of the dimensions didn't yield any values, we can't return any valid vectors.
						currentVector = null;
						break;
					}
					currentVector.Add(0);
					workingStack.Add(0);
				}
				if (currentVector != null)
				{
					yield return currentVector.AsReadOnly();
				}
				workingStack[workingStack.Count - 1]++;
			} while (true);
		}

		/// <summary>
		/// Enumerates all the possible different vectors for the given dimension values.
		/// </summary>
		/// <param name="dimensionValues">The dimension values.</param>
		/// <returns>All the possible different vectors.</returns>
		internal static IEnumerable<Vector> EnumerateAllVectors(ReadOnlyCollection<DimensionWithValues> dimensionValues)
		{
			return GetAllIndexCombinations(dimensionValues.Select(dv => dv.Values.Count))
				.Select(indexes => ConstructVector(dimensionValues, indexes));
		}

		/// <summary>
		/// Constructs a vector given the indexes of the values and the list of all the values for each dimension.
		/// </summary>
		/// <param name="dimensionValues">All the values for the dimensions.</param>
		/// <param name="indexes">The indexes of the wanted values.</param>
		/// <returns>The wanted vector.</returns>
		/// <exception cref="ArgumentException">Thrown if the number of indexes given doesn't match the number of dimensions.</exception>
		internal static Vector ConstructVector(ReadOnlyCollection<DimensionWithValues> dimensionValues, ReadOnlyCollection<int> indexes)
		{
			if (dimensionValues.Count != indexes.Count)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
					"Dimension count {0} does not match indexes count {1}", dimensionValues.Count, indexes.Count));
			}
			Vector ret = new Vector();
			for (int i = 0; i < dimensionValues.Count; i++)
			{
				ret.SetBaseValue(dimensionValues[i].Dimension, dimensionValues[i].Values[indexes[i]]);
			}
			return ret;
		}
	}
}
