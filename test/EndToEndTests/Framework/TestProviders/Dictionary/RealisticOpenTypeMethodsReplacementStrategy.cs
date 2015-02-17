//---------------------------------------------------------------------
// <copyright file="RealisticOpenTypeMethodsReplacementStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Dictionary
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.OData.Framework.TestProviders.Common;

    /// <summary>
    /// A method replacement strategy for open-type-methods that only allows operations between types that make sense for the CLR
    /// </summary>
    public class RealisticOpenTypeMethodsReplacementStrategy : OpenTypeMethodsReplacementStrategy
    {
        /// <summary>
        /// Initializes a new instance of the RealisticOpenTypeMethodsReplacementStrategy class
        /// </summary>
        /// <param name="queryProvider">The current query provider</param>
        public RealisticOpenTypeMethodsReplacementStrategy(IDataServiceQueryProvider queryProvider)
            : base(queryProvider)
        {
        }

        /// <summary>
        /// Adds the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of adding the objects</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Add(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) + Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) + Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) + Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) + Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) + Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) + Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) + Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot add type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        /// <summary>
        /// Ands the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of and-ing the objects</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object AndAlso(object left, object right)
        {
            if (left is bool && right is bool)
            {
                return (bool)left && (bool)right;
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Not valid between types {0} and {1}", left.GetType(), right.GetType()));
        }

        /// <summary>
        /// Gets the ceiling of the object
        /// </summary>
        /// <param name="value">The value to get the ceiling of</param>
        /// <returns>The ceiling of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Ceiling(object value)
        {
            if (value is decimal)
            {
                return Math.Ceiling((decimal)value);
            }
            else if (value is double)
            {
                return Math.Ceiling((double)value);
            }
            else if (value is float)
            {
                return Math.Ceiling((float)value);
            }

            throw new DataServiceException(400, "Cannot use Ceiling with type " + value.GetType());
        }

        /// <summary>
        /// Concats the two objects
        /// </summary>
        /// <param name="first">The first object</param>
        /// <param name="second">The second object</param>
        /// <returns>The result of concatenating the objects</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Concat(object first, object second)
        {
            if (first is string && second is string)
            {
                return (string)first + (string)second;
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Concat not valid between types {0} and {1}", first.GetType(), second.GetType()));
        }

        /// <summary>
        /// Gets the day of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the day of</param>
        /// <returns>The day of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Day(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Day");
        }

        /// <summary>
        /// Divides the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of dividing the objects</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Divide(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) / Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) / Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) / Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) / Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) / Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) / Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) / Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot divide type {0} by {1}", left.GetType(), right.GetType()));
            }
        }

        /// <summary>
        /// Returns whether the target object ends with the substring
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to check for</param>
        /// <returns>Whether it ends with the substring</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object EndsWith(object targetString, object substring)
        {
            if (targetString is string && substring is string)
            {
                return ((string)targetString).EndsWith((string)substring, StringComparison.Ordinal);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Not valid between types {0} and {1}", targetString.GetType(), substring.GetType()));
        }

        /// <summary>
        /// Gets the floor of the object
        /// </summary>
        /// <param name="value">The value to get the floor of</param>
        /// <returns>The floor of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Floor(object value)
        {
            if (value is decimal)
            {
                return Math.Floor((decimal)value);
            }
            else if (value is double)
            {
                return Math.Floor((double)value);
            }
            else if (value is float)
            {
                return Math.Floor((float)value);
            }

            throw new DataServiceException(400, "Floor not valid for type " + value.GetType());
        }

        /// <summary>
        /// Gets the hour of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the hour of</param>
        /// <returns>The hour of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Hour(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Hour");
        }

        /// <summary>
        /// Returns the index where the substring first appears in the target
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to get the index of</param>
        /// <returns>the index where the substring first appears in the target</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object IndexOf(object targetString, object substring)
        {
            if (targetString is string && substring is string)
            {
                return ((string)targetString).IndexOf((string)substring, StringComparison.Ordinal);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Not valid between types {0} and {1}", targetString.GetType(), substring.GetType()));
        }

        /// <summary>
        /// Gets the length of the object
        /// </summary>
        /// <param name="value">The value to get the length of</param>
        /// <returns>The length of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Length(object value)
        {
            if (value is string)
            {
                return ((string)value).Length;
            }
            else if (value is Array)
            {
                return ((Array)value).Length;
            }
            else
            {
                throw new DataServiceException(400, "Length not valid on type " + value.GetType());
            }
        }

        /// <summary>
        /// Gets the minute of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the minute of</param>
        /// <returns>The minute of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Minute(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Minute");
        }

        /// <summary>
        /// Modulos the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of mod-ing the objects</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Modulo(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) % Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) % Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) % Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) % Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) % Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) % Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) % Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot modulo type {0} by {1}", left.GetType(), right.GetType()));
            }
        }

        /// <summary>
        /// Gets the month of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the month of</param>
        /// <returns>The month of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Month(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Month");
        }

        /// <summary>
        /// Multiplies the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of multiplying the objects</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Multiply(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) * Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) * Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) * Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) * Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) * Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) * Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) * Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot multiply type {0} by {1}", left.GetType(), right.GetType()));
            }
        }

        /// <summary>
        /// Gets the negation of the object
        /// </summary>
        /// <param name="value">The value to negate</param>
        /// <returns>The negated object</returns>
        public override object Negate(object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.SByte:
                    return -(byte)value;

                case TypeCode.Int16:
                    return -(short)value;

                case TypeCode.Int32:
                    return -(int)value;

                case TypeCode.Int64:
                    return -(long)value;
            }

            return null;
        }

        /// <summary>
        /// Gets the boolean inverse of the object
        /// </summary>
        /// <param name="value">The value to get the boolean inverse of</param>
        /// <returns>The boolean inverse of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Not(object value)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            else
            {
                return value != null;
            }
        }

        /// <summary>
        /// Elses the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of else-ing the objects</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object OrElse(object left, object right)
        {
            if (left is bool && right is bool)
            {
                return (bool)left || (bool)right;
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Not valid between types {0} and {1}", left.GetType(), right.GetType()));
        }

        /// <summary>
        /// Replaces instances of the given substring with the new string in the target
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to replace</param>
        /// <param name="newString">The object to replace it with</param>
        /// <returns>The object with replacements</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Replace(object targetString, object substring, object newString)
        {
            if (targetString is string && substring is string && newString is string)
            {
                return ((string)targetString).Replace((string)substring, (string)newString);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use with types {0}, {1}, and {2}", targetString.GetType(), substring.GetType(), newString.GetType()));
        }

        /// <summary>
        /// Gets the rounded version of the object
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <returns>The rounded object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Round(object value)
        {
            if (value is decimal)
            {
                return Math.Round((decimal)value);
            }
            else if (value is double)
            {
                return Math.Round((double)value);
            }
            else if (value is float)
            {
                return Math.Round((float)value);
            }

            throw new DataServiceException("Cannot use Round with type " + value.GetType());
        }

        /// <summary>
        /// Gets the second of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the second of</param>
        /// <returns>The second of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Second(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Second");
        }

        /// <summary>
        /// Returns whether the target object starts with the substring
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to check for</param>
        /// <returns>Whether it starts with the substring</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object StartsWith(object targetString, object substring)
        {
            if (targetString is string && substring is string)
            {
                return ((string)targetString).StartsWith((string)substring, StringComparison.Ordinal);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use with types {0} and {1}", substring.GetType(), targetString.GetType()));
        }

        /// <summary>
        /// Gets the substring of the given object
        /// </summary>
        /// <param name="targetString">The object to get the substring of</param>
        /// <param name="startIndex">The start index</param>
        /// <returns>The substring of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Substring(object targetString, object startIndex)
        {
            if (targetString is string && startIndex is int)
            {
                return ((string)targetString).Substring((int)startIndex);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use with types {0} and {1}", targetString.GetType(), startIndex.GetType()));
        }

        /// <summary>
        /// Gets the substring of the given object
        /// </summary>
        /// <param name="targetString">The object to get the substring of</param>
        /// <param name="startIndex">The start index</param>
        /// <param name="length">the length of the substring</param>
        /// <returns>The substring of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Substring(object targetString, object startIndex, object length)
        {
            if (targetString is string && startIndex is int && length is int)
            {
                return ((string)targetString).Substring((int)startIndex, (int)length);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use Substring with types {0}, {1} and {2}", targetString.GetType(), startIndex.GetType(), length.GetType()));
        }

        /// <summary>
        /// Returns whether the target object contains the substring
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to check for</param>
        /// <returns>Whether it contains with the substring</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Contains(object targetString, object substring)
        {
            if (substring is string && targetString is string)
            {
                return ((string)targetString).Contains((string)substring);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use with types {0} and {1}", substring.GetType(), targetString.GetType()));
        }

        /// <summary>
        /// Subtracts the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of subtracting the objects</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Subtract(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) - Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) - Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) - Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) - Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) - Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) - Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) - Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot subtract type {0} from type {1}", left.GetType(), right.GetType()));
            }
        }

        /// <summary>
        /// Gets the lowercase version of the given object
        /// </summary>
        /// <param name="targetString">The object to lowercase</param>
        /// <returns>The lowercase version of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "String is not being normalized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object ToLower(object targetString)
        {
            if (targetString is string)
            {
                return ((string)targetString).ToLowerInvariant();
            }

            throw new DataServiceException(400, "Cannot use with type " + targetString.GetType());
        }

        /// <summary>
        /// Gets the uppercase version of the given object
        /// </summary>
        /// <param name="targetString">The object to uppercase</param>
        /// <returns>The uppercase version of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object ToUpper(object targetString)
        {
            if (targetString is string)
            {
                return ((string)targetString).ToUpperInvariant();
            }

            throw new DataServiceException(400, "Cannot use with type " + targetString.GetType());
        }

        /// <summary>
        /// Trims the given object
        /// </summary>
        /// <param name="targetString">The object to trim</param>
        /// <returns>The trimmed object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Temporary exception for now, no time to refactor everywhere")]
        public override object Trim(object targetString)
        {
            if (targetString is string)
            {
                return ((string)targetString).Trim();
            }

            throw new DataServiceException(400, "Cannot use with type " + targetString.GetType());
        }

        /// <summary>
        /// Gets the year of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the year of</param>
        /// <returns>The year of the object</returns>
        public override object Year(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Year");
        }

        /// <summary>
        /// Converts the given object to the given type
        /// </summary>
        /// <param name="value">The object to convert</param>
        /// <param name="type">The type to convert to</param>
        /// <returns>The converted object</returns>
        public override object Convert(object value, ResourceType type)
        {
            if (value == null)
            {
                return null;
            }

            if (type.InstanceType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            throw new InvalidCastException("Instance of '" + value.GetType() + "' cannot be converted to resource type '" + type.FullName + "'");
        }

        /// <summary>
        /// Returns whether the object is of the given type
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="type">The type to check for</param>
        /// <returns>Whether the object is of the given type</returns>
        public override object TypeIs(object value, ResourceType type)
        {
            if (value == null)
            {
                return false;
            }

            ResourceType instanceType = this.GetResourceType(value);
            if (instanceType != null)
            {
                if (this.GetDerivedTypes(type).Contains(instanceType))
                {
                    return true;
                }

                return type == instanceType;
            }

            return type.InstanceType.IsAssignableFrom(value.GetType());
        }

        /// <summary>
        /// Returns whether the left object is greater than the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is greater than the right object</returns>
        public override object GreaterThan(object left, object right)
        {
            try
            {
                return Compare(left, right) > 0;
            }
            catch (ArgumentException)
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        /// <summary>
        /// Returns whether the left object is greater than or equal to the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is greater than or equal to the right object</returns>
        public override object GreaterThanOrEqual(object left, object right)
        {
            try
            {
                return Compare(left, right) >= 0;
            }
            catch (ArgumentException)
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        /// <summary>
        /// Returns whether the left object is less than the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is less than the right object</returns>
        public override object LessThan(object left, object right)
        {
            try
            {
                return Compare(left, right) < 0;
            }
            catch (ArgumentException)
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        /// <summary>
        /// Returns whether the left object is less than or equal to the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is less than or equal to the right object</returns>
        public override object LessThanOrEqual(object left, object right)
        {
            try
            {
                return this.Compare(left, right) <= 0;
            }
            catch (ArgumentException)
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        /// <summary>
        /// Returns whether the two objects are equal
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>Whether the two object are equal</returns>
        public override object Equal(object left, object right)
        {
            return this.Compare(left, right) == 0;
        }

        /// <summary>
        /// Returns whether the two objects are not equal
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>Whether the two object are not equal</returns>
        public override object NotEqual(object left, object right)
        {
            return this.Compare(left, right) != 0;
        }

        /// <summary>
        /// Handles distance for non-spatial types or throws.
        /// </summary>
        /// <param name="operand1">The first operand.</param>
        /// <param name="operand2">The second operand.</param>
        /// <returns>The distance between the non-spatial values</returns>
        protected override object HandleNonSpatialDistance(object operand1, object operand2)
        {
            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Distance method is only valid for spatial types. Arguments were: '{0}' and '{1}'", operand1, operand2));
        }

        private static object GetPropertyValue(object thing, string propertyName)
        {
            object value;
            if (!TryGetPropertyValue(thing, propertyName, out value))
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use {0} with type {1}", propertyName, thing.GetType()));
            }

            return value;
        }
    }
}