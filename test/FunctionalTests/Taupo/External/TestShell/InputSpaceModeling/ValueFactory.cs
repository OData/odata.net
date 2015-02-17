//---------------------------------------------------------------------
// <copyright file="ValueFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// (Internal usage) Base interface for a non-generic value factory.
	/// </summary>
	public interface IValueFactory
	{
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		object GetBaseValue();
	}

	/// <summary>
	/// A value factory that just contains a single value and returns it every time.
	/// </summary>
	public class SingleValueFactory : IValueFactory
	{
		private readonly object _theValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="SingleValueFactory"/> class.
		/// </summary>
		/// <param name="theValue">The value.</param>
		public SingleValueFactory(object theValue)
		{
			_theValue = theValue;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <returns>The value.</returns>
		public object GetBaseValue()
		{
			return _theValue;
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj"/> parameter is null.
		/// </exception>
		public override bool Equals(object obj)
		{
			SingleValueFactory asSingleValueFactory = obj as SingleValueFactory;
			return asSingleValueFactory != null &&
				EqualsImplementationUtils.SafeEquals(_theValue, asSingleValueFactory._theValue);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return EqualsImplementationUtils.GetSafeHashValue(_theValue);
		}
	}
	/// <summary>
	/// A value factory.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IValueFactory<T> : IValueFactory
	{
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <returns>The value.</returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		T GetValue();
	}

	/// <summary>
	/// A value factory that just contains a single value and returns it every time.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SingleValueFactory<T> : SingleValueFactory, IValueFactory<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SingleValueFactory&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="theValue">The value.</param>
		public SingleValueFactory(T theValue)
			: base(theValue)
		{
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <returns>The value.</returns>
		public T GetValue()
		{
			return (T)GetBaseValue();
		}
	}
}
