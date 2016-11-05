//---------------------------------------------------------------------
// <copyright file="FunctionCallBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    /// <summary>
    /// Unit tests for the FunctionCallBinder class.
    /// TODO: add tests for particular cases.
    /// </summary>
    public class FunctionCallBinderTests
    {
        private const string OpenPropertyName = "SomeProperty";

        private readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);
        private FunctionCallBinder functionCallBinder;

        private MetadataBinder binder;

        public FunctionCallBinderTests()
        {
            this.ResetState();
        }

        // Bind tests
        [Fact]
        public void BindFunction()
        {
            var arguments = new List<QueryToken>() { new LiteralToken("grr"), new LiteralToken(1) };
            var token = new FunctionCallToken("substring", arguments);
            var result = functionCallBinder.BindFunctionCall(token);
            result.ShouldBeSingleValueFunctionCallQueryNode("substring");
            result.As<SingleValueFunctionCallNode>().Parameters.Count().Should().Be(2);
            result.As<SingleValueFunctionCallNode>().TypeReference.Definition.ToString().Should().Contain("String");
        }

        [Fact]
        public void BindFunctionForNullArgumentType()
        {
            this.functionCallBinder = new FunctionCallBinder(FakeBindMethods.BindMethodReturningANullLiteralConstantNode, this.binder.BindingState);
            var arguments = new List<QueryToken>() { new LiteralToken("ignored") };
            var token = new FunctionCallToken("year", arguments);
            var result = functionCallBinder.BindFunctionCall(token);
            result.ShouldBeSingleValueFunctionCallQueryNode("year").And.Parameters.Single().ShouldBeConstantQueryNode<object>(null).And.TypeReference.Should().BeNull();
        }

        [Fact]
        public void BindFunctionAllNullArgumentTypes()
        {
            this.functionCallBinder = new FunctionCallBinder(FakeBindMethods.BindMethodReturningANullLiteralConstantNode, this.binder.BindingState);
            var arguments = new List<QueryToken>() { new LiteralToken("ignored"), new LiteralToken("ignored") };
            var token = new FunctionCallToken("substring", arguments);
            var result = functionCallBinder.BindFunctionCall(token);
            result.ShouldBeSingleValueFunctionCallQueryNode("substring");
            result.As<SingleValueFunctionCallNode>().Parameters.First().ShouldBeConstantQueryNode<object>(null).And.TypeReference.Should().BeNull();
            result.As<SingleValueFunctionCallNode>().Parameters.Last().ShouldBeConstantQueryNode<object>(null).And.TypeReference.Should().BeNull();
        }

        [Fact]
        public void BindFunctionEmptyArguments()
        {
            // regression test for [UriParser] day() allowed. What does that mean?
            this.functionCallBinder = new FunctionCallBinder(FakeBindMethods.BindMethodReturnsNull, this.binder.BindingState);
            var token = new FunctionCallToken("day", new List<QueryToken>());
            Action bindWithEmptyArguments = () => functionCallBinder.BindFunctionCall(token);

            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.GetUriFunctionSignatures("day"); // to match the error message... blah
            bindWithEmptyArguments.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    "day",
                    UriFunctionsHelper.BuildFunctionSignatureListDescription("day", signatures)));
        }

        [Fact]
        public void CannotBindArbitraryNumberOfOpenParametersWithCorrectNonOpenParameters()
        {
            this.functionCallBinder = new FunctionCallBinder(binder.Bind, this.binder.BindingState);
            var arguments = new List<QueryToken>() { new LiteralToken("datetime'1996-02-04'"), new LiteralToken("ignored") };
            var token = new FunctionCallToken("day", arguments);
            Action bindWithEmptyArguments = () => functionCallBinder.BindFunctionCall(token);

            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.GetUriFunctionSignatures("day"); // to match the error message... blah
            bindWithEmptyArguments.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    "day",
                    UriFunctionsHelper.BuildFunctionSignatureListDescription("day", signatures)));
        }

        [Fact]
        public void BindFunctionNullArgumentTypeArgumentCountMatchesFunctionSignature()
        {
            // regression test for: FunctionCallBinder should validate that the number of parameters matches for canonical function calls on open properties
            this.functionCallBinder = new FunctionCallBinder(FakeBindMethods.BindMethodReturningANullLiteralConstantNode, this.binder.BindingState);
            var arguments = new List<QueryToken>() { new LiteralToken("ignored"), new LiteralToken("ignored") };
            var token = new FunctionCallToken("day", arguments);
            Action bindWithTooManyNonTypedArgs = () => functionCallBinder.BindFunctionCall(token);
            bindWithTooManyNonTypedArgs.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionCallBinder_CannotFindASuitableOverload("day", "2"));
        }

        //TypePromoteArguments tests
        [Fact]
        public void TypePromoteArguments()
        {
            var signature = this.GetSingleSubstringFunctionSignatureForTest();
            List<QueryNode> nodes = new List<QueryNode>()
                                        {
                                            new ConstantNode("Hello"),
                                            new ConstantNode(3)
                                        };
            FunctionCallBinder.TypePromoteArguments(signature, nodes);
            AssertSignatureTypesMatchArguments(signature, nodes);
        }

        [Fact]
        public void TypePromoteArgumentsWithNullLiteralExpectConvert()
        {
            var signature = this.GetSingleSubstringFunctionSignatureForTest();
            List<QueryNode> nodes = new List<QueryNode>()
                                        {
                                            new ConstantNode(null),
                                            new ConstantNode(3)
                                        };
            FunctionCallBinder.TypePromoteArguments(signature, nodes);
            nodes[0].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String).And.Source.ShouldBeConstantQueryNode<object>(null);
            nodes[1].ShouldBeConstantQueryNode(3);

            AssertSignatureTypesMatchArguments(signature, nodes);
        }

        [Fact]
        public void TypePromoteAllArgumentsAreNullLiteralsExpectConvert()
        {
            var signature = this.GetSingleSubstringFunctionSignatureForTest();
            List<QueryNode> nodes = new List<QueryNode>()
                                        {
                                            new ConstantNode(null),
                                            new ConstantNode(null)
                                        };
            FunctionCallBinder.TypePromoteArguments(signature, nodes);
            nodes[0].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String).And.Source.ShouldBeConstantQueryNode<object>(null);
            nodes[1].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Int32).And.Source.ShouldBeConstantQueryNode<object>(null);

            AssertSignatureTypesMatchArguments(signature, nodes);
        }

        [Fact]
        public void TypePromoteArgumentsWithOpenPropertyExpectConvert()
        {
            var signature = this.GetSingleSubstringFunctionSignatureForTest();
            List<QueryNode> nodes = new List<QueryNode>()
                                        {
                                            new SingleValueOpenPropertyAccessNode(new ConstantNode(null), OpenPropertyName),
                                            new ConstantNode(3)
                                        };
            FunctionCallBinder.TypePromoteArguments(signature, nodes);
            nodes[0].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String)
                    .And.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
            nodes[1].ShouldBeConstantQueryNode(3);

            AssertSignatureTypesMatchArguments(signature, nodes);
        }

        [Fact]
        public void TypePromoteAllArgumentsAreOpenPropertiesExpectConvert()
        {
            var signature = this.GetSingleSubstringFunctionSignatureForTest();
            List<QueryNode> nodes = new List<QueryNode>()
                                        {
                                            new SingleValueOpenPropertyAccessNode(new ConstantNode(null), OpenPropertyName),
                                            new SingleValueOpenPropertyAccessNode(new ConstantNode(null), OpenPropertyName + "1")
                                        };
            FunctionCallBinder.TypePromoteArguments(signature, nodes);
            nodes[0].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String)
                    .And.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
            nodes[1].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Int32)
                    .And.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName + "1");

            AssertSignatureTypesMatchArguments(signature, nodes);
        }

        [Fact]
        public void TypePromoteArgumentsAreNullAndOpenPropertiesExpectConvert()
        {
            var signature = this.GetSingleSubstringFunctionSignatureForTest();
            List<QueryNode> nodes = new List<QueryNode>()
                                        {
                                            new SingleValueOpenPropertyAccessNode(new ConstantNode(null), OpenPropertyName),
                                            new ConstantNode(null)
                                        };
            FunctionCallBinder.TypePromoteArguments(signature, nodes);
            nodes[0].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String)
                    .And.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
            nodes[1].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Int32)
                    .And.Source.ShouldBeConstantQueryNode<object>(null);

            AssertSignatureTypesMatchArguments(signature, nodes);
        }

        [Fact]
        public void TypePromoteArgumentsMismatchedTypes()
        {
            var signature = this.GetSingleSubstringFunctionSignatureForTest();
            List<QueryNode> nodes = new List<QueryNode>()
                                        {
                                            new ConstantNode(3),
                                            new ConstantNode("Hello"),
                                        };
            Action a = () => FunctionCallBinder.TypePromoteArguments(signature, nodes);
            a.ShouldThrow<ODataException>().WithMessage(Microsoft.OData.Core.Strings.MetadataBinder_CannotConvertToType("Edm.Int32", "Edm.String"));
        }

        [Fact]
        public void TypePromoteArgumentsMismatchedTypeAndNull()
        {
            var signature = this.GetSingleSubstringFunctionSignatureForTest();
            List<QueryNode> nodes = new List<QueryNode>()
                                        {
                                            new ConstantNode(null),
                                            new ConstantNode("Hello")
                                        };
            Action a = () => FunctionCallBinder.TypePromoteArguments(signature, nodes);
            a.ShouldThrow<ODataException>().WithMessage(Microsoft.OData.Core.Strings.MetadataBinder_CannotConvertToType("Edm.String", "Edm.Int32"));
        }


        [Fact]
        public void TypePromoteArgumentsMismatchedTypeAndOpenProperty()
        {
            var signature = this.GetSingleSubstringFunctionSignatureForTest();
            List<QueryNode> nodes = new List<QueryNode>()
                                        {
                                            new SingleValueOpenPropertyAccessNode(new ConstantNode(null), "SomeProperty"),
                                            new ConstantNode("Hello")
                                        };
            Action a = () => FunctionCallBinder.TypePromoteArguments(signature, nodes);
            a.ShouldThrow<ODataException>().WithMessage(Microsoft.OData.Core.Strings.MetadataBinder_CannotConvertToType("Edm.String", "Edm.Int32"));
        }

        //EnsureArgumentsAreSingleValue Tests
        [Fact]
        public void EnsureArgumentsAreSingleValue()
        {
            List<QueryNode> argumentNodes =
               new List<QueryNode>()
                {
                    new ConstantNode(new DateTimeOffset(2012, 11, 19, 1, 1, 1, 1, new TimeSpan(0, 1, 1, 0)))
                };
            var result = FunctionCallBinder.ValidateArgumentsAreSingleValue("year", argumentNodes);
            result.Length.Should().Be(1);
            result[0].TypeReference.Definition.ToString().Should().Contain("DateTimeOffset");
        }

        [Fact]
        public void ShouldThrowWhenArgumentsAreNotSingleValue()
        {
            List<QueryNode> argumentNodes =
                new List<QueryNode>()
                {
                    new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), HardCodedTestModel.GetDogsSet())
                };

            Action bind = () => FunctionCallBinder.ValidateArgumentsAreSingleValue("year", argumentNodes);
            bind.ShouldThrow<ODataException>().WithMessage(
                Strings.MetadataBinder_FunctionArgumentNotSingleValue("year"));
        }

        [Fact]
        public void EnsureArgumentsAreSingleValueNoArguments()
        {
            var result = FunctionCallBinder.ValidateArgumentsAreSingleValue("year", new List<QueryNode>());
            result.Length.Should().Be(0);
        }

        //MatchArgumentsToSignature tests
        [Fact]
        public void MatchArgumentsToSignatureDuplicateSignature()
        {
            List<QueryNode> argumentNodes =
                new List<QueryNode>()
                {
                    new ConstantNode("grr" ),
                    new ConstantNode("grr" )
                };
            var signatures = this.GetDuplicateIndexOfFunctionSignatureForTest();

            Action bind = () => FunctionCallBinder.MatchSignatureToUriFunction(
                "IndexOf",
                new SingleValueNode[] { 
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", argumentNodes[0].GetEdmTypeReference())),
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", argumentNodes[1].GetEdmTypeReference()))},
                signatures);

            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_NoApplicableFunctionFound(
                    "IndexOf",
                    UriFunctionsHelper.BuildFunctionSignatureListDescription("IndexOf", signatures)));
        }

        [Fact]
        public void MatchArgumentsToSignature()
        {
            List<QueryNode> argumentNodes =
                new List<QueryNode>()
                {
                    new ConstantNode(new DateTimeOffset(2012, 11, 19, 1, 1, 1, 1, new TimeSpan(0, 1, 1, 0)))
                };
            var signatures = this.GetHardCodedYearFunctionSignatureForTest();

            var result = FunctionCallBinder.MatchSignatureToUriFunction(
                "year",
                argumentNodes.Select(s => (SingleValueNode)s).ToArray(),
                signatures);

            result.ReturnType.ToString().Should().Contain("Edm.Int32");
            result.ArgumentTypes[0].ToString().Should().Contain("Edm.DateTimeOffset");
        }

        [Fact]
        public void MatchArgumentsToSignatureNoMatchEmpty()
        {
            List<QueryNode> argumentNodes =
                new List<QueryNode>()
                {
                    new ConstantNode(4)
                };

            Action bind = () => FunctionCallBinder.MatchSignatureToUriFunction(
                "year",
                new SingleValueNode[] { 
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", argumentNodes[0].GetEdmTypeReference()))},
                new FunctionSignatureWithReturnType[0]);

            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_NoApplicableFunctionFound(
                    "year",
                    UriFunctionsHelper.BuildFunctionSignatureListDescription("year", new FunctionSignatureWithReturnType[0])));
        }

        [Fact]
        public void MatchArgumentsToSignatureNoMatchContainsSignatures()
        {
            List<QueryNode> argumentNodes =
                new List<QueryNode>()
                {
                    new ConstantNode(4)
                };

            Action bind = () => FunctionCallBinder.MatchSignatureToUriFunction(
                "year",
                new SingleValueNode[] { 
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", argumentNodes[0].GetEdmTypeReference()))},
                this.GetHardCodedYearFunctionSignatureForTest());

            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_NoApplicableFunctionFound(
                    "year",
                    UriFunctionsHelper.BuildFunctionSignatureListDescription("year", this.GetHardCodedYearFunctionSignatureForTest())));
        }

        [Fact]
        public void MatchArgumentsToSignatureWillPickRightSignatureForSomeNullArgumentTypes()
        {
            var result = FunctionCallBinder.MatchSignatureToUriFunction(
                "substring",
                 new SingleValueNode[] { 
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", EdmCoreModel.Instance.GetString(true))), 
                     new SingleValueOpenPropertyAccessNode(new ConstantNode(null)/*parent*/, "myOpenPropertyname")}, // open property's TypeReference is null
                 FunctionCallBinder.GetUriFunctionSignatures("substring"));

            result.ReturnType.ShouldBeEquivalentTo(EdmCoreModel.Instance.GetString(true));
            result.ArgumentTypes.Count().Should().Be(2);
            result.ArgumentTypes.First().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetString(true));
            result.ArgumentTypes.Last().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetInt32(true));
        }

        [Fact]
        public void CastFunctionBindWorksWithTwoArgs()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("stuff"),
                new LiteralToken("Edm.String"),   
            };
            FunctionCallToken functionToken = new FunctionCallToken("cast", args);
            QueryNode node = this.functionCallBinder.BindFunctionCall(functionToken);
            node.ShouldBeSingleValueFunctionCallQueryNode("cast");
            node.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).ShouldBeConstantQueryNode("stuff");
            node.As<SingleValueFunctionCallNode>().Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
        }

        [Fact]
        public void CastFunctionBindWorksWithOneArg()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("Edm.String")
            };
            FunctionCallToken functionToken = new FunctionCallToken("cast", args);
            QueryNode node = this.functionCallBinder.BindFunctionCall(functionToken);
            node.ShouldBeSingleValueFunctionCallQueryNode("cast");
            node.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).ShouldBeEntityRangeVariableReferenceNode(ExpressionConstants.It);
            node.As<SingleValueFunctionCallNode>().Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
        }

        [Fact]
        public void CastMustHaveOneOrTwoArguments()
        {
            QueryToken[] moreThanTwoArgs = new QueryToken[]
            {
                new LiteralToken("stuff"),
                new LiteralToken("more stuff"),
                new LiteralToken("Edm.String")
            };

            FunctionCallToken functionWithMoreThanTwoArgs = new FunctionCallToken("cast", moreThanTwoArgs);

            Action createWithMoreThanTwoArgs = () => this.functionCallBinder.BindFunctionCall(functionWithMoreThanTwoArgs);

            createWithMoreThanTwoArgs.ShouldThrow<ODataException>(ODataErrorStrings.MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands(3));
        }

        [Fact]
        public void CastMustHaveATypeArgument()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("stuff"), 
            };
            FunctionCallToken function = new FunctionCallToken("cast", args);
            Action createWithoutATypeArg = () => this.functionCallBinder.BindFunctionCall(function);
            createWithoutATypeArg.ShouldThrow<ODataException>(ODataErrorStrings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
        }

        [Fact]
        public void CastShouldFailIfTypeArgumentIsACollection()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("Collection(Fully.Qualified.Namespace.Person)"),  
            };

            Action bind = () => this.functionCallBinder.BindFunctionCall(new FunctionCallToken("cast", args));
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
        }

        [Fact]
        public void CastShouldFailIfSourceArgumentIsACollection()
        {
            QueryToken[] args = new QueryToken[]
            {
                new EndPathToken("GeographyCollection", null), 
                new LiteralToken("Edm.Geography"),  
            };

            Action bind = () => this.functionCallBinder.BindFunctionCall(new FunctionCallToken("cast", args));
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
        }

        [Fact]
        public void IsOfShouldFailIfTypeArgumentIsACollection()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("Collection(Fully.Qualified.Namespace.Person)"),  
            };

            Action bind = () => this.functionCallBinder.BindFunctionCall(new FunctionCallToken("isof", args));
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
        }

        [Fact]
        public void IsOfShouldFailIfSourceArgumentIsACollection()
        {
            QueryToken[] args = new QueryToken[]
            {
                new EndPathToken("GeographyCollection", null), 
                new LiteralToken("Edm.Geography"),  
            };

            Action bind = () => this.functionCallBinder.BindFunctionCall(new FunctionCallToken("isof", args));
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
        }

        [Fact]
        public void TypeMustBeLastArgumentToCast()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("Edm.String"), 
                new LiteralToken("stuff")
            };
            FunctionCallToken function = new FunctionCallToken("cast", args);
            Action createWithOutOfOrderArgs = () => this.functionCallBinder.BindFunctionCall(function);
            createWithOutOfOrderArgs.ShouldThrow<ODataException>(ODataErrorStrings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
        }

        [Fact]
        public void CastReturnsAnEntityNodeWhenTheReturnTypeIsAnEntity()
        {
            QueryToken[] args = new QueryToken[]
            {
                new EndPathToken("MyDog", null),
                new LiteralToken("Fully.Qualified.Namespace.Dog")
            };
            FunctionCallToken function = new FunctionCallToken("cast", args);
            SingleEntityFunctionCallNode functionCallNode = this.functionCallBinder.BindFunctionCall(function) as SingleEntityFunctionCallNode;
            functionCallNode.Should().NotBeNull();
            functionCallNode.Parameters.Should().HaveCount(2);
            functionCallNode.Parameters.ElementAt(0).ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp());
            functionCallNode.Parameters.ElementAt(1).ShouldBeConstantQueryNode("Fully.Qualified.Namespace.Dog");
        }

        [Fact]
        public void IsOfFunctionBindReturnsWorksWithTwoArguments()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("stuff"),
                new LiteralToken("Edm.String"),  
            };
            FunctionCallToken functionToken = new FunctionCallToken("isof", args);
            QueryNode node = this.functionCallBinder.BindFunctionCall(functionToken);
            node.ShouldBeSingleValueFunctionCallQueryNode("isof");
            node.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).ShouldBeConstantQueryNode("stuff");
            node.As<SingleValueFunctionCallNode>().Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
        }

        [Fact]
        public void IsOfFunctionBindReturnsWorksWithOneArgument()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("Edm.String"),  
            };
            FunctionCallToken functionToken = new FunctionCallToken("isof", args);
            QueryNode node = this.functionCallBinder.BindFunctionCall(functionToken);
            node.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).ShouldBeEntityRangeVariableReferenceNode(ExpressionConstants.It);
            node.ShouldBeSingleValueFunctionCallQueryNode("isof");
            node.As<SingleValueFunctionCallNode>().Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
        }

        [Fact]
        public void IsOfFunctionMustHaveExactlyTwoArguments()
        {
            QueryToken[] moreThanTwoArgs = new QueryToken[]
            {
                new LiteralToken("stuff"),
                new LiteralToken("more stuff"),
                new LiteralToken("Edm.String"),  
            };

            FunctionCallToken functionWithMoreThanTwoArgs = new FunctionCallToken("isof", moreThanTwoArgs);

            Action createWithMoreThanTwoArgs = () => this.functionCallBinder.BindFunctionCall(functionWithMoreThanTwoArgs);

            createWithMoreThanTwoArgs.ShouldThrow<ODataException>(ODataErrorStrings.MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands(3));
        }

        [Fact]
        public void IsOfMustHaveATypeArgument()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("stuff"), 
            };
            FunctionCallToken function = new FunctionCallToken("cast", args);
            Action createWithoutATypeArg = () => this.functionCallBinder.BindFunctionCall(function);
            createWithoutATypeArg.ShouldThrow<ODataException>(ODataErrorStrings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
        }

        [Fact]
        public void TypeMustBeLastArgumentToIsOf()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("Edm.String"), 
                new LiteralToken("stuff")
            };
            FunctionCallToken function = new FunctionCallToken("IsOf", args);
            Action createWithOutOfOrderArgs = () => this.functionCallBinder.BindFunctionCall(function);
            createWithOutOfOrderArgs.ShouldThrow<ODataException>(ODataErrorStrings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
        }

        [Fact]
        public void LengthWorksWithExactlyOneArgument()
        {
            QueryToken parent = new RangeVariableToken(ExpressionConstants.It);
            QueryToken[] args = new QueryToken[]
            {
                new EndPathToken("GeometryLineString", parent),
                new LiteralToken("bob"), 
            };
            FunctionCallToken function = new FunctionCallToken("geo.length", args);
            Action createWithMoreThanOneArg = () => this.functionCallBinder.BindFunctionCall(function);

            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.GetUriFunctionSignatures(function.Name);
            createWithMoreThanOneArg.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    function.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(function.Name, signatures)));
        }

        [Fact]
        public void LengthArgMustBeSingleValueNode()
        {
            QueryToken[] args = new QueryToken[]
            {
                new CustomQueryOptionToken("stuff", "stuff") 
            };

            FunctionCallToken function = new FunctionCallToken("geo.length", args);
            Action createWithNonSingleValueNode = () => this.functionCallBinder.BindFunctionCall(function);
            createWithNonSingleValueNode.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_UnsupportedQueryTokenKind("CustomQueryOption"));
        }

        [Fact]
        public void LengthArgMustBeLineStringType()
        {
            QueryToken parent = new RangeVariableToken(ExpressionConstants.It);
            QueryToken[] args = new QueryToken[]
            {
                new EndPathToken("GeometryLinePolygon", parent),
                new LiteralToken("bob"), 
            };
            FunctionCallToken function = new FunctionCallToken("geo.length", args);
            Action createWithNonLineStringType = () => this.functionCallBinder.BindFunctionCall(function);

            createWithNonLineStringType.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "GeometryLinePolygon"));
        }

        [Fact]
        public void LengthProducesSingleValueFunctionCallNode()
        {
            QueryToken parent = new RangeVariableToken(ExpressionConstants.It);
            QueryToken[] args = new QueryToken[]
            {
                new EndPathToken("GeometryLineString", parent)
            };
            FunctionCallToken function = new FunctionCallToken("geo.length", args);
            SingleValueFunctionCallNode node = this.functionCallBinder.BindFunctionCall(function) as SingleValueFunctionCallNode;
            node.Parameters.Count().Should().Be(1);
            node.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryLineStringProp());
        }

        [Fact]
        public void IntersectsMustBeCalledWithTwoArgs()
        {
            QueryToken parent = new RangeVariableToken(ExpressionConstants.It);
            QueryToken[] oneArg = new QueryToken[]
            {
                new EndPathToken("GeometryLineString", parent), 
            };

            QueryToken[] moreThanTwoArgs = new QueryToken[]
            {
                new EndPathToken("GeometryLineString", parent),
                new EndPathToken("GeographyLineString", parent),
                new EndPathToken("GeometryPolygon", parent)
            };

            FunctionCallToken functionWithOneArg = new FunctionCallToken("geo.intersects", oneArg);
            FunctionCallToken functionWithMoreThanTwoArgs = new FunctionCallToken("geo.intersects", moreThanTwoArgs);

            Action createWithOneArg = () => this.functionCallBinder.BindFunctionCall(functionWithOneArg);
            Action createWithMoreThanTwoArgs = () => this.functionCallBinder.BindFunctionCall(functionWithMoreThanTwoArgs);

            FunctionSignatureWithReturnType[] signatures;
            BuiltInUriFunctions.TryGetBuiltInFunction(functionWithOneArg.Name, out signatures);

            createWithOneArg.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    functionWithOneArg.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(functionWithOneArg.Name, signatures)));

            BuiltInUriFunctions.TryGetBuiltInFunction(functionWithMoreThanTwoArgs.Name, out signatures);

            createWithMoreThanTwoArgs.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    functionWithMoreThanTwoArgs.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(functionWithMoreThanTwoArgs.Name, signatures)));
        }

        [Fact]
        public void IntersectArgsMustBeOrderedCorrectly()
        {
            //Args can be in any order, but there must be a Point and a Polygon (either geography or geometry)
            QueryToken parent = new RangeVariableToken(ExpressionConstants.It);

            //positive tests
            QueryToken[] geometryPointGeometryPoly = new QueryToken[]
            {
                new EndPathToken("GeometryPoint", parent),
                new EndPathToken("GeometryPolygon", parent) 
            };

            QueryToken[] geometryPolyGeometryPoint = new QueryToken[]
            {
                new EndPathToken("GeometryPolygon", parent),
                new EndPathToken("GeometryPoint", parent)
            };

            QueryToken[] geographyPointGeographyPoly = new QueryToken[]
            {
                new EndPathToken("GeographyPoint", parent),
                new EndPathToken("GeographyPolygon", parent) 
            };

            QueryToken[] geographyPolyGeographyPoint = new QueryToken[]
            {
                new EndPathToken("GeographyPolygon", parent),
                new EndPathToken("GeographyPoint", parent)
            };

            FunctionCallToken geometryPointGeometryPolyToken = new FunctionCallToken("geo.intersects", geometryPointGeometryPoly);
            FunctionCallToken geometryPolyGeometryPointToken = new FunctionCallToken("geo.intersects", geometryPolyGeometryPoint);
            FunctionCallToken geographyPointGeographyPolyToken = new FunctionCallToken("geo.intersects", geographyPointGeographyPoly);
            FunctionCallToken geographyPolyGeographyPointToken = new FunctionCallToken("geo.intersects", geographyPolyGeographyPoint);

            SingleValueFunctionCallNode geometryPointGeometryPolyFunction = this.functionCallBinder.BindFunctionCall(geometryPointGeometryPolyToken) as SingleValueFunctionCallNode;
            SingleValueFunctionCallNode geometryPolyGeometryPointFunction = this.functionCallBinder.BindFunctionCall(geometryPolyGeometryPointToken) as SingleValueFunctionCallNode;
            SingleValueFunctionCallNode geographyPointGeographyPolyFunction = this.functionCallBinder.BindFunctionCall(geographyPointGeographyPolyToken) as SingleValueFunctionCallNode;
            SingleValueFunctionCallNode geographyPolyGeographyPointFunction = this.functionCallBinder.BindFunctionCall(geographyPolyGeographyPointToken) as SingleValueFunctionCallNode;

            geometryPointGeometryPolyFunction.Parameters.Count().Should().Be(2);
            geometryPointGeometryPolyFunction.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPointProp());
            geometryPointGeometryPolyFunction.Parameters.ElementAt(1).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPolygonProp());

            geometryPolyGeometryPointFunction.Parameters.Count().Should().Be(2);
            geometryPolyGeometryPointFunction.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPolygonProp());
            geometryPolyGeometryPointFunction.Parameters.ElementAt(1).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPointProp());

            geographyPointGeographyPolyFunction.Parameters.Count().Should().Be(2);
            geographyPointGeographyPolyFunction.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyPointProp());
            geographyPointGeographyPolyFunction.Parameters.ElementAt(1).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyPolygonProp());

            geographyPolyGeographyPointFunction.Parameters.Count().Should().Be(2);
            geographyPolyGeographyPointFunction.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyPolygonProp());
            geographyPolyGeographyPointFunction.Parameters.ElementAt(1).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyPointProp());

            //negative tests

            QueryToken[] geometryPointNonGeometryPoly = new QueryToken[]
            {
                new EndPathToken("GeometryPoint", parent),
                new LiteralToken("bob") 
            };

            QueryToken[] geometryPolyNonGeometryPoint = new QueryToken[]
            {
                new EndPathToken("GeometryPolygon", parent),
                new LiteralToken("bob") 
            };

            QueryToken[] geographyPointNonGeograpyPoly = new QueryToken[]
            {
                new EndPathToken("GeographyPoint", parent),
                new LiteralToken("bob") 
            };

            QueryToken[] geographyPolyNonGeographyPoint = new QueryToken[]
            {
                new EndPathToken("GeographyPolygon", parent),
                new LiteralToken("bob") 
            };

            QueryToken[] garbage = new QueryToken[]
            {
                new LiteralToken("tex"),
                new LiteralToken("slim") 
            };

            FunctionCallToken geometryPointNonGeometryPolyToken = new FunctionCallToken("geo.intersects", geometryPointNonGeometryPoly);
            FunctionCallToken geometryPolyNonGeometryPointToken = new FunctionCallToken("geo.intersects", geometryPolyNonGeometryPoint);
            FunctionCallToken geographyPointNonGeographyPolyToken = new FunctionCallToken("geo.intersects", geographyPointNonGeograpyPoly);
            FunctionCallToken geographyPolyNonGeographyPointToken = new FunctionCallToken("geo.intersects", geographyPolyNonGeographyPoint);
            FunctionCallToken garbageToken = new FunctionCallToken("geo.intersects", garbage);

            Action createWithGeometryPointNonGeometryPoly = () => this.functionCallBinder.BindFunctionCall(geometryPointNonGeometryPolyToken);
            Action createWithGeometryPolyNonGeometryPoint = () => this.functionCallBinder.BindFunctionCall(geometryPolyNonGeometryPointToken);
            Action createWithGeographyPointNonGeographyPoly = () => this.functionCallBinder.BindFunctionCall(geographyPointNonGeographyPolyToken);
            Action createWithGeographyPolyNonGeographyPoint = () => this.functionCallBinder.BindFunctionCall(geographyPolyNonGeographyPointToken);
            Action createWithGarbage = () => this.functionCallBinder.BindFunctionCall(garbageToken);

            FunctionSignatureWithReturnType[] signatures;
            BuiltInUriFunctions.TryGetBuiltInFunction(geometryPointNonGeometryPolyToken.Name, out signatures);

            createWithGeometryPointNonGeometryPoly.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geometryPointNonGeometryPolyToken.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(geometryPointNonGeometryPolyToken.Name, signatures)));

            BuiltInUriFunctions.TryGetBuiltInFunction(geometryPolyNonGeometryPointToken.Name, out signatures);

            createWithGeometryPolyNonGeometryPoint.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geometryPolyNonGeometryPointToken.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(geometryPolyNonGeometryPointToken.Name, signatures)));

            BuiltInUriFunctions.TryGetBuiltInFunction(geographyPointNonGeographyPolyToken.Name, out signatures);

            createWithGeographyPointNonGeographyPoly.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geographyPointNonGeographyPolyToken.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(geographyPointNonGeographyPolyToken.Name, signatures)));

            BuiltInUriFunctions.TryGetBuiltInFunction(geographyPolyNonGeographyPointToken.Name, out signatures);

            createWithGeographyPolyNonGeographyPoint.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geographyPolyNonGeographyPointToken.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(geographyPolyNonGeographyPointToken.Name, signatures)));

            BuiltInUriFunctions.TryGetBuiltInFunction(garbageToken.Name, out signatures);

            createWithGarbage.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    garbageToken.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(garbageToken.Name, signatures)));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchHourOnTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("hour", EdmCoreModel.Instance.GetDuration(true));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchHourOnNullableTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("hour", EdmCoreModel.Instance.GetDuration(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchMinuteOnTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("minute", EdmCoreModel.Instance.GetDuration(true));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchMinuteOnNullableTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("minute", EdmCoreModel.Instance.GetDuration(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchSecondOnTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("second", EdmCoreModel.Instance.GetDuration(true));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchSecondOnNullableTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("second", EdmCoreModel.Instance.GetDuration(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchYear()
        {
            this.TestUnaryCanonicalFunctionBinding("year",
                EdmCoreModel.Instance.GetDateTimeOffset(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("year",
                EdmCoreModel.Instance.GetDateTimeOffset(false),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("year",
                EdmCoreModel.Instance.GetDate(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("year",
                EdmCoreModel.Instance.GetDate(false),
                EdmCoreModel.Instance.GetInt32(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchMonth()
        {
            this.TestUnaryCanonicalFunctionBinding("month",
                EdmCoreModel.Instance.GetDateTimeOffset(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("month",
                EdmCoreModel.Instance.GetDateTimeOffset(false),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("month",
                EdmCoreModel.Instance.GetDate(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("month",
                EdmCoreModel.Instance.GetDate(false),
                EdmCoreModel.Instance.GetInt32(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchDay()
        {
            this.TestUnaryCanonicalFunctionBinding("day",
                EdmCoreModel.Instance.GetDateTimeOffset(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("day",
                EdmCoreModel.Instance.GetDateTimeOffset(false),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("day",
                EdmCoreModel.Instance.GetDate(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("day",
                EdmCoreModel.Instance.GetDate(false),
                EdmCoreModel.Instance.GetInt32(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchHour()
        {
            this.TestUnaryCanonicalFunctionBinding("hour",
                EdmCoreModel.Instance.GetDateTimeOffset(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("hour",
                EdmCoreModel.Instance.GetDateTimeOffset(false),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("hour",
                EdmCoreModel.Instance.GetDuration(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("hour",
                EdmCoreModel.Instance.GetDuration(false),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("hour",
                EdmCoreModel.Instance.GetTimeOfDay(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("hour",
                EdmCoreModel.Instance.GetTimeOfDay(false),
                EdmCoreModel.Instance.GetInt32(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchMinute()
        {
            this.TestUnaryCanonicalFunctionBinding("minute",
                EdmCoreModel.Instance.GetDateTimeOffset(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("minute",
                EdmCoreModel.Instance.GetDateTimeOffset(false),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("minute",
                EdmCoreModel.Instance.GetDuration(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("minute",
                EdmCoreModel.Instance.GetDuration(false),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("minute",
                EdmCoreModel.Instance.GetTimeOfDay(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("minute",
                EdmCoreModel.Instance.GetTimeOfDay(false),
                EdmCoreModel.Instance.GetInt32(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchSecond()
        {
            this.TestUnaryCanonicalFunctionBinding("second",
                EdmCoreModel.Instance.GetDateTimeOffset(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("second",
                EdmCoreModel.Instance.GetDateTimeOffset(false),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("second",
                EdmCoreModel.Instance.GetDuration(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("second",
                EdmCoreModel.Instance.GetDuration(false),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("second",
                EdmCoreModel.Instance.GetTimeOfDay(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("second",
                EdmCoreModel.Instance.GetTimeOfDay(false),
                EdmCoreModel.Instance.GetInt32(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchDate()
        {
            this.TestUnaryCanonicalFunctionBinding("date",
                EdmCoreModel.Instance.GetDateTimeOffset(true),
                EdmCoreModel.Instance.GetDate(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("date",
                EdmCoreModel.Instance.GetDateTimeOffset(false),
                EdmCoreModel.Instance.GetDate(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchTime()
        {
            this.TestUnaryCanonicalFunctionBinding("time",
                EdmCoreModel.Instance.GetDateTimeOffset(true),
                EdmCoreModel.Instance.GetTimeOfDay(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("time",
                EdmCoreModel.Instance.GetDateTimeOffset(false),
                EdmCoreModel.Instance.GetTimeOfDay(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchFractionalseconds()
        {
            this.TestUnaryCanonicalFunctionBinding("fractionalseconds",
                EdmCoreModel.Instance.GetDateTimeOffset(true),
                EdmCoreModel.Instance.GetDecimal(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("fractionalseconds",
                EdmCoreModel.Instance.GetDateTimeOffset(false),
                EdmCoreModel.Instance.GetDecimal(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("fractionalseconds",
                EdmCoreModel.Instance.GetTimeOfDay(true),
                EdmCoreModel.Instance.GetDecimal(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("fractionalseconds",
                EdmCoreModel.Instance.GetTimeOfDay(false),
                EdmCoreModel.Instance.GetDecimal(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchTotaloffsetminutes()
        {
            this.TestUnaryCanonicalFunctionBinding("totaloffsetminutes",
                EdmCoreModel.Instance.GetDateTimeOffset(true),
                EdmCoreModel.Instance.GetInt32(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("totaloffsetminutes",
                EdmCoreModel.Instance.GetDateTimeOffset(false),
                EdmCoreModel.Instance.GetInt32(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchNow()
        {
            this.TestNullaryFunctionBinding("now",
                EdmCoreModel.Instance.GetDateTimeOffset(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchMaxdatetime()
        {
            this.TestNullaryFunctionBinding("maxdatetime",
                EdmCoreModel.Instance.GetDateTimeOffset(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchMindatetime()
        {
            this.TestNullaryFunctionBinding("mindatetime",
                EdmCoreModel.Instance.GetDateTimeOffset(false));
        }

        [Fact]
        public void FunctionCallBinderShouldMatchTotalseconds()
        {
            this.TestUnaryCanonicalFunctionBinding("totalseconds",
                EdmCoreModel.Instance.GetDuration(true),
                EdmCoreModel.Instance.GetDecimal(false));
            this.ResetState();
            this.TestUnaryCanonicalFunctionBinding("totalseconds",
                EdmCoreModel.Instance.GetDuration(false),
                EdmCoreModel.Instance.GetDecimal(false));
        }

        [Fact]
        public void EndPathAsFunctionCallReturnsSingleValueFunctionCallNodeWhenThereAreNoOverloads()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.HasJob", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasJobFunction());
        }

        [Fact]
        public void EndPathAsFunctionCallReturnsCorrectOverload()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.HasDog", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadWithOneParameter());
        }

        [Fact]
        public void EndPathAsFunctionCallReturnsFalseIfNoFunctionExists()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Gobbldygook", out functionCallNode).Should().BeFalse();
            functionCallNode.Should().BeNull();
        }

        [Fact]
        public void EndPathAsFuctionCallReturnsFalseForServiceOps()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("GetCoolPeople", out functionCallNode).Should().BeFalse();
            functionCallNode.Should().BeNull();
        }

        [Fact]
        public void EndPathAsFunctionCallShouldNotThrowsIfOverloadsHaveDifferentReturnTypesButOneOverloadResolvesByParameters()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.HasHat", out functionCallNode);
        }

        [Fact]
        public void SingleEntityFunctionHasResultEntitySet()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.GetMyDog", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeSingleEntityFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyDog())
                .And.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void EntityCollectionFunctionHasResultEntitySet()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.AllMyFriendsDogs", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeEntityCollectionFunctionCallNode(HardCodedTestModel.GetFunctionForAllMyFriendsDogs())
                .And.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void EntityCollectionFunctionDoesNotHaveResultEntitySetIfFunctionImportDidntSpecifyOne()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.AllMyFriendsDogs_NoSet", out functionCallNode).Should().BeTrue();
            functionCallNode.As<EntityCollectionFunctionCallNode>().NavigationSource.Should().BeNull();
        }

        [Fact]
        public void DottedIdentifierAsFunctionCallReturnsSingleValueFunctionCallNodeWhenThereAreNoOverloads()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("Fully.Qualified.Namespace.HasJob", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasJobFunction());
        }

        [Fact]
        public void DottedIdentifierAsFunctionCallReturnsCorrectOverload()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("Fully.Qualified.Namespace.HasDog", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOneParameterOverload());
        }

        [Fact]
        public void DottedIdentifierAsFunctionCallReturnsFalseIfNoFunctionExists()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("Gobbl.dygook", out functionCallNode).Should().BeFalse();
            functionCallNode.Should().BeNull();
        }

        [Fact]
        public void FunctionCallBinderFindsGlobalFunctions()
        {
            FunctionCallToken functionCallToken = new FunctionCallToken(
                "Fully.Qualified.Namespace.FindMyOwner",
                new FunctionParameterToken[]
                {
                    new FunctionParameterToken("dogsName", new LiteralToken("fido","'fido'"))
                },
                null);
            QueryNode functionCallNode = this.functionCallBinder.BindFunctionCall(functionCallToken);
            functionCallNode.ShouldBeSingleEntityFunctionCallNode(HardCodedTestModel.GetFunctionForFindMyOwner());
        }

        [Fact]
        public void DottedIdentiferAsFunctionCallReturnsFalseForActions()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("AlternateContext.Move", out functionCallNode).Should().BeFalse();
            functionCallNode.Should().BeNull();
        }

        [Fact]
        public void DottedIdentifierAsFunctionReturnsFalseForServiceOps()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("AlternateContext.GetCoolPeople", out functionCallNode).Should().BeFalse();
            functionCallNode.Should().BeNull();
        }

        [Fact]
        public void GetFunctionSignaturesShouldThrowOnIncorrectName()
        {
            Action bind = () => FunctionCallBinder.GetUriFunctionSignatures("rarg");

            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_UnknownFunction("rarg"));
        }

        [Fact]
        public void BuiltInFunctionsCannotHaveParentNodes()
        {
            FunctionCallToken functionCallToken = new FunctionCallToken("substring", null, new EndPathToken("Name", null));
            Action bindWithNonNullParent = () => this.functionCallBinder.BindFunctionCall(functionCallToken);
            bindWithNonNullParent.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionCallBinder_UriFunctionMustHaveHaveNullParent("substring"));
        }

        [Fact]
        public void FunctionsMustHaveEntityBindingTypes()
        {
            FunctionCallToken functionCallToken = new FunctionCallToken(
                "ChangeOwner",
                null,
                new EndPathToken("Color", new InnerPathToken("MyDog", null, null)));
            Action bindWithNonEntityBindingType = () => this.functionCallBinder.BindFunctionCall(functionCallToken);
            bindWithNonEntityBindingType.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionCallBinder_UriFunctionMustHaveHaveNullParent("ChangeOwner"));
        }

        [Fact]
        public void DottedIdentifierAsFunctionCallShouldNotThrowsIfOverloadsHaveDifferentReturnTypesButOneResolvesByParameters()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("Fully.Qualified.Namespace.HasHat", out functionCallNode);
        }

        [Fact]
        public void CannotCallFunctionInOpenTypeSpace()
        {
            FunctionCallToken functionCall = new FunctionCallToken(
                "Fully.Qualified.Namespace.FindMyOwner",
                new FunctionParameterToken[]
                {
                    new FunctionParameterToken("dogsName", new LiteralToken("fido","'fido'"))
                },
                new EndPathToken("OpenProperty", new InnerPathToken("MyFavoritePainting", null, null)));
            Action bindOpenFunction = () => this.functionCallBinder.BindFunctionCall(functionCall);
            bindOpenFunction.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionCallBinder_CallingFunctionOnOpenProperty("Fully.Qualified.Namespace.FindMyOwner"));
        }

        [Fact]
        public void GetUriFunction_CombineCustomFunctionsAndBuiltInFunctions()
        {
            const string BUILT_IN_GEODISTANCE_FUNCTION_NAME = "geo.distance";

            try
            {
                FunctionSignatureWithReturnType customFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false),
                                                        EdmCoreModel.Instance.GetBoolean(false));

                // Add with 'addAsOverload' 'true'
                CustomUriFunctions.AddCustomUriFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME, customFunctionSignature);

                FunctionSignatureWithReturnType[] resultUriFunctionSignatures =
                    FunctionCallBinder.GetUriFunctionSignatures(BUILT_IN_GEODISTANCE_FUNCTION_NAME);

                // Assert
                FunctionSignatureWithReturnType[] existsBuiltInSignature;
                BuiltInUriFunctions.TryGetBuiltInFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME, out existsBuiltInSignature).Should().BeTrue();

                resultUriFunctionSignatures.Should().NotBeNull();
                resultUriFunctionSignatures.Length.Should().Be(existsBuiltInSignature.Length + 1);
                resultUriFunctionSignatures.All(x => x == customFunctionSignature || existsBuiltInSignature.Any(y => x == y)).Should().BeTrue();
            }
            finally
            {
                // Clean from CustomFunctions cache
                CustomUriFunctions.RemoveCustomUriFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME);
            }
        }

        [Fact]
        public void GetUriFunction_OnlyBuiltInFunctionsExist()
        {
            const string BUILT_IN_FUNCTION_NAME = "geo.intersects";

            FunctionSignatureWithReturnType[] resultUriFunctionSignatures =
                FunctionCallBinder.GetUriFunctionSignatures(BUILT_IN_FUNCTION_NAME);

            // Assert
            FunctionSignatureWithReturnType[] existsBuiltInSignature;
            BuiltInUriFunctions.TryGetBuiltInFunction(BUILT_IN_FUNCTION_NAME, out existsBuiltInSignature).Should().BeTrue();

            resultUriFunctionSignatures.Should().NotBeNull();
            resultUriFunctionSignatures.Length.Should().Be(existsBuiltInSignature.Length);
            resultUriFunctionSignatures.All(x => existsBuiltInSignature.Any(y => x == y)).Should().BeTrue();
        }

        [Fact]
        public void GetUriFunction_OnlyCustomFunctionsExist()
        {
            const string CUSTOM_FUNCTION_NAME = "myCustomFunction";

            try
            {
                FunctionSignatureWithReturnType customFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false),
                                                        EdmCoreModel.Instance.GetBoolean(false));

                // Add with override 'true'
                CustomUriFunctions.AddCustomUriFunction(CUSTOM_FUNCTION_NAME, customFunctionSignature);


                FunctionSignatureWithReturnType[] resultUriFunctionSignatures =
                    FunctionCallBinder.GetUriFunctionSignatures(CUSTOM_FUNCTION_NAME);

                // Assert
                resultUriFunctionSignatures.Should().NotBeNull();
                resultUriFunctionSignatures.Length.Should().Be(1);
                resultUriFunctionSignatures.All(x => x == customFunctionSignature).Should().BeTrue();
            }
            finally
            {
                // Clean from CustomFunctions cache
                CustomUriFunctions.RemoveCustomUriFunction(CUSTOM_FUNCTION_NAME);
            }
        }

        private bool RunBindEndPathAsFunctionCall(string endPathIdentifier, out QueryNode functionCallNode)
        {
            var boundFunctionCallToken = new EndPathToken(endPathIdentifier, null);
            return this.functionCallBinder.TryBindEndPathAsFunctionCall(boundFunctionCallToken,
                                                                 new EntityRangeVariableReferenceNode(
                                                                     ExpressionConstants.It,
                                                                     (EntityRangeVariable)
                                                                     this.binder.BindingState.ImplicitRangeVariable),
                                                                 this.binder.BindingState, out functionCallNode);
        }

        private bool RunDottedIdentifierAsFunctionCall(string identifier, out QueryNode functionCallNode)
        {
            var dottedIdentifierToken = new DottedIdentifierToken(identifier, null);
            return this.functionCallBinder.TryBindDottedIdentifierAsFunctionCall(dottedIdentifierToken,
                                                                          new EntityRangeVariableReferenceNode(
                                                                              ExpressionConstants.It,
                                                                              (EntityRangeVariable)
                                                                              this.binder.BindingState
                                                                                  .ImplicitRangeVariable),
                                                                          out functionCallNode);
        }

        private void TestUnaryCanonicalFunctionBinding(string functionName, IEdmTypeReference typeReference, IEdmTypeReference returnTypeReference = null)
        {
            this.binder.BindingState.RangeVariables.Push(new NonentityRangeVariable("a", typeReference, null));
            this.functionCallBinder.BindFunctionCall(
                new FunctionCallToken(functionName, new[] { new RangeVariableToken("a") }))
                .ShouldBeSingleValueFunctionCallQueryNode(functionName, returnTypeReference);
        }

        private void TestNullaryFunctionBinding(string functionName, IEdmTypeReference returnTypeReference)
        {
            this.functionCallBinder.BindFunctionCall(
                new FunctionCallToken(functionName, null))
                .ShouldBeSingleValueFunctionCallQueryNode(functionName, returnTypeReference);
        }

        //Support Methods
        internal FunctionSignatureWithReturnType[] GetHardCodedYearFunctionSignatureForTest()
        {
            FunctionSignatureWithReturnType[] signatures = null;
            BuiltInUriFunctions.TryGetBuiltInFunction("year", out signatures);
            return signatures;
        }

        internal FunctionSignatureWithReturnType[] GetDuplicateIndexOfFunctionSignatureForTest()
        {
            FunctionSignatureWithReturnType[] signatures = null;
            BuiltInUriFunctions.TryGetBuiltInFunction("indexof", out signatures);

            return new FunctionSignatureWithReturnType[] { signatures[0], signatures[0] };
        }

        internal FunctionSignatureWithReturnType GetSingleSubstringFunctionSignatureForTest()
        {
            FunctionSignatureWithReturnType[] signatures = null;
            BuiltInUriFunctions.TryGetBuiltInFunction("substring", out signatures);
            return signatures[0];
        }

        private static void AssertSignatureTypesMatchArguments(FunctionSignatureWithReturnType signature, List<QueryNode> nodes)
        {
            for (int i = 0; i < signature.ArgumentTypes.Length; ++i)
            {
                signature.ArgumentTypes[i].Definition.Should().Be(MetadataBindingUtils.GetEdmType(nodes[i]));
            }
        }

        private void ResetState()
        {
            BindingState state = new BindingState(this.configuration);
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            binder = new MetadataBinder(state);
            this.functionCallBinder = new FunctionCallBinder(binder.Bind, state);
        }
    }
}
