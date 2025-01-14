﻿//---------------------------------------------------------------------
// <copyright file="SpatialFuntionalTests.cs" company="Microsoft">
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
using Microsoft.OData.Core;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
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
                ParseFilter("GeographyPoint eq geography'POINT(10 30)'", HardCodedTestModel.TestModel,
                                           HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterWithGeography.Throws<ODataException>(
                Error.Format(SRResources.MetadataBinder_IncompatibleOperandsError,
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
            filterWithGeography.Throws<ODataException>(
                Error.Format(SRResources.MetadataBinder_IncompatibleOperandsError,
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true).FullName(),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true).FullName(),
                    BinaryOperatorKind.Equal));
        }

        [Fact]
        public void DistanceFunctionDefinedForGeography()
        {
            FilterClause filter = ParseFilter("geo.distance(GeographyPoint, geography'POINT(10 30)') eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var bon = Assert.IsType<BinaryOperatorNode>(filter.Expression);
            bon.Left.ShouldBeSingleValueFunctionCallQueryNode("geo.distance");
            bon.Right.ShouldBeConstantQueryNode(2d);
        }

        [Fact]
        public void DistanceFunctionDefinedForGeometry()
        {
            FilterClause filter = ParseFilter("geo.distance(GeometryPoint, geometry'POINT(10 30)') eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var bon = Assert.IsType<BinaryOperatorNode>(filter.Expression);
            bon.Left.ShouldBeSingleValueFunctionCallQueryNode("geo.distance");
            bon.Right.ShouldBeConstantQueryNode(2d);
        }

        [Fact]
        public void DistanceOnlyWorksBetweenTwoPoints()
        {
            string functionName = "geo.distance";
            FunctionSignatureWithReturnType[] signatures;
            BuiltInUriFunctions.TryGetBuiltInFunction(functionName, out signatures);

            Action parseDistanceWithNonPointOperand = () => ParseFilter("geo.distance(GeometryLineString, geometry'POINT(10 30)') eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseDistanceWithNonPointOperand.Throws<ODataException>(Error.Format(SRResources.MetadataBinder_NoApplicableFunctionFound, functionName, UriFunctionsHelper.BuildFunctionSignatureListDescription(functionName, signatures)));
        }

        [Fact]
        public void LengthFunctionWorksInFilter()
        {
            FilterClause filter = ParseFilter("geo.length(GeometryLineString) eq 2.0", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var bon = Assert.IsType<BinaryOperatorNode>(filter.Expression);
            var functionCallNode = bon.Left.ShouldBeSingleValueFunctionCallQueryNode("geo.length");
            var parameter = Assert.Single(functionCallNode.Parameters);
            parameter.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryLineStringProp());
            bon.Right.ShouldBeConstantQueryNode(2.0d);
        }

        [Fact]
        public void LengthFunctionWorksInOrderBy()
        {
            OrderByClause orderBy = ParseOrderBy("geo.length(GeographyLineString)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            orderBy.Expression.ShouldBeSingleValueFunctionCallQueryNode("geo.length");

            var functionCallNode = Assert.IsType<SingleValueFunctionCallNode>(orderBy.Expression);
            var parameter = Assert.Single(functionCallNode.Parameters);
            parameter.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyLineStringProp());
        }

        [Fact]
        public void IntersectsWorksInFilter()
        {
            FilterClause filter = ParseFilter("geo.intersects(GeometryPoint, GeometryPolygon)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            filter.Expression.ShouldBeSingleValueFunctionCallQueryNode("geo.intersects");
            var functionCallNode = Assert.IsType<SingleValueFunctionCallNode>(filter.Expression);
            Assert.Equal(2, functionCallNode.Parameters.Count());
            functionCallNode.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPointProp());
            functionCallNode.Parameters.ElementAt(1).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeometryPolygonProp());
        }

        [Fact]
        public void IntersectsWorksInOrderBy()
        {
            OrderByClause orderby = ParseOrderBy("geo.intersects(GeographyPoint, GeographyPolygon)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            orderby.Expression.ShouldBeSingleValueFunctionCallQueryNode("geo.intersects");
            var functionCallNode = Assert.IsType<SingleValueFunctionCallNode>(orderby.Expression);
            Assert.Equal(2, functionCallNode.Parameters.Count());
            functionCallNode.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyPointProp());
            functionCallNode.Parameters.ElementAt(1).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonGeographyPolygonProp());
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
