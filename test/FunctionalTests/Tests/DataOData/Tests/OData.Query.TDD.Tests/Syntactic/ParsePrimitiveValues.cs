﻿//---------------------------------------------------------------------
// <copyright file="ParsePrimitiveValues.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    [TestClass]
    public class ParsePrimitiveValues
    {
        private static readonly IEdmTemporalTypeReference DATE_TIME_OFFSET = EdmCoreModel.Instance.GetDateTimeOffset(false);

        [TestMethod]
        public void DateShouldParseCorrectly()
        {
            Date realResult;
            TryParse("2012-07-28", EdmCoreModel.Instance.GetDate(false), out realResult).Should().BeTrue();
            realResult.Should().Be(new Date(2012, 7, 28));
        }

        [TestMethod]
        public void TimeOfDayShouldParseCorrectly()
        {
            TimeOfDay realResult;
            TryParse("19:30:5.005", EdmCoreModel.Instance.GetTimeOfDay(false), out realResult).Should().BeTrue();
            realResult.Should().Be(new TimeOfDay(19, 30, 5, 5));
        }

        [TestMethod]
        public void DateTimeOffsetThatProvidesEverythingShouldParseCorrectly()
        {
            DateTimeOffset realResult;
            TryParse("2012-07-28T13:22:16.123-07:15", DATE_TIME_OFFSET, out realResult).Should().BeTrue();
            realResult.Should().Be(new DateTimeOffset(2012, 7, 28, 13, 22, 16, 123, new TimeSpan(-7, -15, 0)));
        }

        [TestMethod]
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
            var parseSuceeded = UriPrimitiveTypeParser.TryUriStringToPrimitive(input, asType, out result);
            if (parseSuceeded)
            {
                realResult = (T)result;
            }

            return parseSuceeded;
        }
    }
}