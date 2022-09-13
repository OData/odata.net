//---------------------------------------------------------------------
// <copyright file="Category.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// <see cref="Category&lt;T&gt;.GetValue"/> is called on an empty category.
	/// </summary>
    [Serializable]
	public class NoValuesInCategoryException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NoValuesInCategoryException"/> class.
		/// </summary>
		public NoValuesInCategoryException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="NoValuesInCategoryException"/> class.
		/// </summary>
		/// <param name="category">The category.</param>
		public NoValuesInCategoryException(Category category)
			: base(string.Format(CultureInfo.InvariantCulture, "Category {0} is empty", category)) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="NoValuesInCategoryException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public NoValuesInCategoryException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="NoValuesInCategoryException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="inner">The inner.</param>
		public NoValuesInCategoryException(string message, Exception inner) : base(message, inner) { }
		
        /// <summary>
		/// Initializes a new instance of the <see cref="NoValuesInCategoryException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected NoValuesInCategoryException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// (Internal usage non-generic version) A category in a dimension is an equivalence class where values are more or less the same.
	/// </summary>
	/// <remarks>
	/// This encapsulates the non-generic portion of class Category, used for internal code
	/// when manipulating dimensions where we don't know the types.
	/// </remarks>
	public abstract class Category : IValueFactory
	{
		private string _name;

		/// <summary>
		/// Initializes a new instance of the <see cref="Category"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is null</exception>
		protected Category(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			_name = name;
		}

		/// <summary>
		/// The name of the category.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// (Internal usage non-generic version/required for VectorInfo) Gets a single (random) value from the category.
		/// </summary>
		/// <returns></returns>
		protected abstract object GetBaseValueInternal();

		public object GetBaseValue()
		{
            return GetBaseValueInternal();
		}
	}

	/// <summary>
	/// A category in a dimension is an equivalence class where values are more or less the same.
	/// </summary>
	/// <typeparam name="T">The type of values in the dimension.</typeparam>
	/// <example>For a RowCount dimension, the category "Small" could include the values [10,100].</example>
	public abstract class Category<T> : Category, IValueFactory<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Category&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="name">The name of the category.</param>
		protected Category(string name)
			: base(name)
		{
		}

		/// <summary>
		/// (Internal usage non-generic version/required for VectorInfo) Gets a single (random) value from the category.
		/// </summary>
		/// <remarks>Just calls the generic version.</remarks>
		/// <returns></returns>
		protected override object GetBaseValueInternal()
		{
			return GetValue();
		}

		/// <summary>
		/// Retrieves a single (random) value from the category.
		/// </summary>
		/// <returns>The chosen value.</returns>
		/// <exception cref="NoValuesInCategoryException">The category is empty.</exception>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
			Justification = "GetValue typically returns a different value each time, which makes it unsuitable to be a property")]
		public abstract T GetValue();

		/// <summary>
		/// Checks if the given value is within this category.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is included; otherwise, <c>false</c>.
		/// </returns>
		public abstract bool IsIncluded(T value);

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
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Name.Equals(((Category<T>)obj).Name);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}
}
