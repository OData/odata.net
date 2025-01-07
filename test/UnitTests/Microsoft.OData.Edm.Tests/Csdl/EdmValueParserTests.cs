//---------------------------------------------------------------------
// <copyright file="EdmValueParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class EdmValueParserTests
    {
        #region Date
        [Fact]
        public void ParseDateNullShouldThrowFormatException()
        {
            Date? result;
            Assert.False(EdmValueParser.TryParseDate(null, out result));
        }

        [Fact]
        public void ParseDateWithEmptyStringShouldThrowFormatException()
        {
            Date? result;
            Assert.False(EdmValueParser.TryParseDate(string.Empty, out result));
        }

        [Fact]
        public void ParseDateWithSpaceShouldThrowFormatException()
        {
            Date? result;
            Assert.False(EdmValueParser.TryParseDate(" ", out result));
        }

        [Fact]
        public void ParseDateWithInvalidParameterShouldThrowFormatException()
        {
            var invalidDates = new[]
            { 
                 "0000-01-01",
                 "0001-13-12",
                 "0001-12-98",
                 "0001-12-98",
                 "-0000-01-01",
                 "-0001-13-12",
                 "-0001-12-98",
                 "-0001-12-98",
                 "99999-01-01",
                 "-99999-01-01",
                 "-0000-01-01",
                 "2001-02-29",
                 "2001-04-31",
                 "-0001-01-01",
                 "-9999-12-12",
                 "001-01-01",
                 "01-01-01",
                 "1-01-01",
            };

            foreach (var invalidDate in invalidDates)
            {
                Date? result;
                Assert.False(EdmValueParser.TryParseDate(invalidDate, out result));
            }
        }

        [Fact]
        public void TryParseDateWithValidParameterShouldParseCorrectly()
        {
            Date? result;
            Assert.True(EdmValueParser.TryParseDate("2012-07-28", out result));
            Assert.Equal(new Date(2012, 07, 28), result);
        }
        #endregion

        #region DateTimeOffset
        /// <summary>
        ///A test for TryParseDateTimeOffset
        ///</summary>
        [Fact]
        public void TryParseDateTimeOffsetThatProvidesEverythingShouldParseCorrectly()
        {
            DateTimeOffset? result;
            Assert.True(EdmValueParser.TryParseDateTimeOffset("2012-07-28T13:22:16.123-07:15", out result));
            Assert.Equal(new DateTimeOffset(2012, 7, 28, 13, 22, 16, 123, new TimeSpan(-7, -15, 0)), result);
        }

        [Fact]
        public void TryParseDateTimeOffsetWithInvalidFormatShouldReturnFalse()
        {
            DateTimeOffset? result;
            Assert.False(EdmValueParser.TryParseDateTimeOffset("0001+01+01T00:00:00.000+00:01", out result));
            Assert.Null(result);
        }

        [Fact]
        public void TryParseDateTimeOffsetThatOverFlowsShouldReturnFalse()
        {
            DateTimeOffset? result;
            Assert.False(EdmValueParser.TryParseDateTimeOffset("0001-01-01T00:00:00.000+00:01", out result));
            Assert.Null(result);
        }
        #endregion

        #region Duration

        #region Validation of XmlConvert in Boundary situation
        // EdmValueParser.ParseDuration uses XmlConvert.ToTimeSpan to parse the values, so this unit tests are not strictly needed, 
        // Adding these tests due to issues we found previously when parsing DateTime values, and to prevent regressions.

        [Fact]
        public void ParseDurationWith1DecimalPlaceShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.9S");
            Assert.Equal(new TimeSpan(9000000), result);
        }

        [Fact]
        public void ParseDurationWith2DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.09S");
            Assert.Equal(new TimeSpan(900000), result);
        }

        [Fact]
        public void ParseDurationWith3DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.009S");
            Assert.Equal(new TimeSpan(90000), result);
        }

        [Fact]
        public void ParseDurationWith4DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.0009S");
            Assert.Equal(new TimeSpan(9000), result);
        }

        [Fact]
        public void ParseDurationWith5DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.00009S");
            Assert.Equal(new TimeSpan(900), result);
        }

        [Fact]
        public void ParseDurationWith6DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.000009S");
            Assert.Equal(new TimeSpan(90), result);
        }

        [Fact]
        public void ParseDurationWith7DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.0000009S");
            Assert.Equal(new TimeSpan(9), result);
        }

        [Fact]
        public void ParseDurationWithMoreThan7DecimalPlacesShouldNotLosePrecisionUpTo7()
        {
            // 12 is the max precison supported in OData protocol, but Clr TimeSpan only supports up to 7 decimal places
            TimeSpan result = EdmValueParser.ParseDuration("PT0.123456789012S");
            Assert.Equal(new TimeSpan(1234567), result);
        }

        [Fact]
        public void ParseDurationWithTrailingSpaces()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT1S  ");
            Assert.Equal(new TimeSpan(0, 0, 1), result);
        }

        [Fact]
        public void ParseDurationWithLeadingSpaces()
        {
            TimeSpan result = EdmValueParser.ParseDuration("  PT1S");
            Assert.Equal(new TimeSpan(0, 0, 1), result);
        }

        [Fact]
        public void ParseDurationWithMaxValueShouldReturnCorrectTimeSpan()
        {
            TimeSpan result = EdmValueParser.ParseDuration("P10675199DT2H48M5.4775807S");
            Assert.Equal(TimeSpan.MaxValue, result);
        }

        [Fact]
        public void ParseDurationWithMaxValueMinusOneShouldReturnCorrectTimeSpan()
        {
            TimeSpan result = EdmValueParser.ParseDuration("P10675199DT2H48M5.4775806S");
            Assert.Equal(TimeSpan.MaxValue - new TimeSpan(1), result);
        }

        [Fact]
        public void ParseDurationWithMinValueShouldReturnCorrectTimeSpan()
        {
            TimeSpan result = EdmValueParser.ParseDuration("-P10675199DT2H48M5.4775808S");
            Assert.Equal(TimeSpan.MinValue, result);
        }

        [Fact]
        public void ParseDurationWithMinValuePlusOneShouldReturnCorrectTimeSpan()
        {
            TimeSpan result = EdmValueParser.ParseDuration("-P10675199DT2H48M5.4775807S");
            Assert.Equal(TimeSpan.MinValue + new TimeSpan(1), result);
        }
        #endregion

        [Fact]
        public void ParseDurationWithNullShouldThrowFormatException()
        {
            Action parseDuration = () =>
            {
                TimeSpan result = EdmValueParser.ParseDuration(null);
            };

            Assert.Throws<FormatException>(parseDuration);
        }

        [Fact]
        public void ParseDurationWithEmptyStringShouldThrowFormatException()
        {
            Action parseDuration = () =>
            {
                TimeSpan result = EdmValueParser.ParseDuration(string.Empty);
            };

            Assert.Throws<FormatException>(parseDuration);
        }

        [Fact]
        public void ParseDurationWithWhiteSpaceShouldThrowFormatException()
        {
            Action parseDuration = () =>
            {
                TimeSpan result = EdmValueParser.ParseDuration(" ");
            };

            Assert.Throws<FormatException>(parseDuration);
        }

        [Fact]
        public void ParseDurationWithYearPartShouldThrowFormatException()
        {
            // The following values are valid xs:duration so they are successfully converted to TimeSpan if we use XmlConvert.ToTimeSpan alone
            var invalidDurations = new[]
            { 
                "P1Y",
                "P1Y1D",
                "P1Y1DT1H",
                "P1Y1DT1H1M",
                "P1Y1DT1H1M1S",
                "P1YT1H",
                "P1YT1H1M",
                "P1YT1H1M1S",
                "-P1Y1D",
                "-P1Y1DT1H",
                "-P1Y1DT1H1M",
                "-P1Y1DT1H1M1S",
                "-P1YT1H",
                "-P1YT1H1M",
                "-P1YT1H1M1S",
            };

            foreach (var invalidDuration in invalidDurations)
            {
                Action parseDuration = () =>
                {
                    TimeSpan result = EdmValueParser.ParseDuration(invalidDuration);
                };

                Assert.Throws<FormatException>(parseDuration);
            }
        }

        [Fact]
        public void ParseDurationWithMonthPartShouldThrowFormatException()
        {
            // The following values are valid xs:duration so they are successfully converted to TimeSpan if we use XmlConvert.ToTimeSpan alone
            var invalidDayTimeDurations = new[]
            { 
                 "P1M",
                 "P1M1D",
                 "P1M1DT1H",
                 "P1M1DT1H1M",
                 "P1M1DT1H1M1S",
                 "P1MT1H",
                 "P1MT1H1M",
                 "P1MT1H1M1S",
                "-P1M1D",
                "-P1M1DT1H",
                "-P1M1DT1H1M",
                "-P1M1DT1H1M1S",
                "-P1MT1H",
                "-P1MT1H1M",
                "-P1MT1H1M1S",
            };

            foreach (var invalidDuration in invalidDayTimeDurations)
            {
                Action parseDuration = () =>
                {
                    TimeSpan result = EdmValueParser.ParseDuration(invalidDuration);
                };

                Assert.Throws<FormatException>(parseDuration);
            }
        }

        [Fact]
        public void ParseDurationWithYearAndMonthPartsShouldThrowFormatException()
        {
            // The following values are valid xs:duration so they are successfully converted to TimeSpan if we use XmlConvert.ToTimeSpan alone
            var invalidDayTimeDurations = new[]
            { 
                 "P1Y1M",
                 "P1Y1M1D",
                 "P1Y1M1DT1H",
                 "P1Y1M1DT1H1M",
                 "P1Y1M1DT1H1M1S",
                 "P1Y1MT1H",
                 "P1Y1MT1H1M",
                 "P1Y1MT1H1M1S",
                "-P1Y1M1D",
                "-P1Y1M1DT1H",
                "-P1Y1M1DT1H1M",
                "-P1Y1M1DT1H1M1S",
                "-P1Y1MT1H",
                "-P1Y1MT1H1M",
                "-P1Y1MT1H1M1S",
            };

            foreach (var invalidDuration in invalidDayTimeDurations)
            {
                Action parseDuration = () =>
                {
                    TimeSpan result = EdmValueParser.ParseDuration(invalidDuration);
                };

                Assert.Throws<FormatException>(parseDuration);
            }
        }

        [Fact]
        public void ParseDurationWithNonDurationValueShouldThrowFormatException()
        {
            Action parseDuration = () =>
            {
                TimeSpan result = EdmValueParser.ParseDuration("+P1D");
            };

            Assert.Throws<FormatException>(parseDuration);
        }

        [Fact]
        public void ParseDurationThatOverflowsShouldThrowOverflowException()
        {
            Action tryParseDuration = () =>
            {
                TimeSpan result = EdmValueParser.ParseDuration("P10675199DT2H48M5.4775808S");
            };

            Assert.Throws<OverflowException>(tryParseDuration);
        }

        [Fact]
        public void TryParseDurationWithInvalidDurationShouldBeFalse()
        {
            TimeSpan? result;
            Assert.False(EdmValueParser.TryParseDuration(null, out result));
            Assert.Null(result);
        }

        [Fact]
        public void TryParseDurationWithValidDurationShouldBeTrue()
        {
            TimeSpan? result;
            Assert.False(EdmValueParser.TryParseDuration(string.Empty, out result));
            Assert.Null(result);
        }

        [Fact]
        public void TryParseDurationThatOverflowsShouldBeFalse()
        {
            TimeSpan? result;
            Assert.False(EdmValueParser.TryParseDuration("P10675199DT2H48M5.4775808S", out result));
            Assert.Null(result);
        }
        #endregion

        #region Decimal
        [Fact]
        public void TryParseIntThatOverFlowsShouldBeFalse()
        {
            int? result;
            Assert.True(EdmValueParser.TryParseInt("-2147483648", out result));
            Assert.Equal(int.MinValue, result);
            Assert.False(EdmValueParser.TryParseInt("-2147483649", out result));
            Assert.Null(result);
        }

        [Fact]
        public void TryParseLongThatOverFlowsShouldBeFalse()
        {
            long? result;
            Assert.True(EdmValueParser.TryParseLong("9223372036854775807", out result));
            Assert.Equal(long.MaxValue, result);
            Assert.False(EdmValueParser.TryParseLong("9223372036854775808", out result));
            Assert.Null(result);
        }

        [Fact]
        public void TryParseDecimalThatOverFlowsShouldBeFalse()
        {
            decimal? result;
            Assert.True(EdmValueParser.TryParseDecimal("-79228162514264337593543950335", out result));
            Assert.Equal(decimal.MinValue, result);
            Assert.False(EdmValueParser.TryParseDecimal("-79228162514264337593543950336", out result));
            Assert.Null(result);
        }

        [Fact]
        public void TryParseFloatThatOverFlowsShouldBeFalse()
        {
            double? result;
            Assert.True(EdmValueParser.TryParseFloat("1.7976931348623157E+308", out result));
            Assert.Equal(double.MaxValue, result);
            Assert.True(EdmValueParser.TryParseFloat("1.7976931348623157E+309", out result));
        }
#endregion Decimal

#region TimeOfDay
        [Fact]
        public void ParseTimeOfDayNullShouldThrowFormatException()
        {
            TimeOfDay? result;
            Assert.False(EdmValueParser.TryParseTimeOfDay(null, out result));
        }

        [Fact]
        public void ParseTimeOfDayWithEmptyStringShouldThrowFormatException()
        {
            TimeOfDay? result;
            Assert.False(EdmValueParser.TryParseTimeOfDay(string.Empty, out result));
        }

        [Fact]
        public void ParseTimeOfDayWithSpaceShouldThrowFormatException()
        {
            TimeOfDay? result;
            Assert.False(EdmValueParser.TryParseTimeOfDay(" ", out result));
        }

        [Fact]
        public void ParseTimeOfDayWithInvalidParameterShouldThrowFormatException()
        {
            var invalidTimeOfDays = new[]
            { 
                 "-1:0:0.0",
                 "0:60:59.1",
                 "0:59:60.2",
                 "0:30:14.10000000",
            };

            foreach (var invalidTimeOfDay in invalidTimeOfDays)
            {
                TimeOfDay? result;
                Assert.False(EdmValueParser.TryParseTimeOfDay(invalidTimeOfDay, out result));
            }
        }

        [Fact]
        public void TryParseTimeOfDayWithValidParameterShouldParseCorrectly()
        {
            TimeOfDay? result;
            Assert.True(EdmValueParser.TryParseTimeOfDay("1:12:5.009000", out result));
            Assert.Equal(new TimeOfDay(1, 12, 5, 9), result);
        }
#endregion
    }
}
