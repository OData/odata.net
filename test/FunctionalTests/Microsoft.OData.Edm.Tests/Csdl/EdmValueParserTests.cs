//---------------------------------------------------------------------
// <copyright file="EdmValueParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Library;
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
            EdmValueParser.TryParseDate(null, out result).Should().BeFalse();
        }

        [Fact]
        public void ParseDateWithEmptyStringShouldThrowFormatException()
        {
            Date? result;
            EdmValueParser.TryParseDate(string.Empty, out result).Should().BeFalse();
        }

        [Fact]
        public void ParseDateWithSpaceShouldThrowFormatException()
        {
            Date? result;
            EdmValueParser.TryParseDate(" ", out result).Should().BeFalse();
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
                EdmValueParser.TryParseDate(invalidDate, out result).Should().BeFalse();
            }
        }

        [Fact]
        public void TryParseDateWithValidParameterShouldParseCorrectly()
        {
            Date? result;
            EdmValueParser.TryParseDate("2012-07-28", out result).Should().BeTrue();
            result.Should().Be(new Date(2012, 07, 28));
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
            EdmValueParser.TryParseDateTimeOffset("2012-07-28T13:22:16.123-07:15", out result).Should().BeTrue();
            result.Should().Be(new DateTimeOffset(2012, 7, 28, 13, 22, 16, 123, new TimeSpan(-7, -15, 0)));
        }

        [Fact]
        public void TryParseDateTimeOffsetWithInvalidFormatShouldReturnFalse()
        {
            DateTimeOffset? result;
            EdmValueParser.TryParseDateTimeOffset("0001+01+01T00:00:00.000+00:01", out result).Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void TryParseDateTimeOffsetThatOverFlowsShouldReturnFalse()
        {
            DateTimeOffset? result;
            EdmValueParser.TryParseDateTimeOffset("0001-01-01T00:00:00.000+00:01", out result).Should().BeFalse();
            result.Should().BeNull();
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
            result.Should().Be(new TimeSpan(9000000));
        }

        [Fact]
        public void ParseDurationWith2DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.09S");
            result.Should().Be(new TimeSpan(900000));
        }

        [Fact]
        public void ParseDurationWith3DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.009S");
            result.Should().Be(new TimeSpan(90000));
        }

        [Fact]
        public void ParseDurationWith4DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.0009S");
            result.Should().Be(new TimeSpan(9000));
        }

        [Fact]
        public void ParseDurationWith5DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.00009S");
            result.Should().Be(new TimeSpan(900));
        }

        [Fact]
        public void ParseDurationWith6DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.000009S");
            result.Should().Be(new TimeSpan(90));
        }

        [Fact]
        public void ParseDurationWith7DecimalPlacesShouldNotLosePrecision()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT0.0000009S");
            result.Should().Be(new TimeSpan(9));
        }

        [Fact]
        public void ParseDurationWithMoreThan7DecimalPlacesShouldNotLosePrecisionUpTo7()
        {
            // 12 is the max precison supported in OData protocol, but Clr TimeSpan only supports up to 7 decimal places
            TimeSpan result = EdmValueParser.ParseDuration("PT0.123456789012S");
            result.Should().Be(new TimeSpan(1234567));
        }

        [Fact]
        public void ParseDurationWithTrailingSpaces()
        {
            TimeSpan result = EdmValueParser.ParseDuration("PT1S  ");
            result.Should().Be(new TimeSpan(0, 0, 1));
        }

        [Fact]
        public void ParseDurationWithLeadingSpaces()
        {
            TimeSpan result = EdmValueParser.ParseDuration("  PT1S");
            result.Should().Be(new TimeSpan(0, 0, 1));
        }

        [Fact]
        public void ParseDurationWithMaxValueShouldReturnCorrectTimeSpan()
        {
            TimeSpan result = EdmValueParser.ParseDuration("P10675199DT2H48M5.4775807S");
            result.ShouldBeEquivalentTo(TimeSpan.MaxValue);
        }

        [Fact]
        public void ParseDurationWithMaxValueMinusOneShouldReturnCorrectTimeSpan()
        {
            TimeSpan result = EdmValueParser.ParseDuration("P10675199DT2H48M5.4775806S");
            result.ShouldBeEquivalentTo(TimeSpan.MaxValue - new TimeSpan(1));
        }

        [Fact]
        public void ParseDurationWithMinValueShouldReturnCorrectTimeSpan()
        {
            TimeSpan result = EdmValueParser.ParseDuration("-P10675199DT2H48M5.4775808S");
            result.ShouldBeEquivalentTo(TimeSpan.MinValue);
        }

        [Fact]
        public void ParseDurationWithMinValuePlusOneShouldReturnCorrectTimeSpan()
        {
            TimeSpan result = EdmValueParser.ParseDuration("-P10675199DT2H48M5.4775807S");
            result.ShouldBeEquivalentTo(TimeSpan.MinValue + new TimeSpan(1));
        }
        #endregion

        [Fact]
        public void ParseDurationWithNullShouldThrowFormatException()
        {
            Action parseDuration = () =>
            {
                TimeSpan result = EdmValueParser.ParseDuration(null);
            };

            parseDuration.ShouldThrow<FormatException>();
        }

        [Fact]
        public void ParseDurationWithEmptyStringShouldThrowFormatException()
        {
            Action parseDuration = () =>
            {
                TimeSpan result = EdmValueParser.ParseDuration(string.Empty);
            };

            parseDuration.ShouldThrow<FormatException>();
        }

        [Fact]
        public void ParseDurationWithWhiteSpaceShouldThrowFormatException()
        {
            Action parseDuration = () =>
            {
                TimeSpan result = EdmValueParser.ParseDuration(" ");
            };

            parseDuration.ShouldThrow<FormatException>();
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

                parseDuration.ShouldThrow<FormatException>();
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

                parseDuration.ShouldThrow<FormatException>();
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

                parseDuration.ShouldThrow<FormatException>();
            }
        }

        [Fact]
        public void ParseDurationWithNonDurationValueShouldThrowFormatException()
        {
            Action parseDuration = () =>
            {
                TimeSpan result = EdmValueParser.ParseDuration("+P1D");
            };

            parseDuration.ShouldThrow<FormatException>();
        }

        [Fact]
        public void ParseDurationThatOverflowsShouldThrowOverflowException()
        {
            Action tryParseDuration = () =>
            {
                TimeSpan result = EdmValueParser.ParseDuration("P10675199DT2H48M5.4775808S");
            };

            tryParseDuration.ShouldThrow<OverflowException>();
        }

        [Fact]
        public void TryParseDurationWithInvalidDurationShouldBeFalse()
        {
            TimeSpan? result;
            EdmValueParser.TryParseDuration(null, out result).Should().BeFalse();
            result.Should().NotHaveValue();
        }

        [Fact]
        public void TryParseDurationWithValidDurationShouldBeTrue()
        {
            TimeSpan? result;
            EdmValueParser.TryParseDuration(string.Empty, out result).Should().BeFalse();
            result.Should().NotHaveValue();
        }

        [Fact]
        public void TryParseDurationThatOverflowsShouldBeFalse()
        {
            TimeSpan? result;
            EdmValueParser.TryParseDuration("P10675199DT2H48M5.4775808S", out result).Should().BeFalse();
            result.Should().NotHaveValue();
        }
        #endregion

        #region Decimal
        [Fact]
        public void TryParseIntThatOverFlowsShouldBeFalse()
        {
            int? result;
            EdmValueParser.TryParseInt("-2147483648", out result).Should().BeTrue();
            result.Should().Be(int.MinValue);
            EdmValueParser.TryParseInt("-2147483649", out result).Should().BeFalse();
            result.Should().NotHaveValue();
        }

        [Fact]
        public void TryParseLongThatOverFlowsShouldBeFalse()
        {
            long? result;
            EdmValueParser.TryParseLong("9223372036854775807", out result).Should().BeTrue();
            result.Should().Be(long.MaxValue);
            EdmValueParser.TryParseLong("9223372036854775808", out result).Should().BeFalse();
            result.Should().NotHaveValue();
        }

        [Fact]
        public void TryParseDecimalThatOverFlowsShouldBeFalse()
        {
            decimal? result;
            EdmValueParser.TryParseDecimal("-79228162514264337593543950335", out result).Should().BeTrue();
            result.Should().Be(decimal.MinValue);
            EdmValueParser.TryParseDecimal("-79228162514264337593543950336", out result).Should().BeFalse();
            result.Should().NotHaveValue();
        }

        [Fact]
        public void TryParseFloatThatOverFlowsShouldBeFalse()
        {
            double? result;
            EdmValueParser.TryParseFloat("1.7976931348623157E+308", out result).Should().BeTrue();
            result.Should().Be(double.MaxValue);
            EdmValueParser.TryParseFloat("1.7976931348623157E+309", out result).Should().BeFalse();
            result.Should().NotHaveValue();
        }
        #endregion Decimal

        #region TimeOfDay
        [Fact]
        public void ParseTimeOfDayNullShouldThrowFormatException()
        {
            TimeOfDay? result;
            EdmValueParser.TryParseTimeOfDay(null, out result).Should().BeFalse();
        }

        [Fact]
        public void ParseTimeOfDayWithEmptyStringShouldThrowFormatException()
        {
            TimeOfDay? result;
            EdmValueParser.TryParseTimeOfDay(string.Empty, out result).Should().BeFalse();
        }

        [Fact]
        public void ParseTimeOfDayWithSpaceShouldThrowFormatException()
        {
            TimeOfDay? result;
            EdmValueParser.TryParseTimeOfDay(" ", out result).Should().BeFalse();
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
                EdmValueParser.TryParseTimeOfDay(invalidTimeOfDay, out result).Should().BeFalse();
            }
        }

        [Fact]
        public void TryParseTimeOfDayWithValidParameterShouldParseCorrectly()
        {
            TimeOfDay? result;
            EdmValueParser.TryParseTimeOfDay("1:12:5.009000", out result).Should().BeTrue();
            result.Should().Be(new TimeOfDay(1, 12, 5, 9));
        }
        #endregion
    }
}
