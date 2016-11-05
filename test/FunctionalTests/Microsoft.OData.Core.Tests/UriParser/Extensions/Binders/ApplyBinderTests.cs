//---------------------------------------------------------------------
// <copyright file="ApplyBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.Tests.UriParser.Binders;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Aggregation;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Syntactic;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Extensions.Binders
{
    public class ApplyBinderTests
    {
        UriQueryExpressionParser _parser = new UriQueryExpressionParser(50);

        private static readonly ODataUriParserConfiguration _configuration =
            new ODataUriParserConfiguration(HardCodedTestModel.TestModel);
        private BindingState _bindingState = new BindingState(_configuration);

        public ApplyBinderTests()
        {
            var implicitRangeVariable = new EntityRangeVariable(ExpressionConstants.It,
                HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            this._bindingState = new BindingState(_configuration) { ImplicitRangeVariable = implicitRangeVariable };
            this._bindingState.RangeVariables.Push(
                new BindingState(_configuration) { ImplicitRangeVariable = implicitRangeVariable }.ImplicitRangeVariable);
        }

        [Fact]
        public void BindApplyWithNullShouldThrow()
        {
            var binder = new ApplyBinder(FakeBindMethods.BindSingleValueProperty, _bindingState);
            Action bind = () => binder.BindApply(null);
            bind.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void BindApplyWithAggregateShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("aggregate(UnitPrice with sum as TotalPrice)");

            var binder = new ApplyBinder(FakeBindMethods.BindSingleValueProperty, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            var transformations = actual.Transformations.ToList();
            var aggregate = transformations[0] as AggregateTransformationNode;

            aggregate.Should().NotBeNull();
            aggregate.Kind.Should().Be(TransformationNodeKind.Aggregate);
            aggregate.Expressions.Should().NotBeNull();
            aggregate.Expressions.Should().HaveCount(1);

            var statements = aggregate.Expressions.ToList();
            var statement = statements[0];
            VerifyIsFakeSingleValueNode(statement.Expression);
            statement.Method.Should().Be(AggregationMethod.Sum);
            statement.Alias.Should().Be("TotalPrice");
        }

        [Fact]
        public void BindApplyWithAggregateAndFilterShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("aggregate(StockQuantity with sum as TotalPrice)/filter(TotalPrice eq 100)");
            var metadataBiner = new MetadataBinder(_bindingState);
            var binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(2);

            var transformations = actual.Transformations.ToList();
            var filter = transformations[1] as FilterTransformationNode;

            filter.Should().NotBeNull();
            filter.Kind.Should().Be(TransformationNodeKind.Filter);

            var filtareClause = filter.FilterClause;
            filtareClause.Expression.Should().NotBeNull();
            var binaryOperation = filtareClause.Expression as BinaryOperatorNode;
            binaryOperation.Should().NotBeNull();
            var propertyConvertNode = binaryOperation.Left as ConvertNode;
            propertyConvertNode.Should().NotBeNull();
            var propertyAccess = propertyConvertNode.Source as SingleValueOpenPropertyAccessNode;
            propertyAccess.Should().NotBeNull();
            propertyAccess.Name.Should().Be("TotalPrice");
        }

        [Fact]
        public void BindApplyWitGroupByShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("groupby((UnitPrice, SalePrice))");

            var binder = new ApplyBinder(FakeBindMethods.BindSingleValueProperty, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            var transformations = actual.Transformations.ToList();
            var groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(2);

            var groupingProperties = groupBy.GroupingProperties.ToList();
            VerifyIsFakeSingleValueNode(groupingProperties[0].Expression);
            VerifyIsFakeSingleValueNode(groupingProperties[1].Expression);

            groupBy.ChildTransformations.Should().BeNull();
        }

        [Fact]
        public void BindApplyWitGroupByWithNavigationShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("groupby((MyDog/City))");

            var binder = new ApplyBinder(FakeBindMethods.BindMethodReturnsPersonDogNameNavigation, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            var transformations = actual.Transformations.ToList();
            var groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);

            var groupingProperties = groupBy.GroupingProperties.ToList();
            var dogNode = groupingProperties[0];
            dogNode.Expression.Should().BeNull();
            dogNode.Name.Should().Be("MyDog");
            dogNode.ChildTransformations.Should().HaveCount(1);

            var nameNode = dogNode.ChildTransformations[0];

            dogNode.Name.Should().Be("MyDog");

            nameNode.Expression.Should().BeSameAs(FakeBindMethods.FakePersonDogNameNode);

            groupBy.ChildTransformations.Should().BeNull();
        }

        [Fact]
        public void BindApplyWitGroupByWithAggregateShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("groupby((UnitPrice, SalePrice), aggregate(UnitPrice with sum as TotalPrice))");

            var binder = new ApplyBinder(FakeBindMethods.BindSingleValueProperty, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            var transformations = actual.Transformations.ToList();
            var groupBy = transformations[0] as GroupByTransformationNode;

            var aggregate = groupBy.ChildTransformations;
            aggregate.Should().NotBeNull();
        }

        [Fact]
        public void BindApplyWitFilterShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("filter(UnitPrice eq 5)");

            var binder = new ApplyBinder(BindMethodReturnsBooleanPrimitive, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            var transformations = actual.Transformations.ToList();
            var filter = transformations[0] as FilterTransformationNode;

            filter.Should().NotBeNull();
            filter.Kind.Should().Be(TransformationNodeKind.Filter);
            filter.FilterClause.Expression.Should().NotBeNull();
            filter.FilterClause.Expression.Should().BeSameAs(_booleanPrimitiveNode);
        }

        [Fact]
        public void BindApplyWitMultipleTokensShouldReturnApplyClause()
        {
            var tokens =
                _parser.ParseApply(
                    "groupby((ID, SSN, LifeTime))/aggregate(LifeTime with sum as TotalLife)/groupby((TotalLife))/aggregate(TotalLife with sum as TotalTotalLife)");

            var state = new BindingState(_configuration);
            var metadataBiner = new MetadataBinder(_bindingState);

            var binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(4);

            var transformations = actual.Transformations.ToList();
            var firstGroupBy = transformations[0] as GroupByTransformationNode;
            firstGroupBy.Should().NotBeNull();
            var firstAggregate = transformations[1] as AggregateTransformationNode;
            firstAggregate.Should().NotBeNull();
            var scecondGroupBy = transformations[2] as GroupByTransformationNode;
            scecondGroupBy.Should().NotBeNull();
            var scecondAggregate = transformations[3] as AggregateTransformationNode;
            scecondAggregate.Should().NotBeNull();
        }

        private static ConstantNode _booleanPrimitiveNode = new ConstantNode(true);

        private static SingleValueNode BindMethodReturnsBooleanPrimitive(QueryToken token)
        {
            return _booleanPrimitiveNode;
        }

        public static void VerifyIsFakeSingleValueNode(QueryNode node)
        {
            node.Should().NotBeNull();
            node.Should().BeSameAs(FakeBindMethods.FakeSingleValueProperty);
        }
    }
}
