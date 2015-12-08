//---------------------------------------------------------------------
// <copyright file="EdmValueWriterTests.cs" company="Microsoft">
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
    public class EdmValueWriterTests
    {
        #region Date
        private Date[] validDate = new[]
        {
            new Date(), 
            new Date(1, 1, 1),
            new Date(2014, 8, 8),
            new Date(9999, 12 ,31),
            
        };

        [Fact]
        public void DateAsXmlWithValidShouldRoundtripWhenParsed()
        {
            foreach (var date in this.validDate)
            {
                Date? parsedDate;
                var result = EdmValueWriter.DateAsXml(date);
                EdmValueParser.TryParseDate(result, out parsedDate).Should().BeTrue();

                parsedDate.Should().Be(date);
            }
        }
        #endregion

        #region Duration
        private TimeSpan[] validTimeSpans = new[]
        {
            TimeSpan.MaxValue,
            TimeSpan.MaxValue - new TimeSpan(1),
            TimeSpan.MaxValue - new TimeSpan(2),
            TimeSpan.MinValue,
            TimeSpan.MinValue + new TimeSpan(1),
            TimeSpan.MinValue + new TimeSpan(2),
            TimeSpan.Zero,
            TimeSpan.Zero + new TimeSpan(1),
            TimeSpan.Zero - new TimeSpan(1),
            new TimeSpan(0 /*hours*/, 0 /*minutes*/, 1 /*seconds*/),
            new TimeSpan(0 /*hours*/, 1 /*minutes*/, 1 /*seconds*/),
            new TimeSpan(1 /*hours*/, 1 /*minutes*/, 1 /*seconds*/),
            new TimeSpan(1 /*days*/, 1 /*hours*/, 1 /*minutes*/, 1 /*seconds*/),
            new TimeSpan(32 /*days*/, 1 /*hours*/, 1 /*minutes*/, 1 /*seconds*/),
            new TimeSpan(366 /*days*/, 1 /*hours*/, 1 /*minutes*/, 1 /*seconds*/),
        };

        [Fact]
        public void DurationAsXmlWithValidDurationShouldNotContainYearOrMonth()
        {
            foreach (var timeSpan in this.validTimeSpans)
            {
                var result = EdmValueWriter.DurationAsXml(timeSpan);
                EdmValueParser.DayTimeDurationValidator.IsMatch(result).Should().BeTrue();
            }
        }

        [Fact]
        public void DurationAsXmlWithValidShouldRoundtripWhenParsed()
        {
            foreach (var timeSpan in this.validTimeSpans)
            {
                TimeSpan? parsedTimeSpan;
                var result = EdmValueWriter.DurationAsXml(timeSpan);
                EdmValueParser.TryParseDuration(result, out parsedTimeSpan).Should().BeTrue();

                parsedTimeSpan.Should().Be(timeSpan);
            }
        }
        #endregion

        #region TimeOfDay
        private TimeOfDay[] validTime = new[]
        {
            new TimeOfDay(), 
            new TimeOfDay(0, 0, 0, 0),
            new TimeOfDay(23, 59, 59, 999),
            new TimeOfDay(TimeOfDay.MaxTickValue),
            new TimeOfDay(TimeOfDay.MinTickValue),
            new TimeOfDay(5, 30, 5, 30), 
        };

        [Fact]
        public void TimeOfDayyAsXmlWithValidShouldRoundtripWhenParsed()
        {
            foreach (var time in this.validTime)
            {
                TimeOfDay? parsedTime;
                var result = EdmValueWriter.TimeOfDayAsXml(time);
                EdmValueParser.TryParseTimeOfDay(result, out parsedTime).Should().BeTrue();

                parsedTime.Should().Be(time);
            }
        }
        #endregion
    }
}
