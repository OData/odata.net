//---------------------------------------------------------------------
// <copyright file="FunctionCallBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Binders
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
            var functionCallNode = result.ShouldBeSingleValueFunctionCallQueryNode("substring");
            Assert.Equal(2, functionCallNode.Parameters.Count());
            Assert.Equal("Edm.String", functionCallNode.TypeReference.Definition.FullTypeName());
        }

        [Fact]
        public void BindFunctionForNullArgumentType()
        {
            this.functionCallBinder = new FunctionCallBinder(FakeBindMethods.BindMethodReturningANullLiteralConstantNode, this.binder.BindingState);
            var arguments = new List<QueryToken>() { new LiteralToken("ignored") };
            var token = new FunctionCallToken("year", arguments);
            var result = functionCallBinder.BindFunctionCall(token);
            Assert.Null(result.ShouldBeSingleValueFunctionCallQueryNode("year").Parameters.Single().ShouldBeConstantQueryNode<object>(null).TypeReference);
        }

        [Fact]
        public void BindFunctionAllNullArgumentTypes()
        {
            this.functionCallBinder = new FunctionCallBinder(FakeBindMethods.BindMethodReturningANullLiteralConstantNode, this.binder.BindingState);
            var arguments = new List<QueryToken>() { new LiteralToken("ignored"), new LiteralToken("ignored") };
            var token = new FunctionCallToken("substring", arguments);
            var result = functionCallBinder.BindFunctionCall(token);
            var functionCallNode = result.ShouldBeSingleValueFunctionCallQueryNode("substring");
            Assert.Equal(2, functionCallNode.Parameters.Count());
            Assert.Null(functionCallNode.Parameters.First().ShouldBeConstantQueryNode<object>(null).TypeReference);
            Assert.Null(functionCallNode.Parameters.Last().ShouldBeConstantQueryNode<object>(null).TypeReference);
        }

        [Fact]
        public void BindFunctionEmptyArguments()
        {
            // regression test for [UriParser] day() allowed. What does that mean?
            this.functionCallBinder = new FunctionCallBinder(FakeBindMethods.BindMethodReturnsNull, this.binder.BindingState);
            var token = new FunctionCallToken("day", new List<QueryToken>());
            Action bindWithEmptyArguments = () => functionCallBinder.BindFunctionCall(token);

            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.ExtractSignatures(
                FunctionCallBinder.GetUriFunctionSignatures("day")); // to match the error message... blah
            bindWithEmptyArguments.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
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

            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.ExtractSignatures(
                FunctionCallBinder.GetUriFunctionSignatures("day")); // to match the error message... blah
            bindWithEmptyArguments.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
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
            bindWithTooManyNonTypedArgs.Throws<ODataException>(ODataErrorStrings.FunctionCallBinder_CannotFindASuitableOverload("day", "2"));
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
            nodes[0].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String).Source.ShouldBeConstantQueryNode<object>(null);
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
            nodes[0].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String).Source.ShouldBeConstantQueryNode<object>(null);
            nodes[1].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Int32).Source.ShouldBeConstantQueryNode<object>(null);

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
                    .Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
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
                    .Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
            nodes[1].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Int32)
                    .Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName + "1");

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
                    .Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
            nodes[1].ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Int32)
                    .Source.ShouldBeConstantQueryNode<object>(null);

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
            a.Throws<ODataException>(Strings.MetadataBinder_CannotConvertToType("Edm.Int32", "Edm.String"));
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
            a.Throws<ODataException>(Strings.MetadataBinder_CannotConvertToType("Edm.String", "Edm.Int32"));
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
            a.Throws<ODataException>(Strings.MetadataBinder_CannotConvertToType("Edm.String", "Edm.Int32"));
        }

        [Fact]
        public void TypePromoteEnumArguments()
        {
            var enumType = new EdmEnumType("MyNS", "MyName", false);
            enumType.AddMember("MyValue", new EdmEnumMemberValue(0));
            var enumTypeRef = new EdmEnumTypeReference(enumType, false);
            
            FunctionSignatureWithReturnType signature =
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), enumTypeRef);

            List<QueryNode> nodes = new List<QueryNode>()
                                        {
                                            new ConstantNode("MyValue", "MyNS.MyName'MyValue'", enumTypeRef),
                                        };
            FunctionCallBinder.TypePromoteArguments(signature, nodes);
            AssertSignatureTypesMatchArguments(signature, nodes);
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
            Assert.Single(result);
            Assert.Equal("Edm.DateTimeOffset", result[0].TypeReference.Definition.FullTypeName());
        }

        [Fact]
        public void ShouldThrowWhenArgumentsAreNotSingleValue()
        {
            List<QueryNode> argumentNodes =
                new List<QueryNode>()
                {
                    new CollectionNavigationNode(HardCodedTestModel.GetDogsSet(), HardCodedTestModel.GetDogMyPeopleNavProp(), new EdmPathExpression("MyDog"))
                };

            Action bind = () => FunctionCallBinder.ValidateArgumentsAreSingleValue("year", argumentNodes);
            bind.Throws<ODataException>(Strings.MetadataBinder_FunctionArgumentNotSingleValue("year"));
        }

        [Fact]
        public void EnsureArgumentsAreSingleValueNoArguments()
        {
            var result = FunctionCallBinder.ValidateArgumentsAreSingleValue("year", new List<QueryNode>());
            Assert.Empty(result);
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
            var nameSignatures = this.GetDuplicateIndexOfFunctionSignatureForTest();

            Action bind = () => FunctionCallBinder.MatchSignatureToUriFunction(
                "IndexOf",
                new SingleValueNode[] {
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", argumentNodes[0].GetEdmTypeReference())),
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", argumentNodes[1].GetEdmTypeReference()))},
                nameSignatures);

            bind.Throws<ODataException>(Strings.MetadataBinder_NoApplicableFunctionFound(
                    "IndexOf",
                    UriFunctionsHelper.BuildFunctionSignatureListDescription("IndexOf", nameSignatures.Select(nameSig => nameSig.Value))));
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

            Assert.Equal("Edm.Int32", result.Value.ReturnType.FullName());
            Assert.Equal("Edm.DateTimeOffset", result.Value.ArgumentTypes[0].FullName());
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
                new List<KeyValuePair<string, FunctionSignatureWithReturnType>>());

            bind.Throws<ODataException>(Strings.MetadataBinder_NoApplicableFunctionFound(
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

            bind.Throws<ODataException>(Strings.MetadataBinder_NoApplicableFunctionFound(
                    "year",
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(
                        "year",
                        this.GetHardCodedYearFunctionSignatureForTest().Select( _ => _.Value))));
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

            FunctionSignatureWithReturnType sig = result.Value;
            Assert.True(sig.ReturnType.IsEquivalentTo(EdmCoreModel.Instance.GetString(true)));
            Assert.Equal(2, sig.ArgumentTypes.Count());
            Assert.True(sig.ArgumentTypes.First().IsEquivalentTo(EdmCoreModel.Instance.GetString(true)));
            Assert.True(sig.ArgumentTypes.Last().IsEquivalentTo(EdmCoreModel.Instance.GetInt32(true)));
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
            var functionCallNode = node.ShouldBeSingleValueFunctionCallQueryNode("cast");
            functionCallNode.Parameters.ElementAt(0).ShouldBeConstantQueryNode("stuff");
            functionCallNode.Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
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
            var functionCallNode = node.ShouldBeSingleValueFunctionCallQueryNode("cast");
            functionCallNode.Parameters.ElementAt(0).ShouldBeResourceRangeVariableReferenceNode(ExpressionConstants.It);
            functionCallNode.Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
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

            createWithMoreThanTwoArgs.Throws<ODataErrorException>(ODataErrorStrings.MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands(3));
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
            createWithoutATypeArg.Throws<ODataException>(ODataErrorStrings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
        }

        [Fact]
        public void CastShouldFailIfTypeArgumentIsACollection()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("Collection(Fully.Qualified.Namespace.Person)"),
            };

            Action bind = () => this.functionCallBinder.BindFunctionCall(new FunctionCallToken("cast", args));
            bind.Throws<ODataException>(Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
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
            bind.Throws<ODataException>(Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
        }

        [Fact]
        public void IsOfShouldFailIfTypeArgumentIsACollection()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("Collection(Fully.Qualified.Namespace.Person)"),
            };

            Action bind = () => this.functionCallBinder.BindFunctionCall(new FunctionCallToken("isof", args));
            bind.Throws<ODataException>(Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
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
            bind.Throws<ODataException>(Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
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
            createWithOutOfOrderArgs.Throws<ODataException>(ODataErrorStrings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
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
            SingleResourceFunctionCallNode functionCallNode = this.functionCallBinder.BindFunctionCall(function) as SingleResourceFunctionCallNode;
            Assert.NotNull(functionCallNode);
            Assert.Equal(2, functionCallNode.Parameters.Count());
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
            var functionCallNode = node.ShouldBeSingleValueFunctionCallQueryNode("isof");
            functionCallNode.Parameters.ElementAt(0).ShouldBeConstantQueryNode("stuff");
            functionCallNode.Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
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
            var functionCallNode = node.ShouldBeSingleValueFunctionCallQueryNode("isof");
            functionCallNode.Parameters.ElementAt(0).ShouldBeResourceRangeVariableReferenceNode(ExpressionConstants.It);
            functionCallNode.Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
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

            createWithMoreThanTwoArgs.Throws<ODataErrorException>(ODataErrorStrings.MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands(3));
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
            createWithoutATypeArg.Throws<ODataException>(ODataErrorStrings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
        }

        [Fact]
        public void TypeMustBeLastArgumentToIsOf()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("Edm.String"),
                new LiteralToken("stuff")
            };
            FunctionCallToken function = new FunctionCallToken("isof", args);
            Action createWithOutOfOrderArgs = () => this.functionCallBinder.BindFunctionCall(function);
            createWithOutOfOrderArgs.Throws<ODataException>(ODataErrorStrings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
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

            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.ExtractSignatures(
                FunctionCallBinder.GetUriFunctionSignatures(function.Name));
            createWithMoreThanOneArg.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
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
            createWithNonSingleValueNode.Throws<ODataException>(ODataErrorStrings.MetadataBinder_UnsupportedQueryTokenKind("CustomQueryOption"));
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

            createWithNonLineStringType.Throws<ODataException>(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "GeometryLinePolygon"));
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
            Assert.Single(node.Parameters);
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

            createWithOneArg.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    functionWithOneArg.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(functionWithOneArg.Name, signatures)));

            BuiltInUriFunctions.TryGetBuiltInFunction(functionWithMoreThanTwoArgs.Name, out signatures);

            createWithMoreThanTwoArgs.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
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

            Assert.Equal(2, geometryPointGeometryPolyFunction.Parameters.Count());
            geometryPointGeometryPolyFunction.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPointProp());
            geometryPointGeometryPolyFunction.Parameters.ElementAt(1).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPolygonProp());

            Assert.Equal(2, geometryPolyGeometryPointFunction.Parameters.Count());
            geometryPolyGeometryPointFunction.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPolygonProp());
            geometryPolyGeometryPointFunction.Parameters.ElementAt(1).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPointProp());

            Assert.Equal(2, geographyPointGeographyPolyFunction.Parameters.Count());
            geographyPointGeographyPolyFunction.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyPointProp());
            geographyPointGeographyPolyFunction.Parameters.ElementAt(1).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyPolygonProp());

            Assert.Equal(2, geographyPolyGeographyPointFunction.Parameters.Count());
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

            createWithGeometryPointNonGeometryPoly.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geometryPointNonGeometryPolyToken.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(geometryPointNonGeometryPolyToken.Name, signatures)));

            BuiltInUriFunctions.TryGetBuiltInFunction(geometryPolyNonGeometryPointToken.Name, out signatures);

            createWithGeometryPolyNonGeometryPoint.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geometryPolyNonGeometryPointToken.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(geometryPolyNonGeometryPointToken.Name, signatures)));

            BuiltInUriFunctions.TryGetBuiltInFunction(geographyPointNonGeographyPolyToken.Name, out signatures);

            createWithGeographyPointNonGeographyPoly.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geographyPointNonGeographyPolyToken.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(geographyPointNonGeographyPolyToken.Name, signatures)));

            BuiltInUriFunctions.TryGetBuiltInFunction(geographyPolyNonGeographyPointToken.Name, out signatures);

            createWithGeographyPolyNonGeographyPoint.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geographyPolyNonGeographyPointToken.Name,
                    UriFunctionsHelper.BuildFunctionSignatureListDescription(geographyPolyNonGeographyPointToken.Name, signatures)));

            BuiltInUriFunctions.TryGetBuiltInFunction(garbageToken.Name, out signatures);

            createWithGarbage.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
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
            Assert.True(RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.HasJob", out functionCallNode));
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasJobFunction());
        }

        [Fact]
        public void EndPathAsFunctionCallReturnsCorrectOverload()
        {
            QueryNode functionCallNode;
            Assert.True(RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.HasDog", out functionCallNode));
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadWithOneParameter());
        }

        [Fact]
        public void EndPathAsFunctionCallReturnsFalseIfNoFunctionExists()
        {
            QueryNode functionCallNode;
            Assert.False(RunBindEndPathAsFunctionCall("Gobbldygook", out functionCallNode));
            Assert.Null(functionCallNode);
        }

        [Fact]
        public void EndPathAsFuctionCallReturnsFalseForServiceOps()
        {
            QueryNode functionCallNode;
            Assert.False(RunBindEndPathAsFunctionCall("GetCoolPeople", out functionCallNode));
            Assert.Null(functionCallNode);
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
            Assert.True(RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.GetMyDog", out functionCallNode));
            Assert.Same(HardCodedTestModel.GetDogsSet(), 
                functionCallNode.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyDog()).NavigationSource);
        }

        [Fact]
        public void EntityCollectionFunctionHasResultEntitySet()
        {
            QueryNode functionCallNode;
            Assert.True(RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.AllMyFriendsDogs", out functionCallNode));
            Assert.Same(HardCodedTestModel.GetDogsSet(),
                functionCallNode.ShouldBeCollectionResourceFunctionCallNode(HardCodedTestModel.GetFunctionForAllMyFriendsDogs()).NavigationSource);
        }

        [Fact]
        public void EntityCollectionFunctionDoesNotHaveResultEntitySetIfFunctionImportDidntSpecifyOne()
        {
            QueryNode functionCallNode;
            Assert.True(RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.AllMyFriendsDogs_NoSet", out functionCallNode));
            var collectFunctionCallNode = Assert.IsType<CollectionResourceFunctionCallNode>(functionCallNode);
            Assert.Null(collectFunctionCallNode.NavigationSource);
        }

        [Fact]
        public void DottedIdentifierAsFunctionCallReturnsSingleValueFunctionCallNodeWhenThereAreNoOverloads()
        {
            QueryNode functionCallNode;
            Assert.True(RunDottedIdentifierAsFunctionCall("Fully.Qualified.Namespace.HasJob", out functionCallNode));
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasJobFunction());
        }

        [Fact]
        public void DottedIdentifierAsFunctionCallReturnsCorrectOverload()
        {
            QueryNode functionCallNode;
            Assert.True(RunDottedIdentifierAsFunctionCall("Fully.Qualified.Namespace.HasDog", out functionCallNode));
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOneParameterOverload());
        }

        [Fact]
        public void DottedIdentifierAsFunctionCallReturnsFalseIfNoFunctionExists()
        {
            QueryNode functionCallNode;
            Assert.False(RunDottedIdentifierAsFunctionCall("Gobbl.dygook", out functionCallNode));
            Assert.Null(functionCallNode);
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
            functionCallNode.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForFindMyOwner());
        }

        [Fact]
        public void DottedIdentiferAsFunctionCallReturnsFalseForActions()
        {
            QueryNode functionCallNode;
            Assert.False(RunDottedIdentifierAsFunctionCall("AlternateContext.Move", out functionCallNode));
            Assert.Null(functionCallNode);
        }

        [Fact]
        public void DottedIdentifierAsFunctionReturnsFalseForServiceOps()
        {
            QueryNode functionCallNode;
            Assert.False(RunDottedIdentifierAsFunctionCall("AlternateContext.GetCoolPeople", out functionCallNode));
            Assert.Null(functionCallNode);
        }

        [Fact]
        public void GetFunctionSignaturesShouldThrowOnIncorrectName()
        {
            Action bind = () => FunctionCallBinder.GetUriFunctionSignatures("rarg");

            bind.Throws<ODataException>(Strings.MetadataBinder_UnknownFunction("rarg"));
        }

        [Fact]
        public void BuiltInFunctionsCannotHaveParentNodes()
        {
            FunctionCallToken functionCallToken = new FunctionCallToken("substring", null, new EndPathToken("Name", null));
            Action bindWithNonNullParent = () => this.functionCallBinder.BindFunctionCall(functionCallToken);
            bindWithNonNullParent.Throws<ODataException>(ODataErrorStrings.FunctionCallBinder_UriFunctionMustHaveHaveNullParent("substring"));
        }

        [Fact]
        public void FunctionsMustHaveEntityBindingTypes()
        {
            FunctionCallToken functionCallToken = new FunctionCallToken(
                "ChangeOwner",
                null,
                new EndPathToken("Color", new InnerPathToken("MyDog", null, null)));
            Action bindWithNonEntityBindingType = () => this.functionCallBinder.BindFunctionCall(functionCallToken);
            bindWithNonEntityBindingType.Throws<ODataException>(ODataErrorStrings.FunctionCallBinder_UriFunctionMustHaveHaveNullParent("ChangeOwner"));
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
            bindOpenFunction.Throws<ODataException>(ODataErrorStrings.FunctionCallBinder_CallingFunctionOnOpenProperty("Fully.Qualified.Namespace.FindMyOwner"));
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

                FunctionSignatureWithReturnType[] resultUriFunctionSignatures = FunctionCallBinder.ExtractSignatures(
                    FunctionCallBinder.GetUriFunctionSignatures(BUILT_IN_GEODISTANCE_FUNCTION_NAME));

                // Assert
                FunctionSignatureWithReturnType[] existsBuiltInSignature;
                Assert.True(BuiltInUriFunctions.TryGetBuiltInFunction(BUILT_IN_GEODISTANCE_FUNCTION_NAME, out existsBuiltInSignature));

                Assert.NotNull(resultUriFunctionSignatures);
                Assert.Equal(existsBuiltInSignature.Length + 1, resultUriFunctionSignatures.Length);
                Assert.True(resultUriFunctionSignatures.All(x => x == customFunctionSignature || existsBuiltInSignature.Any(y => x == y)));
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

            FunctionSignatureWithReturnType[] resultUriFunctionSignatures = FunctionCallBinder.ExtractSignatures(
                FunctionCallBinder.GetUriFunctionSignatures(BUILT_IN_FUNCTION_NAME));

            // Assert
            FunctionSignatureWithReturnType[] existsBuiltInSignature;
            Assert.True(BuiltInUriFunctions.TryGetBuiltInFunction(BUILT_IN_FUNCTION_NAME, out existsBuiltInSignature));

            Assert.NotNull(resultUriFunctionSignatures);
            Assert.Equal(existsBuiltInSignature.Length, resultUriFunctionSignatures.Length);
            Assert.True(resultUriFunctionSignatures.All(x => existsBuiltInSignature.Any(y => x == y)));
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


                FunctionSignatureWithReturnType[] resultUriFunctionSignatures = FunctionCallBinder.ExtractSignatures(
                    FunctionCallBinder.GetUriFunctionSignatures(CUSTOM_FUNCTION_NAME));

                // Assert
                Assert.NotNull(resultUriFunctionSignatures);
                Assert.Single(resultUriFunctionSignatures);
                Assert.True(resultUriFunctionSignatures.All(x => x == customFunctionSignature));
            }
            finally
            {
                // Clean from CustomFunctions cache
                CustomUriFunctions.RemoveCustomUriFunction(CUSTOM_FUNCTION_NAME);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetUriFunction_CanBindEnumArgument(bool explicitEnum)
        {
            const string CUSTOM_FUNCTION_NAME = "myCustomFunction";

            try
            {
                var enumType = new EdmEnumType("MyNS", "MyName", false);
                enumType.AddMember("MyValue", new EdmEnumMemberValue(0));
                var enumTypeRef = new EdmEnumTypeReference(enumType, false);

                FunctionSignatureWithReturnType customFunctionSignature =
                    new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), enumTypeRef);

                CustomUriFunctions.AddCustomUriFunction(CUSTOM_FUNCTION_NAME, customFunctionSignature);

                var arguments = explicitEnum ? new List<QueryToken>() { new LiteralToken("MyValue", "MyNS.MyName'MyValue'", enumTypeRef) }
                                             : new List<QueryToken>() { new LiteralToken("MyValue") };
                var token = new FunctionCallToken(CUSTOM_FUNCTION_NAME, arguments);
                var result = functionCallBinder.BindFunctionCall(token);
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
                                                                 new ResourceRangeVariableReferenceNode(
                                                                     ExpressionConstants.It,
                                                                     (ResourceRangeVariable)
                                                                     this.binder.BindingState.ImplicitRangeVariable),
                                                                 this.binder.BindingState, out functionCallNode);
        }

        private bool RunDottedIdentifierAsFunctionCall(string identifier, out QueryNode functionCallNode)
        {
            var dottedIdentifierToken = new DottedIdentifierToken(identifier, null);
            return this.functionCallBinder.TryBindDottedIdentifierAsFunctionCall(dottedIdentifierToken,
                                                                          new ResourceRangeVariableReferenceNode(
                                                                              ExpressionConstants.It,
                                                                              (ResourceRangeVariable)
                                                                              this.binder.BindingState
                                                                                  .ImplicitRangeVariable),
                                                                          out functionCallNode);
        }

        private void TestUnaryCanonicalFunctionBinding(string functionName, IEdmTypeReference typeReference, IEdmTypeReference returnTypeReference = null)
        {
            this.binder.BindingState.RangeVariables.Push(new NonResourceRangeVariable("a", typeReference, null));
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
        internal IList<KeyValuePair<string, FunctionSignatureWithReturnType>> GetHardCodedYearFunctionSignatureForTest()
        {
            FunctionSignatureWithReturnType[] signatures = null;
            BuiltInUriFunctions.TryGetBuiltInFunction("year", out signatures);

            return signatures.Select(sig => new KeyValuePair<string, FunctionSignatureWithReturnType>("year", sig)).ToList();
        }

        internal IList<KeyValuePair<string, FunctionSignatureWithReturnType>> GetDuplicateIndexOfFunctionSignatureForTest()
        {
            FunctionSignatureWithReturnType[] signatures = null;
            BuiltInUriFunctions.TryGetBuiltInFunction("indexof", out signatures);

            IList<KeyValuePair<string, FunctionSignatureWithReturnType>> nameSignatures = new List<KeyValuePair<string, FunctionSignatureWithReturnType>>();
            const string sampleName = "indexof";
            nameSignatures.Add(new KeyValuePair<string, FunctionSignatureWithReturnType>(sampleName, signatures[0]));
            nameSignatures.Add(new KeyValuePair<string, FunctionSignatureWithReturnType>(sampleName, signatures[0]));
            return nameSignatures;
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
                Assert.Same(MetadataBindingUtils.GetEdmType(nodes[i]), signature.ArgumentTypes[i].Definition);
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
