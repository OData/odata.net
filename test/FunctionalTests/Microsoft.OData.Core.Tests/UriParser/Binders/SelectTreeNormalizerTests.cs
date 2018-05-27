//---------------------------------------------------------------------
// <copyright file="SelectTreeNormalizerUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    public class SelectTreeNormalizerTests
    {
        [Fact]
        public void NormalizeTreeResultsInReversedPath()
        {
            // $select=1/2/3
            NonSystemToken endPath = new NonSystemToken("3", null, new NonSystemToken("2", null, new NonSystemToken("1", null, null)));
            SelectToken selectToken = new SelectToken(new NonSystemToken[]{endPath});
            SelectToken normalizedToken = SelectTreeNormalizer.NormalizeSelectTree(selectToken);
            normalizedToken.Properties.Single().ShouldBeNonSystemToken("1")
                           .And.NextToken.ShouldBeNonSystemToken("2")
                           .And.NextToken.ShouldBeNonSystemToken("3");
        }

        [Fact]
        public void NormalizeTreeWorksForMultipleTerms()
        {
            // $select=1/2/3,4/5/6
            NonSystemToken endPath = new NonSystemToken("3", null, new NonSystemToken("2", null, new NonSystemToken("1", null, null)));
            NonSystemToken endPath1 = new NonSystemToken("6", null, new NonSystemToken("5", null, new NonSystemToken("4", null, null)));
            SelectToken selectToken = new SelectToken(new NonSystemToken[]{endPath, endPath1});
            SelectToken normalizedToken = SelectTreeNormalizer.NormalizeSelectTree(selectToken);
            List<PathSegmentToken> tokens = normalizedToken.Properties.ToList();
            tokens.Should().HaveCount(2);
            tokens.ElementAt(0).ShouldBeNonSystemToken("1")
                  .And.NextToken.ShouldBeNonSystemToken("2")
                  .And.NextToken.ShouldBeNonSystemToken("3");
            tokens.ElementAt(1).ShouldBeNonSystemToken("4")
                  .And.NextToken.ShouldBeNonSystemToken("5")
                  .And.NextToken.ShouldBeNonSystemToken("6");
        }
    }
}
