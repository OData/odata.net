//---------------------------------------------------------------------
// <copyright file="Type.Ext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;

    /// <summary>
    /// The type Extensions
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Convert to clr type
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="value">The value</param>
        /// <param name="escapeSingleQuote">Escape signle quota</param>
        /// <returns>Return clr object</returns>
        public static object ConvertToClr(this Type type, string value, bool escapeSingleQuote = true)
        {
            if (type == typeof(string))
            {
                if (value == null)
                {
                    return null;
                }

                if (value.StartsWith("'") && value.EndsWith("'") && value.Length >= 2 && escapeSingleQuote)
                {
                    return value.Substring(1, value.Length - 2);
                }

                return value;
            }

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (type == typeof(short))
            {
                return short.Parse(value);
            }

            if (type == typeof(int))
            {
                return int.Parse(value);
            }

            if (type == typeof(double))
            {
                return double.Parse(value);
            }

            if (type == typeof(bool))
            {
                return bool.Parse(value);
            }

            if (type == typeof(float))
            {
                return float.Parse(value);
            }

            if (type == typeof(byte))
            {
                return byte.Parse(value);
            }

            if (type == typeof(long))
            {
                return long.Parse(value);
            }

            if (type == typeof(decimal))
            {
                return decimal.Parse(value);
            }

            if (type == typeof(DateTime))
            {
                if (value.StartsWith("datetime", StringComparison.CurrentCultureIgnoreCase))
                {
                    value = value.Substring("datetime".Length).Trim(new[] { '\'' });
                }

                return DateTime.Parse(value);
            }

            if (type == typeof(Guid))
            {
                if (value.StartsWith("guid", StringComparison.CurrentCultureIgnoreCase))
                {
                    value = value.Substring("guid".Length).Trim(new[] { '\'' });
                }

                return Guid.Parse(value);
            }

            if (type == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Parse(value);
            }

            if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(value);
            }

            if (type == typeof(sbyte))
            {
                return sbyte.Parse(value);
            }

            if (type == typeof(byte[]))
            {
                return System.Text.Encoding.Unicode.GetBytes(value);
            }

            throw new NotImplementedException(
                string.Format("Don't know how to parse type: {0}", type.Name));
        }
    }
}
