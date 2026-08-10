//---------------------------------------------------------------------
// <copyright file="LiteralParserTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    public class LiteralParserTest
    {
        [Fact]
        public void LargeInt32ValueReturnsFalse()
        {
            // regression test for: [UriParser] When int32 key value is too big for int32 throws System.OverflowException
            LiteralParser parser = LiteralParser.ForETags;
            object output;
            Assert.False(parser.TryParseLiteral(typeof(int), "23500000000000000", out output));
        }

        [Fact]
        public void TryParseLiteralWithDurationLiteralForParenthesesKeyDelimiterShouldReturnValidTimeSpan()
        {
            var parser = LiteralParser.ForKeys(false/*keyAsSegment*/);
            object output;
            Assert.True(parser.TryParseLiteral(typeof(TimeSpan), "duration'P1D'", out output));
            Assert.Equal(new TimeSpan(1, 0, 0, 0), output);
        }

        [Fact]
        public void TryParseLiteralWithDurationLiteralForSlashKeyDelimiterShouldReturnValidTimeSpan()
        {
            var parser = LiteralParser.ForKeys(true/*keyAsSegment*/);
            object output;
            Assert.True(parser.TryParseLiteral(typeof(TimeSpan), "P1D", out output));
            Assert.Equal(new TimeSpan(1, 0, 0, 0), output);
        }

        [Fact]
        public void TryParseLiteralWithDateForSlashKeyDelimiterShouldReturnValidDate()
        {
            var parser = LiteralParser.ForKeys(true /*keyAsSegment*/);
            object output;
            Assert.True(parser.TryParseLiteral(typeof(DateOnly), "2015-09-28", out output));
            Assert.Equal(new DateOnly(2015, 09, 28), output);
        }

        [Fact]
        public void TryParseLiteralWithDateForParenthesesKeyDelimiterShouldReturnValidDate()
        {
            var parser = LiteralParser.ForKeys(false /*keyAsSegment*/);
            object output;
            Assert.True(parser.TryParseLiteral(typeof(DateOnly), "2015-09-28", out output));
            Assert.Equal(new DateOnly(2015, 09, 28), output);
        }

        [Fact]
        public void TryParseLiteralWithTimeOfDayForSlashKeyDelimiterShouldReturnValidTimeOfDay()
        {
            var parser = LiteralParser.ForKeys(true /*keyAsSegment*/);
            object output;
            Assert.True(parser.TryParseLiteral(typeof(TimeOnly), "13:20:00", out output));
            Assert.Equal(new TimeOnly(13, 20, 00, 0), output);
        }

        [Fact]
        public void TryParseLiteralWithTimeOfDayForParenthesesKeyDelimiterShouldReturnValidTimeOfDay()
        {
            var parser = LiteralParser.ForKeys(false /*keyAsSegment*/);
            object output;
            Assert.True(parser.TryParseLiteral(typeof(TimeOnly), "13:20:00", out output));
            Assert.Equal(new TimeOnly(13, 20, 00, 0), output);
        }

        [Theory]
        [InlineData("1:20 PM")] // 12-hour/culture-specific format is not a valid OData time-of-day literal
        [InlineData("24:00:00")] // hours out of range
        [InlineData("13:60:00")] // minutes out of range
        [InlineData("not-a-time")]
        public void TryParseLiteralWithInvalidTimeOfDayFormatShouldReturnFalse(string literal)
        {
            var parser = LiteralParser.ForKeys(false /*keyAsSegment*/);
            object output;
            Assert.False(parser.TryParseLiteral(typeof(TimeOnly), literal, out output));
        }
    }
}
