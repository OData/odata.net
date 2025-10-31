//---------------------------------------------------------------------
// <copyright file="DateAndTimeOfDayTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class DateAndTimeOfDayTests
    {
        #region DateOnly
        [Fact]
        public void TestDateCtor()
        {
            Action test = () => new DateOnly(-2013, 8, 12);

            var exception = Assert.Throws<FormatException>(test);
            Assert.Equal(Error.Format(SRResources.Date_InvalidDateParameters, -2013, 8, 12), exception.Message);
        }

        [Fact]
        public void TestDateToDateTime()
        {
            DateOnly date = new DateOnly(2013, 8, 12);
            DateTime dt = date.ToDateTime(TimeOnly.MinValue);
            Assert.Equal(new DateTime(2013, 8, 12), dt);
        }

        [Fact]
        public void TestDateTimeToDate()
        {
            DateTime dateTime = new DateTime(2013, 8, 12);
            DateOnly d = DateOnly.FromDateTime(dateTime);
            Assert.Equal(new DateOnly(2013, 8, 12), d);
        }

        [Fact]
        public void TestDateAddYears()
        {
            DateOnly date = new DateOnly(2013, 8, 12);
            DateOnly result = date.AddYears(100);
            Assert.Equal(new DateOnly(2113, 8, 12), result);
        }

        [Fact]
        public void TestDateAddYearsInvalidResults()
        {
            DateOnly date = new DateOnly(2013, 8, 12);
            Action test = () => date.AddYears(-5000);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            Assert.Equal(SRResources.Date_InvalidAddedOrSubtractedResults + " (Parameter 'value')", exception.Message);
        }

        [Fact]
        public void TestDateAddYearsInvalidParameters()
        {
            DateOnly date = new DateOnly(2013, 8, 12);
            Action test = () => date.AddYears(12000);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            Assert.Equal(SRResources.Date_InvalidAddedOrSubtractedResults + " (Parameter 'value')", exception.Message);
        }

        [Fact]
        public void TesDateAddMonths()
        {
            DateOnly date = new DateOnly(2013, 8, 12);
            DateOnly result = date.AddMonths(1);
            Assert.Equal(new DateOnly(2013, 9, 12), result);
        }

        [Fact]
        public void TestDateAddMonthsInvalidResults()
        {
            DateOnly date = new DateOnly(1, 1, 1);
            Action test = () => date.AddMonths(-5000);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            Assert.Equal(SRResources.Date_InvalidAddedOrSubtractedResults + " (Parameter 'value')", exception.Message);
        }

        [Fact]
        public void TestDateAddMonthsInvalidParameters()
        {
            DateOnly date = new DateOnly(1, 1, 1);
            Action test = () => date.AddMonths(120001);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            Assert.Equal(SRResources.Date_InvalidMonthsAddedOrSubtractedResults + " (Parameter 'value')", exception.Message);
        }

        [Fact]
        public void TestDateAddDays()
        {
            DateOnly date = new DateOnly(2013, 8, 12);
            DateOnly result = date.AddDays(1);
            Assert.Equal(new DateOnly(2013, 8, 13), result);
        }

        [Fact]
        public void TestDateAddDaysInvalidResults()
        {
            DateOnly date = new DateOnly(1, 1, 1);
            Action test = () => date.AddDays(-2);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            Assert.Equal(SRResources.Date_InvalidAddedOrSubtractedResults + " (Parameter 'value')", exception.Message);
        }

        [Fact]
        public void TestDateAddDaysInvalidParameters()
        {
            DateOnly date = new DateOnly(1, 1, 1);
            Action test = () => date.AddDays(999999999);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            Assert.Equal(SRResources.Date_InvalidAddedOrSubtractedResults + " (Parameter 'value')", exception.Message);
        }

        [Fact]
        public void TestMinDate()
        {
            DateOnly date = DateOnly.MinValue;
            Assert.Equal(new DateOnly(1, 1, 1), date);
        }

        [Fact]
        public void TestMaxDate()
        {
            DateOnly date = DateOnly.MaxValue;
            Assert.Equal(new DateOnly(9999, 12, 31), date);
        }

        [Fact]
        public void TestNowDate()
        {
            DateOnly date = DateOnly.FromDateTime(DateTime.Now);
            DateTime dt = DateTime.Now;
            Assert.Equal(new DateOnly(dt.Year, dt.Month, dt.Day), date);
        }

        [Theory]
        [InlineData("2001-01-2", 2001, 1, 2)]
        [InlineData("2001-1-02", 2001, 1, 2)]
        [InlineData("0001-12-13", 1, 12, 13)]
        public void TestParseDateSuccess(string input, int year, int month, int day)
        {
            var expected = new DateOnly(year, month, day);

            #region Test Parse
            DateOnly date = DateOnly.Parse(input, CultureInfo.InvariantCulture);
            Assert.Equal(expected, date);
            #endregion

            #region Test TryParse
            bool result = DateOnly.TryParse(input, CultureInfo.InvariantCulture, out date);
            Assert.True(result);
            Assert.Equal(expected, date);
            #endregion
        }

        [Theory]
        [InlineData("2001-01-41")]
        [InlineData("2001-13-02")]
        [InlineData("-001-12-13")]
        [InlineData("V001-12-13")]
        [InlineData("2001-00-00")]
        [InlineData("2001-00-")]
        public void TestParseDateFailure(string input)
        {
            #region Test Parse
            Action test = () => DateOnly.Parse(input, CultureInfo.InvariantCulture);
            var exception = Assert.Throws<FormatException>(test);
            Assert.Equal(Error.Format(SRResources.Date_InvalidParsingString, input), exception.Message);
            #endregion

            #region Test TryParse
            DateOnly date;
            bool result = DateOnly.TryParse(input, CultureInfo.InvariantCulture, out date);
            Assert.False(result);
            Assert.Equal(DateOnly.MinValue, date);
            #endregion
        }

        [Fact]
        public void TestDateEquals()
        {
            var list = new List<Tuple<DateOnly, DateOnly, bool>>()
            {
                new Tuple<DateOnly, DateOnly, bool>(DateOnly.MinValue, DateOnly.MinValue, true),
                new Tuple<DateOnly, DateOnly, bool>(DateOnly.MaxValue, DateOnly.MaxValue, true), 
                new Tuple<DateOnly, DateOnly, bool>(new DateOnly(2010, 12, 31), new DateOnly(2010, 12, 31), true),
                new Tuple<DateOnly, DateOnly, bool>(DateOnly.MinValue, DateOnly.MaxValue, false),
                new Tuple<DateOnly, DateOnly, bool>(new DateOnly(2010, 12, 31), new DateOnly(2010, 12, 30), false),
                new Tuple<DateOnly, DateOnly, bool>(new DateOnly(1, 1, 1), DateOnly.MinValue, true),
            };

            foreach (var tuple in list)
            {
                bool result = tuple.Item1.Equals(tuple.Item2);
                Assert.Equal(tuple.Item3, result);
            }
        }

        [Fact]
        public void TestDateEqualsObject()
        {
            var list = new List<Tuple<DateOnly, object, bool>>()
            {
                new Tuple<DateOnly, object, bool>(DateOnly.MinValue, DateOnly.MinValue, true),
                new Tuple<DateOnly, object, bool>(DateOnly.MinValue, TimeOnly.MinValue, false),
            };

            foreach (var tuple in list)
            {
                bool result = tuple.Item1.Equals(tuple.Item2);
                Assert.Equal(tuple.Item3, result);
            }
        }

        [Fact]
        public void TestDateCompareTo()
        {
            var list = new List<Tuple<DateOnly, DateOnly, int>>() 
            {
                new Tuple<DateOnly, DateOnly, int>(DateOnly.MinValue, DateOnly.MaxValue, -1),
                new Tuple<DateOnly, DateOnly, int>(DateOnly.MaxValue, DateOnly.MinValue, 1),
                new Tuple<DateOnly, DateOnly, int>(DateOnly.MinValue, DateOnly.MinValue, 0),
                new Tuple<DateOnly, DateOnly, int>(DateOnly.MaxValue, DateOnly.MaxValue, 0),
                new Tuple<DateOnly, DateOnly, int>(DateOnly.MinValue, new DateOnly(2013, 12, 31), -1),
                new Tuple<DateOnly, DateOnly, int>(new DateOnly(2013, 12, 30), new DateOnly(2013, 12, 31), -1),
                new Tuple<DateOnly, DateOnly, int>(new DateOnly(2013, 12, 31), DateOnly.MaxValue, -1),
                new Tuple<DateOnly, DateOnly, int>(new DateOnly(2013, 12, 31), DateOnly.MinValue, 1),
                new Tuple<DateOnly, DateOnly, int>(new DateOnly(2013, 12, 31), new DateOnly(2013, 12, 30), 1),
                new Tuple<DateOnly, DateOnly, int>(DateOnly.MaxValue, new DateOnly(2013, 12, 31), 1),
                new Tuple<DateOnly, DateOnly, int>(new DateOnly(2013, 12, 31), new DateOnly(2013, 12, 31), 0),
            };

            foreach (var tuple in list)
            {
                int result = tuple.Item1.CompareTo(tuple.Item2);
                Assert.Equal(tuple.Item3, result);
            }
        }

        [Fact]
        public void TestDateCompareToInvalidTarget()
        {
            DateOnly date = new DateOnly(1, 1, 1);
            DateTimeOffset now = DateTimeOffset.Now;
            Action test = () => date.CompareTo(now);

            var exception = Assert.Throws<ArgumentException>(test);
            Assert.Equal(Error.Format(SRResources.Date_InvalidCompareToTarget, now), exception.Message);
        }

        [Fact]
        public void TestDateGetHashCode()
        {
            var list = new List<Tuple<DateOnly, DateOnly, bool>>() 
            {
                new Tuple<DateOnly, DateOnly, bool>(DateOnly.MinValue, DateOnly.MaxValue, false),
                new Tuple<DateOnly, DateOnly, bool>(DateOnly.MinValue, new DateOnly(1, 1, 1), true),
                new Tuple<DateOnly, DateOnly, bool>(new DateOnly(2013, 12, 30), new DateOnly(2013, 12, 31), false),
                new Tuple<DateOnly, DateOnly, bool>(new DateOnly(2013, 12, 31), new DateOnly(2013, 12, 31), true),
            };

            foreach (var tuple in list)
            {
                var hashCode1 = tuple.Item1.GetHashCode();
                var hashCode2 = tuple.Item2.GetHashCode();
                Assert.Equal(tuple.Item3, hashCode1 == hashCode2);
            }
        }

        [Fact]
        public void TestDateToString()
        {
            var list = new List<Tuple<DateOnly, string>>()
            {
                new Tuple<DateOnly, string>(DateOnly.MinValue, "0001-01-01"),
                new Tuple<DateOnly, string>(DateOnly.MaxValue, "9999-12-31"),
                new Tuple<DateOnly, string>(new DateOnly(2014, 12, 31), "2014-12-31"),
            };

            foreach (var tuple in list)
            {
                Assert.Equal(tuple.Item2, tuple.Item1.ToString());
            }
        }

        [Fact]
        public void TestDateOperator()
        {
            DateOnly d1 = new DateOnly(2014, 9, 18);
            DateOnly d2 = new DateOnly(2014, 9, 20);
            DateOnly d3 = new DateOnly(2014, 9, 20);
            DateOnly d4 = new DateOnly(2014, 10, 1);

            Assert.True(d1 < d2);
            Assert.False(d2 < d3);

            Assert.True(d2 == d3);
            Assert.False(d1 == d2);

            Assert.True(d1 <= d2);
            Assert.True(d2 <= d3);
            Assert.False(d4 <= d1);

            Assert.True(d1 != d2);
            Assert.False(d2 != d3);

            Assert.True(d4 > d3);
            Assert.False(d1 > d3);

            Assert.True(d4 >= d3);
            Assert.True(d3 >= d2);
            Assert.False(d1 >= d2);
        }
#endregion

#region TimeOnly
        [Theory]
        [InlineData(1, 1, 1, 1)]
        [InlineData(-1, 1, 1, 1)]
        [InlineData(1, -1, 1, 1)]
        [InlineData(1, 1, -1, 1)]
        [InlineData(1, 1, 1, -1)]
        [InlineData(24, 1, 1, 1)]
        [InlineData(23, 60, 1, 1)]
        [InlineData(23, 59, 60, 1)]
        [InlineData(23, 59, 59, 1000)]
        public void TestTimeOfDayCtorInvalid(int hour, int minute, int second, int millisecond)
        {
            Action test = () => new TimeOnly(hour, minute, second, millisecond);
            var exception = Assert.Throws<FormatException>(test);
            Assert.Equal(Error.Format(SRResources.TimeOfDay_InvalidTimeOfDayParameters, hour, minute, second, millisecond), exception.Message);
        }

        [Theory]
        [InlineData(0 - 1)] // TimeOnly.MinValue.Ticks - 1
        [InlineData(863999999999 + 1)] // TimeOnly.MaxValue.Ticks + 1
        public void TestTimeOfDayTicksCtorInvalid(long ticks)
        {
            Action test = () => new TimeOnly(ticks);
            var exception = Assert.Throws<FormatException>(test);
            Assert.Equal(Error.Format(SRResources.TimeOfDay_TicksOutOfRange, ticks), exception.Message);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(23, 59, 59, 999)]
        [InlineData(12, 0, 0, 0)]
        [InlineData(0, 1, 2, 12)]
        [InlineData(3, 12, 13, 9)]
        [InlineData(23, 59, 59, 1)]
        public void TestTimeOfDayCtor(int hour, int minute, int second, int millisecond)
        {
            var time = new TimeOnly(hour, minute, second, millisecond);
            Assert.Equal(hour, time.Hour);
            Assert.Equal(minute, time.Minute);
            Assert.Equal(second, time.Second);
            Assert.Equal(millisecond, time.Millisecond);
        }

        public static TheoryData<long, TimeOnly> TestTimeOfDayTicksCtorData()
        {
            return new TheoryData<long, TimeOnly>
            {
                { TimeOnly.MinValue.Ticks, new TimeOnly(TimeOnly.MinValue.Ticks) },
                { TimeOnly.MaxValue.Ticks, new TimeOnly(TimeOnly.MaxValue.Ticks) }
            };
        }

        [Theory]
        [MemberData(nameof(TestTimeOfDayTicksCtorData))]
        public void TestTimeOfDayTicksCtor(long ticks, TimeOnly expected)
        {
            var time = new TimeOnly(ticks);

            Assert.Equal(expected, time);
            Assert.Equal(ticks, time.Ticks);
        }

        [Theory]
        [InlineData(12, 59, 0, 123)]
        [InlineData(23, 59, 58, 1)]
        [InlineData(23, 59, 58, 345)]
        [InlineData(0, 0, 0, 0)]
        public void TestTimeOfDayToTimeSpan(int hour, int minute, int second, int millisecond)
        {
            var timeOnly = new TimeOnly(hour, minute, second, millisecond);
            TimeSpan timeSpan = timeOnly.ToTimeSpan();
            Assert.Equal(timeOnly.Ticks, timeSpan.Ticks);
        }

        [Theory]
        [InlineData(0)] // TimeOnly.MaxValue.Ticks
        [InlineData(163999999999)]
        [InlineData(863999999999)] // TimeOnly.MinValue.Ticks
        public void TestTimeOfDayToTimeSpan_ticks(long ticks)
        {
            var timeOnly = new TimeOnly(ticks);
            TimeSpan timeSpan = timeOnly.ToTimeSpan();
            Assert.Equal(timeOnly.Ticks, timeSpan.Ticks);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(123)]
        [InlineData(863999999999)] // TimeOnly.MaxValue.Ticks
        public void TestTimeSpanToTimeOfDay(long ticks)
        {
            var timeSpan = new TimeSpan(ticks);
            TimeOnly timeOfDay = TimeOnly.FromTimeSpan(timeSpan);
            Assert.Equal(timeSpan.Ticks, timeOfDay.Ticks);
        }

        [Theory]
        [InlineData(0 - 1)] // TimeOnly.MinValue.Ticks - 1
        [InlineData(863999999999 + 1)] // TimeOnly.MaxValue.Ticks + 1
        public void TestTimeSpanToTimeOfDayException(long ticks)
        {
            TimeSpan timeSpan = new TimeSpan(ticks);
            Action test = () => { TimeOnly timeOfDay = TimeOnly.FromTimeSpan(timeSpan); };
            var exception = Assert.Throws<FormatException>(test);
            Assert.Equal(Error.Format(SRResources.TimeOfDay_ConvertErrorFromTimeSpan, timeSpan), exception.Message);
        }

        public static TheoryData<TimeOnly, TimeOnly, bool> TestTimeOfDayEqualsData()
        {
            return new TheoryData<TimeOnly, TimeOnly, bool>
            {
                { TimeOnly.MinValue, TimeOnly.MinValue, true },
                { TimeOnly.MaxValue, TimeOnly.MaxValue, true },
                { new TimeOnly(21, 12, 31, 0), new TimeOnly(21, 12, 31, 0), true },
                { TimeOnly.MinValue, TimeOnly.MaxValue, false },
                { new TimeOnly(1, 2, 3, 1), new TimeOnly(1, 2, 3, 10), false },
                { new TimeOnly(TimeOnly.MinValue.Ticks), TimeOnly.MinValue, true },
                { new TimeOnly(TimeOnly.MaxValue.Ticks), TimeOnly.MaxValue, true },
            };
        }

        [Theory]
        [MemberData(nameof(TestTimeOfDayEqualsData))]
        public void TestTimeOfDayEquals(TimeOnly time1, TimeOnly time2, bool expected)
        {
            bool result = time1.Equals(time2);
            Assert.Equal(expected, result);
        }

        public static TheoryData<TimeOnly, object, bool> TestTimeOfDayEqualsObjectData()
        {
            return new TheoryData<TimeOnly, object, bool>
            {
                { TimeOnly.MinValue, TimeOnly.MinValue, true },
                { TimeOnly.MaxValue, DateOnly.MaxValue, false }
            };
        }

        [Theory]
        [MemberData(nameof(TestTimeOfDayEqualsObjectData))]
        public void TestTimeOfDayEqualsObject(TimeOnly time, object someObject, bool expected)
        {
            bool result = time.Equals(someObject);
            Assert.Equal(expected, result);
        }

        public static TheoryData<TimeOnly, TimeOnly, bool> TestTimeOfDayGetHashCodeData()
        {
            return new TheoryData<TimeOnly, TimeOnly, bool>
            {
                { TimeOnly.MinValue, TimeOnly.MaxValue, false },
                { TimeOnly.MinValue, new TimeOnly(0), true },
                { new TimeOnly(1, 2, 3, 0), new TimeOnly(1, 2, 4, 0), false },
                { new TimeOnly(23, 59, 59, 0), new TimeOnly(23, 59, 59, 0), true },
            };
        }

        [Theory]
        [MemberData(nameof(TestTimeOfDayGetHashCodeData))]
        public void TestTimeOfDayGetHashCode(TimeOnly time1, TimeOnly time2, bool expected)
        {
            var hashCode1 = time1.GetHashCode();
            var hashCode2 = time2.GetHashCode();
            Assert.Equal(expected, hashCode1 == hashCode2);
        }

        public static TheoryData<TimeOnly, TimeOnly, int> TestTimeOfDayCompareToData()
        {
            return new TheoryData<TimeOnly, TimeOnly, int>
            {
                {TimeOnly.MinValue, TimeOnly.MaxValue, -1  },
                { TimeOnly.MaxValue, TimeOnly.MinValue, 1 },
                { TimeOnly.MinValue, TimeOnly.MinValue, 0 },
                { TimeOnly.MaxValue, TimeOnly.MaxValue, 0 },
                { TimeOnly.MinValue, new TimeOnly(23, 59, 59, 0), -1 },
                { new TimeOnly(23, 59, 59, 0), new TimeOnly(23, 59, 59, 1), -1 },
                { new TimeOnly(23, 59, 59, 1), new TimeOnly(23, 59, 59, 10), -1 },
                { new TimeOnly(23, 59, 59, 1), TimeOnly.MaxValue, -1 },
                { new TimeOnly(0, 0, 0, 1), TimeOnly.MinValue, 1 },
                { new TimeOnly(23, 59, 59, 100), new TimeOnly(23, 59, 59, 10), 1 },
                { TimeOnly.MinValue, new TimeOnly(0, 0, 0, 0), 0 },
                { TimeOnly.MaxValue, new TimeOnly(23, 59, 59, 999), 1 },
                { new TimeOnly(12, 13, 14, 15), new TimeOnly(12, 13, 14, 15), 0 },
            };
        }

        [Theory]
        [MemberData(nameof(TestTimeOfDayCompareToData))]
        public void TestTimeOfDayCompareTo(TimeOnly time1, TimeOnly time2, int expected)
        {
            int result = time1.CompareTo(time2);
            Assert.Equal(expected, result);
        }

        public static TheoryData<TimeOnly, string> TestTimeOfDayToStringData()
        {
            return new TheoryData<TimeOnly, string>
            {
                { TimeOnly.MinValue, "00:00:00.0000000" },
                { TimeOnly.MaxValue, "23:59:59.9999999" },
                { new TimeOnly(12, 0, 1, 1), "12:00:01.0010000" },
                { new TimeOnly(1, 0, 1, 10), "01:00:01.0100000" },
                { new TimeOnly(2, 1, 2, 100), "02:01:02.1000000" },
                { new TimeOnly(10, 12, 30, 100), "10:12:30.1000000" },
            };
        }

        [Theory]
        [MemberData(nameof(TestTimeOfDayToStringData))]
        public void TestTimeOfDayToString(TimeOnly input, string expected)
        {
            Assert.Equal(expected, input.ToString());
        }

        public static TheoryData<string, TimeOnly> TestParseTimeOfDaySuccessData()
        {
            return new TheoryData<string, TimeOnly>
            {
                { "00:00:00.0000000", TimeOnly.MinValue },
                { "0:0:0.0", TimeOnly.MinValue },
                { "23:59:59.9999999", TimeOnly.MaxValue },
                { "12:00:01.0010000", new TimeOnly(12, 0, 1, 1) },
                { "12:0:1.0010000", new TimeOnly(12, 0, 1, 1) },
                { "01:00:01.0100000", new TimeOnly(1, 0, 1, 10) },
                { "1:0:1.01", new TimeOnly(1, 0, 1, 10) },
                { "02:01:02.01", new TimeOnly(2, 1, 2, 10) },
                { "2:1:2.01", new TimeOnly(2, 1, 2, 10) },
                { "10:12:30.1000000", new TimeOnly(10, 12, 30, 100) },
                { "10:12:30.1", new TimeOnly(10, 12, 30, 100) },
                { "00:00:00.0000001", new TimeOnly(1) },
                { "10:00", new TimeOnly(10, 0, 0, 0) },
                { "13:30:25", new TimeOnly(13, 30, 25, 0) },
            };
        }

        [Theory]
        [MemberData(nameof(TestParseTimeOfDaySuccessData))]
        public void TestParseTimeOfDaySuccess(string input, TimeOnly expected)
        {
            #region Test Parse
            TimeOnly time = TimeOnly.Parse(input);
            Assert.Equal(expected, time);
            #endregion

            #region Test TryParse
            bool result = TimeOnly.TryParse(input, out time);
            Assert.True(result);
            Assert.Equal(expected, time);
            #endregion
        }

        [Theory]
        [InlineData("12:15:60.0000000")]
        [InlineData("12:60:59.0000000")]
        [InlineData("24:59:59.0000000")]
        [InlineData("124:59:59.0000000")]
        [InlineData("-1:59:59.0000000")]
        [InlineData("T4:59:59.0000000")]
        [InlineData("14:59:59.")]
        [InlineData("4:59:")]
        public void TestParseTimeOfDayFailure(string input)
        {
            #region Test Parse
            Action test = () => TimeOnly.Parse(input);
            var exception = Assert.Throws<FormatException>(test);
            Assert.Equal(Error.Format(SRResources.TimeOfDay_InvalidParsingString, input), exception.Message);
            #endregion

            #region Test TryParse
            TimeOnly time;
            bool result = TimeOnly.TryParse(input, out time);
            Assert.False(result);
            Assert.Equal(TimeOnly.MinValue, time);
            #endregion
        }

        [Fact]
        public void TestTimeOfDayCompareToInvalidTarget()
        {
            TimeOnly time = new TimeOnly(0);
            DateTimeOffset now = DateTimeOffset.Now;
            Action test = () => time.CompareTo(now);
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.Equal(Error.Format(SRResources.TimeOfDay_InvalidCompareToTarget, now), exception.Message);
        }
        [Fact]
        public void TestTimeOfDayOperator()
        {
            TimeOnly t1 = new TimeOnly(14, 9, 18, 0);
            TimeOnly t2 = new TimeOnly(14, 9, 20, 0);
            TimeOnly t3 = new TimeOnly(14, 9, 20, 0);
            TimeOnly t4 = new TimeOnly(14, 10, 1, 0);

            Assert.True(t1 < t2);
            Assert.False(t2 < t3);

            Assert.True(t2 == t3);
            Assert.False(t1 == t2);

            Assert.True(t1 <= t2);
            Assert.True(t2 <= t3);
            Assert.False(t4 <= t1);

            Assert.True(t1 != t2);
            Assert.False(t2 != t3);

            Assert.True(t4 > t3);
            Assert.False(t1 > t3);

            Assert.True(t4 >= t3);
            Assert.True(t3 >= t2);
            Assert.False(t1 >= t2);
        }
#endregion
    }
}