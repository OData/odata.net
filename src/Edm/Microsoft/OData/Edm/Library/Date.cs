//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Date type for Edm.Date
    /// </summary>
    public struct Date : IComparable, IComparable<Date>, IEquatable<Date>
    {
        /// <summary>
        /// Min value of <see cref="Microsoft.OData.Edm.Library.Date"/>
        /// </summary>
        public static readonly Date MinValue = new Date(1, 1, 1);

        /// <summary>
        /// Max value of <see cref="Microsoft.OData.Edm.Library.Date"/>
        /// </summary>
        public static readonly Date MaxValue = new Date(9999, 12, 31);

        /// <summary>
        /// Internal using System.DateTime
        /// </summary>
        private DateTime dateTime;

        /// <summary>
        /// Consturctor of <see cref="Microsoft.OData.Edm.Library.Date"/>
        /// </summary>
        /// <param name="year">Year value of date</param>
        /// <param name="month">Month value of date</param>
        /// <param name="day">Day value of date</param>
        public Date(int year, int month, int day)
            : this()
        {
            try
            {
                this.dateTime = new DateTime(year, month, day);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                throw new FormatException(Strings.Date_InvalidDateParameters(year, month, day));
            }
        }

        /// <summary>
        /// Gets a <see cref="Microsoft.OData.Edm.Library.Date"/> object that is set to current date on this computer, expressed as the local time.
        /// </summary>
        public static Date Now
        {
            get { return DateTime.Now; }
        }

        /// <summary>
        /// Gets the year component of the date represented by this instance.
        /// </summary>
        public int Year
        {
            get
            {
                return dateTime.Year;
            }
        }

        /// <summary>
        /// Gets the month component of the date represented by this instance.
        /// </summary>
        public int Month
        {
            get
            {
                return dateTime.Month;
            }
        }

        /// <summary>
        /// Gets the day of the month represented by this instance.
        /// </summary>
        public int Day
        {
            get
            {
                return dateTime.Day;
            }
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="Microsoft.OData.Edm.Library.Date"/> are equal.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if d1 and d2 represent the same date; otherwise, false.</returns>
        public static bool operator ==(Date d1, Date d2)
        {
            return d1.dateTime == d2.dateTime;
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="Microsoft.OData.Edm.Library.Date"/> are not equal.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if d1 and d2 do not represent the same date; otherwise, false.</returns>
        public static bool operator !=(Date d1, Date d2)
        {
            return d1.dateTime != d2.dateTime;
        }

        /// <summary>
        /// Determines whether one specified <see cref="Microsoft.OData.Edm.Library.Date"/> is less than
        /// another specified <see cref="Microsoft.OData.Edm.Library.Date"/>
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if d1 is less than d2; otherwise, false.</returns>
        public static bool operator <(Date d1, Date d2)
        {
            return d1.dateTime < d2.dateTime;
        }

        /// <summary>
        /// Determines whether one specified <see cref="Microsoft.OData.Edm.Library.Date"/> is less equal to
        /// another specified <see cref="Microsoft.OData.Edm.Library.Date"/>
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if d1 is less equal to d2; otherwise, false.</returns>
        public static bool operator <=(Date d1, Date d2)
        {
            return d1.dateTime <= d2.dateTime;
        }

        /// <summary>
        /// Determines whether one specified <see cref="Microsoft.OData.Edm.Library.Date"/> is greater than
        /// another specified <see cref="Microsoft.OData.Edm.Library.Date"/>
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if d1 is greater than d2; otherwise, false.</returns>
        public static bool operator >(Date d1, Date d2)
        {
            return d1.dateTime > d2.dateTime;
        }

        /// <summary>
        /// Determines whether one specified <see cref="Microsoft.OData.Edm.Library.Date"/> is greater equal to
        /// another specified <see cref="Microsoft.OData.Edm.Library.Date"/>
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if d1 is greater equal to d2; otherwise, false.</returns>
        public static bool operator >=(Date d1, Date d2)
        {
            return d1.dateTime >= d2.dateTime;
        }

        /// <summary>
        /// Returns a new Date that adds the specified number of years to the value of this instance.
        /// </summary>
        /// <param name="value">A number of years. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of years represented by value</returns>
        public Date AddYears(int value)
        {
            try
            {
                return this.dateTime.AddYears(value);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException("value", Strings.Date_InvalidAddedOrSubtractedResults);
            }
        }

        /// <summary>
        /// Returns a new Date that adds the specified number of months to the value of this instance.
        /// </summary>
        /// <param name="value">A number of months. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of months represented by value</returns>
        public Date AddMonths(int value)
        {
            try
            {
                return this.dateTime.AddMonths(value);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException("value", Strings.Date_InvalidAddedOrSubtractedResults);
            }
        }

        /// <summary>
        /// Returns a new Date that adds the specified number of days to the value of this instance.
        /// </summary>
        /// <param name="value">A number of days. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of days represented by value</returns>
        public Date AddDays(int value)
        {
            try
            {
                return this.dateTime.AddDays(value);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException("value", Strings.Date_InvalidAddedOrSubtractedResults);
            }
        }

        /// <summary>
        /// Convert Date to Clr DateTime
        /// </summary>
        /// <param name="d">Date Value</param>
        /// <returns>DateTime Value which represent the Date</returns>
        public static implicit operator DateTime(Date d)
        {
            return d.dateTime;
        }

        /// <summary>
        /// Convert Clr DateTime to Date
        /// </summary>
        /// <param name="d">DateTime Value</param>
        /// <returns>Date Value from DateTime</returns>
        public static implicit operator Date(DateTime d)
        {
            return new Date(d.Year, d.Month, d.Day);
        }

        /// <summary>
        /// Convert Date to String
        /// </summary>
        /// <returns>string value of Date</returns>
        public override string ToString()
        {
            return this.dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Compares the value of this instance to a object value
        /// and returns an integer that indicates whether this instance is earlier than,
        /// the same as, or later than the object if it is a Date.
        /// </summary>
        /// <param name="obj">The object to compare to the current instance</param>
        /// <returns>Value Description Less than zero This instance is earlier than value. 
        /// Zero This instance is the same as value.
        /// Greater than zero This instance is later than value.</returns>
        public int CompareTo(object obj)
        {
            if (obj is Date)
            {
                Date date2 = (Date)obj;
                return this.CompareTo(date2);
            }
            else
            {
                throw new ArgumentException(Strings.Date_InvalidCompareToTarget(obj));
            }
        }

        /// <summary>
        /// Compares the value of this instance to a specified Date value
        /// and returns an integer that indicates whether this instance is earlier than,
        /// the same as, or later than the specified Date value.
        /// </summary>
        /// <param name="other">The object to compare to the current instance</param>
        /// <returns>Value Description Less than zero This instance is earlier than value. 
        /// Zero This instance is the same as value.
        /// Greater than zero This instance is later than value.</returns>
        public int CompareTo(Date other)
        {
            return this.dateTime.CompareTo(other.dateTime);
        }

        /// <summary>
        /// Compares the value of this instance to a specified Date value
        /// and returns an bool that indicates whether this instance is same as the specified Date value.
        /// </summary>
        /// <param name="other">The object to compare to the current instance</param>
        /// <returns>True for equal, false for non-equal.</returns>
        public bool Equals(Date other)
        {
            return this.dateTime.Equals(other.dateTime);
        }

        /// <summary>
        /// Compares the value of this instance to a specified object value
        /// and returns an bool that indicates whether the specified object is <see cref="Microsoft.OData.Edm.Library.Date"/> 
        /// and this instance is same as the specified <see cref="Microsoft.OData.Edm.Library.Date"/> value.
        /// </summary>
        /// <param name="obj">The object to compare to the current instance</param>
        /// <returns>True for equal, false for non-equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Date))
            {
                return false;
            }

            return this.dateTime.Equals(((Date)obj).dateTime);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.dateTime.GetHashCode();
        }

        /// <summary>
        /// Converts a specified string representation of a date to <see cref="Microsoft.OData.Edm.Library.Date"/> with CurrentCulture format.
        /// </summary>
        /// <param name="text">A string that represent a date to convert.</param>
        /// <returns>The <see cref="Microsoft.OData.Edm.Library.Date"/> instance represented by text </returns>
        public static Date Parse(string text)
        {
            return Parse(text, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts a specified string representation of a date to <see cref="Microsoft.OData.Edm.Library.Date"/>.
        /// </summary>
        /// <param name="text">A string that represent a date to convert.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about text.</param>
        /// <returns>The <see cref="Microsoft.OData.Edm.Library.Date"/> instance represented by text </returns>
        public static Date Parse(string text, IFormatProvider provider)
        {
            Date result;
            if (Date.TryParse(text, provider, out result))
            {
                return result;
            }
            else
            {
                throw new FormatException(Strings.Date_InvalidParsingString(text));
            }
        }

        /// <summary>
        /// Try converts a specified string representation of a date to <see cref="Microsoft.OData.Edm.Library.Date"/> with CurrentCulture format.
        /// </summary>
        /// <param name="text">A string that represent a date to convert.</param>
        /// <param name="result">A <see cref="Microsoft.OData.Edm.Library.Date"/> object equivalent to the date input, if the conversion succeeded
        /// or <see cref="Microsoft.OData.Edm.Library.Date.MinValue"/>, if the conversion failed.</param>
        /// <returns>True if the input parameter is successfully converted; otherwise, false.</returns>
        public static bool TryParse(string text, out Date result)
        {
            return TryParse(text, CultureInfo.CurrentCulture, out result);
        }

        /// <summary>
        /// Try converts a specified string representation of a date to <see cref="Microsoft.OData.Edm.Library.Date"/>
        /// </summary>
        /// <param name="text">A string that represent a date to convert.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about text.</param>
        /// <param name="result">A <see cref="Microsoft.OData.Edm.Library.Date"/> object equivalent to the date input, if the conversion succeeded
        /// or <see cref="Microsoft.OData.Edm.Library.Date.MinValue"/>, if the conversion failed.</param>
        /// <returns>True if the input parameter is successfully converted; otherwise, false.</returns>
        public static bool TryParse(string text, IFormatProvider provider, out Date result)
        {
            DateTime date;
            var b = DateTime.TryParse(text, provider, DateTimeStyles.None, out date);
            result = date;
            return b;
        }
    }
}
