//---------------------------------------------------------------------
// <copyright file="SemanticTreeFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Legacy long-span integration tests that use SemanticTree to test various features.
    /// 
    /// TODO: remove tests that have already been covered by unit tests or functional tests through our actual public APIs.
    /// Note that there is some value is having these as integration tests for (what will become) ParseUri(). But the issues found
    /// here should be coverable in more targeted unit tests.
    /// </summary>
    public class SemanticTreeFunctionalTests
    {
        private readonly IEdmModel edmModel = HardCodedTestModel.TestModel;

        [Fact]
        public void OrderByAndFilterShouldPopulateBothProperties()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=true&$orderby=Shoe", this.edmModel);
            semanticTree.Filter.Should().NotBeNull();
            semanticTree.OrderBy.Should().NotBeNull();
        }

        [Fact]
        public void PathShouldBeAssociatedWithCorrectSemanticNodes()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs(2)/MyPeople?$filter=true", this.edmModel);

            VerificationHelpers.VerifyPath(semanticTree.Path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet()),
                s => s.ShouldBeSimpleKeySegment(2),
                s => s.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp())
            });

            semanticTree.Filter.ItemType.Definition.Should().Be(HardCodedTestModel.GetPersonType());
        }

        [Fact]
        public void CastInFilterShouldCreateCorrectSemanticNode()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/Fully.Qualified.Namespace.Dog/Color eq 'Black'", this.edmModel);

            var cmp = this.edmModel.FindType("Fully.Qualified.Namespace.Dog");
            semanticTree.Filter.Expression.As<BinaryOperatorNode>().Left.As
                <SingleValuePropertyAccessNode>().Source.As<SingleEntityCastNode>().EntityTypeReference.Definition.Should().Be(cmp);
        }

        [Fact]
        public void CastShouldWorkAsFirstSegmentInFilter()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=Fully.Qualified.Namespace.Employee/WorkEmail eq 'bob@yahoo.com'", this.edmModel);

            var cmp = this.edmModel.FindType("Fully.Qualified.Namespace.Employee");
            semanticTree.Filter.Expression.As<BinaryOperatorNode>().Left.As
                <SingleValuePropertyAccessNode>().Source.As<SingleEntityCastNode>().EntityTypeReference.Definition.Should().Be(cmp);
        }

        [Fact]
        public void InvalidCastInFilterShouldFail()
        {
            Action test = () => HardCodedTestModel.ParseUri("People?$filter=Fully.Qualified.Namespace.Dog/Color eq 'White'", this.edmModel);
            test.ShouldThrow<ODataException>();
        }

        [Fact]
        public void InvalidPropertyShouldFailUnderCastInFilter()
        {
            Action test = () => HardCodedTestModel.ParseUri("People?$filter=Fully.Qualified.Namespace.Person/WorkEmail eq 'crimson@harvard.edu'", this.edmModel);
            test.ShouldThrow<ODataException>();
        }

        [Fact]
        public void UpCastIsPermittedInFilter()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Person/Shoe eq 'Sketchers'", this.edmModel);
            var cmpPerson = this.edmModel.FindType("Fully.Qualified.Namespace.Person");
            semanticTree.Filter.Expression.As<BinaryOperatorNode>().Left.As<SingleValuePropertyAccessNode>().Source.As<SingleEntityCastNode>().EntityTypeReference.Definition.Should().Be(cmpPerson);
        }

        [Fact]
        public void CastShouldBeAllowedInsideAny()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/MyPeople/any(a: a/Fully.Qualified.Namespace.Employee/Shoe eq 'Calvin Klein' )", this.edmModel);
            var cmpEmployee = this.edmModel.FindType("Fully.Qualified.Namespace.Employee");
            semanticTree.Filter.Expression.As<AnyNode>().Body.As
                <BinaryOperatorNode>().Left.As<SingleValuePropertyAccessNode>().Source.As<SingleEntityCastNode>().EntityTypeReference.Definition.Should().Be(cmpEmployee);
        }

        [Fact]
        public void CastMayLeadAny()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/MyPeople/Fully.Qualified.Namespace.Employee/any(a: a/Shoe eq 'Calvin Klein' )", this.edmModel);
            semanticTree.Filter.Expression.ShouldBeAnyQueryNode().
                And.Body.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                And.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp()).
                And.Source.ShouldBeEntityRangeVariableReferenceNode("a").
                And.TypeReference.Definition.Should().Be(HardCodedTestModel.GetEmployeeType());
        }

        [Fact]
        public void CastsShouldBeNestableInFilter()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=Fully.Qualified.Namespace.Person/Fully.Qualified.Namespace.Person/Fully.Qualified.Namespace.Employee/WorkEmail eq 'foobarstu@ucla.edu'", this.edmModel);
            var cmpEmployee = this.edmModel.FindType("Fully.Qualified.Namespace.Employee");
            var cmpPerson = this.edmModel.FindType("Fully.Qualified.Namespace.Person");
            semanticTree.Filter.Expression.As
                <BinaryOperatorNode>().Left.As<SingleValuePropertyAccessNode>().Source.As<SingleEntityCastNode>().EntityTypeReference.Definition.Should().Be(cmpEmployee);
            semanticTree.Filter.Expression.As
                <BinaryOperatorNode>().Left.As<SingleValuePropertyAccessNode>().Source.As<SingleEntityCastNode>().Source.As<SingleEntityCastNode>().EntityTypeReference.Definition.Should().Be(cmpPerson);
        }

        [Fact(Skip = "This test currently fails.")]
        public void CollectionDowncastInPathShouldBeAllowed()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People/Fully.Qualified.Namespace.Employee", this.edmModel);
            semanticTree.Path.LastSegment.ShouldBeTypeSegment(HardCodedTestModel.GetEmployeeType());
            semanticTree.Path.FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void FilterOnAnyShouldCreateSemanticNodeWithCorrectLambda()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any(a: a/Shoe eq 'Adidas')", this.edmModel);
            var cmpLeft = this.edmModel.FindType("Fully.Qualified.Namespace.Person");
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().
                Left.As<SingleValuePropertyAccessNode>().Property.DeclaringType.Should().Be(cmpLeft);
        }

        [Fact]
        public void DollarItInLambdaShouldReferToPathResult()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any(a: a/Shoe eq $it/Color)", this.edmModel);
            var cmpRight = this.edmModel.FindType("Fully.Qualified.Namespace.Dog");
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().
                Right.As<SingleValuePropertyAccessNode>().Property.DeclaringType.Should().Be(cmpRight);
        }

        [Fact]
        public void AllShouldParseTheSameAsAny()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/all(a: a/Shoe eq 'Adidas' )", this.edmModel);
            var cmpLeft = this.edmModel.FindType("Fully.Qualified.Namespace.Person");
            semanticTree.Filter.Expression.As<AllNode>().Body.As<BinaryOperatorNode>().
                Left.As<SingleValuePropertyAccessNode>().Property.DeclaringType.Should().Be(cmpLeft);
        }

        [Fact]
        public void RangeVariableNameUsedOutsideOfScopeShouldBeOpenPropertyIfTypeIsOpen()
        {
            // Repro for: Syntactic parser assumes any token which matches the name of a previously used range variable is also a range variable, even after the scope has been exited
            var semanticTree = HardCodedTestModel.ParseUri("Paintings?$filter=Colors/all(c: true) and c", this.edmModel);
            semanticTree.Filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And)
                .And.Right.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Boolean)
                    .And.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("c");
        }

        [Fact]
        public void RangeVariableNameUsedOutsideOfScopeShouldFailIfTypeIsNotOpen()
        {
            // Repro for: Syntactic parser assumes any token which matches the name of a previously used range variable is also a range variable, even after the scope has been exited
            Action parse = () => HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/all(a: true) and a ne null", this.edmModel);
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Dog", "a"));
        }

        [Fact]
        public void RangeVariableRedefinedInsideScopeShouldFailWithUsefulError()
        {
            // Repro for: Semantic binding fails with useless error message when a range variable is redefined within a nested any/all
            Action parse = () => HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/all(a: a/MyPaintings/any(a:true))", this.edmModel);
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_RangeVariableAlreadyDeclared("a"));
        }

        [Fact]
        public void WhenAnyUnderNonCollectionShouldFail()
        {
            try
            {
                HardCodedTestModel.ParseUri("People?$filter=MyDog/any(a: a/Color eq 'Brown' )", this.edmModel);
            }
            catch (ODataException)
            {
                return;
            }
            throw new Exception();
        }

        [Fact]
        public void WhenRepeatedVisitorShouldFail()
        {
            try
            {
                HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any(a: a/MyDog/MyPeople/any(a: a/Shoe eq a/Shoe) )", this.edmModel);
            }
            catch (Exception)
            {
                return;
            }
            throw new Exception();
        }

        [Fact]
        public void WhenMissingColonShouldFail()
        {
            try
            {
                HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any(a a/Shoe eq 'Adidas' )", this.edmModel);
            }
            catch (ODataException)
            {
                return;
            }
            throw new Exception();
        }

        [Fact]
        public void WhenMissingParameterShouldFail()
        {
            try
            {
                HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any( : a/Shoe eq 'Adidas' )", this.edmModel);
            }
            catch (ODataException)
            {
                return;
            }
            throw new Exception();
        }

        [Fact]
        public void ParameterWithPropertyNameIsAllowed()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any(Shoe: Shoe/Shoe eq 'Adidas' )", this.edmModel);
            var cmpLeft = this.edmModel.FindType("Fully.Qualified.Namespace.Person");
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().
                Left.As<SingleValuePropertyAccessNode>().Property.DeclaringType.Should().Be(cmpLeft);
        }

        [Fact]
        public void MultipleNavigationPropertiesCreateCorrectSemanticNodes()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/MyPeople/any(a: a/MyDog/Color eq 'Simba' )", this.edmModel);
            var cmpPerson = this.edmModel.FindType("Fully.Qualified.Namespace.Person");
            var cmpDog = this.edmModel.FindType("Fully.Qualified.Namespace.Dog");
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().
                Left.As<SingleValuePropertyAccessNode>().Source.As<SingleNavigationNode>().TypeReference.As
                <IEdmEntityTypeReference>().Definition.Should().Be(cmpDog);
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().
                Left.As<SingleValuePropertyAccessNode>().Source.As<SingleNavigationNode>().Source.As<SingleValueNode>().TypeReference.As
                <IEdmEntityTypeReference>().Definition.Should().Be(cmpPerson);
        }

        [Fact]
        public void AnyQuerysShouldNest()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any(a: a/MyDog/MyPeople/any(b: b/Shoe eq a/Shoe) )", this.edmModel);

            var cmpPerson = this.edmModel.FindType("Fully.Qualified.Namespace.Person");
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<AnyNode>().Body.As
                <BinaryOperatorNode>().Left.As<SingleValuePropertyAccessNode>().Source.As<EntityRangeVariableReferenceNode>().
                TypeReference.Definition.Should().Be(cmpPerson);
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<AnyNode>().Body.As
                <BinaryOperatorNode>().Right.As<SingleValuePropertyAccessNode>().Source.As<EntityRangeVariableReferenceNode>().
                TypeReference.Definition.Should().Be(cmpPerson);
        }

        [Fact]
        public void ParametersShouldBeAllowedToRepeatOutsideOfEachOthersScope()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any(a: a/MyDog/MyPeople/any(b: b/Shoe eq $it/Color) and a/MyDog/MyPeople/any(b: b/Shoe eq a/Shoe))", this.edmModel);

            var cmpPerson = this.edmModel.FindType("Fully.Qualified.Namespace.Person");
            var cmpDog = this.edmModel.FindType("Fully.Qualified.Namespace.Dog");
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().Left.As<AnyNode>().Body.As
                <BinaryOperatorNode>().Left.As<SingleValuePropertyAccessNode>().Source.As<EntityRangeVariableReferenceNode>().
                TypeReference.Definition.Should().Be(cmpPerson);
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().Left.As<AnyNode>().Body.As
                <BinaryOperatorNode>().Right.As<SingleValuePropertyAccessNode>().Source.As<EntityRangeVariableReferenceNode>().
                TypeReference.Definition.Should().Be(cmpDog);
        }

        [Fact]
        public void FilterShouldHandleEmptyArgument()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any()", this.edmModel);
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<ConstantNode>().Value.Should().Be(true);
        }

        [Fact]
        public void OrderbyShouldHandleEmptyArgument()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$orderby=MyPeople/any()", this.edmModel);
            semanticTree.OrderBy.Expression.As<AnyNode>().Body.As<ConstantNode>().Value.Should().Be(true);
        }

        [Fact]
        public void OrderbyWithBasicAny()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$orderby=MyPeople/any(a: a/Shoe eq $it/Color)", this.edmModel);
            var anyNode = semanticTree.OrderBy.Expression.ShouldBeAnyQueryNode().And;
            anyNode.RangeVariables.Count.Should().Be(2);
            anyNode.RangeVariables.Should().Contain(n => n.Name == "a");
            anyNode.Body.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Fact]
        public void CountQueryWithInvalidArgument()
        {
            string[] args = { 
                                "Dogs?$count='true'", 
                                "Dogs?$count=invalidValue", 
                                "Dogs?$count=true/$count", 
                                "Dogs/$count=true"
                            };
            foreach (var arg in args)
            {
                Action test = () => HardCodedTestModel.ParseUri(arg, this.edmModel);
                test.ShouldThrow<ODataException>();
            }
        }

        [Fact]
        public void CountQueryWithDuplicateCount()
        {
            Action test = () => HardCodedTestModel.ParseUri("Dogs?$count=true&$count=true", this.edmModel);
            test.ShouldThrow<ODataException>().WithMessage(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce("$count"));
        }

        [Fact]
        public void NavigationPropertiesShouldCreateCorrectSemanticNode()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/Color eq 'Magenta'", this.edmModel);

            var cmpDog = this.edmModel.FindType("Fully.Qualified.Namespace.Dog");
            semanticTree.Filter.Expression.As<BinaryOperatorNode>().Left.As
                <SingleValuePropertyAccessNode>().Source.As<SingleNavigationNode>().TypeReference.Definition.Should().Be(cmpDog);
        }

        [Fact]
        public void LongFilterWithNot()
        {
            // Query like "People?$filter=not not not not not not not true"
            StringBuilder sb = new StringBuilder();
            int nestingLevel = 750;
            for (int i = 0; i < nestingLevel; i++)
            {
                sb.Append("not ");
            }

            var uri = string.Format("People?$filter={0}true", sb.ToString());

            var semanticTree = HardCodedTestModel.ParseUri(uri, this.edmModel);

            var node = semanticTree.Filter.Expression.As<UnaryOperatorNode>();
            for (int i = 0; i < nestingLevel - 1; i++)
            {
                node = node.Operand.As<UnaryOperatorNode>();
            }

            node.Operand.As<ConstantNode>().Value.Should().Be(true);
        }

        [Fact]
        public void LongFilterWithTrim()
        {
            // Query like "People?$filter=trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(Shoe)))))))))))))))))))) eq 'somevalue'";
            int nestingLevel = 400;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nestingLevel; i++)
            {
                sb.Append("trim(");
            }

            sb.Append("Shoe");

            for (int i = 0; i < nestingLevel; i++)
            {
                sb.Append(")");
            }

            var uri = string.Format("People?$filter={0} eq 'ShoeBrand'", sb.ToString());
            var semanticTree = HardCodedTestModel.ParseUri(uri, this.edmModel, 4000);

            var personType = this.edmModel.FindType("Fully.Qualified.Namespace.Person");

            var node = semanticTree.Filter.Expression.As<BinaryOperatorNode>().Left.As<SingleValueFunctionCallNode>();
            for (int i = 0; i < nestingLevel - 1; i++)
            {
                node = node.Parameters.Single().As<SingleValueFunctionCallNode>();
            }

            node.Parameters.Single()
                .As<SingleValuePropertyAccessNode>()
                .Source.As<EntityRangeVariableReferenceNode>()
                .TypeReference.Definition.Should().Be(personType);

        }

        [Fact]
        public void LongLongPath()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1", this.edmModel);

            var personType = this.edmModel.FindType("Fully.Qualified.Namespace.Person");

            semanticTree
                .Filter.Expression.As<BinaryOperatorNode>()
                .Left.As<BinaryOperatorNode>()
                .Left.As<BinaryOperatorNode>()
                .Left.As<BinaryOperatorNode>()
                .Left.As<BinaryOperatorNode>()
                .Left.As<BinaryOperatorNode>()
                .Left.As<BinaryOperatorNode>()
                .Left.As<BinaryOperatorNode>()
                .Left.As<BinaryOperatorNode>()
                .Left.As<BinaryOperatorNode>()
                .Left.As<BinaryOperatorNode>()
                .Left.As<BinaryOperatorNode>()
                 .Left.As<SingleValuePropertyAccessNode>()
                .Source.As<EntityRangeVariableReferenceNode>()
               .TypeReference.Definition.Should().Be(personType);
        }

        [Fact]
        public void NavigationPropertiesShouldHaveAssociatedTargetMultiplicity()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/Color eq 'Magenta'", this.edmModel);

            semanticTree.Filter.Expression.As<BinaryOperatorNode>().Left.As
                <SingleValuePropertyAccessNode>().Source.As<SingleNavigationNode>().TargetMultiplicity.Should().Be(EdmMultiplicity.ZeroOrOne);
        }

        [Fact]
        public void WhenAnyIsLedBySingletonNavigationPropertyShouldFail()
        {
            try
            {
                HardCodedTestModel.ParseUri("People?$filter=MyDog/MyPeople/MyDog/MyPeople/any(a: a/MyDog/Color eq 'Simba' )", this.edmModel);
            }
            catch (ODataException)
            {
                return;
            }
            throw new ODataException();
        }

        [Fact]
        public void ComplexTypeShouldCreateCorrectSemanticNode()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyAddress/City eq 'Tacoma'", this.edmModel);

            var cmpAddress = this.edmModel.FindType("Fully.Qualified.Namespace.Address");
            semanticTree.Filter.Expression.As<BinaryOperatorNode>().
                Left.As<SingleValuePropertyAccessNode>().Source.As<SingleValuePropertyAccessNode>().TypeReference.Definition.Should().Be(cmpAddress);
        }

        [Fact]
        public void ComplexTypesShouldBeNestable()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyAddress/NextHome/NextHome/City eq 'Tacoma'", this.edmModel);

            var cmpAddress = this.edmModel.FindType("Fully.Qualified.Namespace.Address");
            semanticTree.Filter.Expression.As<BinaryOperatorNode>().
                Left.As<SingleValuePropertyAccessNode>().Source.As<SingleValuePropertyAccessNode>().TypeReference.Definition.Should().Be(cmpAddress);
            semanticTree.Filter.Expression.As<BinaryOperatorNode>().
                Left.As<SingleValuePropertyAccessNode>().Source.As<SingleValuePropertyAccessNode>().Source.As<SingleValuePropertyAccessNode>().TypeReference.Definition.Should().Be(cmpAddress);
        }

        [Fact]
        public void ComplexTypesShouldBeAllowedInAny()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/MyPeople/any(a: a/MyAddress/City eq 'Renton')", this.edmModel);

            var cmpAddress = this.edmModel.FindType("Fully.Qualified.Namespace.Address");
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().
                Left.As<SingleValuePropertyAccessNode>().Source.As<SingleValuePropertyAccessNode>().TypeReference.Definition.Should().Be(cmpAddress);
        }

        [Fact]
        public void PrimitiveCollectionTypeShouldBeAllowedUnderComplexType()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyAddress/MyNeighbors/any(a: a eq 'Christy' )", this.edmModel);

            var cmp = this.edmModel.FindType("Edm.String");
            semanticTree.Filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().
                Left.As<NonentityRangeVariableReferenceNode>().TypeReference.Definition.Should().Be(cmp);
        }

        [Fact]
        public void FilterThroughMissingNavigationOrComplexPropertyShouldThrowOurException()
        {
            Action parse = () => HardCodedTestModel.ParseUri("People?$filter=Missing/ID eq 1", this.edmModel);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "Missing"));
        }

        [Fact]
        public void FilterThroughMissingPropertyShouldThrowOurException()
        {
            Action parse = () => HardCodedTestModel.ParseUri("People?$filter=Missing eq 1", this.edmModel);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "Missing"));
        }
    }
}
