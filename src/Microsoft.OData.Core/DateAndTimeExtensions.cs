//---------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Globalization;

namespace Microsoft.OData
{
    /// <summary>
    /// Provides extension methods for working with date and time values.
    /// </summary>
    internal static class DateAndTimeExtensions
    {
        /// <summary>
        /// Converts the specified <see cref="TimeOnly"/> value to its string representation using the 24-hour format
        /// with fractional seconds.
        /// </summary>
        /// <param name="time">The <see cref="TimeOnly"/> value to convert to a string.</param>
        /// <returns>A string that represents the time in the format "HH:mm:ss.fffffff" using invariant culture.</returns>
        public static string ToStringValue(this TimeOnly time)
        {
            return time.ToString("HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the specified <see cref="DateOnly"/> value to its ISO 8601 string representation in the format
        /// "yyyy-MM-dd".
        /// </summary>
        /// <param name="date">The <see cref="DateOnly"/> value to convert to a string.</param>
        /// <returns>A string that represents the date in ISO 8601 format ("yyyy-MM-dd").</returns>
        public static string ToStringValue(this DateOnly date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
