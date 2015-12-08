//---------------------------------------------------------------------
// <copyright file="DateAndTimeOfDayTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class DateAndTimeOfDayTests
    {
        #region Date
        [Fact]
        public void TestDateCtor()
        {
            Action test = () => new Date(-2013, 8, 12);
            test.ShouldThrow<FormatException>().WithMessage(Strings.Date_InvalidDateParameters(-2013, 8, 12));
        }

        [Fact]
        public void TestDateToDateTime()
        {
            Date date = new Date(2013, 8, 12);
            DateTime dt = date;
            dt.Should().Be(new DateTime(2013, 8, 12));
        }

        [Fact]
        public void TestDateTimeToDate()
        {
            DateTime dateTime = new DateTime(2013, 8, 12);
            Date d = dateTime;
            d.Should().Be(new Date(2013, 8, 12));
        }

        [Fact]
        public void TestDateAddYears()
        {
            Date date = new Date(2013, 8, 12);
            Date result = date.AddYears(100);
            result.Should().Be(new Date(2113, 8, 12));
        }

        [Fact]
        public void TestDateAddYearsInvalidResults()
        {
            Date date = new Date(2013, 8, 12);
            Action test = () => date.AddYears(-5000);
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.Date_InvalidAddedOrSubtractedResults + "\r\nParameter name: value");
        }

        [Fact]
        public void TestDateAddYearsInvalidParmeters()
        {
            Date date = new Date(2013, 8, 12);
            Action test = () => date.AddYears(12000);
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.Date_InvalidAddedOrSubtractedResults + "\r\nParameter name: value");
        }

        [Fact]
        public void TesDatetAddMonths()
        {
            Date date = new Date(2013, 8, 12);
            Date result = date.AddMonths(1);
            result.Should().Be(new Date(2013, 9, 12));
        }

        [Fact]
        public void TestDateAddMonthsInvalidResults()
        {
            Date date = new Date(1, 1, 1);
            Action test = () => date.AddMonths(-5000);
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.Date_InvalidAddedOrSubtractedResults + "\r\nParameter name: value");
        }

        [Fact]
        public void TestDateAddMonthsInvalidParmeters()
        {
            Date date = new Date(1, 1, 1);
            Action test = () => date.AddMonths(120001);
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.Date_InvalidAddedOrSubtractedResults + "\r\nParameter name: value");
        }

        [Fact]
        public void TestDateAddDays()
        {
            Date date = new Date(2013, 8, 12);
            Date result = date.AddDays(1);
            result.Should().Be(new Date(2013, 8, 13));
        }

        [Fact]
        public void TestDateAddDaysInvalidResults()
        {
            Date date = new Date(1, 1, 1);
            Action test = () => date.AddDays(-2);
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.Date_InvalidAddedOrSubtractedResults + "\r\nParameter name: value");
        }

        [Fact]
        public void TestDateAddDaysInvalidParmeters()
        {
            Date date = new Date(1, 1, 1);
            Action test = () => date.AddDays(999999999);
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.Date_InvalidAddedOrSubtractedResults + "\r\nParameter name: value");
        }

        [Fact]
        public void TestMinDate()
        {
            Date date = Date.MinValue;
            date.Should().Be(new Date(1, 1, 1));
        }

        [Fact]
        public void TestMaxDate()
        {
            Date date = Date.MaxValue;
            date.Should().Be(new Date(9999, 12, 31));
        }

        [Fact]
        public void TestNowDate()
        {
            Date date = Date.Now;
            DateTime dt = DateTime.Now;
            date.Should().Be(new Date(dt.Year, dt.Month, dt.Day));
        }

        [Fact]
        public void TestParseDateSuccess()
        {
            var lists = new List<Tuple<string, Date>>()
            {
                new Tuple<string, Date>("2001-01-2", new Date(2001, 1, 2)),
                new Tuple<string, Date>("2001-1-02", new Date(2001, 1, 2)),
                new Tuple<string, Date>("0001-12-13", new Date(1, 12, 13)),
            };

            #region Test Parse
            foreach (var tuple in lists)
            {
                Date date = Date.Parse(tuple.Item1, CultureInfo.InvariantCulture);
                date.Should().Be(tuple.Item2);
            }
            #endregion

            #region Test TryParse
            foreach (var tuple in lists)
            {
                Date date;
                bool result = Date.TryParse(tuple.Item1, CultureInfo.InvariantCulture, out date);
                result.Should().Be(true);
                date.Should().Be(tuple.Item2);
            }
            #endregion
        }

        [Fact]
        public void TestParseDateFailure()
        {
            var lists = new List<Tuple<string, Date>>()
            {
                new Tuple<string, Date>("2001-01-41", Date.MinValue),
                new Tuple<string, Date>("2001-13-02", Date.MinValue),
                new Tuple<string, Date>("-001-12-13", Date.MinValue),
                new Tuple<string, Date>("V001-12-13", Date.MinValue),
                new Tuple<string, Date>("2001-00-00", Date.MinValue),
                new Tuple<string, Date>("2001-00-", Date.MinValue),
            };

            #region Test Parse
            foreach (var tuple in lists)
            {
                Action test = () => Date.Parse(tuple.Item1, CultureInfo.InvariantCulture);
                test.ShouldThrow<FormatException>().WithMessage(Strings.Date_InvalidParsingString(tuple.Item1));
            }
            #endregion

            #region Test TryParse
            foreach (var tuple in lists)
            {
                Date date;
                bool result = Date.TryParse(tuple.Item1, CultureInfo.InvariantCulture, out date);
                result.Should().Be(false);
                date.Should().Be(tuple.Item2);
            }
            #endregion
        }

        [Fact]
        public void TestDateEquals()
        {
            var list = new List<Tuple<Date, Date, bool>>()
            {
                new Tuple<Date, Date, bool>(Date.MinValue, Date.MinValue, true),
                new Tuple<Date, Date, bool>(Date.MaxValue, Date.MaxValue, true), 
                new Tuple<Date, Date, bool>(new Date(2010, 12, 31), new Date(2010, 12, 31), true),
                new Tuple<Date, Date, bool>(Date.MinValue, Date.MaxValue, false),
                new Tuple<Date, Date, bool>(new Date(2010, 12, 31), new Date(2010, 12, 30), false),
                new Tuple<Date, Date, bool>(new Date(1, 1, 1), Date.MinValue, true),
            };

            foreach (var tuple in list)
            {
                bool result = tuple.Item1.Equals(tuple.Item2);
                result.Should().Be(tuple.Item3);
            }
        }

        [Fact]
        public void TestDateEqualsObject()
        {
            var list = new List<Tuple<Date, object, bool>>()
            {
                new Tuple<Date, object, bool>(Date.MinValue, Date.MinValue, true),
                new Tuple<Date, object, bool>(Date.MinValue, TimeOfDay.MinValue, false),
            };

            foreach (var tuple in list)
            {
                bool result = tuple.Item1.Equals(tuple.Item2);
                result.Should().Be(tuple.Item3);
            }
        }

        [Fact]
        public void TestDateCompareTo()
        {
            var list = new List<Tuple<Date, Date, int>>() 
            {
                new Tuple<Date, Date, int>(Date.MinValue, Date.MaxValue, -1),
                new Tuple<Date, Date, int>(Date.MaxValue, Date.MinValue, 1),
                new Tuple<Date, Date, int>(Date.MinValue, Date.MinValue, 0),
                new Tuple<Date, Date, int>(Date.MaxValue, Date.MaxValue, 0),
                new Tuple<Date, Date, int>(Date.MinValue, new Date(2013, 12, 31), -1),
                new Tuple<Date, Date, int>(new Date(2013, 12, 30), new Date(2013, 12, 31), -1),
                new Tuple<Date, Date, int>(new Date(2013, 12, 31), Date.MaxValue, -1),
                new Tuple<Date, Date, int>(new Date(2013, 12, 31), Date.MinValue, 1),
                new Tuple<Date, Date, int>(new Date(2013, 12, 31), new Date(2013, 12, 30), 1),
                new Tuple<Date, Date, int>(Date.MaxValue, new Date(2013, 12, 31), 1),
                new Tuple<Date, Date, int>(new Date(2013, 12, 31), new Date(2013, 12, 31), 0),
            };

            foreach (var tuple in list)
            {
                int result = tuple.Item1.CompareTo(tuple.Item2);
                result.Should().Be(tuple.Item3);
            }
        }

        [Fact]
        public void TestDateCompareToInvalidTarget()
        {
            Date date = new Date(1, 1, 1);
            DateTimeOffset now = DateTimeOffset.Now;
            Action test = () => date.CompareTo(now);
            test.ShouldThrow<ArgumentException>().WithMessage(Strings.Date_InvalidCompareToTarget(now));
        }

        [Fact]
        public void TestDateGetHashCode()
        {
            var list = new List<Tuple<Date, Date, bool>>() 
            {
                new Tuple<Date, Date, bool>(Date.MinValue, Date.MaxValue, false),
                new Tuple<Date, Date, bool>(Date.MinValue, new Date(1, 1, 1), true),
                new Tuple<Date, Date, bool>(new Date(2013, 12, 30), new Date(2013, 12, 31), false),
                new Tuple<Date, Date, bool>(new Date(2013, 12, 31), new Date(2013, 12, 31), true),
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
            var list = new List<Tuple<Date, string>>()
            {
                new Tuple<Date, string>(Date.MinValue, "0001-01-01"),
                new Tuple<Date, string>(Date.MaxValue, "9999-12-31"),
                new Tuple<Date, string>(new Date(2014, 12, 31), "2014-12-31"),
            };

            foreach (var tuple in list)
            {
                Assert.Equal(tuple.Item2, tuple.Item1.ToString());
            }
        }

        [Fact]
        public void TestDateOperator()
        {
            Date d1 = new Date(2014, 9, 18);
            Date d2 = new Date(2014, 9, 20);
            Date d3 = new Date(2014, 9, 20);
            Date d4 = new Date(2014, 10, 1);

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

        #region TimeOfDay
        [Fact]
        public void TestTimeOfDayCtorInvalid()
        {
            var list = new List<Tuple<int, int, int, int>>()
            {
                new Tuple<int, int, int, int>(-1, 1, 1, 1),
                new Tuple<int, int, int, int>(1, -1, 1, 1),
                new Tuple<int, int, int, int>(1, 1, -1, 1),
                new Tuple<int, int, int, int>(1, 1, 1, -1),
                new Tuple<int, int, int, int>(24, 1, 1, 1),
                new Tuple<int, int, int, int>(23, 60, 1, 1),
                new Tuple<int, int, int, int>(23, 59, 60, 1),
                new Tuple<int, int, int, int>(23, 59, 59, 1000),
            };

            foreach (var tuple in list)
            {
                Action test = () => new TimeOfDay(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
                test.ShouldThrow<FormatException>().WithMessage(Strings.TimeOfDay_InvalidTimeOfDayParameters(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));
            }
        }

        [Fact]
        public void TestTimeOfDayTicksCtorInvalid()
        {
            var list = new List<long> { TimeOfDay.MinTickValue - 1, TimeOfDay.MaxTickValue + 1 };
            foreach (var value in list)
            {
                Action test = () => new TimeOfDay(value);
                test.ShouldThrow<FormatException>().WithMessage(Strings.TimeOfDay_TicksOutOfRange(value));
            }
        }

        [Fact]
        public void TestTimeOfDayCtor()
        {
            var list = new List<Tuple<int, int, int, int>>()
            {
                new Tuple<int, int, int, int>(0, 0, 0, 0),
                new Tuple<int, int, int, int>(23, 59, 59, 999),
                new Tuple<int, int, int, int>(12, 0, 0, 0),
                new Tuple<int, int, int, int>(0, 1, 2, 12),
                new Tuple<int, int, int, int>(3, 12, 13, 9),
                new Tuple<int, int, int, int>(23, 59, 59, 1),
            };

            foreach (var tuple in list)
            {
                var time = new TimeOfDay(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
                Assert.Equal(tuple.Item1, time.Hours);
                Assert.Equal(tuple.Item2, time.Minutes);
                Assert.Equal(tuple.Item3, time.Seconds);
                Assert.Equal(tuple.Item4, time.Milliseconds);
            }
        }

        [Fact]
        public void TestTimeOfDayTicksCtor()
        {
            var list = new List<Tuple<long, TimeOfDay>>()
            {
                new Tuple<long, TimeOfDay>(TimeOfDay.MinTickValue, new TimeOfDay(TimeOfDay.MinTickValue)),
                new Tuple<long, TimeOfDay>(TimeOfDay.MaxTickValue, new TimeOfDay(TimeOfDay.MaxTickValue)),
            };

            foreach (var tuple in list)
            {
                var time = new TimeOfDay(tuple.Item1);
                Assert.Equal(tuple.Item2, time);
                Assert.Equal(tuple.Item1, time.Ticks);
            }
        }

        [Fact]
        public void TestTimeOfDayToTimeSpan()
        {
            var list = new List<TimeOfDay>()
            {
                new TimeOfDay(12, 59, 0, 123),
                new TimeOfDay(23, 59, 58, 1),
                new TimeOfDay(23, 59, 58, 345),
                new TimeOfDay(0, 0, 0, 0),
                new TimeOfDay(163999999999),
                new TimeOfDay(TimeOfDay.MaxTickValue),
                new TimeOfDay(TimeOfDay.MinTickValue),
            };

            foreach (var timeOfDay in list)
            {
                TimeSpan timeSpan = timeOfDay;
                Assert.Equal(timeOfDay.Ticks, timeSpan.Ticks);
            }
        }

        [Fact]
        public void TestTimeSpanToTimeOfDay()
        {
            var list = new List<TimeSpan>()
            {
                new TimeSpan(0), 
                new TimeSpan(123), 
                new TimeSpan(TimeOfDay.MaxTickValue),
            };

            foreach (var timeSpan in list)
            {
                TimeOfDay timeOfDay = timeSpan;
                Assert.Equal(timeSpan.Ticks, timeOfDay.Ticks);
            }
        }

        [Fact]
        public void TestTimeSpanToTimeOfDayException()
        {
            var list = new List<long>() { TimeOfDay.MinTickValue - 1, TimeOfDay.MaxTickValue + 1 };
            foreach (var value in list)
            {
                TimeSpan timeSpan = new TimeSpan(value);
                Action test = () => { TimeOfDay timeOfDay = timeSpan; };
                test.ShouldThrow<FormatException>().WithMessage(Strings.TimeOfDay_ConvertErrorFromTimeSpan(timeSpan));
            }
        }

        [Fact]
        public void TestTimeOfDayEquals()
        {
            var list = new List<Tuple<TimeOfDay, TimeOfDay, bool>>()
            {
                new Tuple<TimeOfDay, TimeOfDay, bool>(TimeOfDay.MinValue, TimeOfDay.MinValue, true),
                new Tuple<TimeOfDay, TimeOfDay, bool>(TimeOfDay.MaxValue, TimeOfDay.MaxValue, true), 
                new Tuple<TimeOfDay, TimeOfDay, bool>(new TimeOfDay(21, 12, 31, 0), new TimeOfDay(21, 12, 31, 0), true),
                new Tuple<TimeOfDay, TimeOfDay, bool>(TimeOfDay.MinValue, TimeOfDay.MaxValue, false),
                new Tuple<TimeOfDay, TimeOfDay, bool>(new TimeOfDay(1, 2, 3, 1), new TimeOfDay(1, 2, 3, 10), false),
                new Tuple<TimeOfDay, TimeOfDay, bool>(new TimeOfDay(TimeOfDay.MinTickValue), TimeOfDay.MinValue, true),
                new Tuple<TimeOfDay, TimeOfDay, bool>(new TimeOfDay(TimeOfDay.MaxTickValue), TimeOfDay.MaxValue, true),
            };

            foreach (var tuple in list)
            {
                bool result = tuple.Item1.Equals(tuple.Item2);
                result.Should().Be(tuple.Item3);
            }
        }

        [Fact]
        public void TestTimeOfDayEqualsObject()
        {
            var list = new List<Tuple<TimeOfDay, object, bool>>()
            {
                new Tuple<TimeOfDay, object, bool>(TimeOfDay.MinValue, TimeOfDay.MinValue, true),
                new Tuple<TimeOfDay, object, bool>(TimeOfDay.MaxValue, Date.MaxValue, false), 
            };

            foreach (var tuple in list)
            {
                bool result = tuple.Item1.Equals(tuple.Item2);
                result.Should().Be(tuple.Item3);
            }
        }

        [Fact]
        public void TestTimeOfDayGetHashCode()
        {
            var list = new List<Tuple<TimeOfDay, TimeOfDay, bool>>() 
            {
                new Tuple<TimeOfDay, TimeOfDay, bool>(TimeOfDay.MinValue, TimeOfDay.MaxValue, false),
                new Tuple<TimeOfDay, TimeOfDay, bool>(TimeOfDay.MinValue, new TimeOfDay(0), true),
                new Tuple<TimeOfDay, TimeOfDay, bool>(new TimeOfDay(1, 2, 3, 0), new TimeOfDay(1, 2, 4, 0), false),
                new Tuple<TimeOfDay, TimeOfDay, bool>(new TimeOfDay(23, 59, 59, 0), new TimeOfDay(23, 59, 59, 0), true),
            };

            foreach (var tuple in list)
            {
                var hashCode1 = tuple.Item1.GetHashCode();
                var hashCode2 = tuple.Item2.GetHashCode();
                Assert.Equal<bool>(tuple.Item3, hashCode1 == hashCode2);
            }
        }

        [Fact]
        public void TestTimeOfDayCompareTo()
        {
            var list = new List<Tuple<TimeOfDay, TimeOfDay, int>>() 
            {
                new Tuple<TimeOfDay, TimeOfDay, int>(TimeOfDay.MinValue, TimeOfDay.MaxValue, -1),
                new Tuple<TimeOfDay, TimeOfDay, int>(TimeOfDay.MaxValue, TimeOfDay.MinValue, 1),
                new Tuple<TimeOfDay, TimeOfDay, int>(TimeOfDay.MinValue, TimeOfDay.MinValue, 0),
                new Tuple<TimeOfDay, TimeOfDay, int>(TimeOfDay.MaxValue, TimeOfDay.MaxValue, 0),
                new Tuple<TimeOfDay, TimeOfDay, int>(TimeOfDay.MinValue, new TimeOfDay(23, 59, 59, 0), -1),
                new Tuple<TimeOfDay, TimeOfDay, int>(new TimeOfDay(23, 59, 59, 0), new TimeOfDay(23, 59, 59, 1), -1),
                new Tuple<TimeOfDay, TimeOfDay, int>(new TimeOfDay(23, 59, 59, 1), new TimeOfDay(23, 59, 59, 10), -1),
                new Tuple<TimeOfDay, TimeOfDay, int>(new TimeOfDay(23, 59, 59, 1), TimeOfDay.MaxValue, -1),
                new Tuple<TimeOfDay, TimeOfDay, int>(new TimeOfDay(0, 0, 0, 1), TimeOfDay.MinValue, 1),
                new Tuple<TimeOfDay, TimeOfDay, int>(new TimeOfDay(23, 59, 59, 100), new TimeOfDay(23, 59, 59, 10), 1),
                new Tuple<TimeOfDay, TimeOfDay, int>(TimeOfDay.MinValue, new TimeOfDay(0, 0, 0, 0), 0),
                new Tuple<TimeOfDay, TimeOfDay, int>(TimeOfDay.MaxValue, new TimeOfDay(23, 59, 59, 999), 1),
                new Tuple<TimeOfDay, TimeOfDay, int>(new TimeOfDay(12, 13, 14, 15), new TimeOfDay(12, 13, 14, 15), 0),
            };

            foreach (var tuple in list)
            {
                int result = tuple.Item1.CompareTo(tuple.Item2);
                result.Should().Be(tuple.Item3);
            }
        }

        [Fact]
        public void TestTimeOfDayToString()
        {
            var list = new List<Tuple<TimeOfDay, string>>()
            {
                new Tuple<TimeOfDay, string>(TimeOfDay.MinValue, "00:00:00.0000000"),
                new Tuple<TimeOfDay, string>(TimeOfDay.MaxValue, "23:59:59.9999999"),
                new Tuple<TimeOfDay, string>(new TimeOfDay(12, 0, 1, 1), "12:00:01.0010000"),
                new Tuple<TimeOfDay, string>(new TimeOfDay(1, 0, 1, 10), "01:00:01.0100000"),
                new Tuple<TimeOfDay, string>(new TimeOfDay(2, 1, 2, 100), "02:01:02.1000000"),
                new Tuple<TimeOfDay, string>(new TimeOfDay(10, 12, 30, 100), "10:12:30.1000000"),
            };

            foreach (var tuple in list)
            {
                Assert.Equal(tuple.Item2, tuple.Item1.ToString());
            }
        }

        [Fact]
        public void TestParseTimeOfDaySuccess()
        {
            var lists = new List<Tuple<string, TimeOfDay>>()
            {
                new Tuple<string, TimeOfDay>("00:00:00.0000000", TimeOfDay.MinValue),
                new Tuple<string, TimeOfDay>("0:0:0.0", TimeOfDay.MinValue),
                new Tuple<string, TimeOfDay>("23:59:59.9999999", TimeOfDay.MaxValue),
                new Tuple<string, TimeOfDay>("12:00:01.0010000", new TimeOfDay(12, 0, 1, 1)),
                new Tuple<string, TimeOfDay>("12:0:1.0010000", new TimeOfDay(12, 0, 1, 1)),
                new Tuple<string, TimeOfDay>("01:00:01.0100000", new TimeOfDay(1, 0, 1, 10)),
                new Tuple<string, TimeOfDay>("1:0:1.01", new TimeOfDay(1, 0, 1, 10)),
                new Tuple<string, TimeOfDay>("02:01:02.01", new TimeOfDay(2, 1, 2, 10)),
                new Tuple<string, TimeOfDay>("2:1:2.01", new TimeOfDay(2, 1, 2, 10)),
                new Tuple<string, TimeOfDay>("10:12:30.1000000", new TimeOfDay(10, 12, 30, 100)),
                new Tuple<string, TimeOfDay>("10:12:30.1", new TimeOfDay(10, 12, 30, 100)),
                new Tuple<string, TimeOfDay>("00:00:00.0000001", new TimeOfDay(1)),
                new Tuple<string, TimeOfDay>("10:00", new TimeOfDay(10, 0, 0, 0)),
                new Tuple<string, TimeOfDay>("13:30:25", new TimeOfDay(13, 30, 25, 0)),
            };

            #region Test Parse
            foreach (var tuple in lists)
            {
                TimeOfDay time = TimeOfDay.Parse(tuple.Item1);
                time.Should().Be(tuple.Item2);
            }
            #endregion

            #region Test TryParse
            foreach (var tuple in lists)
            {
                TimeOfDay time;
                bool result = TimeOfDay.TryParse(tuple.Item1, out time);
                result.Should().Be(true);
                time.Should().Be(tuple.Item2);
            }
            #endregion
        }

        [Fact]
        public void TestParseTimeOfDayFailure()
        {
            var lists = new List<Tuple<string, TimeOfDay>>()
            {
                new Tuple<string, TimeOfDay>("12:15:60.0000000", TimeOfDay.MinValue),
                new Tuple<string, TimeOfDay>("12:60:59.0000000", TimeOfDay.MinValue),
                new Tuple<string, TimeOfDay>("24:59:59.0000000", TimeOfDay.MinValue),
                new Tuple<string, TimeOfDay>("124:59:59.0000000", TimeOfDay.MinValue),
                new Tuple<string, TimeOfDay>("-1:59:59.0000000", TimeOfDay.MinValue),
                new Tuple<string, TimeOfDay>("T4:59:59.0000000", TimeOfDay.MinValue),
                new Tuple<string, TimeOfDay>("14:59:59.", TimeOfDay.MinValue),
                new Tuple<string, TimeOfDay>("4:59:", TimeOfDay.MinValue),
            };

            #region Test Parse
            foreach (var tuple in lists)
            {
                Action test = () => TimeOfDay.Parse(tuple.Item1);
                test.ShouldThrow<FormatException>().WithMessage(Strings.TimeOfDay_InvalidParsingString(tuple.Item1));
            }
            #endregion

            #region Test TryParse
            foreach (var tuple in lists)
            {
                TimeOfDay time;
                bool result = TimeOfDay.TryParse(tuple.Item1, out time);
                result.Should().Be(false);
                time.Should().Be(tuple.Item2);
            }
            #endregion
        }

        [Fact]
        public void TestTimeOfDayCompareToInvalidTarget()
        {
            TimeOfDay time = new TimeOfDay(0);
            DateTimeOffset now = DateTimeOffset.Now;
            Action test = () => time.CompareTo(now);
            test.ShouldThrow<ArgumentException>().WithMessage(Strings.TimeOfDay_InvalidCompareToTarget(now));
        }
        [Fact]
        public void TestTimeOfDayOperator()
        {
            TimeOfDay t1 = new TimeOfDay(14, 9, 18, 0);
            TimeOfDay t2 = new TimeOfDay(14, 9, 20, 0);
            TimeOfDay t3 = new TimeOfDay(14, 9, 20, 0);
            TimeOfDay t4 = new TimeOfDay(14, 10, 1, 0);

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