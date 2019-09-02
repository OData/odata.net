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
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyDog/Color))");

            ApplyBinder binder = new ApplyBinder(FakeBindMethods.BindMethodReturnsPersonDogColorNavigation, _bindingState);
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

            GroupByPropertyNode colorNode = dogNode.ChildTransformations[0];
            colorNode.Name.Should().Be("Color");
            colorNode.Expression.Should().BeSameAs(FakeBindMethods.FakePersonDogColorNode);
            colorNode.ChildTransformations.Should().HaveCount(0);
        }

        [Fact]
        public void BindApplyWitGroupByWithComplexShouldReturnApplyClause()
        {
            IEnumerable<QueryToken> tokens = _parser.ParseApply("groupby((MyAddress/City))");

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
        public void BindApplyWitComputeShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("compute(UnitPrice mul 5 as BigPrice)");

            var binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            var transformations = actual.Transformations.ToList();
            var compute = transformations[0] as ComputeTransformationNode;

            compute.Should().NotBeNull();
            compute.Kind.Should().Be(TransformationNodeKind.Compute);
            compute.Expressions.Should().HaveCount(1);

            var statements = compute.Expressions.ToList();
            var statement = statements[0];
            VerifyIsFakeSingleValueNode(statement.Expression);
            statement.Alias.ShouldBeEquivalentTo("BigPrice");
        }

        [Fact]
        public void BindApplyWithComputeAfterGroupByShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("groupby((ID, SSN), aggregate(LifeTime with sum as TotalLife))/compute(TotalLife as TotalLife2)");

            BindingState state = new BindingState(_configuration);
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(2);

            var compute = actual.Transformations.Last() as ComputeTransformationNode;

            compute.Should().NotBeNull();
            compute.Kind.Should().Be(TransformationNodeKind.Compute);
            compute.Expressions.Should().HaveCount(1);

            var statements = compute.Expressions.ToList();
            var statement = statements[0];
            statement.Alias.ShouldBeEquivalentTo("TotalLife2");
        }


        [Fact]
        public void BindApplyWithMultipleGroupBysShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("groupby((MyDog/Color, MyDog/Breed))/groupby((MyDog/Color), aggregate(MyDog/Breed with max as MaxBreed))");

            BindingState state = new BindingState(_configuration);
            MetadataBinder metadataBiner = new MetadataBinder(_bindingState);

            ApplyBinder binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(2);

            var groupBy = actual.Transformations.Last() as GroupByTransformationNode;
            groupBy.Should().NotBeNull();
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
        public void BindApplyWithComputeShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("compute(UnitPrice mul 5 as BigPrice)");

            var binder = new ApplyBinder(FakeBindMethods.BindSingleComplexProperty, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            var transformations = actual.Transformations.ToList();
            var compute = transformations[0] as ComputeTransformationNode;

            compute.Should().NotBeNull();
            compute.Kind.Should().Be(TransformationNodeKind.Compute);
            compute.Expressions.Should().HaveCount(1);

            var statements = compute.Expressions.ToList();
            var statement = statements[0];
            VerifyIsFakeSingleValueNode(statement.Expression);
            statement.Alias.ShouldBeEquivalentTo("BigPrice");
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

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            AggregateTransformationNode aggregate = actual.Transformations.First() as AggregateTransformationNode;
            aggregate.Should().NotBeNull();
            aggregate.AggregateExpressions.Should().HaveCount(1);

            EntitySetAggregateExpression entitySetAggregate = aggregate.AggregateExpressions.First() as EntitySetAggregateExpression;
            entitySetAggregate.Should().NotBeNull();
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

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(2);

            ExpandTransformationNode expand = actual.Transformations.First() as ExpandTransformationNode;
            expand.Should().NotBeNull();
            expand.ExpandClause.Should().NotBeNull();
            expand.ExpandClause.SelectedItems.Should().HaveCount(1);
            ExpandedNavigationSelectItem expandItem = expand.ExpandClause.SelectedItems.First() as ExpandedNavigationSelectItem;
            expandItem.Should().NotBeNull();
            expandItem.NavigationSource.Name.ShouldBeEquivalentTo("Paintings");
            expandItem.FilterOption.Should().NotBeNull();

            GroupByTransformationNode groupBy = actual.Transformations.Last() as GroupByTransformationNode;
            groupBy.Should().NotBeNull();
            groupBy.GroupingProperties.Should().HaveCount(1);

            AggregateTransformationNode aggregate = groupBy.ChildTransformations as AggregateTransformationNode;
            aggregate.Should().NotBeNull();
            aggregate.AggregateExpressions.Should().HaveCount(1);

            EntitySetAggregateExpression entitySetAggregate = aggregate.AggregateExpressions.First() as EntitySetAggregateExpression;
            entitySetAggregate.Should().NotBeNull();
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

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            ExpandTransformationNode expand = actual.Transformations.First() as ExpandTransformationNode;
            expand.Should().NotBeNull();
            expand.ExpandClause.Should().NotBeNull();
            expand.ExpandClause.SelectedItems.Should().HaveCount(1);
            ExpandedNavigationSelectItem expandItem = expand.ExpandClause.SelectedItems.First() as ExpandedNavigationSelectItem;
            expandItem.Should().NotBeNull();
            expandItem.NavigationSource.Name.ShouldBeEquivalentTo("Paintings");
            expandItem.SelectAndExpand.Should().NotBeNull();
            expandItem.SelectAndExpand.SelectedItems.Should().HaveCount(1);
            expandItem.FilterOption.Should().NotBeNull();

            ExpandedNavigationSelectItem expandItem1 = expandItem.SelectAndExpand.SelectedItems.First() as ExpandedNavigationSelectItem;
            expandItem1.FilterOption.Should().NotBeNull();
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

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(2);
            GroupByTransformationNode groupby = actual.Transformations.First() as GroupByTransformationNode;
            groupby.Should().NotBeNull();
            AggregateTransformationNode aggregate = actual.Transformations.Last() as AggregateTransformationNode;
            aggregate.Should().NotBeNull();
            aggregate.AggregateExpressions.Should().HaveCount(1);
            aggregate.AggregateExpressions.Single().As<AggregateExpression>().Method.ShouldBeEquivalentTo(AggregationMethod.VirtualPropertyCount);

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
