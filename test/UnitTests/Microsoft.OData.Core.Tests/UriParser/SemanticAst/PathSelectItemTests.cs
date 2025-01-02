//---------------------------------------------------------------------
// <copyright file="PathSelectItemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class PathSelectItemTests
    {
        [Fact]
        public void PathCannotBeNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>("selectedPath", () => new PathSelectItem(null));
        }

        [Fact]
        public void ConstructorShouldSetPropertyName()
        {
            // Arrange & Act
            var item = new PathSelectItem(new ODataSelectPath(new DynamicPathSegment("abc")));

            // Assert
            item.SelectedPath.FirstSegment.ShouldBeDynamicPathSegment("abc");

            Assert.Null(item.NavigationSource);
            Assert.Null(item.SelectAndExpand);
            Assert.Null(item.FilterOption);
            Assert.Null(item.OrderByOption);
            Assert.Null(item.TopOption);
            Assert.Null(item.SkipOption);
            Assert.Null(item.CountOption);
            Assert.Null(item.SearchOption);
            Assert.Null(item.ComputeOption);
        }

        [Fact]
        public void NavigationSourceOptionSetCorrectly()
        {
            // Arrange & Act
            var navigationSource = HardCodedTestModel.GetPeopleSet();
            PathSelectItem select = new PathSelectItem(new ODataSelectPath(), navigationSource, null, null, null, null, null, null, null, null);

            // Assert
            Assert.NotNull(select.NavigationSource);
            Assert.Same(navigationSource, select.NavigationSource);
        }

        [Fact]
        public void SelectExpandOptionSetCorrectly()
        {
            // Arrange & Act
            SelectExpandClause selectAndExpand = new SelectExpandClause(null, true);
            PathSelectItem select = new PathSelectItem(new ODataSelectPath(), null, selectAndExpand, null, null, null, null, null, null, null);

            // Assert
            Assert.NotNull(select.SelectAndExpand);
            Assert.Same(selectAndExpand, select.SelectAndExpand);
        }

        [Fact]
        public void FilterOptionSetCorrectly()
        {
            // Arrange & Act
            BinaryOperatorNode filterExpression = new BinaryOperatorNode(BinaryOperatorKind.Equal, new ConstantNode(1), new ConstantNode(1));
            FilterClause filterClause = new FilterClause(filterExpression,
                new ResourceRangeVariable(ExpressionConstants.It, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet()));
            PathSelectItem select = new PathSelectItem(new ODataSelectPath(), null, null, filterClause, null, null, null, null, null, null);

            // Assert
            Assert.NotNull(select.FilterOption);
            Assert.Same(filterClause, select.FilterOption);
        }

        [Fact]
        public void OrderbySetCorrectly()
        {
            // Arrange & Act
            SingleValuePropertyAccessNode propertyAccessNode = new SingleValuePropertyAccessNode(new ConstantNode(1), HardCodedTestModel.GetPersonNameProp());
            OrderByClause orderBy = new OrderByClause(null, propertyAccessNode, OrderByDirection.Descending,
                new ResourceRangeVariable(ExpressionConstants.It, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet()));
            PathSelectItem select = new PathSelectItem(new ODataSelectPath(), null, null, null, orderBy, null, null, null, null, null);

            // Assert
            Assert.NotNull(select.OrderByOption);
            Assert.Same(orderBy, select.OrderByOption);
        }

        [Fact]
        public void TopOptionSetCorrectly()
        {
            // Arrange & Act
            PathSelectItem select = new PathSelectItem(new ODataSelectPath(), null, null, null, null, 5, null, null, null, null);

            // Assert
            Assert.NotNull(select.TopOption);
            Assert.Equal(5, select.TopOption.Value);
        }

        [Fact]
        public void SkipOptionSetCorrectly()
        {
            // Arrange & Act
            PathSelectItem select = new PathSelectItem(new ODataSelectPath(), null, null, null, null, null, 5, null, null, null);

            // Assert
            Assert.NotNull(select.SkipOption);
            Assert.Equal(5, select.SkipOption.Value);
        }

        [Fact]
        public void CountQueryOptionSetCorrectly()
        {
            // Arrange & Act
            PathSelectItem select = new PathSelectItem(new ODataSelectPath(), null, null, null, null, null, null, true, null, null);

            // Assert
            Assert.NotNull(select.CountOption);
            Assert.True(select.CountOption.Value);
        }

        [Fact]
        public void SearchOptionSetCorrectly()
        {
            // Arrange & Act
            SearchClause search = new SearchClause(new SearchTermNode("SearchMe"));
            PathSelectItem select = new PathSelectItem(new ODataSelectPath(), null, null, null, null, null, null, null, search, null);

            // Assert
            Assert.NotNull(select.SearchOption);
            select.SearchOption.Expression.ShouldBeSearchTermNode("SearchMe");
        }

        [Fact]
        public void ComputeOptionSetCorrectly()
        {
            // Arrange & Act
            ComputeClause compute = new ComputeClause(null);
            PathSelectItem select = new PathSelectItem(new ODataSelectPath(), null, null, null, null, null, null, null, null, compute);

            // Assert
            Assert.NotNull(select.ComputeOption);
            Assert.Same(compute, select.ComputeOption);
        }
    }
}
