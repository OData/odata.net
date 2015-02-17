//---------------------------------------------------------------------
// <copyright file="Vector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A single point in the input space.
	/// </summary>
	/// <remarks>
	/// Instance methods in this class are not thread-safe.
	/// </remarks>
	// DEVNOTE:
	// As it stands, it can also potentially be a hyperplane not just a point if only a subset of the dimensions are specified.
	// I think this can come in handy while exploring in combinatorial strategies, so we shouldn't really disallow that.
	// Maybe we can even create a new static singleton NotSpecified for this...
	public class Vector
	{
		#region Private data members
		private Dictionary<Dimension, ValueFactoryWithOptionalConcreteValue> _dimensionValues =
			new Dictionary<Dimension, ValueFactoryWithOptionalConcreteValue>();
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs an empty vector.
		/// </summary>
		public Vector()
		{
		}

		/// <summary>
		/// Constructs a vector with the given dimension values.
		/// </summary>
		/// <param name="dimensionValues">The dimension values.</param>
		public Vector(IEnumerable<DimensionValuePair> dimensionValues)
		{
			if (dimensionValues == null)
			{
				throw new ArgumentNullException("dimensionValues");
			}
			foreach (DimensionValuePair dimensionValuePair in dimensionValues)
			{
				if (_dimensionValues.ContainsKey(dimensionValuePair.BaseDimension))
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
						"Dimension {0} is specified multiple times in the given dimensionValues.",
						dimensionValuePair.BaseDimension.Name));
				}
				SetBaseValue(dimensionValuePair.BaseDimension, dimensionValuePair.BaseValue);
			}
		}

		/// <summary>
		/// Constructs a vector with the given dimension values.
		/// </summary>
		/// <param name="dimensionValues">The dimension values.</param>
		public Vector(params DimensionValuePair[] dimensionValues)
			: this((IEnumerable<DimensionValuePair>)dimensionValues)
		{
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Gets the value of the given dimension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <returns>The value of the dimension.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		/// <exception cref="KeyNotFoundException">No value was ever set for this dimension.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "I use the derived generic type for type-safety")]
		public T GetValue<T>(Dimension<T> dimension)
		{
			return (T)GetBaseValue(dimension);
		}

		/// <summary>
		/// Gets the value of the given dimension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <returns>The value of the dimension.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		/// <exception cref="KeyNotFoundException">No value was ever set for this dimension.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "I use the derived generic type for type-safety")]
		public T GetValue<T>(QualifiedDimension<T> dimension)
		{
			return (T)GetBaseValue(dimension);
		}

        /// <summary>
        /// Gets the value of the given dimension.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <returns>The value of the dimension.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
        /// <exception cref="KeyNotFoundException">No value was ever set for this dimension.</exception>
        public object GetValue(QualifiedDimension dimension)
        {
            return GetBaseValue(dimension);
        }

		/// <summary>
		/// Determines whether this vector has a value for the specified dimension.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <returns>
		/// 	<c>true</c> if the vector has a value for the dimension; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		public bool HasValue(Dimension dimension)
		{
			if (dimension == null)
				throw new ArgumentNullException("dimension");
			return _dimensionValues.ContainsKey(dimension);
		}

		/// <summary>
		/// Determines whether this vector has a value for the specified dimension.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <returns>
		/// 	<c>true</c> if the vector has a value for the dimension; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		public bool HasValue(QualifiedDimension dimension)
		{
			if (dimension == null)
				throw new ArgumentNullException("dimension");
			Vector innerVector = this;
			foreach (Matrix innerMatrix in dimension.Path)
			{
				if (!innerVector.HasValue(innerMatrix))
				{
					return false;
				}
				innerVector = innerVector.GetValue(innerMatrix);
			}
			return innerVector._dimensionValues.ContainsKey(dimension.BaseDimension);
		}

		/// <summary>
		/// Gets the category from which the value of the given dimension was chosen (can be null if it wasn't from a categorical exploration).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		/// <exception cref="KeyNotFoundException">No value was ever set for this dimension.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "I use the derived generic type for type-safety")]
		public Category<T> GetCategory<T>(Dimension<T> dimension)
		{
			return (Category<T>)GetBaseCategory(dimension);
		}

		/// <summary>
		/// Gets the category from which the value of the given dimension was chosen (can be null if it wasn't from a categorical exploration).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		/// <exception cref="KeyNotFoundException">No value was ever set for this dimension.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "I use the derived generic type for type-safety")]
		public Category<T> GetCategory<T>(QualifiedDimension<T> dimension)
		{
			return (Category<T>)GetBaseCategory(dimension);
		}

		/// <summary>
		/// Sets the value for the given dimension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="value">The value.</param>
		/// <remarks>
		/// If this overload is used, no category is associated with the given value.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "I use the derived generic type for type-safety")]
		public void SetValue<T>(Dimension<T> dimension, T value)
		{
			SetBaseValue(dimension, value);
		}

		/// <summary>
		/// Sets the value for a given dimension, associating it with a category.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="value">The value.</param>
		/// <param name="category">The category.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "I use the derived generic type for type-safety")]
		public void SetValue<T>(Dimension<T> dimension, T value, Category<T> category)
		{
			SetBaseValue(dimension, value, category);
		}

		/// <summary>
		/// Sets the value for the given dimension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="value">The value.</param>
		/// <remarks>
		/// If this overload is used, no category is associated with the given value.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "I use the derived generic type for type-safety")]
		public void SetValue<T>(QualifiedDimension<T> dimension, T value)
		{
			SetBaseValue(dimension, value);
		}

		/// <summary>
		/// Sets the value for a given dimension, associating it with a category.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="value">The value.</param>
		/// <param name="category">The category.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "I use the derived generic type for type-safety")]
		public void SetValue<T>(QualifiedDimension<T> dimension, T value, Category<T> category)
		{
			SetBaseValue(dimension, value, category);
		}

		/// <summary>
		/// Clears the dimension value.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		public void ClearDimensionValue(Dimension dimension)
		{
			_dimensionValues.Remove(dimension);
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			_dimensionValues.Clear();
		}

		/// <summary>
		/// Determines whether this vector and the specified second vector are equivalent.
		/// </summary>
		/// <param name="secondVector">The second vector.</param>
		/// <returns>
		/// 	<c>true</c> if the specified second vector is equivalent; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// 	<para>
		/// Two vectors are considered equivalent if they have the same dimensions
		/// (dimensions are the same if they refer to the same physical objects),
		/// and for each dimension, either the categories are equal or the values are equal.
		/// </para>
		/// 	<para>
		/// This method is symmetric, transitive and reflexive (an equivalence relation).
		/// </para>
		/// </remarks>
		public bool IsEquivalent(Vector secondVector)
		{
			if (secondVector == null)
				return false; // Mimic the recommended Object.Equals behavior

			if (_dimensionValues.Count != secondVector._dimensionValues.Count)
				return false;
			foreach (Dimension dimension in _dimensionValues.Keys)
			{
				if (!secondVector._dimensionValues.ContainsKey(dimension))
				{
					return false;
				}
				if (_dimensionValues[dimension].IsConcrete && secondVector._dimensionValues[dimension].IsConcrete)
				{
					Vector asInnerFirstVector, asInnerSecondVector;
					if ((asInnerFirstVector = _dimensionValues[dimension].GetConcreteValue() as Vector) != null &&
						(asInnerSecondVector = secondVector._dimensionValues[dimension].GetConcreteValue() as Vector) != null)
					{
						if (!asInnerFirstVector.IsEquivalent(asInnerSecondVector))
						{
							return false;
						}
					}
					else if (!EqualsImplementationUtils.SafeEquals(_dimensionValues[dimension].GetConcreteValue(), secondVector._dimensionValues[dimension].GetConcreteValue()) &&
						!_dimensionValues[dimension].ValueFactory.Equals(secondVector._dimensionValues[dimension].ValueFactory))
					{
						return false;
					}
				}
				else
				{
					if (!_dimensionValues[dimension].ValueFactory.Equals(secondVector._dimensionValues[dimension].ValueFactory))
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			Vector secondVector = (Vector)obj;
			if (_dimensionValues.Count != secondVector._dimensionValues.Count)
			{
				return false;
			}
			foreach (Dimension dimension in _dimensionValues.Keys)
			{
				if (!secondVector._dimensionValues.ContainsKey(dimension))
				{
					return false;
				}
				if (!_dimensionValues[dimension].Equals(secondVector._dimensionValues[dimension]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			int ret = 0;
			foreach (Dimension dimension in _dimensionValues.Keys)
			{
				unchecked
				{
					ret += dimension.GetHashCode();
					ret += _dimensionValues[dimension].GetHashCode();
				}
			}
			return ret;
		}

		/// <summary>
		/// Determines whether the specified set contains dupe of the given vector.
		/// </summary>
		/// <param name="set">The set to check.</param>
		/// <param name="vector">The vector to check.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <parmref name="set"/> contains dupe of <paramref name="vector"/>; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsDupe(IEnumerable<Vector> set, Vector vector)
		{
			if (set == null)
			{
				throw new ArgumentNullException("set");
			}
			if (vector == null)
			{
				throw new ArgumentNullException("vector");
			}
			foreach (Vector currentVector in set)
			{
				if (currentVector.IsEquivalent(vector))
				{
					Debug.Assert(vector.IsEquivalent(currentVector));
					return true;
				}
				else
				{
					Debug.Assert(!vector.IsEquivalent(currentVector));
				}
			}
			return false;
		}

		/// <summary>
		/// Returns the UNION (with duplicate removal) of two vectors,
		/// where two values are considered equal if they are in the same category or if they are identical.
		/// </summary>
		/// <param name="setOne">The set one.</param>
		/// <param name="setTwo">The set two.</param>
		/// <returns></returns>
		/// <remarks>
		/// 	<para>This method uses <see cref="Vector.IsEquivalent"/> to determine equivalence.</para>
		/// 	<para>If <paramref name="setOne"/> originally contains any duplicates, this method won't remove them.</para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="setOne"/> or <paramref name="setTwo"/> is null</exception>
		public static IEnumerable<Vector> Union(IEnumerable<Vector> setOne, IEnumerable<Vector> setTwo)
		{
			if (setOne == null)
				throw new ArgumentNullException("setOne");
			if (setTwo == null)
				throw new ArgumentNullException("setTwo");
			// Given the complex semantics of IsEquivalent, there is no escaping an O(n^2) nested-loop-join algorithm
			// Well, you could theoretically create a hashing function that would work with IsEquivalent semantics,
			// but in the absense of strong performance requirements I'd rather go with the simpler nested-loop solution for now.

			// First, store one of them in a list to act as the inner side of the join
			List<Vector> setOneSpool = new List<Vector>(setOne);
			// Return the entire first set
			foreach (Vector firstVector in setOneSpool)
				yield return firstVector;
			// Then loop over the second one 
			foreach (Vector secondVector in setTwo)
			{
				if (!ContainsDupe(setOneSpool, secondVector))
					yield return secondVector;
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			StringBuilder ret = new StringBuilder("{");
			bool putComma = false;
			foreach (KeyValuePair<Dimension, ValueFactoryWithOptionalConcreteValue> dimensionValue in _dimensionValues)
			{
				if (putComma)
				{
					ret.Append(",");
				}
				putComma = true;
				Category asCategory = dimensionValue.Value.ValueFactory as Category;
				if (!dimensionValue.Value.IsConcrete)
				{
					ret.AppendFormat("({0},{1})", dimensionValue.Key.Name, dimensionValue.Value.ValueFactory);
				}
				else if (asCategory != null) // Only interested in categories to appear in the ToString of the vector
				{
					ret.AppendFormat("({0},{1},{2})", dimensionValue.Key.Name, asCategory.Name, dimensionValue.Value.GetConcreteValue());
				}
				else
				{
					ret.AppendFormat("({0},{1})", dimensionValue.Key.Name, dimensionValue.Value.GetConcreteValue());
				}
			}
			ret.Append("}");
			return ret.ToString();
		}

		/// <summary>
		/// Gets the string representation for the value of the given dimension.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <returns></returns>
		public string GetValueString(Dimension dimension)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}", GetBaseValue(dimension));
		}
		#endregion

		#region Internal methods
		/// <summary>
		/// (Internal use) Gets the value of the given dimension in a non-generic way.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		/// <exception cref="KeyNotFoundException">No value was ever set for this dimension.</exception>
		internal object GetBaseValue(Dimension dimension)
		{
			lock (_dimensionValues)
			{
				if (!_dimensionValues[dimension].IsConcrete)
				{
					_dimensionValues[dimension] = _dimensionValues[dimension].MakeConcrete();
				}
				return _dimensionValues[dimension].GetConcreteValue();
			}
		}

		/// <summary>
		/// (Internal use) Gets the value of the given dimension in a non-generic way.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		/// <exception cref="KeyNotFoundException">No value was ever set for this dimension.</exception>
		internal object GetBaseValue(QualifiedDimension dimension)
		{
			if (dimension.Path.Any())
			{
				return GetValue(dimension.Path[0]).GetBaseValue(QualifiedDimension.Create(dimension.BaseDimension, dimension.Path.Skip(1)));
			}
			else
			{
				return GetBaseValue(dimension.BaseDimension);
			}
		}

		/// <summary>
		/// (Internal use) Gets the category from which the value of the given dimension was chosen (can be null if it wasn't from a categorical exploration).
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		internal Category GetBaseCategory(Dimension dimension)
		{
			if (dimension == null)
				throw new ArgumentNullException("dimension");
			if (!HasValue(dimension))
				throw new KeyNotFoundException(string.Format(CultureInfo.InvariantCulture, "Vector {0} has no value for dimension {1}", this, dimension));
			return _dimensionValues[dimension].ValueFactory as Category;
		}

		/// <summary>
		/// (Internal use) Gets the category from which the value of the given dimension was chosen (can be null if it wasn't from a categorical exploration).
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		internal Category GetBaseCategory(QualifiedDimension dimension)
		{
			if (dimension == null)
				throw new ArgumentNullException("dimension");
			if (dimension.Path.Any())
			{
				return GetValue(dimension.Path[0]).GetBaseCategory(QualifiedDimension.Create(dimension.BaseDimension, dimension.Path.Skip(1)));
			}
			else
			{
				return GetBaseCategory(dimension.BaseDimension);
			}
		}

		/// <summary>
		/// (Internal use/required for VectorInfo) Sets the value for the given dimension.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		internal void SetBaseValue(QualifiedDimension dimension, object value)
		{
			SetBaseValue(dimension, new ValueFactoryWithOptionalConcreteValue(new SingleValueFactory(value), value));
		}

		/// <summary>
		/// (Internal use/required for VectorInfo) Sets the value for the given dimension.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		internal void SetBaseValue(QualifiedDimension dimension, ValueFactoryWithOptionalConcreteValue value)
		{
			if (dimension.Path.Any())
			{
				Vector innerVector;
				ValueFactoryWithOptionalConcreteValue valueFactory;
				if (_dimensionValues.TryGetValue(dimension.Path[0], out valueFactory))
				{
					innerVector = (Vector)valueFactory.MakeConcrete().GetConcreteValue();
				}
				else
				{
					innerVector = new Vector();
					SetBaseValue(dimension.Path[0], innerVector);
				}
				innerVector.SetBaseValue(QualifiedDimension.Create(dimension.BaseDimension, dimension.Path.Skip(1)), value);
			}
			else
			{
				_dimensionValues[dimension.BaseDimension] = value;
			}
		}

		/// <summary>
		/// (Internal use/required for VectorInfo) Sets the value for the given dimension, associating it with a category.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <param name="value">The value.</param>
		/// <param name="category">The category. Can be null.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		internal void SetBaseValue(QualifiedDimension dimension, object value, Category category)
		{
			SetBaseValue(dimension, new ValueFactoryWithOptionalConcreteValue(
				category == null ? new SingleValueFactory(value) : (IValueFactory)category,
				value));
		}

		/// <summary>
		/// Gets the base value factory.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null</exception>
		/// <exception cref="KeyNotFoundException">No value was ever set for this dimension.</exception>
		internal ValueFactoryWithOptionalConcreteValue GetValueFactoryWithOptionalConcreteValue(Dimension dimension)
		{
			return _dimensionValues[dimension];
		}

        /////// <summary>
        /////// Gets the dimensions for which this vector has values.
        /////// </summary>
        /////// <value>The dimensions.</value>
        ////internal IEnumerable<Dimension> Dimensions
        ////{
        ////    get
        ////    {
        ////        // I need to put it in a list first because in GetBaseValue(), I can modify the dictionary by
        ////        // making values concrete. So if anyone is enumerating over Dimensions and getting the value
        ////        // for each dimension (pretty valid scenario), they'll get an inaccurate exception saying the collection
        ////        // was modified. Technically, it wasn't modified since in GetBaseValue() I never touch the keys
        ////        // (only the values), but apparently Dictionary isn't smart enough to figure this out.
        ////        return new List<Dimension>(_dimensionValues.Keys);
        ////    }
        ////}
		#endregion
	}
}
