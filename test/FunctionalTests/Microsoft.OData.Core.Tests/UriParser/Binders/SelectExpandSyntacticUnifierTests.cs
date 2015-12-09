//---------------------------------------------------------------------
// <copyright file="SelectExpandSyntacticUnifierTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Syntactic;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    public class SelectExpandSyntacticUnifierTests
    {
        [Fact]
        public void NewTopLevelExpandTokenReferencesDollarIt()
        {
            ExpandToken originalExpand = new ExpandToken(
                new List<ExpandTermToken>()
                {
                    new ExpandTermToken(new NonSystemToken("MyDog", /*namedValues*/null, /*nextToken*/null), /*SelectOption*/null, /*ExpandOption*/null)
                });
            SelectToken originalSelect = new SelectToken(
                new List<PathSegmentToken>()
                {
                    new NonSystemToken("Name", /*namedValues*/null, /*nextToken*/null)
                });
            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);
            unifiedExpand.ExpandTerms.Single().ShouldBeExpandTermToken(ExpressionConstants.It, true);
        }
        [Fact]
        public void SelectClauseIsAddedAsNewTopLevelExpandToken()
        {
            ExpandToken originalExpand = new ExpandToken(
                new List<ExpandTermToken>()
                {
                    new ExpandTermToken(new NonSystemToken("MyDog", /*namedValues*/null, /*nextToken*/null), /*SelectOption*/null, /*ExpandOption*/null)
                });
            SelectToken originalSelect = new SelectToken(
                new List<PathSegmentToken>()
                {
                    new NonSystemToken("Name", /*namedValues*/null, /*nextToken*/null)
                });

            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);
            unifiedExpand.ExpandTerms.Single().As<ExpandTermToken>().SelectOption.ShouldBeSelectToken(new string[] {"Name"});
        }

        [Fact]
        public void OriginalExpandTokenIsUnChanged()
        {
            ExpandToken originalExpand = new ExpandToken(
                new List<ExpandTermToken>()
                {
                    new ExpandTermToken(new NonSystemToken("MyDog", /*namedValues*/null, /*nextToken*/null), /*SelectOption*/null, /*ExpandOption*/null)
                });
            SelectToken originalSelect = new SelectToken(
                new List<PathSegmentToken>()
                {
                    new NonSystemToken("Name", /*namedValues*/null, /*nextToken*/null)
                });
            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);
            var subExpand = unifiedExpand.ExpandTerms.Single().As<ExpandTermToken>().ExpandOption;
            subExpand.ExpandTerms.Single().ShouldBeExpandTermToken("MyDog", false);
        }

        [Fact]
        public void NullOriginalSelectTokenIsReflectedInNewTopLevelExpandToken()
        {
            ExpandToken originalExpand = new ExpandToken(
                new List<ExpandTermToken>()
                {
                    new ExpandTermToken(new NonSystemToken("MyDog", /*namedValues*/null, /*nextToken*/null), /*SelectOption*/null, /*ExpandOption*/null)
                });
            SelectToken originalSelect = null;
            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);
            unifiedExpand.ExpandTerms.Single().SelectOption.Should().BeNull();
        }

        [Fact]
        public void NullOriginalExpandTokenIsReflectedInNewTopLevelExpandToken()
        {
            ExpandToken originalExpand = null;
            SelectToken originalSelect = new SelectToken(
                new List<PathSegmentToken>()
                {
                    new NonSystemToken("Name", /*namedValues*/null, /*nextToken*/null)
                });
            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);
            unifiedExpand.ExpandTerms.Single().As<ExpandTermToken>().ExpandOption.Should().BeNull();
        }

        [Fact]
        public void OriginalSelectAndExpandAreBothNull()
        {
            ExpandToken originalExpand = null;
            SelectToken originalSelect = null;
            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);
            var subExpand = unifiedExpand.ExpandTerms.Single().As<ExpandTermToken>();
            subExpand.ExpandOption.Should().BeNull();
            subExpand.SelectOption.Should().BeNull();
        }
    }
}
