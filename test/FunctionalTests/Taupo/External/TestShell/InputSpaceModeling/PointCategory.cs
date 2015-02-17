//---------------------------------------------------------------------
// <copyright file="PointCategory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A category consisting of a set of discrete values (points) from the domain.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <example>
	/// <code><![CDATA[
	/// PointCategory<SqlDataType> integerTypes =
	///		new PointCategory<SqlDataType>("Integer", SqlDataType.TinyInt, SqlDataType.SmallInt, SqlDataType.Int, SqlDataType.BigInt);
	/// ]]></code>
	/// </example>
	public class PointCategory<T> : Category<T>
	{
		private List<T> _chosenPoints;
        private Func<int, int> _nextInt;

		/// <summary>
		/// Initializes a new instance of the <see cref="PointCategory&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="chosenPoints">The chosen points.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is null</exception>
		public PointCategory(string name, Func<int, int> nextInt, params T[] chosenPoints)
            : this(name, nextInt, (IEnumerable<T>)chosenPoints)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PointCategory&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="chosenPoints">The chosen points.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is null</exception>
		public PointCategory(string name, Func<int, int> nextInt, IEnumerable<T> chosenPoints)
			: base(name)
		{
			if (chosenPoints == null)
				throw new ArgumentNullException("chosenPoints");
            if (nextInt == null)
                throw new ArgumentNullException("nextInt");
			_chosenPoints = new List<T>(chosenPoints);
            _nextInt = nextInt;
		}

		/// <summary>
		/// The points in the category.
		/// </summary>
		public ReadOnlyCollection<T> ChosenPoints
		{
			get { return _chosenPoints.AsReadOnly(); }
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			StringBuilder ret = new StringBuilder();
			ret.AppendFormat(CultureInfo.InvariantCulture, "{0}:{{", Name);
			// Limit the number of items to display in case it's a huge list (don't want to make the debugger stop responding if it's shown there)
			int upperLimit = Math.Min(10, _chosenPoints.Count);
			for (int i = 0; i < upperLimit; i++)
			{
				if (i > 0)
					ret.Append(",");
				ret.Append(_chosenPoints[i]);
			}
			if (upperLimit != _chosenPoints.Count)
				ret.Append(",...");
			ret.Append("}");
			return ret.ToString();
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
			PointCategory<T> otherCategory = (PointCategory<T>)obj;
			if (_chosenPoints.Count != otherCategory._chosenPoints.Count)
				return false;
			foreach (T point in _chosenPoints)
				if (!otherCategory._chosenPoints.Contains(point))
					return false;
			foreach (T point in otherCategory._chosenPoints)
				if (!_chosenPoints.Contains(point))
					return false;
			return true;
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			int ret = base.GetHashCode();
			unchecked
			{
				foreach (T point in _chosenPoints)
					if (point != null)
						ret += point.GetHashCode();
			}
			return ret;
		}

		#region Category<T> Members

		/// <summary>
		/// Retrieves a single (random) value from the category.
		/// </summary>
		/// <returns>The chosen value.</returns>
		/// <exception cref="NoValuesInCategoryException">The category is empty.</exception>
		public override T GetValue()
		{
			if (_chosenPoints.Count == 0)
				throw new NoValuesInCategoryException(this);
            return _chosenPoints[_nextInt(_chosenPoints.Count)];
		}

		/// <summary>
		/// Checks if the given value is within this category.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is included; otherwise, <c>false</c>.
		/// </returns>
		public override bool IsIncluded(T value)
		{
			return _chosenPoints.Contains(value);
		}

		#endregion
	}
}
