//---------------------------------------------------------------------
// <copyright file="SelectTermTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SyntacticAst
{
    public class SelectTermTokenTests
    {
        [Fact]
        public void PropertyCannotBeNullInTruncatedConstructor()
        {
            // Arrange & Act
            Action test = () => new SelectTermToken(null);

            // Assert
            Assert.Throws<ArgumentNullException>("property", test);
        }

        [Fact]
        public void PropertyCannotBeNullInFullConstructor()
        {
            // Arrange & Act
            Action test = () => new SelectTermToken(null, null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Throws<ArgumentNullException>("property", test);
        }

        [Fact]
        public void InnerQueryOptionsCanBeNull()
        {
            // Arrange & Act
            SelectTermToken selectTermToken = new SelectTermToken(new NonSystemToken("stuff", null, null),
                null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Null(selectTermToken.FilterOption);
            Assert.Null(selectTermToken.OrderByOptions);
            Assert.Null(selectTermToken.TopOption);
            Assert.Null(selectTermToken.SkipOption);
            Assert.Null(selectTermToken.CountQueryOption);
            Assert.Null(selectTermToken.SearchOption);
            Assert.Null(selectTermToken.SelectOption);
            Assert.Null(selectTermToken.ExpandOption);
        }

        [Fact]
        public void InnerQueryOptionsPropertySetCorrectly()
        {
            // Arrange & Act
            SelectTermToken selectTermToken1 = new SelectTermToken(new NonSystemToken("stuff", null, null));
            SelectTermToken selectTermToken2 = new SelectTermToken(new NonSystemToken("stuff", null, null), null, null);

            // Assert
            Assert.NotNull(selectTermToken1.PathToProperty);
            NonSystemToken nonSystemToken = Assert.IsType<NonSystemToken>(selectTermToken1.PathToProperty);
            Assert.Equal("stuff", nonSystemToken.Identifier);

            Assert.NotNull(selectTermToken2.PathToProperty);
            nonSystemToken = Assert.IsType<NonSystemToken>(selectTermToken2.PathToProperty);
            Assert.Equal("stuff", nonSystemToken.Identifier);
        }

        [Fact]
        public void InnerFilterSetCorrectly()
        {
            // Arrange & Act
            QueryToken filter = new LiteralToken(21);
            SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
                filter, null, null, null, null, null, null, null, null);

            // Assert
            Assert.NotNull(selectTerm.FilterOption);
            LiteralToken literalToken = Assert.IsType<LiteralToken>(selectTerm.FilterOption);
            Assert.Equal(QueryTokenKind.Literal, literalToken.Kind);
            Assert.Equal(21, literalToken.Value);
        }

        [Fact]
        public void InnerOrderBySetCorrectly()
        {
            // Arrange & Act
            OrderByToken orderBy = new OrderByToken(new LiteralToken(42), OrderByDirection.Descending);
            SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
                null, new OrderByToken[] { orderBy }, null, null, null, null, null,null, null);

            // Assert
            Assert.NotNull(selectTerm.OrderByOptions);
            OrderByToken expectedOrderBy = Assert.Single(selectTerm.OrderByOptions);
            Assert.Equal(QueryTokenKind.OrderBy, expectedOrderBy.Kind);

            Assert.NotNull(expectedOrderBy.Expression);
            LiteralToken literalToken = Assert.IsType<LiteralToken>(expectedOrderBy.Expression);
            Assert.Equal(QueryTokenKind.Literal, literalToken.Kind);
            Assert.Equal(42, literalToken.Value);
        }

        [Fact]
        public void InnerTopSetCorrectly()
        {
            // Arrange & Act
            long top = 42;
            SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
                null, null, top, null, null, null, null, null, null);

            // Assert
            Assert.NotNull(selectTerm.TopOption);
            Assert.Equal(42, selectTerm.TopOption);
        }

        [Fact]
        public void InnerSkipSetCorrectly()
        {
            // Arrange & Act
            long skip = 42;
            SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
                null, null, null, skip, null, null, null, null, null);

            // Assert
            Assert.NotNull(selectTerm.SkipOption);
            Assert.Equal(42, selectTerm.SkipOption);
        }

        [Fact]
        public void InnerCountSetCorrectly()
        {
            // Arrange & Act
            SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
                null, null, null, null, false, null, null, null, null);

            // Assert
            Assert.NotNull(selectTerm.CountQueryOption);
            Assert.False(selectTerm.CountQueryOption);
        }

        [Fact]
        public void InnerSearchSetCorrectly()
        {
            // Arrange & Act
            SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
                null, null, null, null, null, new StringLiteralToken("searchMe"), null, null, null);

            // Assert
            Assert.NotNull(selectTerm.SearchOption);
            StringLiteralToken token = Assert.IsType<StringLiteralToken>(selectTerm.SearchOption);
            Assert.Equal("searchMe", token.Text);
        }

        [Fact]
        public void InnerSelectSetCorrectly()
        {
            // Arrange & Act
            SelectToken select = new SelectToken(new PathSegmentToken[] { new NonSystemToken("abc", null, null) });
            SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null), select, null);

            // Assert
            Assert.NotNull(selectTerm.SelectOption);
            PathSegmentToken token = Assert.Single(selectTerm.SelectOption.Properties);
            NonSystemToken nonSystemToken = Assert.IsType<NonSystemToken>(token);
            Assert.Equal("abc", nonSystemToken.Identifier);
        }

        [Fact]
        public void InnerExpandSetCorrectly()
        {
            // Arrange & Act
            ExpandTermToken innerExpandTerm = new ExpandTermToken(new NonSystemToken("abc", null, null), null, null);

            ExpandToken expand = new ExpandToken(new[] { innerExpandTerm });

            SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null), null, expand);

            // Assert
            Assert.NotNull(selectTerm.ExpandOption);
            ExpandTermToken token = Assert.Single(selectTerm.ExpandOption.ExpandTerms);
            ReferenceEquals(token.PathToNavigationProp, token.PathToProperty);
            Assert.Equal("abc", token.PathToProperty.Identifier);
        }

        [Fact]
        public void InnerComputeSetCorrectly()
        {
            // Arrange & Act
            ComputeToken compute = new ComputeToken(new ComputeExpressionToken[] { });
            SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*filterOption*/,
                                                             null /*orderByOption*/,
                                                             null /*topOption*/,
                                                             null /*skipOption*/,
                                                             null /*countQueryOption*/,
                                                             null /*searchOption*/,
                                                             null /*selectOption*/,
                                                             null /*expandOption*/,
                                                             compute);

            // Assert
            Assert.NotNull(selectTerm.ComputeOption);
            Assert.Same(compute, selectTerm.ComputeOption);
        }
    }
}
