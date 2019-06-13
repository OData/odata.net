//---------------------------------------------------------------------
// <copyright file="SelectExpandSyntacticUnifierTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    public class SelectExpandSyntacticUnifierTests
    {
        [Fact]
        public void NewTopLevelExpandTokenReferencesDollarIt()
        {
            // Arrange
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

            // Act
            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);

            // Assert
            ExpandTermToken subExpand = Assert.Single(unifiedExpand.ExpandTerms);
            subExpand.ShouldBeExpandTermToken(ExpressionConstants.It, true);
        }

        [Fact]
        public void SelectClauseIsAddedAsNewTopLevelExpandToken()
        {
            // Arrange
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

            // Act
            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);

            // Arrange
            ExpandTermToken subExpand = Assert.Single(unifiedExpand.ExpandTerms);
            Assert.NotNull(subExpand.SelectOption);
            subExpand.SelectOption.ShouldBeSelectToken(new string[] {"Name"});
        }

        [Fact]
        public void OriginalExpandTokenIsUnChanged()
        {
            // Arrange
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

            // Act
            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);

            // Arrange
            ExpandTermToken subExpand = Assert.Single(unifiedExpand.ExpandTerms);
            Assert.NotNull(subExpand.ExpandOption);
            ExpandTermToken subSubExpand = Assert.Single(subExpand.ExpandOption.ExpandTerms);
            subSubExpand.ShouldBeExpandTermToken("MyDog", false);
        }

        [Fact]
        public void NullOriginalSelectTokenIsReflectedInNewTopLevelExpandToken()
        {
            // Arrange
            ExpandToken originalExpand = new ExpandToken(
                new List<ExpandTermToken>()
                {
                    new ExpandTermToken(new NonSystemToken("MyDog", /*namedValues*/null, /*nextToken*/null), /*SelectOption*/null, /*ExpandOption*/null)
                });
            SelectToken originalSelect = null;

            // Act
            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);

            // Arrange
            ExpandTermToken subExpand = Assert.Single(unifiedExpand.ExpandTerms);
            Assert.Null(subExpand.SelectOption);
        }

        [Fact]
        public void NullOriginalExpandTokenIsReflectedInNewTopLevelExpandToken()
        {
            // Arrange
            ExpandToken originalExpand = null;
            SelectToken originalSelect = new SelectToken(
                new List<PathSegmentToken>()
                {
                    new NonSystemToken("Name", /*namedValues*/null, /*nextToken*/null)
                });

            // Act
            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);

            // Assert
            ExpandTermToken subExpand = Assert.Single(unifiedExpand.ExpandTerms);
            Assert.Null(subExpand.ExpandOption);
        }

        [Fact]
        public void OriginalSelectAndExpandAreBothNull()
        {
            // Arrange
            ExpandToken originalExpand = null;
            SelectToken originalSelect = null;

            // Act
            ExpandToken unifiedExpand = SelectExpandSyntacticUnifier.Combine(originalExpand, originalSelect);

            // Assert
            ExpandTermToken subExpand = Assert.Single(unifiedExpand.ExpandTerms);
            Assert.Null(subExpand.ExpandOption);
            Assert.Null(subExpand.SelectOption);
        }
    }
}
