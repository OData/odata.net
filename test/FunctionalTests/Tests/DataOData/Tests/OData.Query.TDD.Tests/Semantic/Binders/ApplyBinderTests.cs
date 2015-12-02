namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.Binders
{
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using TestUtilities;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm;

    [TestClass]
    public class ApplyBinderTests
    {
        UriQueryExpressionParser _parser = new UriQueryExpressionParser(50);

        private static readonly ODataUriParserConfiguration _configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);
        private BindingState _bindingState = new BindingState(_configuration);

        [TestInitialize]
        public void Init()
        {
            var implicitRangeVariable = new EntityRangeVariable(ExpressionConstants.It, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            this._bindingState = new BindingState(_configuration) { ImplicitRangeVariable = implicitRangeVariable };
            this._bindingState.RangeVariables.Push(new BindingState(_configuration) { ImplicitRangeVariable = implicitRangeVariable }.ImplicitRangeVariable);
        }

        [TestMethod]
        public void BindApplyWithNullShouldThrow()
        {
            var binder = new ApplyBinder(FakeBindMethods.BindSingleValueProperty, _bindingState);
            Action bind = () => binder.BindApply(null);
            bind.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
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
            aggregate.Statements.Should().NotBeNull();
            aggregate.Statements.Should().HaveCount(1);

            var statements = aggregate.Statements.ToList();
            var statement = statements[0];
            VerifyIsFakeSingleValueNode(statement.Expression);
            statement.WithVerb.Should().Be(AggregationVerb.Sum);
            statement.AsAlias.Should().Be("TotalPrice");

            VerifyDynamicTypeReferenceNames(aggregate.ItemType, new string[] { "TotalPrice" });
        }

        [TestMethod]
        public void BindApplyWithAggregateAndFilterShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("aggregate(StockQuantity with sum as TotalPrice)/filter(TotalPrice eq 100)");

            var binder = new ApplyBinder(FakeBindMethods.BindMethodReturningASingleIntPrimitive, _bindingState);
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
            var propertyAccess = binaryOperation.Left as SingleValuePropertyAccessNode;
            propertyAccess.Should().NotBeNull();
            propertyAccess.Property.Name.Should().Be("TotalPrice");
        }

        [TestMethod]
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
            VerifyIsFakeSingleValueNode(groupingProperties[0].Accessor);
            VerifyIsFakeSingleValueNode(groupingProperties[1].Accessor);

            groupBy.ChildTransformation.Should().BeNull();
        }

        [TestMethod]
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
            dogNode.Accessor.Should().BeNull();
            dogNode.Name.Should().Be("MyDog");
            dogNode.Children.Should().HaveCount(1);

            var nameNode = dogNode.Children[0];

            dogNode.Name.Should().Be("MyDog");

            nameNode.Accessor.Should().BeSameAs(FakeBindMethods.FakePersonDogNameNode);

            groupBy.ChildTransformation.Should().BeNull();
        }

        [TestMethod]
        public void BindApplyWitGroupByWithAggregateShouldReturnApplyClause()
        {
            var tokens = _parser.ParseApply("groupby((UnitPrice, SalePrice), aggregate(UnitPrice with sum as TotalPrice))");

            var binder = new ApplyBinder(FakeBindMethods.BindSingleValueProperty, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(1);

            var transformations = actual.Transformations.ToList();
            var groupBy = transformations[0] as GroupByTransformationNode;            
          
            var aggregate = groupBy.ChildTransformation;
            aggregate.Should().NotBeNull();            
            VerifyDynamicTypeReferenceNames(aggregate.ItemType, new string[] { "TotalPrice" });
        }

        [TestMethod]
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

        [TestMethod]
        public void BindApplyWitMultipleTokensShouldReturnApplyClause()
        {

            var tokens = _parser.ParseApply("groupby((ID, SSN, LifeTime))/aggregate(LifeTime with sum as TotalLife)/groupby((TotalLife))");

            var state = new BindingState(_configuration);
            var metadataBiner = new MetadataBinder(_bindingState);

            var binder = new ApplyBinder(metadataBiner.Bind, _bindingState);
            var actual = binder.BindApply(tokens);

            actual.Should().NotBeNull();
            actual.Transformations.Should().HaveCount(3);

            var transformations = actual.Transformations.ToList();
            var groupBy0 = transformations[0] as GroupByTransformationNode;
            groupBy0.Should().NotBeNull();
            var aggregate = transformations[1] as AggregateTransformationNode;
            aggregate.Should().NotBeNull();
            var groupBy1 = transformations[2] as GroupByTransformationNode;
            groupBy1.Should().NotBeNull();            
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

        private static void VerifyDynamicTypeReferenceNames(IEdmTypeReference type, string[] propertyNames)
        {
            type.Should().NotBeNull();

            var definition = type.Definition as EdmStructuredType;
            definition.Should().NotBeNull();
            definition.DeclaredProperties.Should().NotBeNull();
            definition.DeclaredProperties.Should().HaveCount(propertyNames.Length);

            var properties = definition.DeclaredProperties.ToList();

            for (int i = 0; i < properties.Count; i++)
            {
                properties[i].Name.Should().Be(propertyNames[i]);
            }
        }
    }
}
