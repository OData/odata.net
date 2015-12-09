//---------------------------------------------------------------------
// <copyright file="OpenPropertiesFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// This file contains functional tests for the ODataUriParser Filter and OrderBy methods that exercise support for open properties.
    /// </summary>
    public class OpenPropertiesFunctionalTests
    {
        private const string GenrePropertyName = "Genre";

        [Fact]
        public void ParseFilterWithOpenTypeAndDeclaredPropertyExpectSingleValuePropertyAndConstant()
        {
            var filterQueryNode = ParseFilter("Artist ne 'Emily Carr'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var binaryOperatorNode =
                filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual).And;
            binaryOperatorNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPaintingArtistProp());
            binaryOperatorNode.Right.ShouldBeConstantQueryNode("Emily Carr");
        }

        [Fact]
        public void ParseFilterWithOpenTypeAndOpenPropertyExpectSingleValueOpenPropertyAndConstant()
        {
            var filterQueryNode = ParseFilter("-Genre eq 'Abstract'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var binaryOperatorNode =
                filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            
            binaryOperatorNode.Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String)
                              .And.Source.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Negate)
                              .And.Operand.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);
            
            binaryOperatorNode.Right.ShouldBeConstantQueryNode("Abstract");
        }

        [Fact]
        public void ParseFilterWithOpenPropertyAndUnaryOperatorExpectSingleValueOpenProperty()
        {
            var filterQueryNode = ParseFilter("not Genre", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            filterQueryNode.Expression.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not).And
                .Operand.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);
            filterQueryNode.Expression.TypeReference.Should().BeNull();
        }

        [Fact]
        public void ParseFilterWithClosedTypeAndOpenPropertyExpectException()
        {
            var personType = HardCodedTestModel.GetPersonType();
            Action parse = () => ParseFilter("PhantomProperty ne 'Bug'", HardCodedTestModel.TestModel, personType);

            var expectedMessage =
                ODataErrorStrings.MetadataBinder_PropertyNotDeclared(
                    personType.FullTypeName(),
                    "PhantomProperty");

            parse.ShouldThrow<ODataException>().WithMessage(expectedMessage);
        }

        [Fact]
        public void ParseOrderByWithOpenTypeAndDeclaredPropertyExpectSingleValueProperty()
        {
            var orderByNode = ParseOrderBy("Artist desc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            orderByNode.Direction.Should().Be(OrderByDirection.Descending);
            orderByNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPaintingArtistProp());
        }

        [Fact]
        public void ParseOrderByWithOpenTypeAndOpenPropertyExpectSingleValueOpenProperty()
        {
            var orderByNode = ParseOrderBy("Genre asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            orderByNode.Direction.Should().Be(OrderByDirection.Ascending);
            orderByNode.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);
        }

        [Fact]
        public void ParseFilterWithOpenComplexPropertyOnOpenTypeExpectChainOfOpenPropertyNodesAndConstant()
        {
            var filterQueryNode = ParseFilter("Genre/SubGenre ne 'Modern'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var binaryOperatorNode =
                filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual).And;

            binaryOperatorNode.Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String)
                              .And.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("SubGenre")
                              .And.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);
            
            binaryOperatorNode.Right.ShouldBeConstantQueryNode("Modern");
        }

        [Fact]
        public void ParseFilterWithOpenComplexPropertyOnClosedTypeExpectException()
        {
            var personType = HardCodedTestModel.GetPersonType();
            Action parse = () => ParseFilter("PhantomProperty1/PhantomProperty2 eq 'abc'", HardCodedTestModel.TestModel, personType);

            var expectedMessage =
                ODataErrorStrings.MetadataBinder_PropertyNotDeclared(
                    personType.FullTypeName(),
                    "PhantomProperty1");

            parse.ShouldThrow<ODataException>().WithMessage(expectedMessage);
        }

        [Fact]
        public void ParseFilterWithOpenPropertyOnLambdaParameterExpectOpenPropertyNodeInFunctionArgument()
        {
            var filterQueryNode = ParseFilter("MyPaintings/any(p: startswith(-p/Genre, 'Ab'))", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var functionNode =
                filterQueryNode.Expression.ShouldBeAnyQueryNode().And.Body.As<SingleValueFunctionCallNode>();

            var parameterNode = functionNode.Parameters.First();
            parameterNode.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String)
                         .And.Source.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Negate)
                         .And.Operand.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);
        } 
         
        [Fact]
        public void ParseFilterWithOpenPropertyAfterNavigationPropertyExpectOpenPropertyNode()
        {
            var filterQueryNode = ParseFilter("MyFavoritePainting/Genre eq 'Ab'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                                      .And.Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String)
                                      .And.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);
        }

        [Fact]
        public void OpenPropertyInSingleParameterBuiltInFunction()
        {
            var orderBy = ParseOrderBy("day(Genre)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var functionNode = orderBy.Expression.ShouldBeSingleValueFunctionCallQueryNode("day").And;
            functionNode.Parameters.Single().ShouldBeSingleValueOpenPropertyAccessQueryNode("Genre");
            functionNode.TypeReference.Should().BeNull();
        }

        [Fact]
        public void OpenPropertyInMultiParameterBuiltInFunctionShouldBePromoted()
        {
            var filter = ParseOrderBy("substring(Genre, 5)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var functionNode = filter.Expression.ShouldBeSingleValueFunctionCallQueryNode("substring").And;
            functionNode.Parameters.First().ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetString(true)).
                And.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("Genre");
            functionNode.TypeReference.ShouldBeEquivalentTo(EdmCoreModel.Instance.GetString(true));
        }

        [Fact]
        public void ParseFilterWithOpenPropertyAfterCastExpectException()
        {
            Action parse = () => ParseFilter("Critics/Fully.Qualified.Namespace.Dog/any()", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var expectedMessage =
                ODataErrorStrings.MetadataBinder_HierarchyNotFollowed(
                    HardCodedTestModel.GetDogType().FullTypeName(),
                    "<null>");

            parse.ShouldThrow<ODataException>().WithMessage(expectedMessage);
        }

        [Fact]
        public void ParseFilterWithCollectionOpenPropertyExpectCollectionOpenPropertyAccessQueryNode()
        {
            var filterQueryNode = ParseFilter("Critics/any(p:p eq 0)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            var lambdaNode = filterQueryNode.Expression.ShouldBeAnyQueryNode().And.As<LambdaNode>();

            lambdaNode.Source.ShouldBeCollectionOpenPropertyAccessQueryNode("Critics");
            lambdaNode.Body.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        private static FilterClause ParseFilter(string text, IEdmModel edmModel, IEdmEntityType edmEntityType, IEdmEntitySet edmEntitySet = null)
        {
            return new ODataQueryOptionParser(edmModel, edmEntityType, edmEntitySet, new Dictionary<string, string>() { { "$filter", text } }).ParseFilter();
        }

        private static OrderByClause ParseOrderBy(string text, IEdmModel edmModel, IEdmEntityType edmEntityType, IEdmEntitySet edmEntitySet = null)
        {
            return new ODataQueryOptionParser(edmModel, edmEntityType, edmEntitySet, new Dictionary<string, string>() { { "$orderby", text } }).ParseOrderBy();
        }
    }
}
