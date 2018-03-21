//---------------------------------------------------------------------
// <copyright file="ApplyBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Tests.UriParser.Binders;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Extensions.Binders
{
    public class ApplyBinderTests
    {
        UriQueryExpressionParser _parser = new UriQueryExpressionParser(50);

        private static readonly ODataUriParserConfiguration _configuration =
            new ODataUriParserConfiguration(HardCodedTestModel.TestModel);
        private BindingState _bindingState = new BindingState(_configuration);

        public ApplyBinderTests()
        {
            ResourceRangeVariable implicitRangeVariable = new ResourceRangeVariable(ExpressionConstants.It,
                HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            this._bindingState = new BindingState(_configuration) { ImplicitRangeVariable = implicitRangeVariable };
            this._bindingState.RangeVariables.Push(
                new BindingState(_configuration) { ImplicitRangeVariable = implicitRangeVariable }.ImplicitRangeVariable);
        }

        [Fact]
        public void BindApplyWithNullShouldThrow()
        {
            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            Action bind = () => binder.BindApply(null);
            bind.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void BindApplyWithAggregateShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("aggregate(UnitPrice with sum as TotalPrice)");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            AggregateTransformationNode aggregate = transformations[0] as AggregateTransformationNode;

            aggregate.Should().NotBeNull();
            aggregate.Kind.Should().Be(TransformationNodeKind.Aggregate);
            aggregate.AggregateExpressions.Should().NotBeNull();
            aggregate.AggregateExpressions.Should().HaveCount(1);

            List<AggregateExpressionBase> statements = aggregate.AggregateExpressions.ToList();
            AggregateExpression statement = statements[0] as AggregateExpression;
            statement.Should().NotBeNull();
            VerifyIsFakeSingleValueNode(statement.Expression);
            statement.Method.Should().Be(AggregationMethod.Sum);
            statement.Alias.Should().Be("TotalPrice");
        }

        [Fact]
        public void BindApplyWithCountInAggregateShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("aggregate($count as TotalCount)");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            AggregateTransformationNode aggregate = transformations[0] as AggregateTransformationNode;

            aggregate.Should().NotBeNull();
            aggregate.Kind.Should().Be(TransformationNodeKind.Aggregate);
            aggregate.Expressions.Should().NotBeNull();
            aggregate.Expressions.Should().HaveCount(1);

            List<AggregateExpression> statements = aggregate.Expressions.ToList();
            AggregateExpression statement = statements[0];

            statement.Method.Should().Be(AggregationMethod.VirtualPropertyCount);
            statement.Alias.Should().Be("TotalCount");
        }

        [Fact]
        public void BindApplyWithAggregateAndFilterShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("aggregate(StockQuantity with sum as TotalPrice)/filter(TotalPrice eq 100)");
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);
            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(2);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            FilterTransformationNode filter = transformations[1] as FilterTransformationNode;

            filter.Should().NotBeNull();
            filter.Kind.Should().Be(TransformationNodeKind.Filter);

            FilterClause filterClause = filter.FilterClause;
            filterClause.Expression.Should().NotBeNull();
            BinaryOperatorNode binaryOperation = filterClause.Expression as BinaryOperatorNode;
            binaryOperation.Should().NotBeNull();
            ConvertNode propertyConvertNode = binaryOperation.Left as ConvertNode;
            propertyConvertNode.Should().NotBeNull();
            SingleValueOpenPropertyAccessNode propertyAccess = propertyConvertNode.Source as SingleValueOpenPropertyAccessNode;
            propertyAccess.Should().NotBeNull();
            propertyAccess.Name.Should().Be("TotalPrice");
        }

        [Fact]
        public void BindApplyWitGroupByShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((UnitPrice, SalePrice))");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(2);

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            VerifyIsFakeSingleValueNode(groupingProperties[0].Expression);
            VerifyIsFakeSingleValueNode(groupingProperties[1].Expression);

            groupBy.ChildTransformations.Should().BeNull();
        }

        [Fact]
        public void BindApplyWitGroupByWithNavigationShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyDog/City))");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindMethodReturnsPersonDogNameNavigation, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            GroupByPropertyNode dogNode = groupingProperties[0];
            dogNode.Expression.Should().BeNull();
            dogNode.Name.Should().Be("MyDog");
            dogNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode nameNode = dogNode.ChildTransformations[0];

            dogNode.Name.Should().Be("MyDog");

            nameNode.Expression.Should().BeSameAs(FakeBindMethods.FakePersonDogNameNode);

            groupBy.ChildTransformations.Should().BeNull();
        }

        [Fact]
        public void BindApplyWitGroupByWithAggregateShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((UnitPrice, SalePrice), aggregate(UnitPrice with sum as TotalPrice))");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            TransformationNode aggregate = groupBy.ChildTransformations;
            aggregate.Should().NotBeNull();
        }

        [Fact]
        public void BindApplyWitFilterShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("filter(UnitPrice eq 5)");

            ApplyBinder binder = new ApplyBinder(BindMethodReturnsBooleanPrimitive, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);
            actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            FilterTransformationNode filter = transformations[0] as FilterTransformationNode;

            filter.Should().NotBeNull();
            filter.Kind.Should().Be(TransformationNodeKind.Filter);
            filter.FilterClause.Expression.Should().NotBeNull();
            filter.FilterClause.Expression.Should().BeSameAs(_booleanPrimitiveNode);
        }

        [Fact]
        public void BindApplyWitMultipleTokensShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens =
                _parser.ParseApply(
                    "groupby((ID, SSN, LifeTime))/aggregate(LifeTime with sum as TotalLife)/groupby((TotalLife))/aggregate(TotalLife with sum as TotalTotalLife)");

            BindingState state = new BindingState(_configuration);
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(4);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode firstGroupBy = transformations[0] as GroupByTransformationNode;
            firstGroupBy.Should().NotBeNull();
            TransformationNode firstAggregate = transformations[1] as AggregateTransformationNode;
            firstAggregate.Should().NotBeNull();
            TransformationNode scecondGroupBy = transformations[2] as GroupByTransformationNode;
            scecondGroupBy.Should().NotBeNull();
            AggregateTransformationNode scecondAggregate = transformations[3] as AggregateTransformationNode;
            scecondAggregate.Should().NotBeNull();
        }

        [Fact]
        public void BindApplyWithEntitySetAggregationReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens =
                _parser.ParseApply(
                    "groupby((LifeTime),aggregate(MyPaintings($count as Count)))");

            BindingState state = new BindingState(_configuration);
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            GroupByTransformationNode groupBy = actual.Transformations.First() as GroupByTransformationNode;
            groupBy.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);

            AggregateTransformationNode aggregate = groupBy.ChildTransformations as AggregateTransformationNode;
            aggregate.Should().NotBeNull();
            aggregate.AggregateExpressions.Should().HaveCount(1);

            EntitySetAggregateExpression entitySetAggregate = aggregate.AggregateExpressions.First() as EntitySetAggregateExpression;
            entitySetAggregate.Should().NotBeNull();
        }

    private static ConstantNode _booleanPrimitiveNode = new ConstantNode(true);

        private static SingleValueNode BindMethodReturnsBooleanPrimitive(QueryToken token)
        {
            return _booleanPrimitiveNode;
        }

        public static void VerifyIsFakeSingleValueNode(QueryNode node)
        {
            node.Should().NotBeNull();
            node.Should().BeSameAs(FakeBindMethods.FakeSingleComplexProperty);
        }
    }
}
