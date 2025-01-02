//---------------------------------------------------------------------
// <copyright file="ExpandedNavigationSelectItemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class ExpandedNavigationSelectItemTests
    {
        [Fact]
        public void NavPropCannotBeNullInNonOptionConstructor()
        {
            Action createWithNullNavProp = () => new ExpandedNavigationSelectItem((ODataExpandPath)null, HardCodedTestModel.GetPeopleSet(), null);
            Assert.Throws<ArgumentNullException>("pathToNavigationProperty", createWithNullNavProp);
        }

        [Fact]
        public void EntitySetCanBeNullInNonOptionConstructor()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), null, null);
            Assert.Null(expansion.NavigationSource);
        }

        [Fact]
        public void NavPropCannotBeNullInOptionConstructor()
        {
            Action createWithNullNavProp = () => new ExpandedNavigationSelectItem((ODataExpandPath)null, HardCodedTestModel.GetPeopleSet(), null, null, null, null, null, null, null, null);
            Assert.Throws<ArgumentNullException>("pathToNavigationProperty", createWithNullNavProp);
        }

        [Fact]
        public void EntitySetCanBeNullInOptionConstructor()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), null, null, null, null, null, null, null, null, null);
            Assert.Null(expansion.NavigationSource);
        }

        [Fact]
        public void NavPropSetCorrectly()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)), HardCodedTestModel.GetPeopleSet(), null);
            expansion.PathToNavigationProperty.FirstSegment.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [Fact]
        public void EntitySetSetCorrectly()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)), HardCodedTestModel.GetDogsSet(), null);
            Assert.Same(HardCodedTestModel.GetDogsSet(), expansion.NavigationSource);
        }

        [Fact]
        public void FilterOptionSetCorrectly()
        {
            BinaryOperatorNode filterExpression = new BinaryOperatorNode(BinaryOperatorKind.Equal, new ConstantNode(1), new ConstantNode(1));
            FilterClause filterClause = new FilterClause(filterExpression, new ResourceRangeVariable(ExpressionConstants.It, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet()));
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, filterClause, null, null, null, null, null, null);
            expansion.FilterOption.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var bon = Assert.IsType<BinaryOperatorNode>(expansion.FilterOption.Expression);
            bon.Left.ShouldBeConstantQueryNode(1);
            bon.Right.ShouldBeConstantQueryNode(1);
        }

        [Fact]
        public void OrderByOptionSetCorrectly()
        {
            SingleValuePropertyAccessNode propertyAccessNode = new SingleValuePropertyAccessNode(new ConstantNode(1), HardCodedTestModel.GetPersonNameProp());
            OrderByClause orderBy = new OrderByClause(null, propertyAccessNode, OrderByDirection.Descending, new ResourceRangeVariable(ExpressionConstants.It, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet()));
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, orderBy, null, null, null, null, null);
            expansion.OrderByOption.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
        }

        [Fact]
        public void TopOptionSetCorrectly()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, null, 42, null, null, null, null);
            Assert.Equal(42, expansion.TopOption);
        }

        [Fact]
        public void SkipOptionSetCorrectly()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, null, null, 42, null, null, null);
            Assert.Equal(42, expansion.SkipOption);
        }

        [Fact]
        public void CountQueryOptionSetCorrectly()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, null, null, null, true, null, null);
            Assert.True(expansion.CountOption);
        }

        [Fact]
        public void LevelsOptionSetCorrectly()
        {
            LevelsClause levels = new LevelsClause(false, 16384);
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, null, null, null, null, null, levels);
            Assert.False(expansion.LevelsOption.IsMaxLevel);
            Assert.Equal(16384, expansion.LevelsOption.Level);
        }

        [Fact]
        public void SearchOptionSetCorrectly()
        {
            SearchClause search = new SearchClause(new SearchTermNode("SearchMe"));
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, null, null, null, null, search, null);
            expansion.SearchOption.Expression.ShouldBeSearchTermNode("SearchMe");
        }

        [Fact]
        public void SelectExpandOptionSetCorrectly()
        {
            SelectExpandClause selectAndExpand = new SelectExpandClause(null, true);
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), selectAndExpand, null, null, null, null, null, null, null);
            Assert.True(expansion.SelectAndExpand.AllSelected);
            Assert.Empty(expansion.SelectAndExpand.SelectedItems);
        }
    }
}
