//---------------------------------------------------------------------
// <copyright file="ConvertFromUriLiteralTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Spatial;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    [TestClass, TestCase]
    public class ConvertFromUriLiteralTests : ODataReaderTestCase
    {
        #region  Taupo Configuration

        [InjectDependency(IsRequired = true)]
        public ObjectModelToPayloadElementConverter objectModelToPayloadConverter { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataPayloadElementComparer payloadComparer { get; set; }

        [InjectDependency(IsRequired = true)]
        public AnnotatedPayloadElementToJsonLightConverter payloadToJsonLightConverter { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataPayloadElementComparer oDataPayloadElementComparer { get; set; }

        private ODataVersion[] VersionConfigurations = new[] { ODataVersion.V4 };

        #endregion

        #region Format Agnostic

        private const string DefaultNamespaceName = "TestModel";
        private static readonly IEdmStringTypeReference StringNullableTypeRef = EdmCoreModel.Instance.GetString(isNullable: true);
        private static readonly IEdmPrimitiveTypeReference Int32TypeRef = EdmCoreModel.Instance.GetInt32(isNullable: false);
        private static readonly IEdmPrimitiveTypeReference Int32NullableTypeRef = EdmCoreModel.Instance.GetInt32(isNullable: true);

        /// <summary>
        /// Tests that ODataUriUtils.ConvertFromUriLiteral is able to deserialize primitive values from Uri properly.
        /// </summary>
        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriPrimitiveLiteralsTest()
        {
            List<ConvertFromUriLiteralTestCase> testCases = new List<ConvertFromUriLiteralTestCase>();

            IEdmModel edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            #region Primitive without TypeReference and Model
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "''",
                    ExpectedValue = "",
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "'string'",
                    ExpectedValue = "string",
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "'string with a '' in it'",
                    ExpectedValue = "string with a ' in it",
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "'string with back to back '''' in it'",
                    ExpectedValue = "string with back to back '' in it",
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "'string with character not suported by uri < > @ \"'",
                    ExpectedValue = "string with character not suported by uri < > @ \"",
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "true",
                    ExpectedValue = true,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "false",
                    ExpectedValue = false,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "123.456m",
                    ExpectedValue = (Decimal)123.456,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "123M",
                    ExpectedValue = (Decimal)123,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "2147483648.5D",
                    ExpectedValue = (Double)2147483648.5,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "-12d",
                    ExpectedValue = (Double)(-12),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "-INF",
                    ExpectedValue = Double.NegativeInfinity,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "INF",
                    ExpectedValue = Double.PositiveInfinity,
                });
            // TODO: Need to use the Astoria behavior knob.
            // Right now we act like Astoria because the ExpressionLexer does.
            // If not Astoria, the d/D is needed. If Astoria, if CANNOT be there. Make change in Lexer.
            //testCases.Add(
            //    new ConvertFromUriLiteralTestCase()
            //    {
            //        Parameter = "-INFd",
            //        ExpectedValue = Double.NegativeInfinity,
            //    });
            //testCases.Add(
            //    new ConvertFromUriLiteralTestCase()
            //    {
            //        Parameter = "INFD",
            //        ExpectedValue = Double.PositiveInfinity,
            //    });
            //testCases.Add(
            //    new ConvertFromUriLiteralTestCase()
            //    {
            //        Parameter = "NaND",
            //        ExpectedValue = Double.NaN,
            //    });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "42.42f",
                    ExpectedValue = (Single)42.42,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "-INFf",
                    ExpectedValue = Single.NegativeInfinity,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "INFF",
                    ExpectedValue = Single.PositiveInfinity,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "NaNf",
                    ExpectedValue = Single.NaN,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "33",
                    ExpectedValue = (Int32)33,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "-42",
                    ExpectedValue = (Int32)(-42),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "-2147483648",
                    ExpectedValue = Int32.MinValue,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "9223372036854775807L",
                    ExpectedValue = Int64.MaxValue,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "42.42e+3",
                    ExpectedValue = (float)42420,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "9223372E-3",
                    ExpectedValue = (float)9223.372,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "38cf68c2-4010-4ccc-8922-868217f03ddc",
                    ExpectedValue = new Guid("{38CF68C2-4010-4CCC-8922-868217F03DDC}"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "binary'AP8Q'",
                    ExpectedValue = new byte[] { 0, 255, 16 },
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "binary''",
                    ExpectedValue = new byte[] { },
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "binary'AP8Q'",
                    ExpectedValue = new byte[] { 0, 255, 16 },
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "null",
                    ExpectedValue = new ODataNullValue(),
                });
            testCases.Add(
               new ConvertFromUriLiteralTestCase()
               {
                   Parameter = "2011-02-04Z",
                   ExpectedValue = new DateTimeOffset(new DateTime(2011, 02, 04, 00, 00, 00, DateTimeKind.Utc)),
               });
            testCases.Add(
               new ConvertFromUriLiteralTestCase()
               {
                   // datetimeoffset without seconds
                   Parameter = "2011-02-04T12:43Z",
                   ExpectedValue = new DateTimeOffset(new DateTime(2011, 02, 04, 12, 43, 00, DateTimeKind.Utc)),
               });
            testCases.Add(
               new ConvertFromUriLiteralTestCase()
               {
                   Parameter = "0001-01-01T00:00:00Z",
                   ExpectedValue = DateTimeOffset.MinValue
               });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "2011-02-04T09:00:00+08:00",
                    ExpectedValue = new DateTimeOffset(new DateTime(2011, 02, 04, 09, 00, 00), new TimeSpan(8, 0, 0)),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "duration'PT0S'",
                    ExpectedValue = new TimeSpan(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "duration'P2DT12H4M5.055S'",
                    ExpectedValue = new TimeSpan(2, 12, 4, 5, 55),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;POINT (-100.3 32.5)'",
                    ExpectedValue = GeographyFactory.Point(32.5, -100.3).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;LINESTRING (10.0 10.0, 20.0 20.0, 10.0 40.0)'",
                    ExpectedValue = GeographyFactory.LineString(10.0, 10.0).LineTo(20.0, 20.0).LineTo(40.0, 10.0).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;POLYGON ((7.0 5.0, 10.0 10.0, 20.0 10.0, 7.0 5.0))'",
                    ExpectedValue = GeographyFactory.Polygon().Ring(5.0, 7.0).LineTo(10.0, 10.0).LineTo(10.0, 20.0).LineTo(5.0, 7.0).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;MULTIPOINT ((1.5 1.0), (2.5 2.0))'",
                    ExpectedValue = GeographyFactory.MultiPoint().Point(1.0, 1.5).Point(2.0, 2.5).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;MULTILINESTRING  ((10.5 10.5, 20.5 20.5), (40.5 40.5, 30.5 30.5, 40.5 20.5))'",
                    ExpectedValue = GeographyFactory.MultiLineString().LineString(10.5, 10.5).LineTo(20.5, 20.5).LineString(40.5, 40.5).LineTo(30.5, 30.5).LineTo(20.5, 40.5).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;MULTIPOLYGON (((30 20, 10 40, 45 40, 30 20)), ((15 5, 40 10, 10 20, 5 10, 15 5)))'",
                    ExpectedValue = GeographyFactory.MultiPolygon().Polygon().Ring(20, 30).LineTo(40, 10).LineTo(40, 45).LineTo(20, 30).Polygon().Ring(5, 15).LineTo(10, 40).LineTo(20, 10).LineTo(10, 5).LineTo(5, 15).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;POINT (32.5 -100.354)'",
                    ExpectedValue = GeometryFactory.Point(32.5, -100.354).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;LINESTRING (10.0 10.0, 20.0 20.0, 40.0 10.0)'",
                    ExpectedValue = GeometryFactory.LineString(10.0, 10.0).LineTo(20.0, 20.0).LineTo(40.0, 10.0).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;POLYGON ((5.0 7.0, 10.0 10.0, 10.0 20.0, 5.0 7.0))'",
                    ExpectedValue = GeometryFactory.Polygon().Ring(5.0, 7.0).LineTo(10.0, 10.0).LineTo(10.0, 20.0).LineTo(5.0, 7.0).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;MULTIPOINT ((1.5 1.0), (2.5 2.0))'",
                    ExpectedValue = GeometryFactory.MultiPoint().Point(1.5, 1.0).Point(2.5, 2.0).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;MULTILINESTRING  ((10.5 10.5, 20.5 20.5), (40.5 40.5, 30.5 30.5, 20.5 40.5))'",
                    ExpectedValue = GeometryFactory.MultiLineString().LineString(10.5, 10.5).LineTo(20.5, 20.5).LineString(40.5, 40.5).LineTo(30.5, 30.5).LineTo(20.5, 40.5).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;MULTIPOLYGON (((20 30, 40 10, 40 45, 20 30)), ((5 15, 10 40, 20 10, 10 5, 5 15)))'",
                    ExpectedValue = GeometryFactory.MultiPolygon().Polygon().Ring(20, 30).LineTo(40, 10).LineTo(40, 45).LineTo(20, 30).Polygon().Ring(5, 15).LineTo(10, 40).LineTo(20, 10).LineTo(10, 5).LineTo(5, 15).Build(),
                });
            #endregion

            #region Primitive With TypeReference and Model
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "'string'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.String", false),
                    ExpectedValue = "string",
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "true",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Boolean", false),
                    ExpectedValue = true,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "123.456m",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Decimal", false),
                    ExpectedValue = (Decimal)123.456,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "2147483648.5D",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Double", false),
                    ExpectedValue = (Double)2147483648.5,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "2147483648.5e-10",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Double", false),
                    ExpectedValue = (Double)0.21474836485,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "-INF",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Double", false),
                    ExpectedValue = Double.NegativeInfinity,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "42.42f",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Single", false),
                    ExpectedValue = (Single)42.42,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "NaNf",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Single", false),
                    ExpectedValue = Single.NaN,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "33",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int32", false),
                    ExpectedValue = (Int32)33,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "9223372036854775807L",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int64", false),
                    ExpectedValue = Int64.MaxValue,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "38cf68c2-1123-4ccc-8922-868217f03ddc",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Guid", false),
                    ExpectedValue = new Guid("{38CF68C2-1123-4CCC-8922-868217F03DDC}"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "binary'AP8Q'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Binary", false),
                    ExpectedValue = new byte[] { 0, 255, 16 },
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "5",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Double", false),
                    ExpectedValue = 5.0d,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "5.0f",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Double", false),
                    ExpectedValue = 5.0d,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "5L",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Double", false),
                    ExpectedValue = 5.0d,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "5",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Single", false),
                    ExpectedValue = 5.0f,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "5L",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Single", false),
                    ExpectedValue = 5.0f,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "5L",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Decimal", false),
                    ExpectedValue = 5.0M,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "5",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int64", false),
                    ExpectedValue = 5L,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "null",
                    Model = edmModel,
                    TypeReference = EdmCoreModel.Instance.GetInt32(true),
                    ExpectedValue = new ODataNullValue(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "5",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Decimal", false),
                    ExpectedValue = 5M,
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "5L",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Decimal", false),
                    ExpectedValue = 5M,
                });

            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "2011-02-04T09:00:00+08:00",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.DateTimeOffset", false),
                    ExpectedValue = new DateTimeOffset(new DateTime(2011, 02, 04, 09, 00, 00), new TimeSpan(8, 0, 0)),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    // datetimeoffset without seconds
                    Parameter = "2011-02-04T09:15+08:00",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.DateTimeOffset", false),
                    ExpectedValue = new DateTimeOffset(new DateTime(2011, 02, 04, 09, 15, 00), new TimeSpan(8, 0, 0)),
                });
            testCases.Add(
               new ConvertFromUriLiteralTestCase()
               {
                   Parameter = "0001-01-01T00:00:00Z",
                   Model = edmModel,
                   TypeReference = edmModel.ResolveTypeReference("Edm.DateTimeOffset", false),
                   ExpectedValue = DateTimeOffset.MinValue,
               });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "duration'P2DT12H4M5.055S'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Duration", false),
                    ExpectedValue = new TimeSpan(2, 12, 4, 5, 55),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "duration'PT0S'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Duration", false),
                    ExpectedValue = new TimeSpan(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;POINT (-100.3 32.5)'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeographyPoint", true),
                    ExpectedValue = GeographyFactory.Point(32.5, -100.3).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;LINESTRING (10.0 10.0, 20.0 20.0, 10.0 40.0)'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeographyLineString", true),
                    ExpectedValue = GeographyFactory.LineString(10.0, 10.0).LineTo(20.0, 20.0).LineTo(40.0, 10.0).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;POLYGON ((7.0 5.0, 10.0 10.0, 20.0 10.0, 7.0 5.0))'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeographyPolygon", true),
                    ExpectedValue = GeographyFactory.Polygon().Ring(5.0, 7.0).LineTo(10.0, 10.0).LineTo(10.0, 20.0).LineTo(5.0, 7.0).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;MULTIPOINT ((1.5 1.0), (2.5 2.0))'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeographyMultiPoint", true),
                    ExpectedValue = GeographyFactory.MultiPoint().Point(1.0, 1.5).Point(2.0, 2.5).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;MULTILINESTRING  ((10.5 10.5, 20.5 20.5), (40.5 40.5, 30.5 30.5, 40.5 20.5))'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeographyMultiLineString", true),
                    ExpectedValue = GeographyFactory.MultiLineString().LineString(10.5, 10.5).LineTo(20.5, 20.5).LineString(40.5, 40.5).LineTo(30.5, 30.5).LineTo(20.5, 40.5).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geography'SRID=4326;MULTIPOLYGON (((30 20, 10 40, 45 40, 30 20)), ((15 5, 40 10, 10 20, 5 10, 15 5)))'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeographyMultiPolygon", true),
                    ExpectedValue = GeographyFactory.MultiPolygon().Polygon().Ring(20, 30).LineTo(40, 10).LineTo(40, 45).LineTo(20, 30).Polygon().Ring(5, 15).LineTo(10, 40).LineTo(20, 10).LineTo(10, 5).LineTo(5, 15).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;POINT (32.5 -100.354)'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeometryPoint", true),
                    ExpectedValue = GeometryFactory.Point(32.5, -100.354).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;LINESTRING (10.0 10.0, 20.0 20.0, 40.0 10.0)'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeometryLineString", true),
                    ExpectedValue = GeometryFactory.LineString(10.0, 10.0).LineTo(20.0, 20.0).LineTo(40.0, 10.0).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;POLYGON ((5.0 7.0, 10.0 10.0, 10.0 20.0, 5.0 7.0))'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeometryPolygon", true),
                    ExpectedValue = GeometryFactory.Polygon().Ring(5.0, 7.0).LineTo(10.0, 10.0).LineTo(10.0, 20.0).LineTo(5.0, 7.0).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;MULTIPOINT ((1.5 1.0), (2.5 2.0))'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeometryMultiPoint", true),
                    ExpectedValue = GeometryFactory.MultiPoint().Point(1.5, 1.0).Point(2.5, 2.0).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;MULTILINESTRING  ((10.5 10.5, 20.5 20.5), (40.5 40.5, 30.5 30.5, 20.5 40.5))'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeometryMultiLineString", true),
                    ExpectedValue = GeometryFactory.MultiLineString().LineString(10.5, 10.5).LineTo(20.5, 20.5).LineString(40.5, 40.5).LineTo(30.5, 30.5).LineTo(20.5, 40.5).Build(),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "geometry'SRID=0;MULTIPOLYGON (((20 30, 40 10, 40 45, 20 30)), ((5 15, 10 40, 20 10, 10 5, 5 15)))'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.GeometryMultiPolygon", true),
                    ExpectedValue = GeometryFactory.MultiPolygon().Polygon().Ring(20, 30).LineTo(40, 10).LineTo(40, 45).LineTo(20, 30).Polygon().Ring(5, 15).LineTo(10, 40).LineTo(20, 10).LineTo(10, 5).LineTo(5, 15).Build(),
                });

            #endregion

            this.RunTestCases(testCases);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromGuidStartingFromDigitTest()
        {
            List<ConvertFromUriLiteralTestCase> testCases = new List<ConvertFromUriLiteralTestCase>();

            IEdmModel edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "38cf68c2-4010-4ccc-8922-868217f03ddc",
                    ExpectedValue = new Guid("{38CF68C2-4010-4CCC-8922-868217F03DDC}"),
                });

            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "38cf68c2-1123-4ccc-8922-868217f03ddc",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Guid", false),
                    ExpectedValue = new Guid("{38CF68C2-1123-4CCC-8922-868217F03DDC}"),
                });

            this.RunTestCases(testCases);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromGuidStartingWithLetterTest()
        {
            List<ConvertFromUriLiteralTestCase> testCases = new List<ConvertFromUriLiteralTestCase>();

            IEdmModel edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            testCases.Add(
                    new ConvertFromUriLiteralTestCase()
                    {
                        Parameter = "cf3868c2-4010-4ccc-8922-868217f03ddc",
                        ExpectedValue = new Guid("{CF3868C2-4010-4CCC-8922-868217F03DDC}"),
                    });

            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "cf3868c2-1123-4ccc-8922-868217f03ddc",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Guid", false),
                    ExpectedValue = new Guid("{CF3868C2-1123-4CCC-8922-868217F03DDC}"),
                });

            this.RunTestCases(testCases);
        }

        /// <summary>
        /// Tests the scenarios that ODataUriUtils.ConvertFromUriLiteral is expected to fail.
        /// </summary>
        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriFormatAgnosticErrorsTest()
        {
            List<ConvertFromUriLiteralTestCase> testCases = new List<ConvertFromUriLiteralTestCase>();

            IEdmModel edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            #region Bad Literals
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = (String)null,
                    ExpectedException = new ExpectedException(typeof(ArgumentNullException)),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "binar'00FF00'",
                    ExpectedException = ODataExpectedExceptions.ODataException("ExpressionLexer_ExpectedLiteralToken", "binar'00FF00'"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "binary\"00FF00\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ExpressionLexer_ExpectedLiteralToken", "binary"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "date'2011-02-04T00:00:00'",
                    ExpectedException = ODataExpectedExceptions.ODataException("ExpressionLexer_ExpectedLiteralToken", "date'2011-02-04T00:00:00'"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "{\"property1\":123.456abc}",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MissingComma", "Object"),
                });
            #endregion

            #region Missing Model TypeReference
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "{}",
                    TypeReference = edmModel.ResolveTypeReference("TestModel.Address", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeRefWithoutModel"),
                });
            #endregion

            #region Value TypeReference Mismatch
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "76.3",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.String", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.String", "76.3"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "76.3d",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int16", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.Int16", "76.3d"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "76.3f",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int16", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.Int16", "76.3f"),
                });
            testCases.Add(


                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "76.3m",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int16", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.Int16", "76.3m"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "76.3d",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int32", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.Int32", "76.3d"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "76.3f",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int32", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.Int32", "76.3f"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "76.3m",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int32", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.Int32", "76.3m"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "76.3d",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int64", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.Int64", "76.3d"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "76.3f",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int64", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.Int64", "76.3f"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "76.3m",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Int64", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.Int64", "76.3m"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "'string'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Double", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.Double", "'string'"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "true",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.String", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.String", "true"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "2011-02-04T00:00:00-08:00",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.Double", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure", "Edm.Double", "2011-02-04T00:00:00-08:00"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "null''",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.String", true),
                    ExpectedException = ODataExpectedExceptions.ODataException("ExpressionLexer_SyntaxError", "4", "null''"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "null' '",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.String", true),
                    ExpectedException = ODataExpectedExceptions.ODataException("ExpressionLexer_SyntaxError", "4", "null' '"),
                });
            testCases.Add(
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "null'Edm.S'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.String", true),
                    ExpectedException = ODataExpectedExceptions.ODataException("ExpressionLexer_SyntaxError", "4", "null'Edm.S'"),
                });
            testCases.Add( // Not nullable but typed null sent in
                new ConvertFromUriLiteralTestCase()
                {
                    Parameter = "null'Edm.String'",
                    Model = edmModel,
                    TypeReference = edmModel.ResolveTypeReference("Edm.String", false),
                    ExpectedException = ODataExpectedExceptions.ODataException("ExpressionLexer_SyntaxError", "4", "null'Edm.String'"),
                });
            #endregion

            this.RunTestCases(testCases);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldThrowOnNullParameter()
        {
            Action convertFrom = () => ODataUriUtils.ConvertFromUriLiteral(null, ODataVersion.V4, null, null);
            convertFrom.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldThrowOnPrimitiveInsideBraces()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithNumberProperty");
            complexType.AddStructuralProperty("numberProperty", Int32TypeRef);
            edmModel.AddElement(complexType);

            IEdmTypeReference expectedType = edmModel.FindType("TestModel.ComplexTypeWithNumberProperty").ToTypeReference();
            const string text = "{5}";

            Action convertFrom = () => ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            convertFrom.ShouldThrow<ODataException>().Match(e => e.Message.StartsWith("Invalid JSON. A colon character ':' is expected after the property name '5', but none was found."));
        }

        #endregion

        #region JSON Light

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseJsonLightEvenNoModelIsProvided()
        {
            var text = "[1,2,3]";
            object restulTmp = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, null /*edmModel*/, null /*expectedType*/);
            restulTmp.As<ODataCollectionValue>().TypeName.Should().Be(null);
            restulTmp.As<ODataCollectionValue>().Items.Cast<Int32>().Count().Should().Be(3);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseComplexValueWithTypeNameWhenNoExpectedTypeIsProvided()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithNumberProperty");
            complexType.AddStructuralProperty("numberProperty", Int32TypeRef);
            edmModel.AddElement(complexType);

            var text = "{\"@odata.type\":\"#TestModel.ComplexTypeWithNumberProperty\",\"numberProperty\":42}";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, null /*typeReference*/);
            result.Should().BeAssignableTo<ODataComplexValue>();
            result.As<ODataComplexValue>().TypeName.Should().Be("TestModel.ComplexTypeWithNumberProperty");
            result.As<ODataComplexValue>().Properties.Should().OnlyContain(p => p.Name == "numberProperty" && p.Value.Equals(42));
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriThrowsOnComplexValueWithNoTypeWhenNoExpectedTypeIsProvided()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithNumberProperty");
            complexType.AddStructuralProperty("numberProperty", Int32TypeRef);
            edmModel.AddElement(complexType);

            var text = "{\"numberProperty\":42}";

            object resultTmp = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, null /*typeReference*/);
            resultTmp.As<ODataComplexValue>().TypeName.Should().Be(null);
            resultTmp.As<ODataComplexValue>().Properties.Should().OnlyContain(p => p.Name == "numberProperty" && p.Value.Equals(42));
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseComplexValue()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithNumberProperty");
            complexType.AddStructuralProperty("numberProperty", Int32TypeRef);
            edmModel.AddElement(complexType);

            IEdmTypeReference expectedType = edmModel.FindType("TestModel.ComplexTypeWithNumberProperty").ToTypeReference();
            var text = "{\"@odata.type\":\"#TestModel.ComplexTypeWithNumberProperty\",\"numberProperty\":42}";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataComplexValue>();
            result.As<ODataComplexValue>().TypeName.Should().Be("TestModel.ComplexTypeWithNumberProperty");
            result.As<ODataComplexValue>().Properties.Should().OnlyContain(p => p.Name == "numberProperty" && p.Value.Equals(42));
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseComplexValueWithNullProperty()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithNullProperty");
            complexType.AddStructuralProperty("null", Int32NullableTypeRef);
            edmModel.AddElement(complexType);

            IEdmTypeReference expectedType = edmModel.FindType("TestModel.ComplexTypeWithNullProperty").ToTypeReference();
            var text = "{\"@odata.type\":\"#TestModel.ComplexTypeWithNullProperty\",\"null\":null}";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataComplexValue>();
            result.As<ODataComplexValue>().TypeName.Should().Be("TestModel.ComplexTypeWithNullProperty");
            result.As<ODataComplexValue>().Properties.Should().OnlyContain(p => p.Name == "null" && p.Value == null);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseComplexValueWithComplexProperty()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var innerType = new EdmComplexType(DefaultNamespaceName, "InnerComplexTypeWithStringProperty");
            innerType.AddStructuralProperty("foo", StringNullableTypeRef);
            edmModel.AddElement(innerType);

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithPrimitiveAndComplexProperty");
            complexType.AddStructuralProperty("number", Int32TypeRef);
            complexType.AddStructuralProperty("complex", new EdmComplexTypeReference(innerType, isNullable: true));
            edmModel.AddElement(complexType);

            IEdmTypeReference expectedType = edmModel.FindType("TestModel.ComplexTypeWithPrimitiveAndComplexProperty").ToTypeReference();
            var text = "{\"@odata.type\":\"#TestModel.ComplexTypeWithPrimitiveAndComplexProperty\",\"number\":42,\"complex\":{\"@odata.type\":\"#TestModel.InnerComplexTypeWithStringProperty\",\"foo\":\"bar\"}}";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataComplexValue>();
            result.As<ODataComplexValue>().TypeName.Should().Be("TestModel.ComplexTypeWithPrimitiveAndComplexProperty");
            result.As<ODataComplexValue>().Properties.Should().HaveCount(2);
            result.As<ODataComplexValue>().Properties.Should().Contain(p => p.Name == "number" && p.Value.Equals(42));
            result.As<ODataComplexValue>().Properties.Should().Contain(p => p.Name == "complex" && p.Value is ODataComplexValue);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseComplexValueWithSpatialProperty()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithSpatialProperties");
            complexType.AddStructuralProperty("geographyPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false));
            complexType.AddStructuralProperty("geometryPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, false));
            edmModel.AddElement(complexType);

            IEdmTypeReference expectedType = edmModel.FindType("TestModel.ComplexTypeWithSpatialProperties").ToTypeReference();
            var text = "{\"@odata.type\":\"#TestModel.ComplexTypeWithSpatialProperties\",\"geographyPoint\":{\"type\":\"Point\",\"coordinates\":[-200.0,32.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"geometryPoint\":{\"type\":\"Point\",\"coordinates\":[60.5,-50.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}}}";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataComplexValue>();
            result.As<ODataComplexValue>().TypeName.Should().Be("TestModel.ComplexTypeWithSpatialProperties");
            result.As<ODataComplexValue>().Properties.Should().HaveCount(2);
            result.As<ODataComplexValue>().Properties.Should().Contain(p => p.Name == "geographyPoint" && p.Value is GeographyPoint);
            result.As<ODataComplexValue>().Properties.Should().Contain(p => p.Name == "geometryPoint" && p.Value is GeometryPoint);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseComplexValueWithNoTypeOnWire()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithNumberProperty");
            complexType.AddStructuralProperty("numberProperty", Int32TypeRef);
            edmModel.AddElement(complexType);

            IEdmTypeReference expectedType = edmModel.FindType("TestModel.ComplexTypeWithNumberProperty").ToTypeReference();
            var text = "{\"@odata.type\":\"#TestModel.ComplexTypeWithNumberProperty\",\"numberProperty\":42}";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataComplexValue>();
            result.As<ODataComplexValue>().TypeName.Should().Be("TestModel.ComplexTypeWithNumberProperty");
            result.As<ODataComplexValue>().Properties.Should().OnlyContain(p => p.Name == "numberProperty" && p.Value.Equals(42));
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseComplexValueWithNoType()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithNumberProperty");
            complexType.AddStructuralProperty("numberProperty", Int32TypeRef);
            edmModel.AddElement(complexType);

            IEdmTypeReference expectedType = edmModel.FindType("TestModel.ComplexTypeWithNumberProperty").ToTypeReference();
            var text = "{\"numberProperty\":42}";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataComplexValue>();
            result.As<ODataComplexValue>().TypeName.Should().Be("TestModel.ComplexTypeWithNumberProperty");
            result.As<ODataComplexValue>().Properties.Should().OnlyContain(p => p.Name == "numberProperty" && p.Value.Equals(42));
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseEmptyComplexValueWithNoType()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;
            object complextTmp = ODataUriUtils.ConvertFromUriLiteral("{}", ODataVersion.V4, edmModel, null);
            complextTmp.As<ODataComplexValue>().TypeName.Should().Be(null);
            complextTmp.As<ODataComplexValue>().Properties.Count().Should().Be(0);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseInterestingComplexValueWithNoType()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithSpatialProperties");
            complexType.AddStructuralProperty("geographyPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false));
            complexType.AddStructuralProperty("geometryPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, false));
            edmModel.AddElement(complexType);

            IEdmTypeReference expectedType = edmModel.FindType("TestModel.ComplexTypeWithSpatialProperties").ToTypeReference();
            var text = "{\"geographyPoint\":{\"type\":\"Point\",\"coordinates\":[-200.0,32.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}},\"geometryPoint\":{\"type\":\"Point\",\"coordinates\":[60.5,-50.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}}}";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataComplexValue>();
            result.As<ODataComplexValue>().TypeName.Should().Be("TestModel.ComplexTypeWithSpatialProperties");
            result.As<ODataComplexValue>().Properties.Should().HaveCount(2);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseEmptyArray()
        {
            IEdmModel edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEdmTypeReference expectedType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false));
            var text = "[]";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataCollectionValue>();
            result.As<ODataCollectionValue>().TypeName.Should().Be("Collection(Edm.Int32)");
            result.As<ODataCollectionValue>().Items.Should().NotBeNull();
            result.As<ODataCollectionValue>().Items.Should().HaveCount(0);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseCollectionWithOneInt()
        {
            IEdmModel edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmTypeReference expectedType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false));
            var text = "[42]";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataCollectionValue>();
            result.As<ODataCollectionValue>().TypeName.Should().Be("Collection(Edm.Int32)");
            result.As<ODataCollectionValue>().Items.Should().HaveCount(1);
            result.As<ODataCollectionValue>().Items.Should().HaveElementAt(0, 42);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseCollectionOfString()
        {
            IEdmModel edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmTypeReference expectedType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false));
            var text = "[\"Bart\",\"Homer\",\"Marge\"]";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataCollectionValue>();
            result.As<ODataCollectionValue>().TypeName.Should().Be("Collection(Edm.String)");
            result.As<ODataCollectionValue>().Items.Should().HaveCount(3);
            result.As<ODataCollectionValue>().Items.Should().HaveElementAt(0, "Bart");
            result.As<ODataCollectionValue>().Items.Should().HaveElementAt(1, "Homer");
            result.As<ODataCollectionValue>().Items.Should().HaveElementAt(2, "Marge");
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseCollectionOfBool()
        {
            IEdmModel edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmTypeReference expectedType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetBoolean(false));
            var text = "[true,false]";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataCollectionValue>();
            result.As<ODataCollectionValue>().TypeName.Should().Be("Collection(Edm.Boolean)");
            result.As<ODataCollectionValue>().Items.Should().HaveCount(2);
            result.As<ODataCollectionValue>().Items.Should().HaveElementAt(0, true);
            result.As<ODataCollectionValue>().Items.Should().HaveElementAt(1, false);
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldParseCollectionOfComplex()
        {
            var edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexTypeForMultipleItemsCollection");
            complexType.AddStructuralProperty("Name", StringNullableTypeRef);
            edmModel.AddElement(complexType);

            IEdmTypeReference expectedType = EdmCoreModel.GetCollection(edmModel.FindType("TestModel.ComplexTypeForMultipleItemsCollection").ToTypeReference());
            var text = "[{\"@odata.type\":\"#TestModel.ComplexTypeForMultipleItemsCollection\",\"Name\":\"Bart\"},{\"@odata.type\":\"#TestModel.ComplexTypeForMultipleItemsCollection\",\"Name\":\"Homer\"},{\"@odata.type\":\"#TestModel.ComplexTypeForMultipleItemsCollection\",\"Name\":\"Marge\"}]";

            var result = ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            result.Should().BeAssignableTo<ODataCollectionValue>();
            result.As<ODataCollectionValue>().TypeName.Should().Be("Collection(TestModel.ComplexTypeForMultipleItemsCollection)");
            result.As<ODataCollectionValue>().Items.Should().HaveCount(3);
            result.As<ODataCollectionValue>().Items.Cast<object>().All(o => o is ODataComplexValue).Should().BeTrue();
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation]
        public void ConvertFromUriShouldNotParseCollectionOfCollections()
        {
            IEdmModel edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmTypeReference expectedType = EdmCoreModel.GetCollection(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false)));
            var text = "[[1,2],[3,4]]";

            Action parse = () => ODataUriUtils.ConvertFromUriLiteral(text, ODataVersion.V4, edmModel, expectedType);
            parse.ShouldThrow<ODataException>();
            // JSON light fails because of the nested collections, verbose JSON will fail for other reasons, so we are not baselining error message
        }

        #endregion

        #region Test Helpers

        private void RunTestCases(List<ConvertFromUriLiteralTestCase> testCases, ODataVersion version = ODataVersion.V4)
        {
            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                testCase =>
                {
                    this.RunTestCase(testCase, version, testCase.Model, testCase.TypeReference);
                });
        }

        private void RunTestCase(ConvertFromUriLiteralTestCase testCase, ODataVersion version, IEdmModel model, IEdmTypeReference typeReference)
        {
            if (testCase.SkipVersion != null && testCase.SkipVersion(version))
            {
                return;
            }

            // Negative Test
            if (testCase.ExpectedException != null)
            {
                TestExceptionUtils.ExpectedException(
                    this.Assert,
                    () =>
                    {
                        ODataUriUtils.ConvertFromUriLiteral(testCase.Parameter, version, model, typeReference);
                    },
                    testCase.ExpectedException,
                    this.ExceptionVerifier
                    );
            }
            else // Positive Test
            {
                object actualValue = ODataUriUtils.ConvertFromUriLiteral(testCase.Parameter, version, model, typeReference);
                object expectedValue = testCase.ExpectedValue;
                bool fail = false;

                if (expectedValue == null)
                {
                    if (testCase.PayloadElement == null)
                    {
                        fail = actualValue != null;
                    }
                    else
                    {
                        // We have a payload element, convert product OM to test OM and compare there
                        ODataPayloadElement actualAsPayload = objectModelToPayloadConverter.Convert(actualValue, false);
                        RemoveAnnotationsPayloadElementVisitor<SerializationTypeNameTestAnnotation> removeAnnotationVisitor = new RemoveAnnotationsPayloadElementVisitor<SerializationTypeNameTestAnnotation>();
                        removeAnnotationVisitor.RemoveAnnotations(testCase.PayloadElement);
                        removeAnnotationVisitor.RemoveAnnotations(actualAsPayload);
                        RemoveAnnotationsPayloadElementVisitor<EntityModelTypeAnnotation> removeEntityModelTypeAnnotationVisitor = new RemoveAnnotationsPayloadElementVisitor<EntityModelTypeAnnotation>();
                        removeEntityModelTypeAnnotationVisitor.RemoveAnnotations(testCase.PayloadElement);

                        // objectModelToPayloadConverter converts empty complex collection to empty primitive collection as payload element, change expected value to work around
                        if (actualAsPayload.GetType() != testCase.PayloadElement.GetType())
                        {
                            ComplexMultiValue complexMultiValue = testCase.PayloadElement as ComplexMultiValue;
                            if (complexMultiValue != null && complexMultiValue.Count == 0)
                            {
                                testCase.PayloadElement = new PrimitiveMultiValue(complexMultiValue.FullTypeName, complexMultiValue.IsNull);
                            }
                        }

                        this.oDataPayloadElementComparer.Compare(testCase.PayloadElement, actualAsPayload);
                    }
                }
                else if (!expectedValue.Equals(actualValue))
                {
                    // We have a product OM value to compare. Make a direct comparision.
                    // This should only be used for primitives
                    if (expectedValue.GetType().IsArray)
                    {
                        Array expectedArray = (Array)expectedValue;
                        Array actualArray = (Array)actualValue;
                        int index = 0;
                        while (index < expectedArray.Length)
                        {
                            if (!expectedArray.GetValue(index).Equals(actualArray.GetValue(index)))
                            {
                                fail = true;
                                break;
                            }
                            index++;
                        }
                    }
                    else
                    {
                        fail = true;

                        ODataNullValue expectedNullValue = expectedValue as ODataNullValue;
                        ODataNullValue actualNullValue = actualValue as ODataNullValue;
                        if (expectedNullValue != null && actualNullValue != null)
                        {
                            fail = false;
                        }
                    }
                }

                this.ThrowIfTestFailed(fail, expectedValue, actualValue);
            }
        }

        private void ThrowIfTestFailed(bool fail, object expectedValue, object actualValue)
        {
            if (fail)
            {
                throw new ODataTestException(string.Format(
                    "Different Object.{0}Expected:{0}{1}{0}Actual:{0}{2}{0}",
                    Environment.NewLine,
                    expectedValue,
                    actualValue));
            }
        }

        private class ConvertFromUriLiteralTestCase
        {
            public string Parameter { get; set; }
            public ODataPayloadElement PayloadElement { get; set; }
            public IEdmModel Model { get; set; }
            public IEdmTypeReference TypeReference { get; set; }
            public object ExpectedValue { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public Func<ODataVersion, bool> SkipVersion { get; set; }
        }

        #endregion
    }
}
