//---------------------------------------------------------------------
// <copyright file="ToStringConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Common
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Xml;

    /// <summary>
    /// Implements similar Logic to Convert.ToString, only it won't cause loss of data for floating points 
    /// </summary>
    public static class ToStringConverter
    {
        /// <summary>
        /// Converts any object to a string, if its a primitive value the conversion is precise, otherwise
        /// it falls back to Convert.ToString
        /// Use this for Debugging only
        /// </summary>
        /// <param name="value">Value to Convert to String</param>
        /// <returns>A string representation of the object</returns>
        public static string ConvertObjectToString(object value)
        {
            string newStringValue = null;
            Type objectType = null;
            bool converted = TryConvertPrimitiveValueToString(value, out newStringValue, out objectType);

            if (converted == false)
            {
                newStringValue = Convert.ToString(value, CultureInfo.InvariantCulture);
            }

            return newStringValue;
        }

        /// <summary>
        /// Converts a Primitive Value to a string
        ///  Implemented only types supported by Convert.ToString
        /// </summary>
        /// <param name="value">Object value</param>
        /// <returns>A string, returns null if value is null</returns>
        public static string ConvertPrimitiveValueToString(object value)
        {
            string newStringValue = null;
            Type objectType = null;
            bool converted = TryConvertPrimitiveValueToString(value, out newStringValue, out objectType);

            if (converted == false)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot convert Type '{0}' to a string", objectType.Name));
            }

            return newStringValue;
        }

        /// <summary>
        /// Converts specified enum value to the value of the underlying type.
        /// </summary>
        /// <param name="enumValue">The enum value to convert from.</param>
        /// <returns>Converted value.</returns>
        public static object ConvertFromEnum(object enumValue)
        {
            if (enumValue == null)
            {
                return null;
            }

            Type enumType = enumValue.GetType();
            ExceptionUtilities.Assert(enumType.IsEnum(), "Type is not an enum type: {0}.", enumType.FullName);

            Type underlyingType = Enum.GetUnderlyingType(enumType);

            return Convert.ChangeType(enumValue, underlyingType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a object to a string value and indicates its type and whether it successfully converted it or not
        /// </summary>
        /// <param name="value">Value to Convert</param>
        /// <param name="newStringValue">New Converted Value</param>
        /// <param name="valueType">Type of the original Value</param>
        /// <returns>True if it was converted or false if it was not</returns>
        internal static bool TryConvertPrimitiveValueToString(object value, out string newStringValue, out Type valueType)
        {
            valueType = null;
            newStringValue = null;
            
            if (value == null)
            {
                return true;
            }

            valueType = value.GetType();

            if (valueType == typeof(bool))
            {
                var valueToConvert = (bool)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(byte))
            {
                var valueToConvert = (byte)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(char))
            {
                var valueToConvert = (char)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(DateTimeOffset))
            {
                var valueToConvert = (DateTimeOffset)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(decimal))
            {
                var valueToConvert = (decimal)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(double))
            {
                var valueToConvert = (double)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(float))
            {
                var valueToConvert = (float)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(Guid))
            {
                var valueToConvert = (Guid)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(int))
            {
                var valueToConvert = (int)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(long))
            {
                var valueToConvert = (long)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(sbyte))
            {
                var valueToConvert = (sbyte)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(short))
            {
                var valueToConvert = (short)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(string))
            {
                newStringValue = (string)value;
            }
            else if (valueType == typeof(TimeSpan))
            {
                var valueToConvert = (TimeSpan)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(uint))
            {
                var valueToConvert = (uint)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(ulong))
            {
                var valueToConvert = (ulong)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType == typeof(ushort))
            {
                var valueToConvert = (ushort)value;
                newStringValue = XmlConvert.ToString(valueToConvert);
            }
            else if (valueType.GetBaseType() == typeof(Enum))
            {
                newStringValue = Enum.GetName(valueType, value);
                if (newStringValue == null)
                {
                    Type underlyingValueType;
                    TryConvertPrimitiveValueToString(ConvertFromEnum(value), out newStringValue, out underlyingValueType);
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Replacement for Type.IsEnum.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        internal static bool IsEnum(this Type type)
        {
#if DNXCORE50
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        /// <summary>
        /// Replacement for Type.BaseType.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        internal static Type GetBaseType(this Type type)
        {
#if DNXCORE50
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }
    }
}
