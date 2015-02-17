//---------------------------------------------------------------------
// <copyright file="UriPrimitiveTypeParserUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UriPrimitiveTypeParserUnitTests
    {
        [TestMethod]
        public void InvalidDateTimeOffsetShouldReturnFalse()
        {
            object output;
            UriPrimitiveTypeParser.TryUriStringToPrimitive("Ct >dvDTrz", EdmCoreModel.Instance.GetDateTimeOffset(true), out output).Should().BeFalse();
        }

        [TestMethod]
        public void InvalidDateShouldReturnFalse()
        {
            object output;
            UriPrimitiveTypeParser.TryUriStringToPrimitive("-1000-00-01", EdmCoreModel.Instance.GetDate(true), out output).Should().BeFalse();
        }

        [TestMethod]
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
                UriPrimitiveTypeParser.TryUriStringToPrimitive(s, EdmCoreModel.Instance.GetTimeOfDay(true), out output).Should().BeFalse();
            }
        }

        [TestMethod]
        public void TryUriStringToPrimitiveWithValidDurationLiteralShouldReturnValidTimeSpan()
        {
            object output;
            UriPrimitiveTypeParser.TryUriStringToPrimitive("duration'P1D'", EdmCoreModel.Instance.GetDuration(false /*isNullable*/), out output).Should().BeTrue();
            output.ShouldBeEquivalentTo(new TimeSpan(1, 0, 0, 0));
        }

        [TestMethod]
        public void TryUriStringToPrimitiveWithInvalidDurationLiteralShouldReturnFalse()
        {
            object output;
            UriPrimitiveTypeParser.TryUriStringToPrimitive("duration'P1Y'", EdmCoreModel.Instance.GetDuration(false /*isNullable*/), out output).Should().BeFalse();
        }

        [TestMethod, Ignore]
        public void TryUriStringToPrimitiveWithOverflowingDurationLiteralShouldReturnFalse()
        {
            object output;
            UriPrimitiveTypeParser.TryUriStringToPrimitive("duration'P999999999D'", EdmCoreModel.Instance.GetDuration(false /*isNullable*/), out output).Should().BeFalse();
        }
    }
}
