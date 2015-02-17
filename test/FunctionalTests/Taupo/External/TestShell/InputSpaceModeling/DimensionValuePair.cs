//---------------------------------------------------------------------
// <copyright file="DimensionValuePair.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A dimension with its value.
	/// </summary>
	public abstract class DimensionValuePair
	{
		private readonly Dimension _dimension;
		private readonly object _value;

		/// <summary>
		/// Constructs a dimension-value pair.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <param name="value">The value.</param>
		protected internal DimensionValuePair(Dimension dimension, object value)
		{
			if (dimension == null)
			{
				throw new ArgumentNullException("dimension");
			}
			_dimension = dimension;
			_value = value;
		}

		/// <summary>
		/// Gets the dimension as the untyped object.
		/// </summary>
		internal Dimension BaseDimension
		{
			get { return _dimension; }
		}

		/// <summary>
		/// Gets the value as the untyped object.
		/// </summary>
		internal object BaseValue
		{
			get { return _value; }
		}
	}

	/// <summary>
	/// A dimension with its value.
	/// </summary>
	/// <typeparam name="T">THe type of the value.</typeparam>
	public class DimensionValuePair<T> : DimensionValuePair
	{
		/// <summary>
		/// Constructs a dimension-value pair.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <param name="value">The value.</param>
		internal DimensionValuePair(Dimension<T> dimension, T value)
			: base(dimension, value)
		{
		}

		/// <summary>
		/// Gets the dimension.
		/// </summary>
		public Dimension<T> Dimension
		{
			get { return (Dimension<T>)BaseDimension; }
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		public T Value
		{
			get { return (T)BaseValue; }
		}
	}
}
