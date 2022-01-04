//---------------------------------------------------------------------
// <copyright file="OpenPropertiesFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
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
                filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual);
            binaryOperatorNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPaintingArtistProp());
            binaryOperatorNode.Right.ShouldBeConstantQueryNode("Emily Carr");
        }

        [Fact]
        public void ParseFilterWithOpenTypeAndOpenPropertyExpectSingleValueOpenPropertyAndConstant()
        {
            var filterQueryNode = ParseFilter("-Genre eq 'Abstract'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var binaryOperatorNode =
                filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            
            binaryOperatorNode.Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String)
                              .Source.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Negate)
                              .Operand.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);
            
            binaryOperatorNode.Right.ShouldBeConstantQueryNode("Abstract");
        }

        [Fact]
        public void ParseFilterWithOpenPropertyAndUnaryOperatorExpectSingleValueOpenProperty()
        {
            var filterQueryNode = ParseFilter("not Genre", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            filterQueryNode.Expression.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not)
                .Operand.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);
            Assert.Null(filterQueryNode.Expression.TypeReference);
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

            parse.Throws<ODataException>(expectedMessage);
        }

        [Fact]
        public void ParseOrderByWithOpenTypeAndDeclaredPropertyExpectSingleValueProperty()
        {
            var orderByNode = ParseOrderBy("Artist desc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            Assert.Equal(OrderByDirection.Descending, orderByNode.Direction);
            orderByNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPaintingArtistProp());
        }

        [Fact]
        public void ParseOrderByWithOpenTypeAndOpenPropertyExpectSingleValueOpenProperty()
        {
            var orderByNode = ParseOrderBy("Genre asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            Assert.Equal(OrderByDirection.Ascending, orderByNode.Direction);
            orderByNode.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);
        }

        [Fact]
        public void ParseFilterWithOpenComplexPropertyOnOpenTypeExpectChainOfOpenPropertyNodesAndConstant()
        {
            var filterQueryNode = ParseFilter("Genre/SubGenre ne 'Modern'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var binaryOperatorNode =
                filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual);

            binaryOperatorNode.Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String)
                              .Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("SubGenre")
                              .Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);

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

            parse.Throws<ODataException>(expectedMessage);
        }

        [Fact]
        public void ParseFilterWithOpenPropertyOnLambdaParameterExpectOpenPropertyNodeInFunctionArgument()
        {
            var filterQueryNode = ParseFilter("MyPaintings/any(p: startswith(-p/Genre, 'Ab'))", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var functionNode = Assert.IsType<SingleValueFunctionCallNode>(filterQueryNode.Expression.ShouldBeAnyQueryNode().Body);

            var parameterNode = functionNode.Parameters.First();
            parameterNode.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String)
                         .Source.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Negate)
                         .Operand.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);
        } 
         
        [Fact]
        public void ParseFilterWithOpenPropertyAfterNavigationPropertyExpectOpenPropertyNode()
        {
            var filterQueryNode = ParseFilter("MyFavoritePainting/Genre eq 'Ab'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                                      .Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String)
                                      .Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(GenrePropertyName);
        }

        [Fact]
        public void OpenPropertyInSingleParameterBuiltInFunction()
        {
            var orderBy = ParseOrderBy("day(Genre)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var functionNode = orderBy.Expression.ShouldBeSingleValueFunctionCallQueryNode("day");
            functionNode.Parameters.Single().ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.DateTimeOffset)
                .Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("Genre");
            Assert.Null(functionNode.TypeReference);
        }

        [Fact]
        public void OpenPropertyInMultiParameterBuiltInFunctionShouldBePromoted()
        {
            var filter = ParseOrderBy("substring(Genre, 5)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var functionNode = filter.Expression.ShouldBeSingleValueFunctionCallQueryNode("substring");
            functionNode.Parameters.First().ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetString(true)).
                Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("Genre");
            Assert.True(functionNode.TypeReference.IsEquivalentTo(EdmCoreModel.Instance.GetString(true)));
        }

        [Fact]
        public void ParseFilterWithOpenPropertyAfterCastExpectException()
        {
            Action parse = () => ParseFilter("Critics/Fully.Qualified.Namespace.Dog/any()", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var expectedMessage =
                ODataErrorStrings.MetadataBinder_HierarchyNotFollowed(
                    HardCodedTestModel.GetDogType().FullTypeName(),
                    "<null>");

            parse.Throws<ODataException>(expectedMessage);
        }

        [Fact]
        public void ParseFilterWithCollectionOpenPropertyExpectCollectionOpenPropertyAccessQueryNode()
        {
            var filterQueryNode = ParseFilter("Critics/any(p:p eq 0)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            var anyNode = Assert.IsType<AnyNode>(filterQueryNode.Expression.ShouldBeAnyQueryNode());

            anyNode.Source.ShouldBeCollectionOpenPropertyAccessQueryNode("Critics");
            anyNode.Body.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
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
