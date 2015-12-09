//---------------------------------------------------------------------
// <copyright file="ExpandTermTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SyntacticAst
{
    public class ExpandTermTokenTests
    {
        [Fact]
        public void PropertyCannotBeNullInTruncatedConstructor()
        {
            Action createWithNullProperty = () => new ExpandTermToken(null);
            createWithNullProperty.ShouldThrow<Exception>(Error.ArgumentNull("property").ToString());
        }
        [Fact]
        public void PropertyCannotBeNullInFullConstructor()
        {
            Action createWithNullProperty =
                () => new ExpandTermToken(null, null, null, null, null, null, null, null, null, null);
            createWithNullProperty.ShouldThrow<Exception>(Error.ArgumentNull("property").ToString());
        }

        [Fact]
        public void ExpansionsCanBeNull()
        {
            ExpandTermToken expandTermToken = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                                  null /*filterOption*/,
                                                                  null /*orderByOption*/,
                                                                  null /*topOption*/,
                                                                  null /*skipOption*/,
                                                                  null /*countQueryOption*/,
                                                                  null /*levelsOption*/,
                                                                  null /*searchOption*/,
                                                                  null /*selectOption*/,
                                                                  null /*expandOption*/);
            expandTermToken.FilterOption.Should().BeNull();
            expandTermToken.OrderByOptions.Should().BeNull();
            expandTermToken.TopOption.Should().Be(null);
            expandTermToken.SkipOption.Should().Be(null);
            expandTermToken.CountQueryOption.Should().Be(null);
            expandTermToken.LevelsOption.Should().Be(null);
            expandTermToken.SearchOption.Should().Be(null);
            expandTermToken.SelectOption.Should().BeNull();
            expandTermToken.ExpandOption.Should().BeNull();
        }

        [Fact]
        public void PropertySetCorrectly()
        {
            ExpandTermToken expandTerm1 = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*selectOption*/,
                                                             null /*expandOption*/);
            expandTerm1.PathToNavProp.ShouldBeNonSystemToken("stuff");

            ExpandTermToken expandTerm2 = new ExpandTermToken(new NonSystemToken("stuff", null, null));
            expandTerm2.PathToNavProp.ShouldBeNonSystemToken("stuff");
        }

        [Fact]
        public void FilterSetCorrectly()
        {
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
            expandTerm.FilterOption.ShouldBeLiteralQueryToken(1);
        }

        [Fact]
        public void OrderBySetCorrectly()
        {
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
            expandTerm.OrderByOptions.Single().Kind.Should().Be(QueryTokenKind.OrderBy);
            expandTerm.OrderByOptions.Single().Expression.ShouldBeLiteralQueryToken(1);
        }

        [Fact]
        public void TopSetCorrectly()
        {
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
            expandTerm.TopOption.Should().Be(1);
        }

        [Fact]
        public void SkipSetCorrectly()
        {
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
            expandTerm.SkipOption.Should().Be(1);
        }

        [Fact]
        public void CountSetCorrectly()
        {
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
            expandTerm.CountQueryOption.Should().BeFalse();
        }

        [Fact]
        public void LevelsSetCorrectly()
        {
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
            expandTerm.LevelsOption.Should().Be(3);
        }

        [Fact]
        public void SearchSetCorrectly()
        {
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
            expandTerm.SearchOption.ShouldBeStringLiteralToken("searchMe");
        }

        [Fact]
        public void SelectSetCorrectly()
        {
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
            expandTerm.SelectOption.Properties.Count().Should().Be(1);
            expandTerm.SelectOption.Properties.ElementAt(0).ShouldBeNonSystemToken("1");
        }

        [Fact]
        public void ExpandSetCorrectly()
        {
            ExpandToken expand = new ExpandToken(new ExpandTermToken[]{new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                                                           null /*selectOption*/,
                                                                                           null /*expandOption*/ )});
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("stuff", null, null),
                                                             null /*selectOption*/,
                                                             expand);
            expandTerm.ExpandOption.Should().NotBeNull();
        }
    }
}
