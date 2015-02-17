//---------------------------------------------------------------------
// <copyright file="NullableRangeCategory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A helper class to create a <see cref="RangeCategory&lt;T&gt;"/> for nullable types.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class NullableRangeCategory<T> : Category<T?> where T : struct, IComparable<T>
	{
		private RangeCategory<T> _originCategory;

		/// <summary>
		/// Initializes a new instance of the <see cref="NullableRangeCategory&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="originCategory">The origin category.</param>
		public NullableRangeCategory(RangeCategory<T> originCategory)
			: base(originCategory.Name)
		{
			_originCategory = originCategory;
		}

		/// <summary>
		/// Inclusive lower limit.
		/// </summary>
		public T LowerLimit
		{
			get { return _originCategory.LowerLimit; }
		}

		/// <summary>
		/// Exclusive upper limit.
		/// </summary>
		public T UpperLimit
		{
			get { return _originCategory.UpperLimit; }
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return "(Nullable)" + _originCategory.ToString();
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
				return false;
			NullableRangeCategory<T> otherRange = (NullableRangeCategory<T>)obj;
			return _originCategory.Equals(otherRange._originCategory);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return unchecked(base.GetHashCode() + _originCategory.GetHashCode());
		}

		/// <summary>
		/// Retrieves a single (random) value from the category.
		/// </summary>
		/// <returns>The chosen value.</returns>
		/// <exception cref="NoValuesInCategoryException">The category is empty.</exception>
		public override T? GetValue()
		{
			return _originCategory.GetValue();
		}

		/// <summary>
		/// Checks if the given value is within this category.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		///    <c>true</c> if the specified value is included; otherwise, <c>false</c>.
		/// </returns>
		public override bool IsIncluded(T? value)
		{
			return value.HasValue && _originCategory.IsIncluded(value.Value);
		}
	}
}