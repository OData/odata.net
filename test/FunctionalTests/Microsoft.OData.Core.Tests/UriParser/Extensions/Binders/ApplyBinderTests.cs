//---------------------------------------------------------------------
// <copyright file="ApplyBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
            Assert.Throws<ArgumentNullException>("tokens", bind);
        }

        [Fact]
        public void BindApplyWithAggregateShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("aggregate(UnitPrice with sum as TotalPrice)");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            AggregateTransformationNode aggregate = Assert.IsType<AggregateTransformationNode>(Assert.Single(actual.Transformations));

            Assert.Equal(TransformationNodeKind.Aggregate, aggregate.Kind);
            Assert.NotNull(aggregate.AggregateExpressions);

            AggregateExpression statement = Assert.IsType<AggregateExpression>(Assert.Single(aggregate.AggregateExpressions));
            VerifyIsFakeSingleValueNode(statement.Expression);
            Assert.Equal(AggregationMethod.Sum, statement.Method);
            Assert.Equal("TotalPrice", statement.Alias);
        }

        [Fact]
        public void BindApplyWithAverageInAggregateShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("aggregate(UnitPrice with average as AveragePrice)");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindMethodReturningASingleFloatPrimitive, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            AggregateTransformationNode aggregate = Assert.IsType<AggregateTransformationNode>(Assert.Single(actual.Transformations));

            Assert.Equal(TransformationNodeKind.Aggregate, aggregate.Kind);
            Assert.NotNull(aggregate.AggregateExpressions);

            AggregateExpression statement = Assert.IsType<AggregateExpression>(Assert.Single(aggregate.AggregateExpressions));
            Assert.NotNull(statement.Expression);
            Assert.Same(FakeBindMethods.FakeSingleFloatPrimitive, statement.Expression);
            Assert.Equal(AggregationMethod.Average, statement.Method);
            Assert.Equal("AveragePrice", statement.Alias);
        }

        [Fact]
        public void BindApplyWithCountInAggregateShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("aggregate($count as TotalCount)");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            AggregateTransformationNode aggregate = Assert.IsType<AggregateTransformationNode>(Assert.Single(actual.Transformations));

            Assert.Equal(TransformationNodeKind.Aggregate, aggregate.Kind);
            Assert.NotNull(aggregate.AggregateExpressions);
            AggregateExpressionBase statementBase = Assert.Single(aggregate.AggregateExpressions);
            AggregateExpression statement = Assert.IsType<AggregateExpression>(statementBase);

            Assert.Equal(AggregationMethod.VirtualPropertyCount, statement.Method);
            Assert.Equal("TotalCount", statement.Alias);
        }

        [Fact]
        public void BindApplyWithAggregateAndFilterShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("aggregate(StockQuantity with sum as TotalPrice)/filter(TotalPrice eq 100)");
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);
            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Transformations.Count());

            List<TransformationNode> transformations = actual.Transformations.ToList();
            Assert.NotNull(transformations[1]);
            FilterTransformationNode filter = Assert.IsType<FilterTransformationNode>(transformations[1]);

            Assert.Equal(TransformationNodeKind.Filter, filter.Kind);

            FilterClause filterClause = filter.FilterClause;
            Assert.NotNull(filterClause.Expression);
            BinaryOperatorNode binaryOperation = Assert.IsType<BinaryOperatorNode>(filterClause.Expression);

            Assert.NotNull(binaryOperation.Left);
            ConvertNode propertyConvertNode = Assert.IsType<ConvertNode>(binaryOperation.Left);
            Assert.NotNull(propertyConvertNode.Source);
            SingleValueOpenPropertyAccessNode propertyAccess = Assert.IsType< SingleValueOpenPropertyAccessNode>(propertyConvertNode.Source);
            Assert.Equal("TotalPrice", propertyAccess.Name);
        }

        [Fact]
        public void BindApplyWitGroupByShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((UnitPrice, SalePrice))");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);

            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(Assert.Single(actual.Transformations));

            Assert.Equal(TransformationNodeKind.GroupBy, groupBy.Kind);
            Assert.NotNull(groupBy.GroupingProperties);
            Assert.Equal(2, groupBy.GroupingProperties.Count());

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            VerifyIsFakeSingleValueNode(groupingProperties[0].Expression);
            VerifyIsFakeSingleValueNode(groupingProperties[1].Expression);

            Assert.Null(groupBy.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitGroupByWithNavigationShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyDog/City))");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindMethodReturnsPersonDogColorNavigation, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(Assert.Single(actual.Transformations));

            Assert.Equal(TransformationNodeKind.GroupBy, groupBy.Kind);
            Assert.NotNull(groupBy.GroupingProperties);
            Assert.Null(groupBy.ChildTransformations);

            GroupByPropertyNode dogNode = Assert.Single(groupBy.GroupingProperties);
            Assert.Null(dogNode.Expression);
            Assert.Equal("MyDog", dogNode.Name);
            Assert.NotNull(dogNode.ChildTransformations);

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            Assert.Null(groupBy.ChildTransformations);

            GroupByPropertyNode colorNode = Assert.Single(dogNode.ChildTransformations);
            Assert.Equal("Color", colorNode.Name);
            Assert.Same(FakeBindMethods.FakePersonDogColorNode, colorNode.Expression);
            Assert.Empty(colorNode.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitGroupByWithComplexShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyAddress/City))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            TransformationNode transformation = Assert.Single(actual.Transformations);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(transformation);

            Assert.Equal(TransformationNodeKind.GroupBy, groupBy.Kind);
            Assert.Null(groupBy.ChildTransformations);
            Assert.NotNull(groupBy.GroupingProperties);
            GroupByPropertyNode addressNode = Assert.Single(groupBy.GroupingProperties);

            Assert.Equal("MyAddress", addressNode.Name);
            Assert.Null(addressNode.Expression);

            GroupByPropertyNode cityNode = Assert.Single(addressNode.ChildTransformations);
            Assert.Equal("City", cityNode.Name);
            Assert.NotNull(cityNode.Expression);
            Assert.Empty(cityNode.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitGroupByWithDeepNavigationShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyDog/FastestOwner/FirstName))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);

            TransformationNode transformation = Assert.Single(actual.Transformations);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(transformation);

            Assert.Equal(TransformationNodeKind.GroupBy, groupBy.Kind);
            Assert.NotNull(groupBy.GroupingProperties);
            Assert.Null(groupBy.ChildTransformations);
            GroupByPropertyNode dogNode = Assert.Single(groupBy.GroupingProperties);
            Assert.Equal("MyDog", dogNode.Name);
            Assert.Null(dogNode.Expression);

            GroupByPropertyNode ownerNode = Assert.Single(dogNode.ChildTransformations);
            Assert.Equal("FastestOwner", ownerNode.Name);
            Assert.Null(ownerNode.Expression);

            GroupByPropertyNode nameNode = Assert.Single(ownerNode.ChildTransformations);
            Assert.Equal("FirstName", nameNode.Name);
            Assert.NotNull(nameNode.Expression);
            Assert.Empty(nameNode.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitGroupByWithDeepComplexShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyAddress/NextHome/City))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);

            var transformation = Assert.Single(actual.Transformations);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(transformation);

            Assert.Equal(TransformationNodeKind.GroupBy, groupBy.Kind);
            Assert.NotNull(groupBy.GroupingProperties);
            Assert.Null(groupBy.ChildTransformations);

            GroupByPropertyNode addressNode = Assert.Single(groupBy.GroupingProperties);
            Assert.Equal("MyAddress", addressNode.Name);
            Assert.Null(addressNode.Expression);

            GroupByPropertyNode nextHomeNode = Assert.Single(addressNode.ChildTransformations);
            Assert.Equal("NextHome", nextHomeNode.Name);
            Assert.Null(nextHomeNode.Expression);

            GroupByPropertyNode cityNode = Assert.Single(nextHomeNode.ChildTransformations);
            Assert.Equal("City", cityNode.Name);
            Assert.NotNull(cityNode.Expression);
            Assert.Empty(cityNode.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitGroupByWithNavigationAndComplexShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyFavoritePainting/ArtistAddress/City))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);

            var transformation = Assert.Single(actual.Transformations);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(transformation);

            Assert.Equal(TransformationNodeKind.GroupBy, groupBy.Kind);
            Assert.Null(groupBy.ChildTransformations);
            Assert.NotNull(groupBy.GroupingProperties);

            GroupByPropertyNode paintingNode = Assert.Single(groupBy.GroupingProperties);
            Assert.Equal("MyFavoritePainting", paintingNode.Name);
            Assert.Null(paintingNode.Expression);

            GroupByPropertyNode artistAddressNode = Assert.Single(paintingNode.ChildTransformations);
            Assert.Equal("ArtistAddress", artistAddressNode.Name);
            Assert.Null(artistAddressNode.Expression);

            GroupByPropertyNode cityNode = Assert.Single(artistAddressNode.ChildTransformations);
            Assert.Equal("City", cityNode.Name);
            Assert.NotNull(cityNode.Expression);
            Assert.Empty(cityNode.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitGroupByWithComplexAndNavigationShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyAddress/PostBoxPainting/Artist))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);

            var transformation = Assert.Single(actual.Transformations);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(transformation);

            Assert.Equal(TransformationNodeKind.GroupBy, groupBy.Kind);
            Assert.Null(groupBy.ChildTransformations);
            Assert.NotNull(groupBy.GroupingProperties);

            GroupByPropertyNode addressNode = Assert.Single(groupBy.GroupingProperties);
            Assert.Equal("MyAddress", addressNode.Name);
            Assert.Null(addressNode.Expression);

            GroupByPropertyNode postBoxPaintingNode = Assert.Single(addressNode.ChildTransformations);
            Assert.Equal("PostBoxPainting", postBoxPaintingNode.Name);
            Assert.Null(postBoxPaintingNode.Expression);

            GroupByPropertyNode artistNode = Assert.Single(postBoxPaintingNode.ChildTransformations);
            Assert.Equal("Artist", artistNode.Name);
            Assert.NotNull(artistNode.Expression);
            Assert.Empty(artistNode.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitGroupByWithDeepNavigationAndComplexShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyDog/LionWhoAteMe/LionHeartbeat/Frequency))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);

            var transformation = Assert.Single(actual.Transformations);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(transformation);

            Assert.Equal(TransformationNodeKind.GroupBy, groupBy.Kind);
            Assert.Null(groupBy.ChildTransformations);
            Assert.NotNull(groupBy.GroupingProperties);

            GroupByPropertyNode dogNode = Assert.Single(groupBy.GroupingProperties);
            Assert.Equal("MyDog", dogNode.Name);
            Assert.Null(dogNode.Expression);

            GroupByPropertyNode lionNode = Assert.Single(dogNode.ChildTransformations);
            Assert.Equal("LionWhoAteMe", lionNode.Name);
            Assert.Null(lionNode.Expression);

            GroupByPropertyNode heartBeatNode = Assert.Single(lionNode.ChildTransformations);
            Assert.Equal("LionHeartbeat", heartBeatNode.Name);
            Assert.Null(heartBeatNode.Expression);

            GroupByPropertyNode frequencyNode = Assert.Single(heartBeatNode.ChildTransformations);
            Assert.Equal("Frequency", frequencyNode.Name);
            Assert.NotNull(frequencyNode.Expression);
            Assert.Empty(frequencyNode.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitGroupByWithDeepComplexAndNavigationShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyAddress/NextHome/PostBoxPainting/Artist))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);

            var transformation = Assert.Single(actual.Transformations);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(transformation);

            Assert.Equal(TransformationNodeKind.GroupBy, groupBy.Kind);
            Assert.Null(groupBy.ChildTransformations);
            Assert.NotNull(groupBy.GroupingProperties);

            GroupByPropertyNode addressNode = Assert.Single(groupBy.GroupingProperties);
            Assert.Equal("MyAddress", addressNode.Name);
            Assert.Null(addressNode.Expression);

            GroupByPropertyNode nextHomeNode = Assert.Single(addressNode.ChildTransformations);
            Assert.Equal("NextHome", nextHomeNode.Name);
            Assert.Null(nextHomeNode.Expression);

            GroupByPropertyNode postBoxPaintingNode = Assert.Single(nextHomeNode.ChildTransformations);
            Assert.Equal("PostBoxPainting", postBoxPaintingNode.Name);
            Assert.Null(postBoxPaintingNode.Expression);

            GroupByPropertyNode artistNode = Assert.Single(postBoxPaintingNode.ChildTransformations);
            Assert.Equal("Artist", artistNode.Name);
            Assert.NotNull(artistNode.Expression);
            Assert.Empty(artistNode.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitGroupByWithComplexAndDeepNavigationAndComplexShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyAddress/PostBoxPainting/Owner/MyAddress/City))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);

            var transformation = Assert.Single(actual.Transformations);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(transformation);

            Assert.Equal(TransformationNodeKind.GroupBy, groupBy.Kind);
            Assert.Null(groupBy.ChildTransformations);
            Assert.NotNull(groupBy.GroupingProperties);

            GroupByPropertyNode addressNode = Assert.Single(groupBy.GroupingProperties);
            Assert.Equal("MyAddress", addressNode.Name);
            Assert.Null(addressNode.Expression);

            GroupByPropertyNode postBoxPaintingNode = Assert.Single(addressNode.ChildTransformations);
            Assert.Equal("PostBoxPainting", postBoxPaintingNode.Name);
            Assert.Null(postBoxPaintingNode.Expression);

            GroupByPropertyNode ownerNode = Assert.Single(postBoxPaintingNode.ChildTransformations);
            Assert.Equal("Owner", ownerNode.Name);
            Assert.Null(ownerNode.Expression);

            GroupByPropertyNode ownerAddressNode = Assert.Single(ownerNode.ChildTransformations);
            Assert.Equal("MyAddress", ownerAddressNode.Name);
            Assert.Null(ownerAddressNode.Expression);

            GroupByPropertyNode cityNode = Assert.Single(ownerAddressNode.ChildTransformations);
            Assert.Equal("City", cityNode.Name);
            Assert.NotNull(cityNode.Expression);
            Assert.Empty(cityNode.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitGroupByWithNavigationAndDeepComplexAndNavigationShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyFavoritePainting/ArtistAddress/NextHome/PostBoxPainting/Artist))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);

            var transformation = Assert.Single(actual.Transformations);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(transformation);

            Assert.Equal(TransformationNodeKind.GroupBy, groupBy.Kind);
            Assert.Null(groupBy.ChildTransformations);
            Assert.NotNull(groupBy.GroupingProperties);

            GroupByPropertyNode favoritePaintingNode = Assert.Single(groupBy.GroupingProperties);
            Assert.Equal("MyFavoritePainting", favoritePaintingNode.Name);
            Assert.Null(favoritePaintingNode.Expression);

            GroupByPropertyNode artistAddressNode = Assert.Single(favoritePaintingNode.ChildTransformations);
            Assert.Equal("ArtistAddress", artistAddressNode.Name);
            Assert.Null(artistAddressNode.Expression);

            GroupByPropertyNode nextHomeNode = Assert.Single(artistAddressNode.ChildTransformations);
            Assert.Equal("NextHome", nextHomeNode.Name);
            Assert.Null(nextHomeNode.Expression);

            GroupByPropertyNode postBoxPaintingNode = Assert.Single(nextHomeNode.ChildTransformations);
            Assert.Equal("PostBoxPainting", postBoxPaintingNode.Name);
            Assert.Null(postBoxPaintingNode.Expression);

            GroupByPropertyNode artistNode = Assert.Single(postBoxPaintingNode.ChildTransformations);
            Assert.Equal("Artist", artistNode.Name);
            Assert.NotNull(artistNode.Expression);
            Assert.Empty(artistNode.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitGroupByWithAggregateShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((UnitPrice, SalePrice), aggregate(UnitPrice with sum as TotalPrice))");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(Assert.Single(actual.Transformations));

            Assert.NotNull(groupBy.ChildTransformations);
        }

        [Fact]
        public void BindApplyWitFilterShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("filter(UnitPrice eq 5)");

            ApplyBinder binder = new ApplyBinder(BindMethodReturnsBooleanPrimitive, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);
            actual = binder.BindApply(tokens);

            Assert.NotNull(actual);

            FilterTransformationNode filter = Assert.IsType<FilterTransformationNode>(Assert.Single(actual.Transformations));

            Assert.Equal(TransformationNodeKind.Filter, filter.Kind);
            Assert.NotNull(filter.FilterClause.Expression);
            Assert.Same(_booleanPrimitiveNode, filter.FilterClause.Expression);
        }

        [Fact]
        public void BindApplyWitComputeShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("compute(UnitPrice mul 5 as BigPrice)");

            var binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            var actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            ComputeTransformationNode compute = Assert.IsType<ComputeTransformationNode>(Assert.Single(actual.Transformations));

            Assert.Equal(TransformationNodeKind.Compute, compute.Kind);
            Assert.NotNull(compute.Expressions);
            ComputeExpression statement = Assert.Single(compute.Expressions);
            VerifyIsFakeSingleValueNode(statement.Expression);
            Assert.Equal("BigPrice", statement.Alias);
        }

        [Fact]
        public void BindApplyWithComputeAfterGroupByShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("groupby((ID, SSN), aggregate(LifeTime with sum as TotalLife))/compute(TotalLife as TotalLife2)");

            BindingState state = new BindingState(_configuration);
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            var actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Transformations.Count());

            ComputeTransformationNode compute = Assert.IsType<ComputeTransformationNode>(actual.Transformations.Last());
            Assert.Equal(TransformationNodeKind.Compute, compute.Kind);
            ComputeExpression statement = Assert.Single(compute.Expressions);
            Assert.Equal("TotalLife2", statement.Alias);
        }

        [Fact]
        public void BindApplyWithMultipleGroupBysShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("groupby((MyDog/Color, MyDog/Breed))/groupby((MyDog/Color), aggregate(MyDog/Breed with max as MaxBreed))");

            BindingState state = new BindingState(_configuration);
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            var actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Transformations.Count());

            Assert.IsType<GroupByTransformationNode>(actual.Transformations.Last());
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

            Assert.NotNull(actual);
            Assert.Equal(4, actual.Transformations.Count());

            List<TransformationNode> transformations = actual.Transformations.ToList();

            Assert.NotNull(transformations[0]);
            Assert.IsType<GroupByTransformationNode>(transformations[0]);

            Assert.NotNull(transformations[1]);
            Assert.IsType<AggregateTransformationNode>(transformations[1]);

            Assert.NotNull(transformations[2]);
            Assert.IsType<GroupByTransformationNode>(transformations[2]);

            Assert.NotNull(transformations[3]);
            Assert.IsType<AggregateTransformationNode>(transformations[3]);
        }

        [Fact]
        public void BindApplyWithComputeShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("compute(UnitPrice mul 5 as BigPrice)");

            var binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            var actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            ComputeTransformationNode compute = Assert.IsType<ComputeTransformationNode>(Assert.Single(actual.Transformations));

            Assert.Equal(TransformationNodeKind.Compute, compute.Kind);
            Assert.NotNull(compute.Expressions);
            ComputeExpression statement = Assert.Single(compute.Expressions);
            VerifyIsFakeSingleValueNode(statement.Expression);
            Assert.Equal("BigPrice", statement.Alias);
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

            Assert.NotNull(actual);
            Assert.NotNull(actual.Transformations);
            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(Assert.Single(actual.Transformations));

            Assert.NotNull(groupBy.GroupingProperties);

            AggregateTransformationNode aggregate = Assert.IsType<AggregateTransformationNode>(groupBy.ChildTransformations);
            Assert.NotNull(aggregate.AggregateExpressions);

            Assert.IsType< EntitySetAggregateExpression>(Assert.Single(aggregate.AggregateExpressions));
        }

        [Fact]
        public void BindApplyWithEntitySetAggregationWithoutGroupByReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens =
                _parser.ParseApply(
                    "aggregate(MyPaintings(Value with sum as TotalValue))");

            BindingState state = new BindingState(_configuration);
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            Assert.NotNull(actual.Transformations);
            AggregateTransformationNode aggregate = Assert.IsType<AggregateTransformationNode>(Assert.Single(actual.Transformations));
            Assert.NotNull(aggregate.AggregateExpressions);
            Assert.IsType<EntitySetAggregateExpression>(Assert.Single(aggregate.AggregateExpressions));
        }

        private readonly ODataUriParserConfiguration V4configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);

        [Fact]
        public void BindApplyWithExpandReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens =
                _parser.ParseApply(
                    "expand(MyPaintings, filter(FrameColor eq 'Red'))/groupby((LifeTime),aggregate(MyPaintings($count as Count)))");

            BindingState state = new BindingState(_configuration);
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState, V4configuration, new ODataPathInfo(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet()));
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Transformations.Count());

            ExpandTransformationNode expand = Assert.IsType<ExpandTransformationNode>(actual.Transformations.First());

            Assert.NotNull(expand.ExpandClause);
            ExpandedNavigationSelectItem expandItem = Assert.IsType<ExpandedNavigationSelectItem>(Assert.Single(expand.ExpandClause.SelectedItems));

            Assert.Equal("Paintings", expandItem.NavigationSource.Name);
            Assert.NotNull(expandItem.FilterOption);

            GroupByTransformationNode groupBy = Assert.IsType<GroupByTransformationNode>(actual.Transformations.Last());
            Assert.Single(groupBy.GroupingProperties);

            AggregateTransformationNode aggregate = Assert.IsType<AggregateTransformationNode>(groupBy.ChildTransformations);
            Assert.IsType<EntitySetAggregateExpression>(Assert.Single(aggregate.AggregateExpressions));
        }

        [Fact]
        public void BindApplyWithNestedExpandReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens =
                _parser.ParseApply(
                    "expand(MyPaintings, filter(FrameColor eq 'Red'), expand(Owner, filter(Name eq 'Me')))");

            BindingState state = new BindingState(_configuration);
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState, V4configuration, new ODataPathInfo(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet()));
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            ExpandTransformationNode expand = Assert.IsType<ExpandTransformationNode>(Assert.Single(actual.Transformations));

            Assert.NotNull(expand.ExpandClause);
            ExpandedNavigationSelectItem expandItem = Assert.IsType<ExpandedNavigationSelectItem>(Assert.Single(expand.ExpandClause.SelectedItems));

            Assert.Equal("Paintings", expandItem.NavigationSource.Name);
            Assert.NotNull(expandItem.SelectAndExpand);
            Assert.NotNull(expandItem.FilterOption);

            ExpandedNavigationSelectItem expandItem1 = Assert.IsType<ExpandedNavigationSelectItem>(Assert.Single(expandItem.SelectAndExpand.SelectedItems));
            Assert.NotNull(expandItem1.FilterOption);
        }

        [Fact]
        public void BindVirtualPropertiesAfterCollapseReturnsApplyClause()
        {
            IEnumerable<QueryToken> tokens =
                _parser.ParseApply(
                    "groupby((ID))/aggregate($count as Count)");

            BindingState state = new BindingState(_configuration);
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Transformations.Count());
            Assert.IsType<GroupByTransformationNode>(actual.Transformations.First());
            AggregateTransformationNode aggregate = Assert.IsType<AggregateTransformationNode>(actual.Transformations.Last());
            AggregateExpression aggExp = Assert.IsType<AggregateExpression>(Assert.Single(aggregate.AggregateExpressions));
            Assert.Equal(AggregationMethod.VirtualPropertyCount, aggExp.Method);
        }

        private static ConstantNode _booleanPrimitiveNode = new ConstantNode(true);

        private static SingleValueNode BindMethodReturnsBooleanPrimitive(QueryToken token)
        {
            return _booleanPrimitiveNode;
        }

        private static void VerifyIsFakeSingleValueNode(QueryNode node)
        {
            Assert.NotNull(node);
            Assert.Same(FakeBindMethods.FakeSingleComplexProperty, node);
        }
    }
}
