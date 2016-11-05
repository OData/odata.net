//---------------------------------------------------------------------
// <copyright file="ParsePrimitiveValuesTests.cs" company="Microsoft">
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

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
{
    public class ParsePrimitiveValuesTests
    {
        private static readonly IEdmTemporalTypeReference DATE_TIME_OFFSET = EdmCoreModel.Instance.GetDateTimeOffset(false);

        [Fact]
        public void DateShouldParseCorrectly()
        {
            Date realResult;
            TryParse("2012-07-28", EdmCoreModel.Instance.GetDate(false), out realResult).Should().BeTrue();
            realResult.Should().Be(new Date(2012, 7, 28));
        }

        [Fact]
        public void TimeOfDayShouldParseCorrectly()
        {
            TimeOfDay realResult;
            TryParse("19:30:5.005", EdmCoreModel.Instance.GetTimeOfDay(false), out realResult).Should().BeTrue();
            realResult.Should().Be(new TimeOfDay(19, 30, 5, 5));
        }

        [Fact]
        public void DateTimeOffsetThatProvidesEverythingShouldParseCorrectly()
        {
            DateTimeOffset realResult;
            TryParse("2012-07-28T13:22:16.123-07:15", DATE_TIME_OFFSET, out realResult).Should().BeTrue();
            realResult.Should().Be(new DateTimeOffset(2012, 7, 28, 13, 22, 16, 123, new TimeSpan(-7, -15, 0)));
        }

        [Fact]
        public void GuidParseTestShouldPass()
        {
            Guid guid;
            IEdmPrimitiveTypeReference guidRefType = EdmCoreModel.Instance.GetGuid(false);
            TryParse("38cf68c2-4010-4ccc-8922-868217f03ddc", guidRefType, out guid).Should().BeTrue();
            guid.Should().Be(new Guid("{38CF68C2-4010-4CCC-8922-868217F03DDC}"));

        }

        private static bool TryParse<T>(string input, IEdmPrimitiveTypeReference asType, out T realResult)
        {
            realResult = default(T);

            object result;
            UriLiteralParsingException exception;
            bool parseSuceeded = UriPrimitiveTypeParser.Instance.TryParseUriStringToType(input, asType, out result, out exception);
            if (parseSuceeded)
            {
                realResult = (T)result;
            }

            return parseSuceeded;
        }
    }
}