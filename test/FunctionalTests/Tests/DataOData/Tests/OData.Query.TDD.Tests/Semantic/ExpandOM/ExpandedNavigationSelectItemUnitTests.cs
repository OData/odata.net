﻿//---------------------------------------------------------------------
// <copyright file="ExpandedNavigationSelectItemUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpandedNavigationSelectItemUnitTests
    {
        [TestMethod]
        public void NavPropCannotBeNullInNonOptionConstructor()
        {
            Action createWithNullNavProp = () => new ExpandedNavigationSelectItem((ODataExpandPath)null, HardCodedTestModel.GetPeopleSet(), null);
            createWithNullNavProp.ShouldThrow<Exception>(Error.ArgumentNull("navigationProperty").ToString());
        }

        [TestMethod]
        public void EntitySetCanBeNullInNonOptionConstructor()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), null, null);
            expansion.NavigationSource.Should().BeNull();
        }

        [TestMethod]
        public void NavPropCannotBeNullInOptionConstructor()
        {
            Action createWithNullNavProp = () => new ExpandedNavigationSelectItem((ODataExpandPath)null, HardCodedTestModel.GetPeopleSet(), null, null, null, null, null, null, null, null);
            createWithNullNavProp.ShouldThrow<Exception>(Error.ArgumentNull("navigationProperty").ToString());
        }

        [TestMethod]
        public void EntitySetCanBeNullInOptionConstructor()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), null, null, null, null, null, null, null, null, null);
            expansion.NavigationSource.Should().BeNull();
        }

        [TestMethod]
        public void NavPropSetCorrectly()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)), HardCodedTestModel.GetPeopleSet(), null);
            expansion.PathToNavigationProperty.FirstSegment.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [TestMethod]
        public void EntitySetSetCorrectly()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)), HardCodedTestModel.GetDogsSet(), null);
            expansion.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void FilterOptionSetCorrectly()
        {
            BinaryOperatorNode filterExpression = new BinaryOperatorNode(BinaryOperatorKind.Equal, new ConstantNode(1), new ConstantNode(1));
            FilterClause filterClause = new FilterClause(filterExpression, new EntityRangeVariable(ExpressionConstants.It, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet()));
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, filterClause, null, null, null, null, null, null);
            expansion.FilterOption.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            expansion.FilterOption.Expression.As<BinaryOperatorNode>().Left.ShouldBeConstantQueryNode(1);
            expansion.FilterOption.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(1);
        }

        [TestMethod]
        public void OrderByOptionSetCorrectly()
        {
            SingleValuePropertyAccessNode propertyAccessNode = new SingleValuePropertyAccessNode(new ConstantNode(1), HardCodedTestModel.GetPersonNameProp());
            OrderByClause orderBy = new OrderByClause(null, propertyAccessNode, OrderByDirection.Descending, new EntityRangeVariable(ExpressionConstants.It, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet()));
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, orderBy, null, null, null, null, null);
            expansion.OrderByOption.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
        }

        [TestMethod]
        public void TopOptionSetCorrectly()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, null, 42, null, null, null, null);
            expansion.TopOption.Should().Be(42);
        }

        [TestMethod]
        public void SkipOptionSetCorrectly()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, null, null, 42, null, null, null);
            expansion.SkipOption.Should().Be(42);
        }

        [TestMethod]
        public void CountQueryOptionSetCorrectly()
        {
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, null, null, null, true, null, null);
            expansion.CountOption.Should().BeTrue();
        }

        [TestMethod]
        public void LevelsOptionSetCorrectly()
        {
            LevelsClause levels = new LevelsClause(false, 16384);
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, null, null, null, null, null, levels);
            expansion.LevelsOption.IsMaxLevel.Should().BeFalse();
            expansion.LevelsOption.Level.Should().Be(16384);
        }

        [TestMethod]
        public void SearchOptionSetCorrectly()
        {
            SearchClause search = new SearchClause(new SearchTermNode("SearchMe"));
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), null, null, null, null, null, null, search, null);
            expansion.SearchOption.Expression.ShouldBeSearchTermNode("SearchMe");
        }

        [TestMethod]
        public void SelectExpandOptionSetCorrectly()
        {
            SelectExpandClause selectAndExpand = new SelectExpandClause(null, true);
            ExpandedNavigationSelectItem expansion = new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), null)), HardCodedTestModel.GetPeopleSet(), selectAndExpand, null, null, null, null, null, null, null);
            expansion.SelectAndExpand.AllSelected.Should().BeTrue();
            expansion.SelectAndExpand.SelectedItems.Should().BeEmpty();
        }
    }
}
