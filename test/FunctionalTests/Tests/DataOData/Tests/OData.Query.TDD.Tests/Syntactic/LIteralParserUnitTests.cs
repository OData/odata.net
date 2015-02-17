//---------------------------------------------------------------------
// <copyright file="LIteralParserUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class LiteralParserUnitTest
    {
        [TestMethod]
        public void LargeInt32ValueReturnsFalse()
        {
            // regression test for: [UriParser] When int32 key value is too big for int32 throws System.OverflowException
            LiteralParser parser = LiteralParser.ForETags;
            object output;
            parser.TryParseLiteral(typeof(int), "23500000000000000", out output).Should().BeFalse();
        }

        [TestMethod]
        public void TryParseLiteralWithDurationLiteralForDefaultUrlConventionsShouldReturnValidTimeSpan()
        {
            var parser = LiteralParser.ForKeys(false/*keyAsSegment*/);
            object output;
            parser.TryParseLiteral(typeof(TimeSpan), "duration'P1D'", out output).Should().BeTrue();
            output.ShouldBeEquivalentTo(new TimeSpan(1, 0, 0, 0));
        }

        [TestMethod]
        public void TryParseLiteralWithDurationLiteralForKeyAsSegmentUrlConventionsShouldReturnValidTimeSpan()
        {
            var parser = LiteralParser.ForKeys(true/*keyAsSegment*/);
            object output;
            parser.TryParseLiteral(typeof(TimeSpan), "P1D", out output).Should().BeTrue();
            output.ShouldBeEquivalentTo(new TimeSpan(1, 0, 0, 0));
        }
    }
}
