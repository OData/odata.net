//---------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Globalization;

namespace Microsoft.OData.Edm
{
    public static class DateAndTimeExtensions
    {
        /// <summary>
        /// Converts the specified <see cref="DateOnly"/> value to its OData string representation
        /// in the ISO 8601 format "yyyy-MM-dd" using invariant culture.
        /// </summary>
        /// <param name="date">The <see cref="DateOnly"/> value to convert.</param>
        /// <returns>A string representing the date in OData ISO 8601 format ("yyyy-MM-dd").</returns>
        public static string ToODataString(this DateOnly date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the specified <see cref="TimeOnly"/> value to its OData string representation
        /// in the ISO 8601 extended format "HH:mm:ss.fffffff" using invariant culture.
        /// </summary>
        /// <param name="time">The <see cref="TimeOnly"/> value to convert.</param>
        /// <returns>A string representing the time in OData ISO 8601 format ("HH:mm:ss.fffffff").</returns>
        public static string ToODataString(this TimeOnly time)
        {
            return time.ToString(@"HH:mm\:ss\.fffffff", CultureInfo.InvariantCulture);
        }
    }
}
