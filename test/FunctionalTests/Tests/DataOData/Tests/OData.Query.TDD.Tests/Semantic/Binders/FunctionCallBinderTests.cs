﻿//---------------------------------------------------------------------
// <copyright file="FunctionCallBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Unit tests for the FunctionCallBinder class.
    /// TODO: add tests for particular cases.
    /// </summary>
    [TestClass]
    public class FunctionCallBinderTests
    {
        private const string OpenPropertyName = "SomeProperty";

        private readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);
        private FunctionCallBinder functionCallBinder;

        private MetadataBinder binder;

        [TestInitialize]
        public void Init()
        {
            this.ResetState();
        }

        // Bind tests
        [TestMethod]
        public void BindFunction()
        {
            var arguments = new List<QueryToken>() { new LiteralToken("grr"), new LiteralToken(1) };
            var token = new FunctionCallToken("substring", arguments);
            var result = functionCallBinder.BindFunctionCall(token);
            result.ShouldBeSingleValueFunctionCallQueryNode("substring");
            result.As<SingleValueFunctionCallNode>().Parameters.Count().Should().Be(2);
            result.As<SingleValueFunctionCallNode>().TypeReference.Definition.ToString().Should().Contain("String");
        }

        [TestMethod]
        public void BindFunctionForNullArgumentType()
        {
            this.functionCallBinder = new FunctionCallBinder(FakeBindMethods.BindMethodReturningANullLiteralConstantNode, this.binder.BindingState);
            var arguments = new List<QueryToken>() { new LiteralToken("ignored") };
            var token = new FunctionCallToken("year", arguments);
            var result = functionCallBinder.BindFunctionCall(token);
            result.ShouldBeSingleValueFunctionCallQueryNode("year").And.Parameters.Single().ShouldBeConstantQueryNode<object>(null).And.TypeReference.Should().BeNull();
        }

        [TestMethod]
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

        [TestMethod]
        public void BindFunctionEmptyArguments()
        {
            // regression test for [UriParser] day() allowed. What does that mean?
            this.functionCallBinder = new FunctionCallBinder(FakeBindMethods.BindMethodReturnsNull, this.binder.BindingState);
            var token = new FunctionCallToken("day", new List<QueryToken>());
            Action bindWithEmptyArguments = () => functionCallBinder.BindFunctionCall(token);

            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.GetBuiltInFunctionSignatures("day"); // to match the error message... blah
            bindWithEmptyArguments.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    "day",
                    BuiltInFunctions.BuildFunctionSignatureListDescription("day", signatures)));
        }

        [TestMethod]
        public void CannotBindArbitraryNumberOfOpenParametersWithCorrectNonOpenParameters()
        {
            this.functionCallBinder = new FunctionCallBinder(binder.Bind, this.binder.BindingState);
            var arguments = new List<QueryToken>() { new LiteralToken("datetime'1996-02-04'"), new LiteralToken("ignored") };
            var token = new FunctionCallToken("day", arguments);
            Action bindWithEmptyArguments = () => functionCallBinder.BindFunctionCall(token);

            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.GetBuiltInFunctionSignatures("day"); // to match the error message... blah
            bindWithEmptyArguments.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    "day",
                    BuiltInFunctions.BuildFunctionSignatureListDescription("day", signatures)));
        }

        [TestMethod]
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
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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


        [TestMethod]
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
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void EnsureArgumentsAreSingleValueNoArguments()
        {
            var result = FunctionCallBinder.ValidateArgumentsAreSingleValue("year", new List<QueryNode>());
            result.Length.Should().Be(0);
        }

        //MatchArgumentsToSignature tests
        [TestMethod]
        public void MatchArgumentsToSignatureDuplicateSignature()
        {
            List<QueryNode> argumentNodes =
                new List<QueryNode>()
                {
                    new ConstantNode("grr" ),
                    new ConstantNode("grr" )
                };
            var signatures = this.GetDuplicateIndexOfFunctionSignatureForTest();

            Action bind = () => FunctionCallBinder.MatchSignatureToBuiltInFunction(
                "IndexOf",
                new SingleValueNode[] { 
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", argumentNodes[0].GetEdmTypeReference())),
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", argumentNodes[1].GetEdmTypeReference()))},
                signatures);

            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_NoApplicableFunctionFound(
                    "IndexOf",
                    BuiltInFunctions.BuildFunctionSignatureListDescription("IndexOf", signatures)));
        }

        [TestMethod]
        public void MatchArgumentsToSignature()
        {
            List<QueryNode> argumentNodes =
                new List<QueryNode>()
                {
                    new ConstantNode(new DateTimeOffset(2012, 11, 19, 1, 1, 1, 1, new TimeSpan(0, 1, 1, 0)))
                };
            var signatures = this.GetHardCodedYearFunctionSignatureForTest();

            var result = FunctionCallBinder.MatchSignatureToBuiltInFunction(
                "year",
                argumentNodes.Select(s => (SingleValueNode)s).ToArray(),
                signatures);

            result.ReturnType.ToString().Should().Contain("Edm.Int32");
            result.ArgumentTypes[0].ToString().Should().Contain("Edm.DateTimeOffset");
        }

        [TestMethod]
        public void MatchArgumentsToSignatureNoMatchEmpty()
        {
            List<QueryNode> argumentNodes =
                new List<QueryNode>()
                {
                    new ConstantNode(4)
                };

            Action bind = () => FunctionCallBinder.MatchSignatureToBuiltInFunction(
                "year",
                new SingleValueNode[] { 
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", argumentNodes[0].GetEdmTypeReference()))},
                new FunctionSignatureWithReturnType[0]);

            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_NoApplicableFunctionFound(
                    "year",
                    BuiltInFunctions.BuildFunctionSignatureListDescription("year", new FunctionSignatureWithReturnType[0])));
        }

        [TestMethod]
        public void MatchArgumentsToSignatureNoMatchContainsSignatures()
        {
            List<QueryNode> argumentNodes =
                new List<QueryNode>()
                {
                    new ConstantNode(4)
                };

            Action bind = () => FunctionCallBinder.MatchSignatureToBuiltInFunction(
                "year",
                new SingleValueNode[] { 
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", argumentNodes[0].GetEdmTypeReference()))},
                this.GetHardCodedYearFunctionSignatureForTest());

            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_NoApplicableFunctionFound(
                    "year",
                    BuiltInFunctions.BuildFunctionSignatureListDescription("year", this.GetHardCodedYearFunctionSignatureForTest())));
        }

        [TestMethod]
        public void MatchArgumentsToSignatureWillPickRightSignatureForSomeNullArgumentTypes()
        {
            var result = FunctionCallBinder.MatchSignatureToBuiltInFunction(
                "substring",
                 new SingleValueNode[] { 
                     new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", EdmCoreModel.Instance.GetString(true))), 
                     new SingleValueOpenPropertyAccessNode(new ConstantNode(null)/*parent*/, "myOpenPropertyname")}, // open property's TypeReference is null
                 FunctionCallBinder.GetBuiltInFunctionSignatures("substring"));

            result.ReturnType.ShouldBeEquivalentTo(EdmCoreModel.Instance.GetString(true));
            result.ArgumentTypes.Count().Should().Be(2);
            result.ArgumentTypes.First().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetString(true));
            result.ArgumentTypes.Last().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetInt32(true));
        }

        //GetBuiltInFunctionSignatures tests
        [TestMethod]
        public void GetBuiltInFunctionSignaturesSubstring()
        {
            var result = FunctionCallBinder.GetBuiltInFunctionSignatures("substring");
            result.Length.Should().Be(6);

        }

        [TestMethod]
        public void GetBuiltInFunctionSignaturesDateTimeOffsetYear()
        {
            var result = FunctionCallBinder.GetBuiltInFunctionSignatures("year");
            result.Length.Should().Be(4); // DateTimeOffset and Date, Nullable and not Nullable
        }

        [TestMethod]
        public void GetBuiltInFunctionSignaturesDateTimeOffsetMonth()
        {
            var result = FunctionCallBinder.GetBuiltInFunctionSignatures("month");
            result.Length.Should().Be(4); // DateTimeOffset and Date, Nullable and not Nullable
        }

        [TestMethod]
        public void GetBuiltInFunctionSignaturesDateTimeOffsetDay()
        {
            var result = FunctionCallBinder.GetBuiltInFunctionSignatures("day");
            result.Length.Should().Be(4); // DateTimeOffset and Date, Nullable and not Nullable
        }

        [TestMethod]
        public void GetBuiltInFunctionSignatureHour()
        {
            var result = FunctionCallBinder.GetBuiltInFunctionSignatures("hour");
            result.Length.Should().Be(6); // DateTimeOffset, Duration and TimeOfDay, Nullable and not Nullable
        }

        [TestMethod]
        public void GetBuiltInFunctionSignatureMinute()
        {
            var result = FunctionCallBinder.GetBuiltInFunctionSignatures("minute");
            result.Length.Should().Be(6); // DateTimeOffset, Duration and TimeOfDay, Nullable and not Nullable
        }

        [TestMethod]
        public void GetBuiltInFunctionSignatureSecond()
        {
            var result = FunctionCallBinder.GetBuiltInFunctionSignatures("second");
            result.Length.Should().Be(6); // DateTimeOffset, Duration and TimeOfDay, Nullable and not Nullable
        }

        [TestMethod]
        public void GetBuiltInFunctionSignatureFractionalseconds()
        {
            var result = FunctionCallBinder.GetBuiltInFunctionSignatures("fractionalseconds");
            result.Length.Should().Be(4); // DateTimeOffset, TimeOfDay, Nullable and not Nullable
        }

        [TestMethod]
        public void CreateSpatialSignaturesCorrectly()
        {
            var functions = new Dictionary<string, FunctionSignatureWithReturnType[]>(StringComparer.Ordinal);
            BuiltInFunctions.CreateSpatialFunctions(functions);
            FunctionSignatureWithReturnType[] signatures;
            functions.TryGetValue("geo.distance", out signatures);
            signatures.Length.Should().Be(2);
            signatures[0].ReturnType.Definition.FullTypeName().Should().Be(EdmCoreModel.Instance.GetDouble(true).FullName());
            signatures[0].ArgumentTypes[0].Definition.FullTypeName().Should()
                                          .Be(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true).FullName());
            signatures[0].ArgumentTypes[1].Definition.FullTypeName().Should()
                                          .Be(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true).FullName());
            signatures[1].ReturnType.Definition.FullTypeName().Should().Be(EdmCoreModel.Instance.GetDouble(true).FullName());
            signatures[1].ArgumentTypes[0].Definition.FullTypeName().Should()
                                          .Be(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true).FullName());
            signatures[1].ArgumentTypes[1].Definition.FullTypeName().Should()
                                          .Be(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true).FullName());
        }

        [TestMethod]
        public void GetBuiltInFunctionSignaturesShouldThrowOnIncorrectName()
        {
            Action bind = () => FunctionCallBinder.GetBuiltInFunctionSignatures("rarg");
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_UnknownFunction("rarg"));
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void CastShouldFailIfTypeArgumentIsACollection()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("Collection(Fully.Qualified.Namespace.Person)"),  
            };

            Action bind = () => this.functionCallBinder.BindFunctionCall(new FunctionCallToken("cast", args));
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
        }

        [TestMethod]
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

        [TestMethod]
        public void IsOfShouldFailIfTypeArgumentIsACollection()
        {
            QueryToken[] args = new QueryToken[]
            {
                new LiteralToken("Collection(Fully.Qualified.Namespace.Person)"),  
            };

            Action bind = () => this.functionCallBinder.BindFunctionCall(new FunctionCallToken("isof", args));
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.GetBuiltInFunctionSignatures(function.Name);
            createWithMoreThanOneArg.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    function.Name,
                    BuiltInFunctions.BuildFunctionSignatureListDescription(function.Name, signatures)));
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.GetBuiltInFunctionSignatures(functionWithOneArg.Name);
            createWithOneArg.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    functionWithOneArg.Name,
                    BuiltInFunctions.BuildFunctionSignatureListDescription(functionWithOneArg.Name, signatures)));

            signatures = FunctionCallBinder.GetBuiltInFunctionSignatures(functionWithMoreThanTwoArgs.Name);
            createWithMoreThanTwoArgs.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    functionWithMoreThanTwoArgs.Name,
                    BuiltInFunctions.BuildFunctionSignatureListDescription(functionWithMoreThanTwoArgs.Name, signatures)));
        }

        [TestMethod]
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

            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.GetBuiltInFunctionSignatures(geometryPointNonGeometryPolyToken.Name);
            createWithGeometryPointNonGeometryPoly.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geometryPointNonGeometryPolyToken.Name,
                    BuiltInFunctions.BuildFunctionSignatureListDescription(geometryPointNonGeometryPolyToken.Name, signatures)));

            signatures = FunctionCallBinder.GetBuiltInFunctionSignatures(geometryPolyNonGeometryPointToken.Name);
            createWithGeometryPolyNonGeometryPoint.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geometryPolyNonGeometryPointToken.Name,
                    BuiltInFunctions.BuildFunctionSignatureListDescription(geometryPolyNonGeometryPointToken.Name, signatures)));

            signatures = FunctionCallBinder.GetBuiltInFunctionSignatures(geographyPointNonGeographyPolyToken.Name);
            createWithGeographyPointNonGeographyPoly.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geographyPointNonGeographyPolyToken.Name,
                    BuiltInFunctions.BuildFunctionSignatureListDescription(geographyPointNonGeographyPolyToken.Name, signatures)));

            signatures = FunctionCallBinder.GetBuiltInFunctionSignatures(geographyPolyNonGeographyPointToken.Name);
            createWithGeographyPolyNonGeographyPoint.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    geographyPolyNonGeographyPointToken.Name,
                    BuiltInFunctions.BuildFunctionSignatureListDescription(geographyPolyNonGeographyPointToken.Name, signatures)));


            signatures = FunctionCallBinder.GetBuiltInFunctionSignatures(garbageToken.Name);
            createWithGarbage.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    garbageToken.Name,
                    BuiltInFunctions.BuildFunctionSignatureListDescription(garbageToken.Name, signatures)));
        }

        [TestMethod]
        public void FunctionCallBinderShouldMatchHourOnTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("hour", EdmCoreModel.Instance.GetDuration(true));
        }

        [TestMethod]
        public void FunctionCallBinderShouldMatchHourOnNullableTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("hour", EdmCoreModel.Instance.GetDuration(false));
        }

        [TestMethod]
        public void FunctionCallBinderShouldMatchMinuteOnTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("minute", EdmCoreModel.Instance.GetDuration(true));
        }

        [TestMethod]
        public void FunctionCallBinderShouldMatchMinuteOnNullableTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("minute", EdmCoreModel.Instance.GetDuration(false));
        }

        [TestMethod]
        public void FunctionCallBinderShouldMatchSecondOnTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("second", EdmCoreModel.Instance.GetDuration(true));
        }

        [TestMethod]
        public void FunctionCallBinderShouldMatchSecondOnNullableTimespan()
        {
            this.TestUnaryCanonicalFunctionBinding("second", EdmCoreModel.Instance.GetDuration(false));
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void FunctionCallBinderShouldMatchNow()
        {
            this.TestNullaryFunctionBinding("now",
                EdmCoreModel.Instance.GetDateTimeOffset(false));
        }

        [TestMethod]
        public void FunctionCallBinderShouldMatchMaxdatetime()
        {
            this.TestNullaryFunctionBinding("maxdatetime",
                EdmCoreModel.Instance.GetDateTimeOffset(false));
        }

        [TestMethod]
        public void FunctionCallBinderShouldMatchMindatetime()
        {
            this.TestNullaryFunctionBinding("mindatetime",
                EdmCoreModel.Instance.GetDateTimeOffset(false));
        }

        [TestMethod]
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

        [TestMethod]
        public void EndPathAsFunctionCallReturnsSingleValueFunctionCallNodeWhenThereAreNoOverloads()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.HasJob", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasJobFunction());
        }

        [TestMethod]
        public void EndPathAsFunctionCallReturnsCorrectOverload()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.HasDog", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadWithOneParameter());
        }

        [TestMethod]
        public void EndPathAsFunctionCallReturnsFalseIfNoFunctionExists()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Gobbldygook", out functionCallNode).Should().BeFalse();
            functionCallNode.Should().BeNull();
        }

        [TestMethod]
        public void EndPathAsFuctionCallReturnsFalseForServiceOps()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("GetCoolPeople", out functionCallNode).Should().BeFalse();
            functionCallNode.Should().BeNull();
        }

        [TestMethod]
        public void EndPathAsFunctionCallShouldNotThrowsIfOverloadsHaveDifferentReturnTypesButOneOverloadResolvesByParameters()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.HasHat", out functionCallNode);
        }

        [TestMethod]
        public void SingleEntityFunctionHasResultEntitySet()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.GetMyDog", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeSingleEntityFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyDog())
                .And.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void EntityCollectionFunctionHasResultEntitySet()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.AllMyFriendsDogs", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeEntityCollectionFunctionCallNode(HardCodedTestModel.GetFunctionForAllMyFriendsDogs())
                .And.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void EntityCollectionFunctionDoesNotHaveResultEntitySetIfFunctionImportDidntSpecifyOne()
        {
            QueryNode functionCallNode;
            RunBindEndPathAsFunctionCall("Fully.Qualified.Namespace.AllMyFriendsDogs_NoSet", out functionCallNode).Should().BeTrue();
            functionCallNode.As<EntityCollectionFunctionCallNode>().NavigationSource.Should().BeNull();
        }

        [TestMethod]
        public void DottedIdentifierAsFunctionCallReturnsSingleValueFunctionCallNodeWhenThereAreNoOverloads()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("Fully.Qualified.Namespace.HasJob", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasJobFunction());
        }

        [TestMethod]
        public void DottedIdentifierAsFunctionCallReturnsCorrectOverload()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("Fully.Qualified.Namespace.HasDog", out functionCallNode).Should().BeTrue();
            functionCallNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOneParameterOverload());
        }

        [TestMethod]
        public void DottedIdentifierAsFunctionCallReturnsFalseIfNoFunctionExists()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("Gobbl.dygook", out functionCallNode).Should().BeFalse();
            functionCallNode.Should().BeNull();
        }

        [TestMethod]
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

        [TestMethod]
        public void DottedIdentiferAsFunctionCallReturnsFalseForActions()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("AlternateContext.Move", out functionCallNode).Should().BeFalse();
            functionCallNode.Should().BeNull();
        }

        [TestMethod]
        public void DottedIdentifierAsFunctionReturnsFalseForServiceOps()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("AlternateContext.GetCoolPeople", out functionCallNode).Should().BeFalse();
            functionCallNode.Should().BeNull();
        }


        [TestMethod]
        public void BuiltInFunctionsCannotHaveParentNodes()
        {
            FunctionCallToken functionCallToken = new FunctionCallToken("substring", null, new EndPathToken("Name", null));
            Action bindWithNonNullParent = () => this.functionCallBinder.BindFunctionCall(functionCallToken);
            bindWithNonNullParent.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionCallBinder_BuiltInFunctionMustHaveHaveNullParent("substring"));
        }

        [TestMethod]
        public void FunctionsMustHaveEntityBindingTypes()
        {
            FunctionCallToken functionCallToken = new FunctionCallToken(
                "ChangeOwner",
                null,
                new EndPathToken("Color", new InnerPathToken("MyDog", null, null)));
            Action bindWithNonEntityBindingType = () => this.functionCallBinder.BindFunctionCall(functionCallToken);
            bindWithNonEntityBindingType.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionCallBinder_BuiltInFunctionMustHaveHaveNullParent("ChangeOwner"));
        }

        [TestMethod]
        public void DottedIdentifierAsFunctionCallShouldNotThrowsIfOverloadsHaveDifferentReturnTypesButOneResolvesByParameters()
        {
            QueryNode functionCallNode;
            RunDottedIdentifierAsFunctionCall("Fully.Qualified.Namespace.HasHat", out functionCallNode);
        }

        [TestMethod]
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
            BuiltInFunctions.TryGetBuiltInFunction("year", out signatures);
            return signatures;
        }

        internal FunctionSignatureWithReturnType[] GetDuplicateIndexOfFunctionSignatureForTest()
        {
            FunctionSignatureWithReturnType[] signatures = null;
            BuiltInFunctions.TryGetBuiltInFunction("indexof", out signatures);

            return new FunctionSignatureWithReturnType[] { signatures[0], signatures[0] };
        }

        internal FunctionSignatureWithReturnType GetSingleSubstringFunctionSignatureForTest()
        {
            FunctionSignatureWithReturnType[] signatures = null;
            BuiltInFunctions.TryGetBuiltInFunction("substring", out signatures);
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
