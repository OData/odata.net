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
        public void BindApplyWithCountInAggregateShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("aggregate($count as TotalCount)");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            AggregateTransformationNode aggregate = Assert.IsType<AggregateTransformationNode>(Assert.Single(actual.Transformations));

            Assert.Equal(TransformationNodeKind.Aggregate, aggregate.Kind);
            Assert.NotNull(aggregate.Expressions);
            AggregateExpression statement = Assert.Single(aggregate.Expressions);

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
            Assert.Equal(colorNode.ChildTransformations.Count(), 0);
        }

        [Fact]
        public void BindApplyWitGroupByWithComplexShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyAddress/City))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            Assert.NotNull(actual);
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);
            groupBy.ChildTransformations.Should().BeNull();

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            GroupByPropertyNode addressNode = groupingProperties[0];
            addressNode.Name.Should().Be("MyAddress");
            addressNode.Expression.Should().BeNull();
            addressNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode cityNode = addressNode.ChildTransformations[0];
            cityNode.Name.Should().Be("City");
            cityNode.Expression.Should().NotBeNull();
            cityNode.ChildTransformations.Should().BeEmpty();
        }

        [Fact]
        public void BindApplyWitGroupByWithDeepNavigationShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyDog/FastestOwner/FirstName))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);
            groupBy.ChildTransformations.Should().BeNull();

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            GroupByPropertyNode dogNode = groupingProperties[0];
            dogNode.Name.Should().Be("MyDog");
            dogNode.Expression.Should().BeNull();
            dogNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode ownerNode = dogNode.ChildTransformations[0];
            ownerNode.Name.Should().Be("FastestOwner");
            ownerNode.Expression.Should().BeNull();
            ownerNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode nameNode = ownerNode.ChildTransformations[0];
            nameNode.Name.Should().Be("FirstName");
            nameNode.Expression.Should().NotBeNull();
            nameNode.ChildTransformations.Should().BeEmpty();
        }

        [Fact]
        public void BindApplyWitGroupByWithDeepComplexShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyAddress/NextHome/City))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);
            groupBy.ChildTransformations.Should().BeNull();

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            GroupByPropertyNode addressNode = groupingProperties[0];
            addressNode.Name.Should().Be("MyAddress");
            addressNode.Expression.Should().BeNull();
            addressNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode nextHomeNode = addressNode.ChildTransformations[0];
            nextHomeNode.Name.Should().Be("NextHome");
            nextHomeNode.Expression.Should().BeNull();
            nextHomeNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode cityNode = nextHomeNode.ChildTransformations[0];
            cityNode.Name.Should().Be("City");
            cityNode.Expression.Should().NotBeNull();
            cityNode.ChildTransformations.Should().BeEmpty();
        }

        [Fact]
        public void BindApplyWitGroupByWithNavigationAndComplexShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyFavoritePainting/ArtistAddress/City))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);
            groupBy.ChildTransformations.Should().BeNull();

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            GroupByPropertyNode paintingNode = groupingProperties[0];
            paintingNode.Name.Should().Be("MyFavoritePainting");
            paintingNode.Expression.Should().BeNull();
            paintingNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode artistAddressNode = paintingNode.ChildTransformations[0];
            artistAddressNode.Name.Should().Be("ArtistAddress");
            artistAddressNode.Expression.Should().BeNull();
            artistAddressNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode cityNode = artistAddressNode.ChildTransformations[0];
            cityNode.Name.Should().Be("City");
            cityNode.Expression.Should().NotBeNull();
            cityNode.ChildTransformations.Should().BeEmpty();
        }

        [Fact]
        public void BindApplyWitGroupByWithComplexAndNavigationShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyAddress/PostBoxPainting/Artist))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);
            groupBy.ChildTransformations.Should().BeNull();

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            GroupByPropertyNode addressNode = groupingProperties[0];
            addressNode.Name.Should().Be("MyAddress");
            addressNode.Expression.Should().BeNull();
            addressNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode postBoxPaintingNode = addressNode.ChildTransformations[0];
            postBoxPaintingNode.Name.Should().Be("PostBoxPainting");
            postBoxPaintingNode.Expression.Should().BeNull();
            postBoxPaintingNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode artistNode = postBoxPaintingNode.ChildTransformations[0];
            artistNode.Name.Should().Be("Artist");
            artistNode.Expression.Should().NotBeNull();
            artistNode.ChildTransformations.Should().BeEmpty();
        }

        [Fact]
        public void BindApplyWitGroupByWithDeepNavigationAndComplexShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyDog/LionWhoAteMe/LionHeartbeat/Frequency))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);
            groupBy.ChildTransformations.Should().BeNull();

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            GroupByPropertyNode dogNode = groupingProperties[0];
            dogNode.Name.Should().Be("MyDog");
            dogNode.Expression.Should().BeNull();
            dogNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode lionNode = dogNode.ChildTransformations[0];
            lionNode.Name.Should().Be("LionWhoAteMe");
            lionNode.Expression.Should().BeNull();
            lionNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode heartBeatNode = lionNode.ChildTransformations[0];
            heartBeatNode.Name.Should().Be("LionHeartbeat");
            heartBeatNode.Expression.Should().BeNull();
            heartBeatNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode frequencyNode = heartBeatNode.ChildTransformations[0];
            frequencyNode.Name.Should().Be("Frequency");
            frequencyNode.Expression.Should().NotBeNull();
            frequencyNode.ChildTransformations.Should().BeEmpty();
        }

        [Fact]
        public void BindApplyWitGroupByWithDeepComplexAndNavigationShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyAddress/NextHome/PostBoxPainting/Artist))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);
            groupBy.ChildTransformations.Should().BeNull();

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            GroupByPropertyNode addressNode = groupingProperties[0];
            addressNode.Name.Should().Be("MyAddress");
            addressNode.Expression.Should().BeNull();
            addressNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode nextHomeNode = addressNode.ChildTransformations[0];
            nextHomeNode.Name.Should().Be("NextHome");
            nextHomeNode.Expression.Should().BeNull();
            nextHomeNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode postBoxPaintingNode = nextHomeNode.ChildTransformations[0];
            postBoxPaintingNode.Name.Should().Be("PostBoxPainting");
            postBoxPaintingNode.Expression.Should().BeNull();
            postBoxPaintingNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode artistNode = postBoxPaintingNode.ChildTransformations[0];
            artistNode.Name.Should().Be("Artist");
            artistNode.Expression.Should().NotBeNull();
            artistNode.ChildTransformations.Should().BeEmpty();
        }

        [Fact]
        public void BindApplyWitGroupByWithComplexAndDeepNavigationAndComplexShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyAddress/PostBoxPainting/Owner/MyAddress/City))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);
            groupBy.ChildTransformations.Should().BeNull();

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            GroupByPropertyNode addressNode = groupingProperties[0];
            addressNode.Name.Should().Be("MyAddress");
            addressNode.Expression.Should().BeNull();
            addressNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode postBoxPaintingNode = addressNode.ChildTransformations[0];
            postBoxPaintingNode.Name.Should().Be("PostBoxPainting");
            postBoxPaintingNode.Expression.Should().BeNull();
            postBoxPaintingNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode ownerNode = postBoxPaintingNode.ChildTransformations[0];
            ownerNode.Name.Should().Be("Owner");
            ownerNode.Expression.Should().BeNull();
            ownerNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode ownerAddressNode = ownerNode.ChildTransformations[0];
            ownerAddressNode.Name.Should().Be("MyAddress");
            ownerAddressNode.Expression.Should().BeNull();
            ownerAddressNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode cityNode = ownerAddressNode.ChildTransformations[0];
            cityNode.Name.Should().Be("City");
            cityNode.Expression.Should().NotBeNull();
            cityNode.ChildTransformations.Should().BeEmpty();
        }

        [Fact]
        public void BindApplyWitGroupByWithNavigationAndDeepComplexAndNavigationShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyFavoritePainting/ArtistAddress/NextHome/PostBoxPainting/Artist))");

            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            ApplyClause actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            List<TransformationNode> transformations = actual.Transformations.ToList();
            GroupByTransformationNode groupBy = transformations[0] as GroupByTransformationNode;

            groupBy.Should().NotBeNull();
            groupBy.Kind.Should().Be(TransformationNodeKind.GroupBy);
            groupBy.GroupingProperties.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);
            groupBy.ChildTransformations.Should().BeNull();

            List<GroupByPropertyNode> groupingProperties = groupBy.GroupingProperties.ToList();
            GroupByPropertyNode favoritePaintingNode = groupingProperties[0];
            favoritePaintingNode.Name.Should().Be("MyFavoritePainting");
            favoritePaintingNode.Expression.Should().BeNull();
            favoritePaintingNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode artistAddressNode = favoritePaintingNode.ChildTransformations[0];
            artistAddressNode.Name.Should().Be("ArtistAddress");
            artistAddressNode.Expression.Should().BeNull();
            artistAddressNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode nextHomeNode = artistAddressNode.ChildTransformations[0];
            nextHomeNode.Name.Should().Be("NextHome");
            nextHomeNode.Expression.Should().BeNull();
            nextHomeNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode postBoxPaintingNode = nextHomeNode.ChildTransformations[0];
            postBoxPaintingNode.Name.Should().Be("PostBoxPainting");
            postBoxPaintingNode.Expression.Should().BeNull();
            postBoxPaintingNode.ChildTransformations.Should().HaveCount(1);

            GroupByPropertyNode artistNode = postBoxPaintingNode.ChildTransformations[0];
            artistNode.Name.Should().Be("Artist");
            artistNode.Expression.Should().NotBeNull();
            artistNode.ChildTransformations.Should().BeEmpty();
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

        public static void VerifyIsFakeSingleValueNode(QueryNode node)
        {
            Assert.NotNull(node);
            Assert.Same(FakeBindMethods.FakeSingleComplexProperty, node);
        }
    }
}
