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
    /// TimeOfDay type for Edm.TimeOfDay
    /// </summary>
    public struct TimeOfDay : IComparable, IComparable<TimeOfDay>, IEquatable<TimeOfDay>
    {
        /// <summary>
        /// Max value of ticks
        /// </summary>
        public const long MaxTickValue = 863999999999;

        /// <summary>
        /// Min value of ticks
        /// </summary>
        public const long MinTickValue = 0;

        /// <summary>
        /// Represents the number of ticks in 1 hour. This field is constant.
        /// </summary>
        public const long TicksPerHour = 36000000000;

        /// <summary>
        /// Represents the number of ticks in 1 minute. This field is constant.
        /// </summary>
        public const long TicksPerMinute = 600000000;

        /// <summary>
        /// Represents the number of ticks in 1 second.
        /// </summary>
        public const long TicksPerSecond = 10000000;

        /// <summary>
        /// Min value of <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/>
        /// </summary>
        public static readonly TimeOfDay MinValue = new TimeOfDay(MinTickValue);

        /// <summary>
        /// Max value of <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/>
        /// </summary>
        public static readonly TimeOfDay MaxValue = new TimeOfDay(MaxTickValue);

        /// <summary>
        /// Internal using System.TimeSpan
        /// </summary>
        private TimeSpan timeSpan;

        /// <summary>
        /// Consturctor of <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/>
        /// </summary>
        /// <param name="hour">Hour value of TimeOfDay</param>
        /// <param name="minute">Minute value of TimeOfDay</param>
        /// <param name="second">Second value of TimeOfDay</param>
        /// <param name="millisecond">Millisecond value of TimeOfDay, whose precision will be millisecond (3 digits).</param>
        public TimeOfDay(int hour, int minute, int second, int millisecond)
            : this()
        {
            if (hour < 0 || hour > 23 || minute < 0 || minute > 59 || second < 0 || second > 59 || millisecond < 0 || millisecond > 999)
            {
                throw new FormatException(Strings.TimeOfDay_InvalidTimeOfDayParameters(hour, minute, second, millisecond));
            }
            else
            {
                timeSpan = new TimeSpan(0, hour, minute, second, millisecond);
            }
        }

        /// <summary>
        /// Consturctor of <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/>
        /// </summary>
        /// <param name="ticks">ticks value of TimeOfDay</param>
        public TimeOfDay(long ticks)
            : this()
        {
            if (ticks < MinTickValue || ticks > MaxTickValue)
            {
                throw new FormatException(Strings.TimeOfDay_TicksOutOfRange(ticks));
            }
            else
            {
                timeSpan = new TimeSpan(ticks);
            }
        }

        /// <summary>
        /// Gets a <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> object that is set to current time on this computer, expressed as the local time.
        /// </summary>
        public static TimeOfDay Now
        {
            get { return DateTime.Now.TimeOfDay; }
        }

        /// <summary>
        /// Gets the hour component of the TimeOfDay represented by this instance.
        /// </summary>
        public int Hours
        {
            get
            {
                return timeSpan.Hours;
            }
        }

        /// <summary>
        /// Gets the minute component of the TimeOfDay represented by this instance.
        /// </summary>
        public int Minutes
        {
            get
            {
                return timeSpan.Minutes;
            }
        }

        /// <summary>
        /// Gets the second component of the TimeOfDay represented by this instance.
        /// </summary>
        public int Seconds
        {
            get
            {
                return timeSpan.Seconds;
            }
        }

        /// <summary>
        /// Gets the millisecond component of the TimeOfDay represented by this instance.
        /// </summary>
        public long Milliseconds
        {
            get
            {
                return timeSpan.Milliseconds;
            }
        }

        /// <summary>
        /// Gets the number of ticks that represent the value of the current TimeOfDay structure
        /// </summary>
        public long Ticks
        {
            get
            {
                return timeSpan.Ticks;
            }
        }

        /// <summary>
        /// Indicates whether two <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> instances are not equal.
        /// </summary>
        /// <param name="t1">The first time interval to compare.</param>
        /// <param name="t2">The second time interval to compare.</param>
        /// <returns>true if the values of t1 and t2 are not equal; otherwise, false.</returns>
        public static bool operator !=(TimeOfDay t1, TimeOfDay t2)
        {
            return t1.timeSpan != t2.timeSpan;
        }

        /// <summary>
        /// Indicates whether two <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> instances are equal.
        /// </summary>
        /// <param name="t1">The first time interval to compare.</param>
        /// <param name="t2">The second time interval to compare.</param>
        /// <returns>true if the values of t1 and t2 are equal; otherwise, false.</returns>
        public static bool operator ==(TimeOfDay t1, TimeOfDay t2)
        {
            return t1.timeSpan == t2.timeSpan;
        }

        /// <summary>
        /// Indicates whether a specified <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> is greater equal to 
        /// another specified <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/>.
        /// </summary>
        /// <param name="t1">The first time interval to compare.</param>
        /// <param name="t2">The second time interval to compare.</param>
        /// <returns>true if the value of t1 is greater equal to the value of t2; otherwise, false.</returns>
        public static bool operator >=(TimeOfDay t1, TimeOfDay t2)
        {
            return t1.timeSpan >= t2.timeSpan;
        }

        /// <summary>
        /// Indicates whether a specified <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> is greater than 
        /// another specified <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/>.
        /// </summary>
        /// <param name="t1">The first time interval to compare.</param>
        /// <param name="t2">The second time interval to compare.</param>
        /// <returns>true if the value of t1 is greater than the value of t2; otherwise, false.</returns>
        public static bool operator >(TimeOfDay t1, TimeOfDay t2)
        {
            return t1.timeSpan > t2.timeSpan;
        }

        /// <summary>
        /// Indicates whether a specified <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> is less equal to 
        /// another specified <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/>.
        /// </summary>
        /// <param name="t1">The first time interval to compare.</param>
        /// <param name="t2">The second time interval to compare.</param>
        /// <returns>true if the value of t1 is less equal to the value of t2; otherwise, false.</returns>
        public static bool operator <=(TimeOfDay t1, TimeOfDay t2)
        {
            return t1.timeSpan <= t2.timeSpan;
        }

        /// <summary>
        /// Indicates whether a specified <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> is less than 
        /// another specified <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/>.
        /// </summary>
        /// <param name="t1">The first time interval to compare.</param>
        /// <param name="t2">The second time interval to compare.</param>
        /// <returns>true if the value of t1 is less than the value of t2; otherwise, false.</returns>
        public static bool operator <(TimeOfDay t1, TimeOfDay t2)
        {
            return t1.timeSpan < t2.timeSpan;
        }

        /// <summary>
        /// Convert TimeOfDay to .Net Clr TimeSpan
        /// </summary>
        /// <param name="time">TimeOfDay Value</param>
        /// <returns>TimeSpan Value which represent the TimeOfDay</returns>
        public static implicit operator TimeSpan(TimeOfDay time)
        {
            return time.timeSpan;
        }

        /// <summary>
        /// Convert .Net Clr TimeSpan to TimeOfDay
        /// </summary>
        /// <param name="timeSpan">TimeSpan Value</param>
        /// <returns>TimeOfDay Value from TimeSpan</returns>
        public static implicit operator TimeOfDay(TimeSpan timeSpan)
        {
            if (timeSpan.Days != 0 || timeSpan.Hours < 0
                || timeSpan.Minutes < 0 || timeSpan.Milliseconds < 0
                || timeSpan.Ticks < MinTickValue || timeSpan.Ticks > MaxTickValue)
            {
                throw new FormatException(Strings.TimeOfDay_ConvertErrorFromTimeSpan(timeSpan));
            }
            else
            {
                return new TimeOfDay(timeSpan.Ticks);
            }
        }

        /// <summary>
        /// Compares the value of this instance to a specified TimeOfDay value
        /// and returns an bool that indicates whether this instance is same as the specified TimeOfDay value.
        /// </summary>
        /// <param name="other">The object to compare to the current instance</param>
        /// <returns>True for equal, false for non-equal.</returns>
        public bool Equals(TimeOfDay other)
        {
            return timeSpan.Equals(other.timeSpan);
        }

        /// <summary>
        /// Compares the value of this instance to a specified object value
        /// and returns an bool that indicates whether the specified object is <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> 
        /// and this instance is same as the specified <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> value.
        /// </summary>
        /// <param name="obj">The object to compare to the current instance</param>
        /// <returns>True for equal, false for non-equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TimeOfDay))
            {
                return false;
            }

            return this.timeSpan.Equals(((TimeOfDay)obj).timeSpan);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return timeSpan.GetHashCode();
        }

        /// <summary>
        /// Convert TimeOfDay to String. The precision will 100ns (7 digits).
        /// </summary>
        /// <returns>string value of timeofday</returns>
        public override string ToString()
        {
#if ORCAS
            return this.timeSpan.ToString();
#else
            return this.timeSpan.ToString(@"hh\:mm\:ss\.fffffff", CultureInfo.InvariantCulture);
#endif
        }

        /// <summary>
        /// Compares the value of this instance to a object value
        /// and returns an integer that indicates whether this instance is earlier than,
        /// the same as, or later than the object if it is a TimeOfDay.
        /// </summary>
        /// <param name="obj">The object to compare to the current instance</param>
        /// <returns>Value Description Less than zero This instance is earlier than value. 
        /// Zero This instance is the same as value.
        /// Greater than zero This instance is later than value.</returns>
        public int CompareTo(object obj)
        {
            if (obj is TimeOfDay)
            {
                TimeOfDay time2 = (TimeOfDay)obj;
                return this.CompareTo(time2);
            }
            else
            {
                throw new ArgumentException(Strings.TimeOfDay_InvalidCompareToTarget(obj));
            }
        }

        /// <summary>
        /// Compares the value of this instance to a specified TimeOfDay value
        /// and returns an integer that indicates whether this instance is earlier than,
        /// the same as, or later than the specified TimeOfDay value.
        /// </summary>
        /// <param name="other">The object to compare to the current instance</param>
        /// <returns>Value Description Less than zero This instance is earlier than value. 
        /// Zero This instance is the same as value.
        /// Greater than zero This instance is later than value.</returns>
        public int CompareTo(TimeOfDay other)
        {
            return this.timeSpan.CompareTo(other.timeSpan);
        }

        /// <summary>
        /// Converts a specified string representation of a date to <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> with CurrentCulture format.
        /// </summary>
        /// <param name="text">A string that represent a timeofday to convert.</param>
        /// <returns>The <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> instance represented by text</returns>
        public static TimeOfDay Parse(string text)
        {
            return Parse(text, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts a specified string representation of a date to <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/>.
        /// </summary>
        /// <param name="text">A string that represent a timeofday to convert.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about text.</param>
        /// <returns>The <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> instance represented by text</returns>
        public static TimeOfDay Parse(string text, IFormatProvider provider)
        {
            TimeOfDay result;
            if (TimeOfDay.TryParse(text, provider, out result))
            {
                return result;
            }
            else
            {
                throw new FormatException(Strings.TimeOfDay_InvalidParsingString(text));
            }
        }

        /// <summary>
        /// Try converts a specified string representation of a timeofday to <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> with CurrentCulture.
        /// </summary>
        /// <param name="text">A string that represent a timeofday to convert.</param>
        /// <param name="result">A <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> object equivalent to the date input, if the conversion succeeded
        /// or <see cref="Microsoft.OData.Edm.Library.TimeOfDay.MinValue"/>, if the conversion failed.</param>
        /// <returns>True if the input parameter is successfully converted; otherwise, false.</returns>
        public static bool TryParse(string text, out TimeOfDay result)
        {
            return TryParse(text, CultureInfo.CurrentCulture, out result);
        }

        /// <summary>
        /// Try converts a specified string representation of a timeofday to <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/>.
        /// </summary>
        /// <param name="text">A string that represent a timeofday to convert.</param>
        /// <param name="provider">>An object that supplies culture-specific formatting information about text.</param>
        /// <param name="result">A <see cref="Microsoft.OData.Edm.Library.TimeOfDay"/> object equivalent to the date input, if the conversion succeeded
        /// or <see cref="Microsoft.OData.Edm.Library.TimeOfDay.MinValue"/>, if the conversion failed.</param>
        /// <returns>True if the input parameter is successfully converted; otherwise, false.</returns>
        public static bool TryParse(string text, IFormatProvider provider, out TimeOfDay result)
        {
            TimeSpan time;
            bool b;
#if ORCAS
            b = TimeSpan.TryParse(text, out time);
#else
            b = TimeSpan.TryParse(text, provider, out time);
#endif
            if (b && time.Ticks >= MinTickValue && time.Ticks <= MaxTickValue)
            {
                result = new TimeOfDay(time.Ticks);
                return true;
            }

            result = TimeOfDay.MinValue;
            return false;
        }
    }
}
