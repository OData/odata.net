//---------------------------------------------------------------------
// <copyright file="SpatialFuntionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Test to make sure that Spatial types fail gracefully.
    /// </summary>
    public class SpatialTests
    {
        [Fact]
        public void EqualityNotDefinedForGeography()
        {
            Action filterWithGeography =
                () =>
                ParseFilter("LocationGeographyPoint eq geography'POINT(10 30)'", HardCodedTestModel.TestModel,
                                           HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterWithGeography.ShouldThrow<ODataException>(
                ODataErrorStrings.MetadataBinder_IncompatibleOperandsError(
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true).FullName(),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true).FullName(),
                    BinaryOperatorKind.Equal));
        }

        [Fact]
        public void EqualityNotDefinedForGeometry()
        {
            Action filterWithGeography =
                () =>
                ParseFilter("GeometryPoint eq geometry'POINT(10 30)'", HardCodedTestModel.TestModel,
                                           HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterWithGeography.ShouldThrow<ODataException>(
                ODataErrorStrings.MetadataBinder_IncompatibleOperandsError(
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true).FullName(),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true).FullName(),
                    BinaryOperatorKind.Equal));
        }

        [Fact]
        public void DistanceFunctionDefinedForGeography()
        {
            FilterClause filter = ParseFilter("geo.distance(GeographyPoint, geography'POINT(10 30)') eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().Left.ShouldBeSingleValueFunctionCallQueryNode("geo.distance");
            filter.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(2d);
        }

        [Fact]
        public void DistanceFunctionDefinedForGeometry()
        {
            FilterClause filter = ParseFilter("geo.distance(GeometryPoint, geometry'POINT(10 30)') eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().Left.ShouldBeSingleValueFunctionCallQueryNode("geo.distance");
            filter.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(2d);
        }

        [Fact]
        public void DistanceOnlyWorksBetweenTwoPoints()
        {
            string functionName = "geo.distance";
            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.GetBuiltInFunctionSignatures(functionName);
            Action parseDistanceWithNonPointOperand = () => ParseFilter("geo.distance(LocationGeometryLine, geometry'POINT(10 30)') eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseDistanceWithNonPointOperand.ShouldThrow<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(functionName, BuiltInFunctions.BuildFunctionSignatureListDescription(functionName, signatures)));
        }

        [Fact]
        public void LengthFunctionWorksInFilter()
        {
            FilterClause filter = ParseFilter("geo.length(GeometryLineString) eq 2.0", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().Left.ShouldBeSingleValueFunctionCallQueryNode("geo.length");
            filter.Expression.As<BinaryOperatorNode>().Left.As<SingleValueFunctionCallNode>().Parameters.Count().Should().Be(1);
            filter.Expression.As<BinaryOperatorNode>().Left.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryLineStringProp());
            filter.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(2.0d);
        }

        [Fact]
        public void LengthFunctionWorksInOrderBy()
        {
            OrderByClause orderBy = ParseOrderBy("geo.length(GeographyLineString)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            orderBy.Expression.ShouldBeSingleValueFunctionCallQueryNode("geo.length");
            orderBy.Expression.As<SingleValueFunctionCallNode>().Parameters.Count().Should().Be(1);
            orderBy.Expression.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyLineStringProp());
        }

        [Fact]
        public void IntersectsWorksInFilter()
        {
            FilterClause filter = ParseFilter("geo.intersects(GeometryPoint, GeometryPolygon)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            filter.Expression.ShouldBeSingleValueFunctionCallQueryNode("geo.intersects");
            filter.Expression.As<SingleValueFunctionCallNode>().Parameters.Count().Should().Be(2);
            filter.Expression.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPointProp());
            filter.Expression.As<SingleValueFunctionCallNode>().Parameters.ElementAt(1).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPolygonProp());
        }

        [Fact]
        public void IntersectsWorksInOrderBy()
        {
            OrderByClause orderby = ParseOrderBy("geo.intersects(GeographyPoint, GeographyPolygon)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            orderby.Expression.ShouldBeSingleValueFunctionCallQueryNode("geo.intersects");
            orderby.Expression.As<SingleValueFunctionCallNode>().Parameters.Count().Should().Be(2);
            orderby.Expression.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyPointProp());
            orderby.Expression.As<SingleValueFunctionCallNode>().Parameters.ElementAt(1).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyPolygonProp());
        }

        [Fact]
        public void GeographyShouldWorkInOrderBy()
        {
            OrderByClause orderBy = ParseOrderBy("GeographyPoint", HardCodedTestModel.TestModel,
                                                                HardCodedTestModel.GetPersonType());
            orderBy.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyPointProp());
        }

        [Fact]
        public void GeometryShouldWorkInOrderBy()
        {
            OrderByClause orderBy = ParseOrderBy("GeometryPoint", HardCodedTestModel.TestModel,
                                                                HardCodedTestModel.GetPersonType());
            orderBy.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPointProp());
        }

        [Fact]
        public void GeographyDistanceFunctionShouldWorkInOrderBy()
        {
            OrderByClause orderBy =
                ParseOrderBy("geo.distance(GeographyPoint, geography'POINT(10 30)')",
                                            HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            orderBy.Expression.ShouldBeSingleValueFunctionCallQueryNode("geo.distance");
        }

        [Fact]
        public void GeometryDistanceFunctionShouldWorkInOrderBy()
        {
            OrderByClause orderBy =
                ParseOrderBy("geo.distance(GeometryPoint, geometry'POINT(10 30)')",
                                            HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            orderBy.Expression.ShouldBeSingleValueFunctionCallQueryNode("geo.distance");
        }

        private static FilterClause ParseFilter(string text, IEdmModel edmModel, IEdmEntityType edmEntityType, IEdmEntitySet edmEntitySet = null)
        {
            return new ODataQueryOptionParser(edmModel, edmEntityType, edmEntitySet, new Dictionary<string, string>() {{"$filter", text}}).ParseFilter();
        }

        private static OrderByClause ParseOrderBy(string text, IEdmModel edmModel, IEdmEntityType edmEntityType, IEdmEntitySet edmEntitySet = null)
        {
            return new ODataQueryOptionParser(edmModel, edmEntityType, edmEntitySet, new Dictionary<string, string>() { { "$orderby", text } }).ParseOrderBy();
        }
    }
}
