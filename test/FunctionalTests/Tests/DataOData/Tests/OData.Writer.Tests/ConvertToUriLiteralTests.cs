//---------------------------------------------------------------------
// <copyright file="ConvertToUriLiteralTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;

    [TestClass, TestCase]
    public class ConvertToUriLiteralTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public AnnotatedPayloadElementToJsonConverter payloadToJsonConverter { get; set; }

        [InjectDependency(IsRequired = true)]
        public EntityModelSchemaToEdmModelConverter emsToEdmConverter { get; set; }

        public ConvertToUriLiteralTests()
        {
            this.Assert = new DefaultAssertionHandler(new TraceLogger());
        }

        /// <summary>
        /// Tests that ODataUtils.ConvertToUriLiteral produces correct complex values.
        /// </summary>
        /* This case is not finished and originally disabled, so just delete it
        //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
        [Ignore]
        [TestMethod, Variation]
        public void ConvertToUriLiteralComplexTest()
        {
            List<ConvertToUriLiteralTestCase> testCases = new List<ConvertToUriLiteralTestCase>();
            EdmModel edmModel = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;
            
            ODataPayloadElementToObjectModelConverter payloadToObjectModelConverter = new ODataPayloadElementToObjectModelConverter(false);

            IEnumerable<ComplexInstance> complexPayloadElements = TestValues.CreateComplexValues(edmModel, true);

            // Build test cases from the ComplexInstances
            foreach (ComplexInstance element in complexPayloadElements)
            {
                object parameter = payloadToObjectModelConverter.Convert(element);

                #region Json Replacement Text (accounts for differences between product and test serializers)

                if (element.StringRepresentation.Equals(" {  }"))
                {
                    element.JsonRepresentation("{}");
                }

                // The test serializer writes __metadata on a complex value that is an element of a collection, but the product serializer does not.
                // Because of this, property4 for payload element described below serializes differently, and we replace the expected value with the version which does not have __metadata on property4.
                if (element.StringRepresentation.Equals("TestModel.ComplexTypeWithCollectionProperties { property0:PrimitiveMultiValue[0], property1:PrimitiveMultiValue[1], property2:PrimitiveMultiValue[3], property3:PrimitiveMultiValue[2], property4:ComplexMultiValue[1] }"))
                {
                    element.JsonRepresentation(
                        "{\"__metadata\":{\"type\":\"TestModel.ComplexTypeWithCollectionProperties\"},\"property0\":{\"__metadata\":{\"type\":\"" + EntityModelUtils.GetCollectionTypeName("Edm.Int32") + "\"},\"results\":[]},\"property1\":{\"__metadata\":{\"type\":\"" + EntityModelUtils.GetCollectionTypeName("Edm.Int32") + "\"},\"results\":[42]},\"property2\":{\"__metadata\":{\"type\":\"" + EntityModelUtils.GetCollectionTypeName("Edm.String") + "\"},\"results\":[\"Bart\",\"Homer\",\"Marge\"]},\"property3\":{\"__metadata\":{\"type\":\"" + EntityModelUtils.GetCollectionTypeName("Edm.Boolean") + "\"},\"results\":[true,false]},\"property4\":{\"__metadata\":{\"type\":\"" + EntityModelUtils.GetCollectionTypeName("TestModel.ComplexTypeWithManyPrimitiveProperties") + "\"},\"results\":[{\"property0\":null,\"property1\":null,\"property2\":null,\"property3\":null,\"property4\":\"stringvalue\",\"property5\":\"\",\"property6\":true,\"property7\":false,\"property8\":33,\"property9\":0,\"property10\":255,\"property11\":22,\"property12\":-22,\"property13\":-128,\"property14\":127,\"property15\":123,\"property16\":-123,\"property17\":-32768,\"property18\":32767,\"property19\":\"123.456\",\"property20\":\"-123.456\",\"property21\":\"-79228162514264337593543950335\",\"property22\":\"79228162514264337593543950335\",\"property23\":42.42,\"property24\":\"Infinity\",\"property25\":\"-Infinity\",\"property26\":\"NaN\",\"property27\":-42,\"property28\":42,\"property29\":\"9223372036854775807\",\"property30\":\"456\",\"property31\":\"-456\",\"property32\":\"-9223372036854775808\",\"property33\":2147483648,\"property34\":42.42,\"property35\":\"Infinity\",\"property36\":\"-Infinity\",\"property37\":\"NaN\",\"property38\":\"2011-02-26T00:00:00\",\"property39\":\"AQID\",\"property40\":\"38cf68c2-4010-4ccc-8922-868217f03ddc\",\"property41\":{\"__metadata\":{\"type\":\"Edm.GeographyPoint\"},\"type\":\"Point\",\"coordinates\":[-100,32],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property42\":{\"__metadata\":{\"type\":\"Edm.GeographyLineString\"},\"type\":\"LineString\",\"coordinates\":[[-110,33.1],[-110,35.97]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property43\":{\"__metadata\":{\"type\":\"Edm.GeographyPolygon\"},\"type\":\"Polygon\",\"coordinates\":[[[-110,33.1],[-110.15,35.97],[87.75,11.45],[-110,33.1]],[[-110,35.97],[-110.15,36.97],[23.18,45.23],[-110,35.97]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property44\":{\"__metadata\":{\"type\":\"Edm.GeographyCollection\"},\"type\":\"GeometryCollection\",\"geometries\":[{\"type\":\"Point\",\"coordinates\":[-12,-19.99]}],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property45\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiPoint\"},\"type\":\"MultiPoint\",\"coordinates\":[[11.2,10.2],[11.6,11.9]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property46\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiLineString\"},\"type\":\"MultiLineString\",\"coordinates\":[[[11.2,10.2],[11.6,11.9]],[[17.2,16.2],[19.6,18.9]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property47\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiPolygon\"},\"type\":\"MultiPolygon\",\"coordinates\":[[[[11.2,10.2],[11.6,11.9],[87.75,11.45],[11.2,10.2]],[[17.2,16.2],[19.6,18.9],[87.75,11.45],[17.2,16.2]]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property48\":{\"__metadata\":{\"type\":\"Edm.GeometryPoint\"},\"type\":\"Point\",\"coordinates\":[32,-10],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property49\":{\"__metadata\":{\"type\":\"Edm.GeometryLineString\"},\"type\":\"LineString\",\"coordinates\":[[33.1,-11.5],[35.97,-11]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property50\":{\"__metadata\":{\"type\":\"Edm.GeometryPolygon\"},\"type\":\"Polygon\",\"coordinates\":[[[33.1,-13.6],[35.97,-11.15],[11.45,87.75],[33.1,-13.6]],[[35.97,-11],[36.97,-11.15],[45.23,23.18],[35.97,-11]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property51\":{\"__metadata\":{\"type\":\"Edm.GeometryCollection\"},\"type\":\"GeometryCollection\",\"geometries\":[{\"type\":\"Point\",\"coordinates\":[-19.99,-12]}],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property52\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiPoint\"},\"type\":\"MultiPoint\",\"coordinates\":[[10.2,11.2],[11.9,11.6]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property53\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiLineString\"},\"type\":\"MultiLineString\",\"coordinates\":[[[10.2,11.2],[11.9,11.6]],[[16.2,17.2],[18.9,19.6]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property54\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiPolygon\"},\"type\":\"MultiPolygon\",\"coordinates\":[[[[10.2,11.2],[11.9,11.6],[11.45,87.75],[10.2,11.2]],[[16.2,17.2],[18.9,19.6],[11.45,87.75],[16.2,17.2]]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property55\":{\"__metadata\":{\"type\":\"Edm.GeographyPoint\"},\"type\":\"Point\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property56\":{\"__metadata\":{\"type\":\"Edm.GeographyLineString\"},\"type\":\"LineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property57\":{\"__metadata\":{\"type\":\"Edm.GeographyPolygon\"},\"type\":\"Polygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property58\":{\"__metadata\":{\"type\":\"Edm.GeographyCollection\"},\"type\":\"GeometryCollection\",\"geometries\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property59\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiPoint\"},\"type\":\"MultiPoint\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property60\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiLineString\"},\"type\":\"MultiLineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property61\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiPolygon\"},\"type\":\"MultiPolygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property62\":{\"__metadata\":{\"type\":\"Edm.GeometryPoint\"},\"type\":\"Point\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property63\":{\"__metadata\":{\"type\":\"Edm.GeometryLineString\"},\"type\":\"LineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property64\":{\"__metadata\":{\"type\":\"Edm.GeometryPolygon\"},\"type\":\"Polygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property65\":{\"__metadata\":{\"type\":\"Edm.GeometryCollection\"},\"type\":\"GeometryCollection\",\"geometries\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property66\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiPoint\"},\"type\":\"MultiPoint\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property67\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiLineString\"},\"type\":\"MultiLineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property68\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiPolygon\"},\"type\":\"MultiPolygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property69\":\"2011-02-26T00:00:00+03:00\",\"property70\":\"2011-02-26T00:00:00+03:00\",\"property71\":\"2011-02-26T00:00:00Z\",\"property72\":\"2011-02-26T00:00:00Z\",\"property73\":\"2011-02-25T21:00:00Z\",\"property74\":\"PT5H\",\"property75\":\"PT0S\",\"property76\":\"-P1DT4H10M40.035S\"}]}}");
                }
                else if (parameter is ODataComplexValue && ((ODataComplexValue)parameter).TypeName == "TestModel.ComplexTypeWithManyPrimitiveProperties")
                {
                    element.JsonRepresentation(
                        "{\"__metadata\":{\"type\":\"TestModel.ComplexTypeWithManyPrimitiveProperties\"},\"property0\":null,\"property1\":null,\"property2\":null,\"property3\":null,\"property4\":\"stringvalue\",\"property5\":\"\",\"property6\":true,\"property7\":false,\"property8\":33,\"property9\":0,\"property10\":255,\"property11\":22,\"property12\":-22,\"property13\":-128,\"property14\":127,\"property15\":123,\"property16\":-123,\"property17\":-32768,\"property18\":32767,\"property19\":\"123.456\",\"property20\":\"-123.456\",\"property21\":\"-79228162514264337593543950335\",\"property22\":\"79228162514264337593543950335\",\"property23\":42.42,\"property24\":\"Infinity\",\"property25\":\"-Infinity\",\"property26\":\"NaN\",\"property27\":-42,\"property28\":42,\"property29\":\"9223372036854775807\",\"property30\":\"456\",\"property31\":\"-456\",\"property32\":\"-9223372036854775808\",\"property33\":2147483648,\"property34\":42.42,\"property35\":\"Infinity\",\"property36\":\"-Infinity\",\"property37\":\"NaN\",\"property38\":\"2011-02-26T00:00:00\",\"property39\":\"AQID\",\"property40\":\"38cf68c2-4010-4ccc-8922-868217f03ddc\",\"property41\":{\"__metadata\":{\"type\":\"Edm.GeographyPoint\"},\"type\":\"Point\",\"coordinates\":[-100,32],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property42\":{\"__metadata\":{\"type\":\"Edm.GeographyLineString\"},\"type\":\"LineString\",\"coordinates\":[[-110,33.1],[-110,35.97]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property43\":{\"__metadata\":{\"type\":\"Edm.GeographyPolygon\"},\"type\":\"Polygon\",\"coordinates\":[[[-110,33.1],[-110.15,35.97],[87.75,11.45],[-110,33.1]],[[-110,35.97],[-110.15,36.97],[23.18,45.23],[-110,35.97]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property44\":{\"__metadata\":{\"type\":\"Edm.GeographyCollection\"},\"type\":\"GeometryCollection\",\"geometries\":[{\"type\":\"Point\",\"coordinates\":[-12,-19.99]}],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property45\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiPoint\"},\"type\":\"MultiPoint\",\"coordinates\":[[11.2,10.2],[11.6,11.9]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property46\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiLineString\"},\"type\":\"MultiLineString\",\"coordinates\":[[[11.2,10.2],[11.6,11.9]],[[17.2,16.2],[19.6,18.9]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property47\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiPolygon\"},\"type\":\"MultiPolygon\",\"coordinates\":[[[[11.2,10.2],[11.6,11.9],[87.75,11.45],[11.2,10.2]],[[17.2,16.2],[19.6,18.9],[87.75,11.45],[17.2,16.2]]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property48\":{\"__metadata\":{\"type\":\"Edm.GeometryPoint\"},\"type\":\"Point\",\"coordinates\":[32,-10],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property49\":{\"__metadata\":{\"type\":\"Edm.GeometryLineString\"},\"type\":\"LineString\",\"coordinates\":[[33.1,-11.5],[35.97,-11]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property50\":{\"__metadata\":{\"type\":\"Edm.GeometryPolygon\"},\"type\":\"Polygon\",\"coordinates\":[[[33.1,-13.6],[35.97,-11.15],[11.45,87.75],[33.1,-13.6]],[[35.97,-11],[36.97,-11.15],[45.23,23.18],[35.97,-11]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property51\":{\"__metadata\":{\"type\":\"Edm.GeometryCollection\"},\"type\":\"GeometryCollection\",\"geometries\":[{\"type\":\"Point\",\"coordinates\":[-19.99,-12]}],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property52\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiPoint\"},\"type\":\"MultiPoint\",\"coordinates\":[[10.2,11.2],[11.9,11.6]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property53\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiLineString\"},\"type\":\"MultiLineString\",\"coordinates\":[[[10.2,11.2],[11.9,11.6]],[[16.2,17.2],[18.9,19.6]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property54\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiPolygon\"},\"type\":\"MultiPolygon\",\"coordinates\":[[[[10.2,11.2],[11.9,11.6],[11.45,87.75],[10.2,11.2]],[[16.2,17.2],[18.9,19.6],[11.45,87.75],[16.2,17.2]]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property55\":{\"__metadata\":{\"type\":\"Edm.GeographyPoint\"},\"type\":\"Point\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property56\":{\"__metadata\":{\"type\":\"Edm.GeographyLineString\"},\"type\":\"LineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property57\":{\"__metadata\":{\"type\":\"Edm.GeographyPolygon\"},\"type\":\"Polygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property58\":{\"__metadata\":{\"type\":\"Edm.GeographyCollection\"},\"type\":\"GeometryCollection\",\"geometries\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property59\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiPoint\"},\"type\":\"MultiPoint\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property60\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiLineString\"},\"type\":\"MultiLineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property61\":{\"__metadata\":{\"type\":\"Edm.GeographyMultiPolygon\"},\"type\":\"MultiPolygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"property62\":{\"__metadata\":{\"type\":\"Edm.GeometryPoint\"},\"type\":\"Point\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property63\":{\"__metadata\":{\"type\":\"Edm.GeometryLineString\"},\"type\":\"LineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property64\":{\"__metadata\":{\"type\":\"Edm.GeometryPolygon\"},\"type\":\"Polygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property65\":{\"__metadata\":{\"type\":\"Edm.GeometryCollection\"},\"type\":\"GeometryCollection\",\"geometries\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property66\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiPoint\"},\"type\":\"MultiPoint\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property67\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiLineString\"},\"type\":\"MultiLineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property68\":{\"__metadata\":{\"type\":\"Edm.GeometryMultiPolygon\"},\"type\":\"MultiPolygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}},\"property69\":\"2011-02-26T00:00:00+03:00\",\"property70\":\"2011-02-26T00:00:00+03:00\",\"property71\":\"2011-02-26T00:00:00Z\",\"property72\":\"2011-02-26T00:00:00Z\",\"property73\":\"2011-02-25T21:00:00Z\",\"property74\":\"PT5H\",\"property75\":\"PT0S\",\"property76\":\"-P1DT4H10M40.035S\"}");
                }
                #endregion

                if (parameter == null)
                {
                    testCases.Add(new ConvertToUriLiteralTestCase()
                    {
                        Parameter = parameter,
                        ExpectedValue = "null"
                    });
                }
                else
                {
                    string expectedValue = payloadToJsonConverter.ConvertToJson(element, string.Empty);

                    testCases.Add(new ConvertToUriLiteralTestCase()
                    {
                        Parameter = parameter,
                        ExpectedValue = expectedValue
                    });
                }
            }

            this.RunTestCases(testCases);
        }
         * */

        /// <summary>
        /// Tests that ODataUtils.ConvertToUriLiteral produces correct primitive values.
        /// </summary>
        [TestMethod, Variation]
        public void ConvertToUriLiteralPrimitiveTest()
        {
            List<ConvertToUriLiteralTestCase> testCases = new List<ConvertToUriLiteralTestCase>();

            #region Primitive
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = string.Empty,
                    ExpectedValue = "''",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = "string with a ' in it",
                    ExpectedValue = "'string with a '' in it'",
                    // use ' to escape ' in uri literals
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = "string with character not suported by uri < > @ \"",
                    ExpectedValue = "'string with character not suported by uri < > @ \"'",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = "string",
                    ExpectedValue = "'string'",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = true,
                    ExpectedValue = "true",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = false,
                    ExpectedValue = "false",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (Decimal)123.456,
                    ExpectedValue = "123.456",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (Decimal)123,
                    ExpectedValue = "123",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (Double)2147483648.5,
                    ExpectedValue = "2147483648.5",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (Double)(-12),
                    ExpectedValue = "-12.0",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = Double.NegativeInfinity,
                    ExpectedValue = "-INF",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = Double.PositiveInfinity,
                    ExpectedValue = "INF",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = Double.NaN,
                    ExpectedValue = "NaN",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (Single)42.42,
                    ExpectedValue = "42.42",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = Single.NegativeInfinity,
                    ExpectedValue = "-INF",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = Single.PositiveInfinity,
                    ExpectedValue = "INF",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = Single.NaN,
                    ExpectedValue = "NaN",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (Byte)33,
                    ExpectedValue = "33",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (SByte)(-22),
                    ExpectedValue = "-22",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (Int16)123,
                    ExpectedValue = "123",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (Int32)(-42),
                    ExpectedValue = "-42",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = Int32.MinValue,
                    ExpectedValue = "-2147483648",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (Int64)Int32.MaxValue,
                    ExpectedValue = "2147483647",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = Int64.MaxValue,
                    ExpectedValue = "9223372036854775807",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new Guid(),
                    ExpectedValue = "00000000-0000-0000-0000-000000000000",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new Guid("{38CF68C2-4010-4CCC-8922-868217F03DDC}"),
                    ExpectedValue = "38cf68c2-4010-4ccc-8922-868217f03ddc",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new byte[] { },
                    ExpectedValue = "binary''",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new byte[] { 0, 255, 16 },
                    ExpectedValue = "binary'AP8Q'",
                });
            #endregion
            this.RunTestCases(testCases);
        }

        /// <summary>
        /// Tests that ODataUtils.ConvertToUriLiteral produces correct primitive values that are supported starting from V3.
        /// </summary>
        [TestMethod, Variation]
        public void ConvertToUriLiteralPrimitiveVersionedTypesTest()
        {
            List<ConvertToUriLiteralTestCase> spatialTestCases = new List<ConvertToUriLiteralTestCase>();
            List<ConvertToUriLiteralTestCase> otherTestCases = new List<ConvertToUriLiteralTestCase>();
            List<ConvertToUriLiteralTestCase> testCases = new List<ConvertToUriLiteralTestCase>();

            #region Other Primitive
            otherTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new DateTimeOffset(),
                    ExpectedValue = "0001-01-01T00:00:00Z",
                });
            otherTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new DateTimeOffset(new DateTime(2011, 2, 4), new TimeSpan(-8, 0, 0)),
                    ExpectedValue = "2011-02-04T00:00:00-08:00",
                });
            otherTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new TimeSpan(),
                    ExpectedValue = "duration'PT0S'",
                });
            otherTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new TimeSpan(2, 12, 4, 5, 55),
                    ExpectedValue = "duration'P2DT12H4M5.055S'",
                });

            #endregion

            #region Spatial Primitives
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeographyFactory.Point(32.5, -100.3).Build(),
                    ExpectedValue = "geography'SRID=4326;POINT (-100.3 32.5)'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeometryFactory.Point().Build(),
                    ExpectedValue = "geometry'SRID=0;POINT EMPTY'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeographyFactory.LineString(1.5, 1.6).LineTo(2.1, 2.5).Build(),
                    ExpectedValue = "geography'SRID=4326;LINESTRING (1.6 1.5, 2.5 2.1)'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeometryFactory.LineString().Build(),
                    ExpectedValue = "geometry'SRID=0;LINESTRING EMPTY'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeographyFactory.Polygon().Ring(33.1, -110.0).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(36.97, -110.15).LineTo(45.23, 23.18).Build(),
                    ExpectedValue = "geography'SRID=4326;POLYGON ((-110 33.1, -110.15 35.97, 87.75 11.45, -110 33.1), (-110 35.97, -110.15 36.97, 23.18 45.23, -110 35.97))'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeometryFactory.Polygon().Build(),
                    ExpectedValue = "geometry'SRID=0;POLYGON EMPTY'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeometryFactory.Collection().Point(-19.99, -12.0).Build(),
                    ExpectedValue = "geometry'SRID=0;GEOMETRYCOLLECTION (POINT (-19.99 -12))'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeographyFactory.Collection().Build(),
                    ExpectedValue = "geography'SRID=4326;GEOMETRYCOLLECTION EMPTY'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeographyFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build(),
                    ExpectedValue = "geography'SRID=4326;MULTIPOINT ((11.2 10.2), (11.6 11.9))'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeometryFactory.MultiPoint().Build(),
                    ExpectedValue = "geometry'SRID=0;MULTIPOINT EMPTY'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeometryFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build(),
                    ExpectedValue = "geometry'SRID=0;MULTILINESTRING ((10.2 11.2, 11.9 11.6), (16.2 17.2, 18.9 19.6))'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeographyFactory.MultiLineString().Build(),
                    ExpectedValue = "geography'SRID=4326;MULTILINESTRING EMPTY'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeographyFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build(),
                    ExpectedValue = "geography'SRID=4326;MULTIPOLYGON (((11.2 10.2, 11.6 11.9, 87.75 11.45, 11.2 10.2), (17.2 16.2, 19.6 18.9, 87.75 11.45, 17.2 16.2)))'",
                });
            spatialTestCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeometryFactory.MultiPolygon().Build(),
                    ExpectedValue = "geometry'SRID=0;MULTIPOLYGON EMPTY'",
                });

            #endregion

            testCases.AddRange(otherTestCases);
            testCases.AddRange(spatialTestCases);

            this.RunTestCases(testCases);
        }

        /// <summary>
        /// Tests that ODataUtils.ConvertToUriLiteral produces correct complex values.
        /// </summary>
        [TestMethod, Variation]
        public void ConvertToUriLiteralMoreComplexTest()
        {
            List<ConvertToUriLiteralTestCase> testCases = new List<ConvertToUriLiteralTestCase>();
            TestModels.BuildTestModel();

            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataComplexValue() { TypeName = "Empty_Complex", Properties = new ODataProperty[0] },
                    ExpectedValue = "{\"@odata.type\":\"#Empty_Complex\"}",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop" } } },
                    ExpectedValue = "{\"@odata.type\":\"#Complex_Type\",\"String_Prop\":\"string_prop\"}",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "Int64_Prop", Value = Int64.MaxValue } } },
                    ExpectedValue = "{\"@odata.type\":\"#Complex_Type\",\"Int64_Prop@odata.type\":\"#Int64\",\"Int64_Prop\":\"9223372036854775807\"}",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "Complex_Prop", Value = new ODataComplexValue() { TypeName = "Sub_Complex_Type", Properties = new[] { new ODataProperty() { Name = "StringProperty", Value = "123" } } } } } },
                    ExpectedValue = "{\"@odata.type\":\"#Complex_Type\",\"Complex_Prop\":{\"@odata.type\":\"#Sub_Complex_Type\",\"StringProperty\":\"123\"}}",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "Collection_Prop", Value = new ODataCollectionValue() { TypeName = EntityModelUtils.GetCollectionTypeName("Edm.String") } } } },
                    ExpectedValue = "{\"@odata.type\":\"#Complex_Type\",\"Collection_Prop@odata.type\":\"#Collection(String)\",\"Collection_Prop\":[]}",
                });
            // Complex with a collection property which has no type name and complex values
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataComplexValue()
                    {
                        TypeName = "Complex_With_Collection_Of_Complex",
                        Properties = new ODataProperty[1]
                            {
                                new ODataProperty()
                                {
                                    Name = "Collection_Of_Complex_Property",
                                    Value = new ODataCollectionValue()
                                    {
                                        Items = new[]
                                        {
                                            new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop1" } } },
                                            new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop2" } } },
                                            new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop3" } } },
                                        }
                                    },
                                }
                            }
                    },
                    ExpectedValue = "{\"@odata.type\":\"#Complex_With_Collection_Of_Complex\",\"Collection_Of_Complex_Property\":[{\"String_Prop\":\"string_prop1\"},{\"String_Prop\":\"string_prop2\"},{\"String_Prop\":\"string_prop3\"}]}",
                });
            // Complex with a collection property which has a type name and complex values
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataComplexValue()
                    {
                        TypeName = "Complex_With_Collection_Of_Complex_Typed",
                        Properties = new ODataProperty[1]
                            {
                                new ODataProperty()
                                {
                                    Name = "Collection_Of_Complex_Property",
                                    Value = new ODataCollectionValue()
                                    {
                                        TypeName = EntityModelUtils.GetCollectionTypeName("Complex_Type"),
                                        Items = new[]
                                        {
                                            new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop1" } } },
                                            new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop2" } } },
                                            new ODataComplexValue() { Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop3" } } },
                                        }
                                    },
                                }
                            }
                    },
                    ExpectedValue = "{\"@odata.type\":\"#Complex_With_Collection_Of_Complex_Typed\",\"Collection_Of_Complex_Property@odata.type\":\"#Collection(Complex_Type)\",\"Collection_Of_Complex_Property\":[{\"String_Prop\":\"string_prop1\"},{\"String_Prop\":\"string_prop2\"},{\"String_Prop\":\"string_prop3\"}]}",
                });

            this.RunTestCases(testCases);
        }

        /// <summary>
        /// Tests that ODataUtils.ConvertToUriLiteral behaves correctly with different ODataVersion for complex values.
        /// </summary>
        [TestMethod, Variation]
        public void ConvertToUriLiteralComplexVersionTest()
        {
            // complex without V3 types
            ConvertToUriLiteralTestCase testCase = new ConvertToUriLiteralTestCase()
            {
                Parameter = new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop" } } },
                ExpectedValue = "{\"@odata.type\":\"#Complex_Type\",\"String_Prop\":\"string_prop\"}",
            };
            this.RunTestCase(testCase);

            testCase = new ConvertToUriLiteralTestCase()
            {
                Parameter = new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "property", Value = new DateTimeOffset() } } },
                ExpectedValue = "{\"@odata.type\":\"#Complex_Type\",\"property@odata.type\":\"#DateTimeOffset\",\"property\":\"0001-01-01T00:00:00Z\"}",
            };
            this.RunTestCase(testCase);

            testCase = new ConvertToUriLiteralTestCase()
            {
                Parameter = new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "property", Value = new TimeSpan() } } },
                ExpectedValue = "{\"@odata.type\":\"#Complex_Type\",\"property@odata.type\":\"#Duration\",\"property\":\"PT0S\"}",
            };
            this.RunTestCase(testCase);

            // complex with Spatial property
            testCase = new ConvertToUriLiteralTestCase()
            {
                Parameter = new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "property", Value = GeographyFactory.Point(32.5, -100.3).Build() } } },
                ExpectedValue = "{\"@odata.type\":\"#Complex_Type\",\"property@odata.type\":\"#GeographyPoint\",\"property\":{\"type\":\"Point\",\"coordinates\":[-100.3,32.5],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
            };
            this.RunTestCase(testCase);

            // complex with collection property
            testCase = new ConvertToUriLiteralTestCase()
            {
                Parameter = new ODataComplexValue()
                {
                    TypeName = "Complex_With_Collection_Of_Primitive",
                    Properties = new ODataProperty[1]
                        {
                            new ODataProperty()
                            {
                                Name = "Collection_Of_Primitive_Property",
                                Value = new ODataCollectionValue()
                                {
                                    Items = new string[] { "PropertyValue1", "PropertyValue2" }
                                },
                            }
                        }
                },
                ExpectedValue = "{\"@odata.type\":\"#Complex_With_Collection_Of_Primitive\",\"Collection_Of_Primitive_Property\":[\"PropertyValue1\",\"PropertyValue2\"]}",
            };
            this.RunTestCase(testCase);
        }

        /// <summary>
        /// Tests that ODataUtils.ConvertToUriLiteral produces correct collection values.
        /// </summary>
        [TestMethod, Variation]
        public void ConvertToUriLiteralCollectionTest()
        {
            List<ConvertToUriLiteralTestCase> testCases = new List<ConvertToUriLiteralTestCase>();

            #region Collection

            // empty collection
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue() { TypeName = EntityModelUtils.GetCollectionTypeName("Edm.String") },
                    ExpectedValue = "[]",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue() { TypeName = EntityModelUtils.GetCollectionTypeName("NameSpace.MyType") },
                    ExpectedValue = "[]",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue() { },
                    ExpectedValue = "[]",
                });

            //// collection with one item
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue() { TypeName = EntityModelUtils.GetCollectionTypeName("Edm.String"), Items = new string[] { "value" } },
                    ExpectedValue = "[\"value\"]",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("NameSpace.MyType"),
                        Items = new[]
                        {
                            new ODataComplexValue() { TypeName = "NameSpace.MyType", Properties = new ODataProperty[1] { new ODataProperty() { Name = "Bool_Prop", Value = false } } },
                        }
                    },
                    ExpectedValue = "[{\"Bool_Prop\":false}]",
                });

            //// collection of spatial type
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue() { TypeName = EntityModelUtils.GetCollectionTypeName("Edm.GeographyPoint"), Items = new[] { GeographyFactory.Point(5.0, -10.0).Build() } },
                    ExpectedValue = "[{\"type\":\"Point\",\"coordinates\":[-10.0,5.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}]",
                });

            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("NameSpace.MyType"),
                        Items = new[]
                        {
                            new ODataComplexValue() { TypeName = "NameSpace.MyType", Properties = new ODataProperty[1] { new ODataProperty() { Name = "Bool_Prop", Value = false } } },
                        }
                    },
                    ExpectedValue = "[{\"Bool_Prop\":false}]",
                });

            // collection with multiple items
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue() { TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Int64"), Items = new Int64[] { Int64.MinValue, Int64.MaxValue } },
                    ExpectedValue = "[\"-9223372036854775808\",\"9223372036854775807\"]",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue() { Items = new Int64[] { Int64.MinValue, Int64.MaxValue } },
                    ExpectedValue = "[\"-9223372036854775808\",\"9223372036854775807\"]",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue()
                    {
                        Items = new[]
                            {
                                new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop1" } } },
                                new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop2" } } },
                                new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop3" } } },
                            }
                    },
                    ExpectedValue = "[{\"String_Prop\":\"string_prop1\"},{\"String_Prop\":\"string_prop2\"},{\"String_Prop\":\"string_prop3\"}]",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("Complex_Type"),
                        Items = new[]
                            {
                                new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop1" } } },
                                new ODataComplexValue() { TypeName = "Complex_Type", Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop2" } } },
                                new ODataComplexValue() { Properties = new ODataProperty[1] { new ODataProperty() { Name = "String_Prop", Value = "string_prop3" } } },
                            }
                    },
                    ExpectedValue = "[{\"String_Prop\":\"string_prop1\"},{\"String_Prop\":\"string_prop2\"},{\"String_Prop\":\"string_prop3\"}]",
                });

            // collection of complex with collection property
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("NameSpace.Complex"),
                        Items = new[]
                            {
                                new ODataComplexValue()
                                {
                                    TypeName = "NameSpace.Complex",
                                    Properties = new ODataProperty[2]
                                    {
                                        new ODataProperty()
                                        {
                                            Name = "Property1",
                                            Value = "Property1_Value",
                                        },
                                        new ODataProperty()
                                        {
                                            Name  = "Property2",
                                            Value = new ODataCollectionValue()
                                            {
                                                TypeName = EntityModelUtils.GetCollectionTypeName("NameSpace.NestCollection"),
                                                Items = new [] {new ODataComplexValue() { TypeName = "NameSpace.NestCollection", Properties = new ODataProperty[1] { new ODataProperty() { Name = "Int_Prop", Value = 12345 } } },},
                                            }
                                        }
                                    }
                            }
                    }
                    },
                    ExpectedValue = "[{\"Property1\":\"Property1_Value\",\"Property2@odata.type\":\"#Collection(NameSpace.NestCollection)\",\"Property2\":[{\"Int_Prop\":12345}]}]",
                });
            #endregion

            this.RunTestCases(testCases);
        }

        /// <summary>
        /// Tests that ODataUtils.ConvertToUriLiteral produces correct null values.
        /// </summary>
        [TestMethod, Variation]
        public void ConvertToUriLiteralNullTest()
        {
            List<ConvertToUriLiteralTestCase> testCases = new List<ConvertToUriLiteralTestCase>();
            IEdmModel edmModel = TestModels.BuildTestModel();

            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = null,
                    Model = edmModel,
                    ExpectedValue = "null",
                });


            this.RunTestCases(testCases);
        }

        /// <summary>
        /// Tests ODataUtils.ConvertToUriLiteral error scenarios
        /// </summary>
        [TestMethod, Variation]
        public void ConvertToUriLiteralErrorTest()
        {
            List<ConvertToUriLiteralTestCase> testCases = new List<ConvertToUriLiteralTestCase>();

            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = '?',
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertToUriLiteralUnsupportedType", "System.Char"),
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataResourceSet(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertToUriLiteralUnsupportedType", "Microsoft.OData.ODataResourceSet"),
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new MemoryStream(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertToUriLiteralUnsupportedType", "System.IO.MemoryStream"),
                });

            this.RunTestCases(testCases);
        }

        /// <summary>
        /// Tests ODataUtils.ConvertToUriLiteral behaves correctly with given model.
        /// </summary>
        [TestMethod, Variation]
        public void ConvertToUriLiteralModelTest()
        {
            List<ConvertToUriLiteralTestCase> testCases = new List<ConvertToUriLiteralTestCase>();

            var edmModel = new EdmModel();
            edmModel.GetUInt32("TestModel", false);
            EdmComplexType addressType = new EdmComplexType("TestModel", "Address");
            addressType.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
            addressType.AddStructuralProperty("Zip", EdmCoreModel.Instance.GetInt32(false));
            edmModel.AddElement(addressType);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            edmModel.AddElement(container);

            #region negative test
            // types that does not exist in model
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataComplexValue() { TypeName = "Empty_Complex", Properties = new ODataProperty[0] },
                    Model = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_UnrecognizedTypeName", "Empty_Complex"),
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataComplexValue()
                    {
                        TypeName = "TestModel.Address",
                        Properties = new ODataProperty[2] { new ODataProperty() { Name = "Street", Value = "street" }, new ODataProperty() { Name = "Zip", Value = "WrongValue" } }
                    },
                    Model = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.String", "True", "Edm.Int32", "False"),
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("TestModel.Address"),
                        Items = new[] { new ODataComplexValue() { TypeName = "TestModel.Address", Properties = new ODataProperty[2] { new ODataProperty() { Name = "Street", Value = "street" }, new ODataProperty() { Name = "Zip", Value = "98052" } } } }
                    },
                    Model = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.String", "True", "Edm.Int32", "False"),
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (UInt16)123,
                    Model = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertToUriLiteralUnsupportedType", "System.UInt16"),
                });
            #endregion

            #region positive test
            // types that exists in model
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataNullValue(),
                    Model = edmModel,
                    ExpectedValue = "null",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataComplexValue()
                    {
                        TypeName = "TestModel.Address",
                        Properties = new ODataProperty[2] { new ODataProperty() { Name = "Street", Value = "street" }, new ODataProperty() { Name = "Zip", Value = 98052 } }
                    },
                    Model = edmModel,
                    ExpectedValue = "{\"@odata.type\":\"#TestModel.Address\",\"Street\":\"street\",\"Zip\":98052}",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("TestModel.Address"),
                        Items = new[] { new ODataComplexValue() { TypeName = "TestModel.Address", Properties = new ODataProperty[2] { new ODataProperty() { Name = "Street", Value = "street" }, new ODataProperty() { Name = "Zip", Value = 98052 } } } }
                    },
                    Model = edmModel,
                    ExpectedValue = "[{\"Street\":\"street\",\"Zip\":98052}]",
                });

            // Do not verify primitive types against model
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = GeographyFactory.Point(32.5, -100.3).Build(),
                    Model = edmModel,
                    ExpectedValue = "geography'SRID=4326;POINT (-100.3 32.5)'",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = new ODataCollectionValue() { TypeName = EntityModelUtils.GetCollectionTypeName("Edm.String"), Items = new string[] { "value" } },
                    Model = edmModel,
                    ExpectedValue = "[\"value\"]",
                });
            testCases.Add(
                new ConvertToUriLiteralTestCase()
                {
                    Parameter = (UInt32)123,
                    Model = edmModel,
                    ExpectedValue = "123",
                });
            #endregion

            this.RunTestCases(testCases);
        }

        private void RunTestCases(List<ConvertToUriLiteralTestCase> testCases, ODataVersion version = ODataVersion.V4)
        {

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.RunTestCase(testCase, version, testCase.Model);
                });
        }

        private void RunTestCase(ConvertToUriLiteralTestCase testCase, ODataVersion version = ODataVersion.V4, IEdmModel model = null)
        {
            // Negative Test
            if (testCase.ExpectedException != null)
            {
                TestExceptionUtils.ExpectedException(
                    this.Assert,
                    () =>
                    {
                        ODataUriUtils.ConvertToUriLiteral(testCase.Parameter, version, model);
                    },
                    testCase.ExpectedException,
                    this.ExceptionVerifier
                    );
            }
            else // Positive Test
            {
                string actualValue = ODataUriUtils.ConvertToUriLiteral(testCase.Parameter, version, model);
                object expectedValue = testCase.ExpectedValues;
                if (expectedValue == null)
                {
                    expectedValue = testCase.ExpectedValue;
                }

                if (!expectedValue.Equals(actualValue))
                {
                    throw new ODataTestException(string.Format(
                        "Different JSON.{0}Expected:{0}{1}{0}Actual:{0}{2}{0}",
                        Environment.NewLine,
                        expectedValue,
                        actualValue));
                }
            }
        }

        /// <summary>
        /// If any one of the values in the list are equal to the value passed in .Equals, it returns true.
        /// </summary>
        private class ExpectedValueList : List<object>
        {
            public ExpectedValueList(params object[] values)
            {
                foreach (object o in values)
                {
                    this.Add(o);
                }
            }

            public override bool Equals(object obj)
            {
                foreach (object t in this)
                {
                    if (t != null && t.Equals(obj))
                    {
                        return true;
                    }
                }

                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                foreach (object o in this)
                {
                    builder.Append("<");
                    builder.Append(o);
                    builder.Append(">");
                    builder.Append(" OR ");
                }

                builder.Remove(builder.Length - 3, 3);
                return builder.ToString();
            }
        }

        private class ConvertToUriLiteralTestCase
        {
            public object Parameter { get; set; }
            public string ExpectedValue { get; set; }
            public ExpectedValueList ExpectedValues { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public IEdmModel Model { get; set; }
        }
    }
}
