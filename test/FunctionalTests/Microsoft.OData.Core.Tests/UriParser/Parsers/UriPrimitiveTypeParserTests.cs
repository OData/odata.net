//---------------------------------------------------------------------
// <copyright file="UriPrimitiveTypeParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Parsers.Common;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    public class UriPrimitiveTypeParserTests
    {
        [Fact]
        public void InvalidDateTimeOffsetShouldReturnFalse()
        {
            object output;
            this.TryParseUriStringToPrimitiveType("Ct >dvDTrz", EdmCoreModel.Instance.GetDateTimeOffset(true), out output).Should().BeFalse();
        }

        [Fact]
        public void InvalidDateShouldReturnFalse()
        {
            object output;
            this.TryParseUriStringToPrimitiveType("-1000-00-01", EdmCoreModel.Instance.GetDate(true), out output).Should().BeFalse();
        }

        [Fact]
        public void InvalidTimeOfDayShouldReturnFalse()
        {
            object output;
            var list = new string[]
            {
                "1:5:20,0",
                "-20:3:40.900",
                "24:14:40.090"
            };
            foreach (var s in list)
            {
                this.TryParseUriStringToPrimitiveType(s, EdmCoreModel.Instance.GetTimeOfDay(true), out output).Should().BeFalse();
            }
        }

        [Fact]
        public void TryUriStringToPrimitiveWithValidDurationLiteralShouldReturnValidTimeSpan()
        {
            object output;
            this.TryParseUriStringToPrimitiveType("duration'P1D'", EdmCoreModel.Instance.GetDuration(false /*isNullable*/), out output).Should().BeTrue();
            output.ShouldBeEquivalentTo(new TimeSpan(1, 0, 0, 0));
        }

        [Fact]
        public void TryUriStringToPrimitiveWithInvalidDurationLiteralShouldReturnFalse()
        {
            object output;
            this.TryParseUriStringToPrimitiveType("duration'P1Y'", EdmCoreModel.Instance.GetDuration(false /*isNullable*/), out output).Should().BeFalse();
        }

        [Fact(Skip = "This test currently fails.")]
        public void TryUriStringToPrimitiveWithOverflowingDurationLiteralShouldReturnFalse()
        {
            object output;
            this.TryParseUriStringToPrimitiveType("duration'P999999999D'", EdmCoreModel.Instance.GetDuration(false /*isNullable*/), out output).Should().BeFalse();
        }

        #region Private Methods

        private bool TryParseUriStringToPrimitiveType(string text, IEdmTypeReference targetType, out object targetValue)
        {
            UriLiteralParsingException exception;

            return UriPrimitiveTypeParser.Instance.TryParseUriStringToType(text, targetType, out targetValue, out exception);
        }

        #endregion

    }
}
