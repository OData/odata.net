//---------------------------------------------------------------------
// <copyright file="SemanticTreeFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
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
            Assert.NotNull(semanticTree.Filter);
            Assert.NotNull(semanticTree.OrderBy);
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

            Assert.Same(HardCodedTestModel.GetPersonType(), semanticTree.Filter.ItemType.Definition);
        }

        [Fact]
        public void CastInFilterShouldCreateCorrectSemanticNode()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/Fully.Qualified.Namespace.Dog/Color eq 'Black'", this.edmModel);

            var cmp = this.edmModel.FindType("Fully.Qualified.Namespace.Dog");
            var bon = Assert.IsType<BinaryOperatorNode>(semanticTree.Filter.Expression);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var castNode = Assert.IsType<SingleResourceCastNode>(propertyAccessNode.Source);
            Assert.Same(castNode.StructuredTypeReference.Definition, cmp);
        }

        [Fact]
        public void CastShouldWorkAsFirstSegmentInFilter()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=Fully.Qualified.Namespace.Employee/WorkEmail eq 'bob@yahoo.com'", this.edmModel);

            var cmp = this.edmModel.FindType("Fully.Qualified.Namespace.Employee");
            var bon = Assert.IsType<BinaryOperatorNode>(semanticTree.Filter.Expression);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var castNode = Assert.IsType<SingleResourceCastNode>(propertyAccessNode.Source);
            Assert.Same(castNode.StructuredTypeReference.Definition, cmp);
        }

        [Fact]
        public void InvalidCastInFilterShouldFail()
        {
            Action test = () => HardCodedTestModel.ParseUri("People?$filter=Fully.Qualified.Namespace.Dog/Color eq 'White'", this.edmModel);
            Assert.Throws<ODataException>(test);
        }

        [Fact]
        public void InvalidPropertyShouldFailUnderCastInFilter()
        {
            Action test = () => HardCodedTestModel.ParseUri("People?$filter=Fully.Qualified.Namespace.Person/WorkEmail eq 'crimson@harvard.edu'", this.edmModel);
            Assert.Throws<ODataException>(test);
        }

        [Fact]
        public void UpCastIsPermittedInFilter()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Person/Shoe eq 'Sketchers'", this.edmModel);
            var cmpPerson = this.edmModel.FindType("Fully.Qualified.Namespace.Person");

            var bon = Assert.IsType<BinaryOperatorNode>(semanticTree.Filter.Expression);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var castNode = Assert.IsType<SingleResourceCastNode>(propertyAccessNode.Source);
            Assert.Same(castNode.StructuredTypeReference.Definition, cmpPerson);
        }

        [Fact]
        public void CastShouldBeAllowedInsideAny()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/MyPeople/any(a: a/Fully.Qualified.Namespace.Employee/Shoe eq 'Calvin Klein' )", this.edmModel);
            var cmpEmployee = this.edmModel.FindType("Fully.Qualified.Namespace.Employee");

            var anyNode = Assert.IsType<AnyNode>(semanticTree.Filter.Expression);
            var bon = Assert.IsType<BinaryOperatorNode>(anyNode.Body);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var castNode = Assert.IsType<SingleResourceCastNode>(propertyAccessNode.Source);
            Assert.Same(castNode.StructuredTypeReference.Definition, cmpEmployee);
        }

        [Fact]
        public void CastMayLeadAny()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/MyPeople/Fully.Qualified.Namespace.Employee/any(a: a/Shoe eq 'Calvin Klein' )", this.edmModel);
            Assert.Same(semanticTree.Filter.Expression.ShouldBeAnyQueryNode().
                Body.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp()).
                Source.ShouldBeResourceRangeVariableReferenceNode("a").
                TypeReference.Definition, HardCodedTestModel.GetEmployeeType());
        }

        [Fact]
        public void CastsShouldBeNestableInFilter()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=Fully.Qualified.Namespace.Person/Fully.Qualified.Namespace.Person/Fully.Qualified.Namespace.Employee/WorkEmail eq 'foobarstu@ucla.edu'", this.edmModel);
            var cmpEmployee = this.edmModel.FindType("Fully.Qualified.Namespace.Employee");
            var cmpPerson = this.edmModel.FindType("Fully.Qualified.Namespace.Person");

            var bon = Assert.IsType<BinaryOperatorNode>(semanticTree.Filter.Expression);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var castNode = Assert.IsType<SingleResourceCastNode>(propertyAccessNode.Source);
            Assert.Same(castNode.StructuredTypeReference.Definition, cmpEmployee);

            var castCastNode = Assert.IsType<SingleResourceCastNode>(castNode.Source);
            Assert.Same(castCastNode.StructuredTypeReference.Definition, cmpPerson);
        }

        [Fact]
        public void CollectionDowncastInPathShouldBeAllowed()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People/Fully.Qualified.Namespace.Employee", this.edmModel);
            semanticTree.Path.LastSegment.ShouldBeTypeSegment(new EdmCollectionType(HardCodedTestModel.GetEmployeeType().GetTypeReference()));
            semanticTree.Path.FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void FilterOnAnyShouldCreateSemanticNodeWithCorrectLambda()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any(a: a/Shoe eq 'Adidas')", this.edmModel);
            var cmpLeft = this.edmModel.FindType("Fully.Qualified.Namespace.Person");

            var anyNode = Assert.IsType<AnyNode>(semanticTree.Filter.Expression);
            var bon = Assert.IsType<BinaryOperatorNode>(anyNode.Body);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            Assert.Same(propertyAccessNode.Property.DeclaringType, cmpLeft);
        }

        [Fact]
        public void DollarItInLambdaShouldReferToPathResult()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any(a: a/Shoe eq $it/Color)", this.edmModel);
            var cmpRight = this.edmModel.FindType("Fully.Qualified.Namespace.Dog");

            var anyNode = Assert.IsType<AnyNode>(semanticTree.Filter.Expression);
            var bon = Assert.IsType<BinaryOperatorNode>(anyNode.Body);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Right);
            Assert.Same(propertyAccessNode.Property.DeclaringType, cmpRight);
        }

        [Fact]
        public void AllShouldParseTheSameAsAny()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/all(a: a/Shoe eq 'Adidas' )", this.edmModel);
            var cmpLeft = this.edmModel.FindType("Fully.Qualified.Namespace.Person");

            var allNode = Assert.IsType<AllNode>(semanticTree.Filter.Expression);
            var bon = Assert.IsType<BinaryOperatorNode>(allNode.Body);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            Assert.Same(propertyAccessNode.Property.DeclaringType, cmpLeft);
        }

        [Fact]
        public void RangeVariableNameUsedOutsideOfScopeShouldBeOpenPropertyIfTypeIsOpen()
        {
            // Repro for: Syntactic parser assumes any token which matches the name of a previously used range variable is also a range variable, even after the scope has been exited
            var semanticTree = HardCodedTestModel.ParseUri("Paintings?$filter=Colors/all(c: true) and c", this.edmModel);
            semanticTree.Filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And)
                .Right.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Boolean)
                    .Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("c");
        }

        [Fact]
        public void RangeVariableNameUsedOutsideOfScopeShouldFailIfTypeIsNotOpen()
        {
            // Repro for: Syntactic parser assumes any token which matches the name of a previously used range variable is also a range variable, even after the scope has been exited
            Action parse = () => HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/all(a: true) and a ne null", this.edmModel);
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Dog", "a"));
        }

        [Fact]
        public void RangeVariableRedefinedInsideScopeShouldFailWithUsefulError()
        {
            // Repro for: Semantic binding fails with useless error message when a range variable is redefined within a nested any/all
            Action parse = () => HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/all(a: a/MyPaintings/any(a:true))", this.edmModel);
            parse.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_RangeVariableAlreadyDeclared("a"));
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

            var anyNode = Assert.IsType<AnyNode>(semanticTree.Filter.Expression);
            var bon = Assert.IsType<BinaryOperatorNode>(anyNode.Body);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            Assert.Same(propertyAccessNode.Property.DeclaringType, cmpLeft);
        }

        [Fact]
        public void MultipleNavigationPropertiesCreateCorrectSemanticNodes()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/MyPeople/any(a: a/MyDog/Color eq 'Simba' )", this.edmModel);
            var cmpPerson = this.edmModel.FindType("Fully.Qualified.Namespace.Person");
            var cmpDog = this.edmModel.FindType("Fully.Qualified.Namespace.Dog");

            var anyNode = Assert.IsType<AnyNode>(semanticTree.Filter.Expression);
            var bon = Assert.IsType<BinaryOperatorNode>(anyNode.Body);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var navNode = Assert.IsType<SingleNavigationNode>(propertyAccessNode.Source);
            var typeReference = navNode.TypeReference as IEdmEntityTypeReference;
            Assert.NotNull(typeReference);
            Assert.Same(typeReference.Definition, cmpDog);

            var valueNode = Assert.IsType<ResourceRangeVariableReferenceNode>(navNode.Source);
            typeReference = valueNode.TypeReference as IEdmEntityTypeReference;
            Assert.NotNull(typeReference);
            Assert.Same(typeReference.Definition, cmpPerson);
        }

        [Fact]
        public void AnyQuerysShouldNest()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any(a: a/MyDog/MyPeople/any(b: b/Shoe eq a/Shoe) )", this.edmModel);

            var cmpPerson = this.edmModel.FindType("Fully.Qualified.Namespace.Person");

            var anyNode = Assert.IsType<AnyNode>(semanticTree.Filter.Expression);
            var anyBody = Assert.IsType<AnyNode>(anyNode.Body);
            var bon = Assert.IsType<BinaryOperatorNode>(anyBody.Body);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var refNode = Assert.IsType<ResourceRangeVariableReferenceNode>(propertyAccessNode.Source);
            Assert.Same(refNode.TypeReference.Definition, cmpPerson);

            propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Right);
            refNode = Assert.IsType<ResourceRangeVariableReferenceNode>(propertyAccessNode.Source);
            Assert.Same(refNode.TypeReference.Definition, cmpPerson);
        }

        [Fact]
        public void ParametersShouldBeAllowedToRepeatOutsideOfEachOthersScope()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any(a: a/MyDog/MyPeople/any(b: b/Shoe eq $it/Color) and a/MyDog/MyPeople/any(b: b/Shoe eq a/Shoe))", this.edmModel);

            var cmpPerson = this.edmModel.FindType("Fully.Qualified.Namespace.Person");
            var cmpDog = this.edmModel.FindType("Fully.Qualified.Namespace.Dog");

            var anyNode = Assert.IsType<AnyNode>(semanticTree.Filter.Expression);
            var bon = Assert.IsType<BinaryOperatorNode>(anyNode.Body);
            var leftAnyNode = Assert.IsType<AnyNode>(bon.Left);
            var leftBody = Assert.IsType<BinaryOperatorNode>(leftAnyNode.Body);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(leftBody.Left);
            var refNode = Assert.IsType<ResourceRangeVariableReferenceNode>(propertyAccessNode.Source);
            Assert.Same(refNode.TypeReference.Definition, cmpPerson);

            propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(leftBody.Right);
            refNode = Assert.IsType<ResourceRangeVariableReferenceNode>(propertyAccessNode.Source);
            Assert.Same(refNode.TypeReference.Definition, cmpDog);
        }

        [Fact]
        public void FilterShouldHandleEmptyArgument()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$filter=MyPeople/any()", this.edmModel);
            var anyNode = Assert.IsType<AnyNode>(semanticTree.Filter.Expression);
            var constNode = Assert.IsType<ConstantNode>(anyNode.Body);
            Assert.Equal(true, constNode.Value);
        }

        [Fact]
        public void OrderbyShouldHandleEmptyArgument()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$orderby=MyPeople/any()", this.edmModel);

            var anyNode = Assert.IsType<AnyNode>(semanticTree.OrderBy.Expression);
            var constNode = Assert.IsType<ConstantNode>(anyNode.Body);
            Assert.Equal(true, constNode.Value);
        }

        [Fact]
        public void OrderbyWithBasicAny()
        {
            var semanticTree = HardCodedTestModel.ParseUri("Dogs?$orderby=MyPeople/any(a: a/Shoe eq $it/Color)", this.edmModel);
            var anyNode = semanticTree.OrderBy.Expression.ShouldBeAnyQueryNode();
            Assert.Equal(2, anyNode.RangeVariables.Count);
            Assert.Contains(anyNode.RangeVariables, n => n.Name == "a");
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
                Assert.Throws<ODataException>(test);
            }
        }

        [Fact]
        public void CountQueryWithDuplicateCount()
        {
            string input = "Dogs?$count=true&$count=true";
            var serviceBaseUri = new Uri("http://server/service/");
            var queryUri = new Uri(serviceBaseUri, input);
            ODataUriParser parser = new ODataUriParser(this.edmModel, serviceBaseUri, queryUri);
            Action test = () => parser.ParseUri();

            bool originalValue = parser.EnableNoDollarQueryOptions;
            try
            {
                //Ensure $-sign is required.
                parser.EnableNoDollarQueryOptions = false;
                test.Throws<ODataException>(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce("$count"));
            }
            finally
            {
                parser.EnableNoDollarQueryOptions = originalValue;
            }
        }

        [Fact]
        public void NavigationPropertiesShouldCreateCorrectSemanticNode()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/Color eq 'Magenta'", this.edmModel);

            var cmpDog = this.edmModel.FindType("Fully.Qualified.Namespace.Dog");

            var bon = Assert.IsType<BinaryOperatorNode>(semanticTree.Filter.Expression);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var navNode = Assert.IsType<SingleNavigationNode>(propertyAccessNode.Source);
            Assert.Same(navNode.TypeReference.Definition, cmpDog);
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

            var node = Assert.IsType<UnaryOperatorNode>(semanticTree.Filter.Expression);
            for (int i = 0; i < nestingLevel - 1; i++)
            {
                node = Assert.IsType<UnaryOperatorNode>(node.Operand);
            }

            var constNode = Assert.IsType<ConstantNode>(node.Operand);
            Assert.Equal(true, constNode.Value);
        }

        [Fact]
        public void LongFilterWithTrim()
        {
            // Query like "People?$filter=trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(trim(Shoe)))))))))))))))))))) eq 'somevalue'";

            // Nesting level = 400 likely will cause stack-overflow in default VS IDE environment.
            int nestingLevel = 300;

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

            var node = (semanticTree.Filter.Expression as BinaryOperatorNode).Left as SingleValueFunctionCallNode;
            for (int i = 0; i < nestingLevel - 1; i++)
            {
                node = node.Parameters.Single() as SingleValueFunctionCallNode;
            }

            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(node.Parameters.Single());
            var refNode = Assert.IsType<ResourceRangeVariableReferenceNode>(propertyAccessNode.Source);
            Assert.Same(refNode.TypeReference.Definition, personType);
        }

        [Fact]
        public void LongLongPath()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1 or ID eq 1", this.edmModel);

            var personType = this.edmModel.FindType("Fully.Qualified.Namespace.Person");

            var bon = Assert.IsType<BinaryOperatorNode>(semanticTree.Filter.Expression);
            var left1 = Assert.IsType<BinaryOperatorNode>(bon.Left);
            var left2 = Assert.IsType<BinaryOperatorNode>(left1.Left);
            var left3 = Assert.IsType<BinaryOperatorNode>(left2.Left);
            var left4 = Assert.IsType<BinaryOperatorNode>(left3.Left);
            var left5 = Assert.IsType<BinaryOperatorNode>(left4.Left);
            var left6 = Assert.IsType<BinaryOperatorNode>(left5.Left);
            var left7 = Assert.IsType<BinaryOperatorNode>(left6.Left);
            var left8 = Assert.IsType<BinaryOperatorNode>(left7.Left);
            var left9 = Assert.IsType<BinaryOperatorNode>(left8.Left);
            var left10 = Assert.IsType<BinaryOperatorNode>(left9.Left);
            var left11 = Assert.IsType<BinaryOperatorNode>(left10.Left);

            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(left11.Left);
            var refNode = Assert.IsType<ResourceRangeVariableReferenceNode>(propertyAccessNode.Source);
            Assert.Same(refNode.TypeReference.Definition, personType);
        }

        [Fact]
        public void NavigationPropertiesShouldHaveAssociatedTargetMultiplicity()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/Color eq 'Magenta'", this.edmModel);

            var bon = Assert.IsType<BinaryOperatorNode>(semanticTree.Filter.Expression);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var navigationNode = Assert.IsType<SingleNavigationNode>(propertyAccessNode.Source);
            Assert.Equal(EdmMultiplicity.ZeroOrOne, navigationNode.TargetMultiplicity);
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

            var bon = Assert.IsType<BinaryOperatorNode>(semanticTree.Filter.Expression);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var complexNode = Assert.IsType<SingleComplexNode>(propertyAccessNode.Source);
            Assert.Same(complexNode.TypeReference.Definition, cmpAddress);
        }

        [Fact]
        public void ComplexTypesShouldBeNestable()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyAddress/NextHome/NextHome/City eq 'Tacoma'", this.edmModel);

            var cmpAddress = this.edmModel.FindType("Fully.Qualified.Namespace.Address");

            var bon = Assert.IsType<BinaryOperatorNode>(semanticTree.Filter.Expression);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var complexNode = Assert.IsType<SingleComplexNode>(propertyAccessNode.Source);
            Assert.Same(complexNode.TypeReference.Definition, cmpAddress);

            var complexSource = Assert.IsType<SingleComplexNode>(complexNode.Source);
            Assert.Same(complexSource.TypeReference.Definition, cmpAddress);
        }

        [Fact]
        public void ComplexTypesShouldBeAllowedInAny()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyDog/MyPeople/any(a: a/MyAddress/City eq 'Renton')", this.edmModel);

            var cmpAddress = this.edmModel.FindType("Fully.Qualified.Namespace.Address");

            var anyNode = Assert.IsType<AnyNode>(semanticTree.Filter.Expression);
            var bon = Assert.IsType<BinaryOperatorNode>(anyNode.Body);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(bon.Left);
            var complexNode = Assert.IsType<SingleComplexNode>(propertyAccessNode.Source);
            Assert.Same(complexNode.TypeReference.Definition, cmpAddress);
        }

        [Fact]
        public void PrimitiveCollectionTypeShouldBeAllowedUnderComplexType()
        {
            var semanticTree = HardCodedTestModel.ParseUri("People?$filter=MyAddress/MyNeighbors/any(a: a eq 'Christy' )", this.edmModel);

            var cmp = this.edmModel.FindType("Edm.String");

            var anyNode = Assert.IsType<AnyNode>(semanticTree.Filter.Expression);
            var bon = Assert.IsType<BinaryOperatorNode>(anyNode.Body);
            var refNode = Assert.IsType<NonResourceRangeVariableReferenceNode>(bon.Left);
            Assert.Same(refNode.TypeReference.Definition, cmp);
        }

        [Fact]
        public void FilterThroughMissingNavigationOrComplexPropertyShouldThrowOurException()
        {
            Action parse = () => HardCodedTestModel.ParseUri("People?$filter=Missing/ID eq 1", this.edmModel);
            parse.Throws<ODataException>(Strings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "Missing"));
        }

        [Fact]
        public void FilterThroughMissingPropertyShouldThrowOurException()
        {
            Action parse = () => HardCodedTestModel.ParseUri("People?$filter=Missing eq 1", this.edmModel);
            parse.Throws<ODataException>(Strings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "Missing"));
        }
    }
}
