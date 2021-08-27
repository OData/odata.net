//---------------------------------------------------------------------
// <copyright file="ParseableCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
    /// <summary>
	/// A type converter for <see cref="ParseableCollection&lt;T&gt;"/>.
	/// </summary>
	internal class ParseableCollectionTypeConverter : TypeConverterForGenericTypes
	{
		private const string _emptyString = "([empty])";

		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
		/// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
		/// <returns>
		/// An <see cref="T:System.Object"/> that represents the converted value.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">
		/// The conversion cannot be performed.
		/// </exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string valueAsString = value as string;
			if (valueAsString != null && valueAsString.StartsWith("(", StringComparison.OrdinalIgnoreCase) && valueAsString.EndsWith(")", StringComparison.OrdinalIgnoreCase))
			{
				if (valueAsString.Equals(_emptyString, StringComparison.OrdinalIgnoreCase))
				{
					return TargetType.GetConstructor(Type.EmptyTypes).Invoke(null);
				}
				return TargetType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single().Invoke(new object[] {
					valueAsString.Substring(1, valueAsString.Length - 2).Split(',').Select(
					s => TypeConversion.FromString(TargetType.GetGenericArguments()[0], s))
				});
			}
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
		/// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
		/// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
		/// <returns>
		/// An <see cref="T:System.Object"/> that represents the converted value.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="destinationType"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// The conversion cannot be performed.
		/// </exception>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			System.Collections.ICollection asCollection;
			if (destinationType == typeof(string) && ((asCollection = value as System.Collections.ICollection) != null))
			{
				if (asCollection.Count == 0)
				{
					return _emptyString;
				}
				return "(" + string.Join(",", asCollection.Cast<object>().Select(v => TypeConversion.ToString(v)).ToArray()) + ")";
			}
			else
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}

	/// <summary>
	/// A collection that can go to/from strings easily, allowing it to be used in ISM with nice vector representations and ability to edit in Pompeii.
	/// </summary>
	/// <typeparam name="T">Type of values in the collection.</typeparam>
	/// <remarks>
	/// Caveat: As of now, this doesn't support values that have the character ',' in them. To do that, we need to support escaping it, which is
	/// still undone.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parseable")]
	[TypeConverter(typeof(ParseableCollectionTypeConverter))]
	public class ParseableCollection<T> : Collection<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ParseableCollection&lt;T&gt;"/> class.
		/// </summary>
		public ParseableCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParseableCollection&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="values">The values.</param>
		public ParseableCollection(IEnumerable<T> values)
		{
			foreach (T v in values)
			{
				Add(v);
			}
		}

		/// <summary>
		/// (FOR TYPE CONVERSION ONLY) Initializes a new instance of the <see cref="ParseableCollection&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="values">The values.</param>
		private ParseableCollection(IEnumerable<object> values)
			: this(values.Cast<T>())
		{
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return TypeConversion.ToString(this);
		}
	}
}
