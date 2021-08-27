//---------------------------------------------------------------------
// <copyright file="TypeConversion.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A type converter to use with generic types.
	/// </summary>
	public class TypeConverterForGenericTypes : TypeConverter
	{
		private Type _targetType;

		internal static TypeConverter FillTargetType(TypeConverter converter, Type targetType)
		{
			TypeConverterForGenericTypes asGeneric;
			if ((asGeneric = converter as TypeConverterForGenericTypes) != null)
			{
				asGeneric._targetType = targetType;
			}
			return converter;
		}

		/// <summary>
		/// Gets the generic target type.
		/// </summary>
		/// <value>The generic target type.</value>
		protected Type TargetType
		{
			get { return _targetType; }
		}
	}

	/// <summary>
	/// Utility methods for doing type conversions, mainly for converting values to/from strings in the UI/repro.
	/// </summary>
	public static class TypeConversion
	{
		private const string _nullString = "(null)";

		/// <summary>
		/// Gets an object from a string.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="sourceString">The source string.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
		public static object FromString(Type targetType, string sourceString)
		{
			if (sourceString.Equals(_nullString, StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			return TypeConverterForGenericTypes.FillTargetType(Utilities.GetTypeConverter(targetType), targetType).ConvertFrom(null, CultureInfo.InvariantCulture, sourceString);
		}

		/// <summary>
		/// Gets a string from the object.
		/// </summary>
		/// <param name="sourceObject">The source object - can be null.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object")]
		public static string ToString(object sourceObject)
		{
			if (sourceObject == null)
			{
				return _nullString;
			}
            return (string)TypeConverterForGenericTypes.FillTargetType(Utilities.GetTypeConverter(sourceObject.GetType()), sourceObject.GetType()).ConvertTo(null, CultureInfo.InvariantCulture, sourceObject, typeof(string));
		}
	}
}
