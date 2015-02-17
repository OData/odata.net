//---------------------------------------------------------------------
// <copyright file="DimensionValueIndexCombination.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A combination of indexes that select values for each dimension.
	/// </summary>
	/// <remarks>
	/// Let's say that the test matrix is {D1, D2, D3}, and exploring each dimension
	/// (by calling CombinatorialStrategy.GetAllDimensionValues()) returns the lists of values:
	/// {S1, S2, S3} (e.g. {{a,b}, {10,20,30}, {x,y}}), then an instance of this class would be of the
	/// form {i1, i2, i3}, such that it represents the vector {S1[i1], S2[i2], S3[i3]}. For example,
	/// the instance {0,2,1} in the previous example represents {a,30,y}.
	/// </remarks>
	internal class DimensionValueIndexCombination
	{
		private readonly ReadOnlyCollection<DimensionWithValues> _dimensionValues;
		private readonly int[] _dimensionValueIndexes;

		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionValueIndexCombination"/> class.
		/// </summary>
		/// <param name="dimensionValues">The dimension values.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dimensionValues"/> is <c>null</c>.</exception>
		public DimensionValueIndexCombination(ReadOnlyCollection<DimensionWithValues> dimensionValues)
		{
			if (dimensionValues == null)
			{
				throw new ArgumentNullException("dimensionValues");
			}
			_dimensionValues = dimensionValues;
			_dimensionValueIndexes = new int[dimensionValues.Count]; // All will be zeros which is good
		}

		/// <summary>
		/// Gets or sets the index into the values of the dimension at the specified index.
		/// </summary>
		/// <value>The value index.</value>
		/// <param name="index">The index of the dimension.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Either <paramref name="index"/> is greater than or equal dimention count or <paramref name="value"/> is greater than or
		/// equal the number of values available for the dimension.
		/// </exception>
		public int this[int index]
		{
			get 
            { 
                return _dimensionValueIndexes[index]; 
            }
			set
			{
				if (value >= GetDimensionSize(index))
				{
					throw new ArgumentOutOfRangeException("value", "Set value (" + value + ") is greater than the size of the dimension: " + _dimensionValues[index].Values.Count);
				}
				_dimensionValueIndexes[index] = value;
			}
		}

		/// <summary>
		/// The size of the dimension at the given index.
		/// </summary>
		/// <param name="index">The index of the dimension.</param>
		/// <returns>The number of values available for that dimension.</returns>
		public int GetDimensionSize(int index)
		{
			return _dimensionValues[index].Values.Count;
		}

		/// <summary>
		/// Generates the vector represented by this instance.
		/// </summary>
		/// <returns>A valid test vector.</returns>
		public Vector GenerateVector()
		{
			Debug.Assert(_dimensionValueIndexes.Length == _dimensionValues.Count);
			Vector ret = new Vector();
			for (int i = 0; i < _dimensionValueIndexes.Length; i++)
			{
				ret.SetBaseValue(_dimensionValues[i].Dimension,
					_dimensionValues[i].Values[_dimensionValueIndexes[i]]);
			}
			return ret;
		}
	}
}
