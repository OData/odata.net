//---------------------------------------------------------------------
// <copyright file="ExpandTermTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SyntacticAst
{
    public class ExpandTermTokenTests
    {
        [Fact]
        public void PropertyCannotBeNullInTruncatedConstructor()
        {
            // Arrange & Act
            Action test = () => new ExpandTermToken(null);

            // Assert
            Assert.Throws<ArgumentNullException>("property", test);
        }

        [Fact]
        public void PropertyCannotBeNullInFullConstructor()
        {
            // Arrange & Act
            Action test = () => new ExpandTermToken(null, null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Throws<ArgumentNullException>("property", test);
        }

        [Fact]
        public void ExpansionsCanBeNull()
        {
            // Arrange & Act
            ExpandTermToken expandTermToken = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                                  null /*filterOption*/,
                                                                  null /*orderByOption*/,
                                                                  null /*topOption*/,
                                                                  null /*skipOption*/,
                                                                  null /*countQueryOption*/,
                                                                  null /*levelsOption*/,
                                                                  null /*searchOption*/,
                                                                  null /*selectOption*/,
                                                                  null /*expandOption*/,
                                                                  null /*computeOption*/,
                                                                  null /*applyOptions*/);

            // Assert
            Assert.Null(expandTermToken.FilterOption);
            Assert.Null(expandTermToken.OrderByOptions);
            Assert.Null(expandTermToken.TopOption);
            Assert.Null(expandTermToken.SkipOption);
            Assert.Null(expandTermToken.CountQueryOption);
            Assert.Null(expandTermToken.LevelsOption);
            Assert.Null(expandTermToken.SearchOption);
            Assert.Null(expandTermToken.SelectOption);
            Assert.Null(expandTermToken.ExpandOption);
            Assert.Null(expandTermToken.ComputeOption);
            Assert.Null(expandTermToken.ApplyOptions);
        }

        [Fact]
        public void PropertySetCorrectly()
        {
            // Arrange & Act
            ExpandTermToken expandTerm1 = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*selectOption*/,
                                                             null /*expandOption*/);

            // Assert
            expandTerm1.PathToNavigationProp.ShouldBeNonSystemToken("stuff");

            // Arrange & Act
            ExpandTermToken expandTerm2 = new ExpandTermToken(new NonSystemToken("stuff", null, null));

            // Assert
            expandTerm2.PathToNavigationProp.ShouldBeNonSystemToken("stuff");
        }

        [Fact]
        public void FilterSetCorrectly()
        {
            // Arrange & Act
            QueryToken filter = new LiteralToken(1);
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             filter,
                                                             null /*orderByOption*/,
                                                             null /*topOption*/,
                                                             null /*skipOption*/,
                                                             null /*countQueryOption*/,
                                                             null /*levelsOption*/,
                                                             null /*searchOption*/,
                                                             null /*selectOption*/,
                                                             null /*expandOption*/);

            // Assert
            Assert.NotNull(expandTerm.FilterOption);
            expandTerm.FilterOption.ShouldBeLiteralQueryToken(1);
        }

        [Fact]
        public void OrderBySetCorrectly()
        {
            // Arrange & Act
            OrderByToken orderBy = new OrderByToken(new LiteralToken(1), OrderByDirection.Descending);
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*filterOption*/,
                                                             new OrderByToken[] { orderBy },
                                                             null /*topOption*/,
                                                             null /*skipOption*/,
                                                             null /*countQueryOption*/,
                                                             null /*levelsOption*/,
                                                             null /*searchOption*/,
                                                             null /*selectOption*/,
                                                             null /*expandOption*/);

            // Assert
            Assert.NotNull(expandTerm.OrderByOptions);
            OrderByToken orderByToken = Assert.Single(expandTerm.OrderByOptions);
            Assert.Equal(QueryTokenKind.OrderBy, orderByToken.Kind);
            orderByToken.Expression.ShouldBeLiteralQueryToken(1);
        }

        [Fact]
        public void TopSetCorrectly()
        {
            // Arrange & Act
            long top = 1;
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*filterOption*/,
                                                             null /*orderByOption*/,
                                                             top,
                                                             null /*skipOption*/,
                                                             null /*countQueryOption*/,
                                                             null /*levelsOption*/,
                                                             null /*searchOption*/,
                                                             null /*selectOption*/,
                                                             null /*expandOption*/);

            // Assert
            Assert.NotNull(expandTerm.TopOption);
            Assert.Equal(top, expandTerm.TopOption);
        }

        [Fact]
        public void SkipSetCorrectly()
        {
            // Arrange & Act
            long skip = 1;
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*filterOption*/,
                                                             null /*orderByOption*/,
                                                             null /*topOption*/,
                                                             skip,
                                                             null /*countQueryOption*/,
                                                             null /*levelsOption*/,
                                                             null /*searchOption*/,
                                                             null /*selectOption*/,
                                                             null /*expandOption*/);

            // Assert
            Assert.NotNull(expandTerm.SkipOption);
            Assert.Equal(skip, expandTerm.SkipOption);
        }

        [Fact]
        public void CountSetCorrectly()
        {
            // Arrange & Act
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*filterOption*/,
                                                             null /*orderByOption*/,
                                                             null /*topOption*/,
                                                             null /*skipOption*/,
                                                             false /*countQueryOption*/,
                                                             null /*levelsOption*/,
                                                             null /*searchOption*/,
                                                             null /*selectOption*/,
                                                             null /*expandOption*/);

            // Assert
            Assert.NotNull(expandTerm.CountQueryOption);
            Assert.False(expandTerm.CountQueryOption);
        }

        [Fact]
        public void LevelsSetCorrectly()
        {
            // Arrange & Act
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*filterOption*/,
                                                             null /*orderByOption*/,
                                                             null /*topOption*/,
                                                             null /*skipOption*/,
                                                             null /*countQueryOption*/,
                                                             3 /*levelsOption*/,
                                                             null /*searchOption*/,
                                                             null /*selectOption*/,
                                                             null /*expandOption*/);

            // Assert
            Assert.NotNull(expandTerm.LevelsOption);
            Assert.Equal(3, expandTerm.LevelsOption);
        }

        [Fact]
        public void SearchSetCorrectly()
        {
            // Arrange & Act
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*filterOption*/,
                                                             null /*orderByOption*/,
                                                             null /*topOption*/,
                                                             null /*skipOption*/,
                                                             null /*countQueryOption*/,
                                                             null /*levelsOption*/,
                                                             new StringLiteralToken("searchMe") /*searchOption*/,
                                                             null /*selectOption*/,
                                                             null /*expandOption*/);

            // Assert
            Assert.NotNull(expandTerm.SearchOption);
            expandTerm.SearchOption.ShouldBeStringLiteralToken("searchMe");
        }

        [Fact]
        public void SelectSetCorrectly()
        {
            // Arrange & Act
            SelectToken select = new SelectToken(new PathSegmentToken[] { new NonSystemToken("1", null, null) });
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*filterOption*/,
                                                             null /*orderByOption*/,
                                                             null /*topOption*/,
                                                             null /*skipOption*/,
                                                             null /*countQueryOption*/,
                                                             null /*levelsOption*/,
                                                             null /*searchOption*/,
                                                             select,
                                                             null /*expandOption*/);

            // Assert
            Assert.NotNull(expandTerm.SelectOption);
            Assert.NotNull(expandTerm.SelectOption.Properties);
            Assert.Single(expandTerm.SelectOption.Properties);

            Assert.NotNull(expandTerm.SelectOption.SelectTerms);
            SelectTermToken selectTerm = Assert.Single(expandTerm.SelectOption.SelectTerms);
            selectTerm.PathToProperty.ShouldBeNonSystemToken("1");
        }

        [Fact]
        public void ExpandSetCorrectly()
        {
            // Arrange & Act
            ExpandToken expand = new ExpandToken(new ExpandTermToken[]{new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                                                           null /*selectOption*/,
                                                                                           null /*expandOption*/ )});
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*selectOption*/,
                                                             expand);

            // Assert
            Assert.NotNull(expandTerm.ExpandOption);
        }

        [Fact]
        public void ComputeSetCorrectly()
        {
            // Arrange & Act
            ComputeToken compute = new ComputeToken(new ComputeExpressionToken[] { });
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*filterOption*/,
                                                             null /*orderByOption*/,
                                                             null /*topOption*/,
                                                             null /*skipOption*/,
                                                             null /*countQueryOption*/,
                                                             null /*levelsOption*/,
                                                             null /*searchOption*/,
                                                             null /*selectOption*/,
                                                             null /*expandOption*/,
                                                             compute);

            // Assert
            Assert.Same(expandTerm.ComputeOption, compute);
        }

        [Fact]
        public void ApplySetCorrectly()
        {
            // Arrange & Act
            IEnumerable<QueryToken> applyOptions = new QueryToken[] { };
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*filterOption*/,
                                                             null /*orderByOption*/,
                                                             null /*topOption*/,
                                                             null /*skipOption*/,
                                                             null /*countQueryOption*/,
                                                             null /*levelsOption*/,
                                                             null /*searchOption*/,
                                                             null /*selectOption*/,
                                                             null /*expandOption*/,
                                                             null /*computeOption*/,
                                                             applyOptions /*applyOptions*/);

            // Assert
            Assert.Same(expandTerm.ApplyOptions, applyOptions);
        }
    }
}
