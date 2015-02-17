//---------------------------------------------------------------------
// <copyright file="ValueFactoryWithOptionalConcreteValue.cs" company="Microsoft">
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
	/// A value factory with an optional concrete value from that factory.
	/// </summary>
	public class ValueFactoryWithOptionalConcreteValue
	{
		private readonly IValueFactory _valueFactory;
		private readonly object _value;
		private readonly bool _isConcrete;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueFactoryWithOptionalConcreteValue"/> class.
		/// </summary>
		/// <param name="valueFactory">The value factory.</param>
		public ValueFactoryWithOptionalConcreteValue(IValueFactory valueFactory)
		{
			if (valueFactory == null)
			{
				throw new ArgumentNullException("valueFactory");
			}
			_valueFactory = valueFactory;
			SingleValueFactory asSingleValueFactory = valueFactory as SingleValueFactory;
			if (asSingleValueFactory != null)
			{
				// If it's a single value, it's concrete.
				_value = asSingleValueFactory.GetBaseValue();
				_isConcrete = true;
			}
			else
			{
				_isConcrete = false;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueFactoryWithOptionalConcreteValue"/> class.
		/// </summary>
		/// <param name="valueFactory">The value factory.</param>
		/// <param name="value">The value.</param>
		internal ValueFactoryWithOptionalConcreteValue(IValueFactory valueFactory, object value)
		{
			if (valueFactory == null)
			{
				throw new ArgumentNullException("valueFactory");
			}
			_valueFactory = valueFactory;
			_value = value;
			_isConcrete = true;
		}

		/// <summary>
		/// Makes this instance concrete.
		/// </summary>
		/// <returns>The concrete value generated.</returns>
		public ValueFactoryWithOptionalConcreteValue MakeConcrete()
		{
			if (_isConcrete)
			{
				return this;
			}
			else
			{
				return new ValueFactoryWithOptionalConcreteValue(_valueFactory, _valueFactory.GetBaseValue());
			}
		}

		/// <summary>
		/// Gets the value factory.
		/// </summary>
		/// <value>The value factory.</value>
		public IValueFactory ValueFactory
		{
			get { return _valueFactory; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is concrete.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is concrete; otherwise, <c>false</c>.
		/// </value>
		public bool IsConcrete
		{
			get { return _isConcrete; }
		}

		/// <summary>
		/// Gets the concrete value.
		/// </summary>
		/// <returns>The concrete value.</returns>
		/// <exception cref="InvalidOperationException"><see cref="IsConcrete"/> is false.</exception>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public object GetConcreteValue()
		{
			if (_isConcrete)
			{
				return _value;
			}
			else
			{
				throw new InvalidOperationException("I need to be concrete");
			}
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
			ValueFactoryWithOptionalConcreteValue asMyType = obj as ValueFactoryWithOptionalConcreteValue;
			return asMyType != null &&
				_valueFactory.Equals(asMyType._valueFactory) &&
				_isConcrete == asMyType._isConcrete &&
				(_isConcrete ? EqualsImplementationUtils.SafeEquals(_value, asMyType._value) : true);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return unchecked(_isConcrete.GetHashCode() + _valueFactory.GetHashCode() + EqualsImplementationUtils.GetSafeHashValue(_value));
		}
	}
}
