//---------------------------------------------------------------------
// <copyright file="RangeCategory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Globalization;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A category consisting of a continuous range of values: the semi-open interval [lowerLimit, upperLimit[.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RangeCategory<T> : Category<T> where T : IComparable<T>
	{
		private T _lowerLimit;
		private T _upperLimit;
        private Func<T, T, T> _nextValueInRange;

		/// <summary>
		/// Inclusive lower limit.
		/// </summary>
		public T LowerLimit
		{
			get { return _lowerLimit; }
		}

		/// <summary>
		/// Exclusive upper limit.
		/// </summary>
		public T UpperLimit
		{
			get { return _upperLimit; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeCategory&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="name">The category name.</param>
		/// <param name="lowerLimit">The inclusive lower limit.</param>
		/// <param name="upperLimit">The exclusive upper limit.</param>
		/// <remarks>
		/// If <paramref name="lowerLimit"/> is greater than or equal to <paramref name="upperLimit"/>,
		/// then the category will be empty.
		/// </remarks>
		/// <exception cref="ArgumentNullException">Any of the arguments is null</exception>
		public RangeCategory(string name, T lowerLimit, T upperLimit, Func<T, T, T> nextValueInRange)
			: base(name)
		{
			if (lowerLimit == null)
				throw new ArgumentNullException("lowerLimit");
			if (upperLimit == null)
				throw new ArgumentNullException("upperLimit");
            if (_nextValueInRange == null)
                throw new ArgumentNullException("nextValueInRange");
			_lowerLimit = lowerLimit;
			_upperLimit = upperLimit;
            _nextValueInRange = nextValueInRange;
		}

        /// <summary>
        /// Retrieves a single (random) value from the category.
        /// </summary>
        /// <returns>The chosen value.</returns>
        /// <exception cref="NoValuesInCategoryException">The category is empty.</exception>
        public override T GetValue()
        {
            if (UpperLimit.CompareTo(LowerLimit) < 0)
                throw new NoValuesInCategoryException(this);

            return _nextValueInRange(LowerLimit, UpperLimit);
        }

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1} - {2}", Name, LowerLimit, UpperLimit);
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
			RangeCategory<T> otherRange = (RangeCategory<T>)obj;
			return LowerLimit.Equals(otherRange.LowerLimit) && UpperLimit.Equals(otherRange.UpperLimit);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return unchecked(base.GetHashCode() + LowerLimit.GetHashCode() + UpperLimit.GetHashCode());
		}

		#region Category<T> Members

		/// <summary>
		/// Checks if the given value is within this category.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is included; otherwise, <c>false</c>.
		/// </returns>
		public override bool IsIncluded(T value)
		{
			return value.CompareTo(_lowerLimit) >= 0 &&
				value.CompareTo(_upperLimit) < 0;
		}

		#endregion
	}
}
