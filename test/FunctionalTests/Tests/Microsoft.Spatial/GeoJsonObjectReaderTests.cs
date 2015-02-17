//---------------------------------------------------------------------
// <copyright file="GeoJsonObjectReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Spatial;
    using System.Text;
    using System.Text.RegularExpressions;
    using DataSpatialUnitTests.Utils;
    using Microsoft.Data.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GeoJsonObjectReaderTests
    {
        private static List<object> arrayOfArrayMultipleElements;
        private static List<object> arrayOfArrayOfArrayMultipleElements;
        private static List<object> arrayOfArrayOfArrayOneElement;
        private static List<object> arrayOfArrayOneElement;
        private static List<object> arrayOfMultiplePositions;
        private static List<object> arrayOfOnePosition;
        private static List<object> arrayOfZeroPosition;
        private static readonly List<object> position2D = new List<object> {100.655, -45.888};
        private static readonly List<object> position3D = new List<object> {150.0, 55.1, 99.33};
        private static readonly List<object> position4D = new List<object> {120, -90, 44.2, 77.9};
        private static readonly List<object> position4DNZ = new List<object> { 120, -90, null, 77.9 };

        [TestMethod]
        public void ErrorOnCoordinatesArrayElementPrimitiveNotDouble_MultipleDimension()
        {
            var coordinates = new object[] {new object[] {new object[] {1.0, 2.0}, new object[] {3.0, "stingVal"}}};
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => ExecuteSendToPipeline(SpatialType.Polygon, coordinates), Strings.GeoJsonReader_ExpectedNumeric);
        }

        [TestMethod]
        public void ErrorOnCoordinatesArrayElementPrimitiveNotDouble_SingleDimension()
        {
            var coordinates = new object[] {1.0, true};
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => ExecuteSendToPipeline(SpatialType.Point,  coordinates), Strings.GeoJsonReader_ExpectedNumeric);
        }

        [TestMethod]
        public void ErrorOnCoordinatesArrayElementPrimitiveIsObject()
        {
            var coordinates = new object[] {new Dictionary<string, object>() {{"prop", "value"}}, 1.0};
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => ExecuteSendToPipeline(SpatialType.Point, coordinates), Strings.GeoJsonReader_ExpectedNumeric);
        }

        [TestMethod]
        public void ErrorOnCoordinatesArrayElementArrayIsObject()
        {
            var coordinates = new object[] { new Dictionary<string, object>() { { "prop", "value" } }, 1.0 };
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => ExecuteSendToPipeline(SpatialType.Polygon, coordinates), Strings.GeoJsonReader_ExpectedArray);
        }

        [TestMethod]
        public void ErrorOnCoordinatesNotArray()
        {
            var input = new Dictionary<string, object>()
                            {
                                {"type", "Point"},
                                {"coordinates", 1.0}
                            };
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => ExecuteSendToPipeline(input), Strings.GeoJsonReader_ExpectedArray);
        }

        [TestMethod]
        public void ErrorOnInvalidCrs_CrsValueNotAnObject()
        {
            TestInvalidCrs("badCRSValue", Strings.JsonReaderExtensions_CannotReadValueAsJsonObject("badCRSValue"));
        }

        [TestMethod]
        public void ErrorOnInvalidCrs_PropertiesValueNotAnObject()
        {
            var crsMembers = new Dictionary<string, object>
                                 {
                                     {
                                         GeoJsonConstants.TypeMemberName,
                                         GeoJsonConstants.CrsTypeMemberValue
                                     },
                                     {
                                         GeoJsonConstants.CrsPropertiesMemberName,
                                         "badPropertiesValue"
                                     }
                                 };

            TestInvalidCrs(crsMembers, Strings.JsonReaderExtensions_CannotReadValueAsJsonObject("badPropertiesValue"));
        }

        [TestMethod]
        public void ErrorOnInvalidCrs_EmptyProperties()
        {
            var crsMembers = new Dictionary<string, object>
                                 {
                                     {
                                         GeoJsonConstants.TypeMemberName,
                                         GeoJsonConstants.CrsTypeMemberValue
                                     },
                                     {
                                         GeoJsonConstants.CrsPropertiesMemberName,
                                         new Dictionary<string, object>
                                             {
                                             }
                                     }
                                 };

            TestInvalidCrs(crsMembers, Strings.GeoJsonReader_MissingRequiredMember(GeoJsonConstants.CrsNameMemberName));
        }

        [TestMethod]
        public void ErrorOnInvalidCrs_InvalidNamePrefix()
        {
            TestCrsWithInvalidName("foo:1234");
        }

        [TestMethod]
        public void ErrorOnInvalidCrs_InvalidNameSeperator()
        {
            TestCrsWithInvalidName("EPSG1234");
        }

        [TestMethod]
        public void ErrorOnInvalidCrs_JustEPSGNoSeperatorOrValue()
        {
            TestCrsWithInvalidName("EPSG");
        }

        [TestMethod]
        public void ErrorOnInvalidCrs_InvalidType()
        {
            var crsMembers = new Dictionary<string, object>
                                 {
                                     {
                                         GeoJsonConstants.TypeMemberName,
                                         "foo"
                                         }
                                 };

            TestInvalidCrs(crsMembers, Strings.GeoJsonReader_InvalidCrsType("foo"));
        }

        [TestMethod]
        public void ErrorOnInvalidCrs_MissingProperties()
        {
            var crsMembers = new Dictionary<string, object>
                                 {
                                     {
                                         GeoJsonConstants.TypeMemberName,
                                         GeoJsonConstants.CrsTypeMemberValue
                                         },
                                 };

            TestInvalidCrs(crsMembers, Strings.GeoJsonReader_MissingRequiredMember(GeoJsonConstants.CrsPropertiesMemberName));
        }

        [TestMethod]
        public void ErrorOnInvalidCrs_MissingType()
        {
            var crsMembers = new Dictionary<string, object>();
            TestInvalidCrs(crsMembers, Strings.GeoJsonReader_MissingRequiredMember(GeoJsonConstants.TypeMemberName));
        }

        [TestMethod]
        public void ErrorOnInvalidCrs_NonIntegerSrid()
        {
            TestCrsWithInvalidName("EPSG:foo");
        }

        [TestMethod]
        public void ErrorOnInvalidCrs_NullName()
        {
            TestCrsWithInvalidName(null);
        }

        // GeoJson spec says you can have "any number of members", so there isn't such a thing as an invalid one

        [TestMethod]
        public void ErrorOnInvalidGeoJSONTypeName_NotAString()
        {
            var input = new Dictionary<string, object>(){{"type", 55}};
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => ExecuteSendToPipeline(input), Strings.JsonReaderExtensions_CannotReadPropertyValueAsString(55, "type"));
        }

        [TestMethod]
        public void ErrorOnInvalidGeoJSONTypeName_Null()
        {
            var input = new Dictionary<string, object>() { { "type", null } };
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => ExecuteSendToPipeline(input), Strings.GeoJsonReader_InvalidTypeName(String.Empty));
        }

        [TestMethod]
        public void ErrorOnInvalidGeoJSONTypeName_UnknownType()
        {
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => ExecuteSendToPipeline(SpatialType.Unknown, null), Strings.GeoJsonReader_InvalidTypeName("Unknown"));
        }

        [TestMethod]
        public void ErrorOnInvalidGeoJSONTypeName_WrongCasing()
        {
            var input = new Dictionary<string, object>() { { "type", "point" } };
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => ExecuteSendToPipeline(input), Strings.GeoJsonReader_InvalidTypeName("point"));
        }

        [TestMethod]
        public void ErrorOnMissingCoordinatesMember()
        {
            var members = new Dictionary<string, object>();
            members.Add(GeoJsonConstants.TypeMemberName, GetGeoJsonTypeName(SpatialType.LineString));

            var pipeline = new CallSequenceLoggingPipeline();

            // This error should occur regardless of if the pipeline is geography or geometry, so just pick one.
            var isGeography = true;
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => SendToPipeline(members, pipeline, isGeography), Strings.GeoJsonReader_MissingRequiredMember("coordinates"));
        }

        [TestMethod]
        public void ErrorOnMissingTypeMember()
        {
            var properties = new Dictionary<string, object>();
            properties.Add(GeoJsonConstants.CoordinatesMemberName, position2D);

            var pipeline = new CallSequenceLoggingPipeline(true);

            // This error should occur regardless of if the pipeline is geography or geometry, so just pick one.
            var isGeography = true;
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => SendToPipeline(properties, pipeline, isGeography), Strings.GeoJsonReader_MissingRequiredMember("type"));
        }

        [TestMethod]
        public void ErrorOnNullArrayElement_X()
        {
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() =>
                                                                    ExecuteSendToPipeline(SpatialType.Point, new List<object> {null, 75}),
                                                                    Strings.GeoJsonReader_InvalidNullElement);
        }

        [TestMethod]
        public void ErrorOnNullArrayElement_Y()
        {
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() =>
                                                                    ExecuteSendToPipeline(SpatialType.Point, new List<object> {32.4, null}),
                                                                    Strings.GeoJsonReader_InvalidNullElement);
        }

        [TestMethod]
        public void ErrorOnPositionLessThan2Elements_MultipleDimensions()
        {
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() =>
                                                                    ExecuteSendToPipeline(SpatialType.Polygon, new List<object> {new List<object> {new List<object> {127.3, -88}, new List<object> {22}}}),
                                                                    Strings.GeoJsonReader_InvalidPosition);
        }

        [TestMethod]
        public void ErrorOnPositionLessThan2Elements_SingleDimension()
        {
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() =>
                                                                    ExecuteSendToPipeline(SpatialType.Point, new List<object> {1.1}),
                                                                    Strings.GeoJsonReader_InvalidPosition);
        }

        [TestMethod]
        public void ErrorOnPositionMoreThan4Elements_MultipleDimensions()
        {
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() =>
                                                                    ExecuteSendToPipeline(SpatialType.Polygon, new List<object> {new List<object> {new List<object> {127.3, -88}}, new List<object> {new List<object> {22, 88, -121.5, 91.2, 10, 102}}}),
                                                                    Strings.GeoJsonReader_InvalidPosition);
        }

        [TestMethod]
        public void ErrorOnPositionMoreThan4Elements_SingleDimension()
        {
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() =>
                                                                    ExecuteSendToPipeline(SpatialType.Point, new List<object> {1.1, 5, -32, 99.11, 6}),
                                                                    Strings.GeoJsonReader_InvalidPosition);
        }

        [TestMethod]
        public void ErrorOnUnexpectedArrayInLineString()
        {
            TestErrorOnUnexpectedArray(SpatialType.LineString, new List<object> {position2D, position3D, arrayOfMultiplePositions});
        }

        [TestMethod]
        public void ErrorOnUnexpectedArrayInMultiPoint()
        {
            TestErrorOnUnexpectedArray(SpatialType.MultiPoint, new List<object> {arrayOfMultiplePositions, position3D, position3D});
        }

        [TestMethod]
        public void ErrorOnUnexpectedArrayInPoint_X()
        {
            TestErrorOnUnexpectedArray(SpatialType.Point, new List<object> {new List<object> {19.8888, -22, 190}, -90, 44.2, 77.9});
        }

        [TestMethod]
        public void ErrorOnUnexpectedArrayInPoint_Z()
        {
            TestErrorOnUnexpectedArray(SpatialType.Point, new List<object> {-83.1, 14.2, 97.9, new List<object> {23, 11.2, -142}});
        }

        [TestMethod]
        public void ErrorOnUnexpectedArrayInPosition_MultiPolygon()
        {
            TestErrorOnUnexpectedArray(SpatialType.MultiPolygon, new List<object> {arrayOfArrayOneElement, arrayOfArrayMultipleElements, new List<object> {new List<object> {new List<object> {67, 48.0}, new List<object> {78.1234, -123, new List<object> {1, 2}}, new List<object> {33.99, -76}}}});
        }

        [TestMethod]
        public void ErrorOnUnexpectedNumericInLineString()
        {
            TestErrorOnUnexpectedNumeric(SpatialType.LineString, new List<object> {-12.3, position3D, position3D});
        }

        [TestMethod]
        public void ErrorOnUnexpectedNumericInMultiLineString()
        {
            TestErrorOnUnexpectedNumeric(SpatialType.MultiLineString, new List<object> {arrayOfOnePosition, arrayOfMultiplePositions, -103});
        }

        [TestMethod]
        public void ErrorOnUnexpectedNumericInMultiPoint()
        {
            TestErrorOnUnexpectedNumeric(SpatialType.MultiPoint, new List<object> {position2D, position3D, 33});
        }

        [TestMethod]
        public void ErrorOnUnexpectedNumericInMultiPolygon_FirstElement()
        {
            TestErrorOnUnexpectedNumeric(SpatialType.MultiPolygon, new List<object> {98.22222, arrayOfArrayMultipleElements});
        }

        [TestMethod]
        public void ErrorOnUnexpectedNumericInMultiPolygon_LastElement()
        {
            TestErrorOnUnexpectedNumeric(SpatialType.MultiPolygon, new List<object> {arrayOfArrayOneElement, 47.12999});
        }

        [TestMethod]
        public void ErrorOnUnexpectedNumericInPolygon()
        {
            TestErrorOnUnexpectedNumeric(SpatialType.Polygon, new List<object> {92.0, arrayOfMultiplePositions, arrayOfMultiplePositions});
        }

        [TestMethod]
        public void ErrorOnUnexpectedPositionInMultiLineString()
        {
            TestErrorOnUnexpectedNumeric(SpatialType.MultiLineString, new List<object> {arrayOfOnePosition, arrayOfMultiplePositions, position4D});
        }

        [TestMethod]
        public void ErrorOnUnexpectedPositionInMultiPolygon_FirstElement()
        {
            TestErrorOnUnexpectedNumeric(SpatialType.MultiPolygon, new List<object> {position3D, arrayOfArrayMultipleElements});
        }

        [TestMethod]
        public void ErrorOnUnexpectedPositionInMultiPolygon_LastElement()
        {
            TestErrorOnUnexpectedNumeric(SpatialType.MultiPolygon, new List<object> {arrayOfArrayOneElement, position2D});
        }

        [TestMethod]
        public void ErrorOnUnexpectedPositionInPolygon()
        {
            TestErrorOnUnexpectedNumeric(SpatialType.Polygon, new List<object> {position2D, arrayOfMultiplePositions, arrayOfOnePosition});
        }

        [TestMethod]
        public void GeoJsonSimpleRoundTripTest()
        {
            var position = new GeographyPosition(12, 34, -12, -34);
            var coordinateSystem = CoordinateSystem.Geography(54321);

            var writer =  new GeoJsonObjectWriter();
            GeographyPipeline pipeline = (SpatialPipeline)writer;

            pipeline.SetCoordinateSystem(coordinateSystem);
            pipeline.BeginGeography(SpatialType.Point);
            pipeline.BeginFigure(position);
            pipeline.EndFigure();
            pipeline.EndGeography();

            var actualPipeline = new CallSequenceLoggingPipeline();
            var reader = new GeoJsonObjectReader(actualPipeline);
            reader.ReadGeography(writer.JsonObject);

            var expectedPipeline = new CallSequenceLoggingPipeline();

            // TODO: move the set of calls back into a delegate if the APIs come back together
            expectedPipeline.GeographyPipeline.SetCoordinateSystem(coordinateSystem);
            expectedPipeline.GeographyPipeline.BeginGeography(SpatialType.Point);
            expectedPipeline.GeographyPipeline.BeginFigure(position);
            expectedPipeline.GeographyPipeline.EndFigure();
            expectedPipeline.GeographyPipeline.EndGeography();

            actualPipeline.Verify(expectedPipeline);
        }

        [TestMethod]
        public void ReadGeometryCollection()
        {
            TestReadMethod(new[] {SpatialType.LineString, SpatialType.Polygon},
                           new[] {arrayOfMultiplePositions, arrayOfArrayMultipleElements},
                           true,
                           GetExpectedCollectionPipeline,
                           new Func<List<object>, bool, ICommonLoggingPipeline>[] {GetExpectedLineStringPipeline, GetExpectedPolygonPipeline});
        }

        [TestMethod]
        public void ReadLineString()
        {
            TestReadMethod(SpatialType.LineString, arrayOfMultiplePositions, GetExpectedLineStringPipeline);
        }

        [TestMethod]
        public void ReadMultiLineString()
        {
            TestReadMethod(SpatialType.MultiLineString, arrayOfArrayMultipleElements, GetExpectedMultiLineStringPipeline);
        }

        [TestMethod]
        public void ReadMultiPoint()
        {
            TestReadMethod(SpatialType.MultiPoint, arrayOfMultiplePositions, GetExpectedMultiPointPipeline);
        }

        [TestMethod]
        public void ReadMultiPolygon()
        {
            TestReadMethod(SpatialType.MultiPolygon, arrayOfArrayOfArrayMultipleElements, GetExpectedMultiPolygonPipeline);
        }

        [TestMethod]
        public void ReadPoint()
        {
            TestReadMethod(SpatialType.Point, position4D, GetExpectedPointPipeline);
        }

        [TestMethod]
        public void ReadPolygon()
        {
            TestReadMethod(SpatialType.Polygon, arrayOfArrayMultipleElements, GetExpectedPolygonPipeline);
        }


        [TestMethod]
        public void SendToPipelineCollection()
        {
            this.TestSendToPipelineCollection(true);
            this.TestSendToPipelineCollection(false);
        }

        [TestMethod]
        public void SendToPipelineCollectionEmpty()
        {
            var members = new Dictionary<string, object>();
            var crs = CreateCrsMembersWithName(GeoJsonConstants.CrsValuePrefix + ":54321");

            members.Add(GeoJsonConstants.TypeMemberName, GetGeoJsonTypeName(SpatialType.Collection));
            members.Add(GeoJsonConstants.CrsMemberName, crs);
            members.Add(GeoJsonConstants.GeometriesMemberName, new List<object>());

            var actualPipeline = new CallSequenceLoggingPipeline();
            SendToPipeline(members, actualPipeline, true);

            var expectedPipeline = GetExpectedCollectionPipeline(
                new Func<List<object>, bool, ICommonLoggingPipeline>[0], 
                new List<object>[0],                
                true,
                true,
                CoordinateSystem.Geography(54321));

            expectedPipeline.VerifyPipeline(actualPipeline);
        }

        [TestMethod]
        public void SendToPipelineCollectionWhichResetsCrsInSubType()
        {
            var members = new Dictionary<string, object>();
            var crs = CreateCrsMembersWithName(GeoJsonConstants.CrsValuePrefix + ":54321");

            members.Add(GeoJsonConstants.TypeMemberName, GetGeoJsonTypeName(SpatialType.Collection));
            members.Add(GeoJsonConstants.CrsMemberName, crs);

            var collectionItems = new List<IDictionary<string, object>>
                                      {
                                          GetJsonMembers(SpatialType.LineString, arrayOfMultiplePositions, crs),
                                          GetJsonMembers(SpatialType.Polygon, arrayOfArrayMultipleElements, crs)
                                      };

            members.Add(GeoJsonConstants.GeometriesMemberName, collectionItems.ConvertAll((o) => (object)o));

            var actualPipeline = new CallSequenceLoggingPipeline();
            SendToPipeline(members, actualPipeline, true);

            var expectedPipeline = GetExpectedCollectionPipeline(
                new Func<List<object>, bool, ICommonLoggingPipeline>[]
                    {
                        (positions, isGeography) => GetExpectedPipeline(
                            SpatialType.LineString,
                            isGeography ? CoordinateSystem.Geography(54321) : CoordinateSystem.Geometry(54321),
                            isGeography,
                            (pipeline) => WritePositionArrayToPipeline(positions, pipeline)),
                        (positions, isGeography) => GetExpectedPipeline(
                            SpatialType.Polygon,
                            isGeography ? CoordinateSystem.Geography(54321) : CoordinateSystem.Geometry(54321),
                            isGeography,
                            (pipeline) => WriteArrayOfPositionArrayToPipeline(positions, pipeline))
                    },
                new[] {arrayOfMultiplePositions, arrayOfArrayMultipleElements},
                true,
                true,
                CoordinateSystem.Geography(54321));

            expectedPipeline.VerifyPipeline(actualPipeline);
        }

        [TestMethod]
        public void SendToPipelineEmptyShape()
        {
            foreach (SpatialType spatialType in Enum.GetValues(typeof(SpatialType)))
            {
                if (spatialType != SpatialType.Unknown && spatialType != SpatialType.FullGlobe && spatialType != SpatialType.Collection)
                {
                    Func<List<object>, bool, ICommonLoggingPipeline> getExpectedPipeline = (coords, isGeography) => GetExpectedPipeline(
                        spatialType,
                        isGeography ? CoordinateSystem.DefaultGeography : CoordinateSystem.DefaultGeometry,
                        isGeography,
                        (pipe) => { });

                    TestSendToPipeline(spatialType, arrayOfZeroPosition, true, getExpectedPipeline);
                }
            }
        }

        [TestMethod]
        public void SendToPipelineLineStringMultiplePositions()
        {
            TestSendToPipelineLineString(arrayOfMultiplePositions);
        }

        [TestMethod]
        public void SendToPipelineLineStringSinglePosition()
        {
            TestSendToPipelineLineString(arrayOfOnePosition);
        }

        [TestMethod]
        public void SendToPipelineMultiLineStringMultipleDimensions()
        {
            TestSendToPipelineMultiLineString(arrayOfArrayMultipleElements);
        }

        [TestMethod]
        public void SendToPipelineMultiLineStringSingleDimension()
        {
            TestSendToPipelineMultiLineString(arrayOfArrayOneElement);
        }

        [TestMethod]
        public void SendToPipelineMultiPointMultiplePoints()
        {
            TestSendToPipelineMultiPoint(arrayOfMultiplePositions);
        }

        [TestMethod]
        public void SendToPipelineMultiPointSinglePoint()
        {
            TestSendToPipelineMultiPoint(arrayOfOnePosition);
        }

        [TestMethod]
        public void SendToPipelineMultiPolygonMultipleDimensions()
        {
            TestSendToPipelineMultiPolygon(arrayOfArrayOfArrayMultipleElements);
        }

        [TestMethod]
        public void SendToPipelineMultiPolygonSingleDimension()
        {
            TestSendToPipelineMultiPolygon(arrayOfArrayOfArrayOneElement);
        }

        [TestMethod]
        public void SendToPipelinePoint2D()
        {
            TestSendToPipelinePoint(position2D);
        }

        [TestMethod]
        public void SendToPipelinePoint3D()
        {
            TestSendToPipelinePoint(position3D);
        }

        [TestMethod]
        public void SendToPipelinePoint4D()
        {
            TestSendToPipelinePoint(position4D);
        }

        [TestMethod]
        public void SendToPipelinePoint4DNullZ()
        {
            TestSendToPipelinePoint(position4DNZ);
        }

        [TestMethod]
        public void SendToPipelinePolygonMultipleDimensions()
        {
            TestSendToPipelinePolygon(arrayOfArrayMultipleElements);
        }

        [TestMethod]
        public void SendToPipelinePolygonSingleDimension()
        {
            TestSendToPipelinePolygon(arrayOfArrayOneElement);
        }

        [TestMethod]
        public void SendToPipeline_CrsSpecified()
        {
            var expectedPipeline = new CallSequenceLoggingPipeline();

            var geoPipeline = expectedPipeline.GeometryPipeline;
            geoPipeline.SetCoordinateSystem(CoordinateSystem.Geometry(54321));
            geoPipeline.BeginGeometry(SpatialType.Point);
            geoPipeline.BeginFigure(new GeometryPosition(1, 2, null, null));
            geoPipeline.EndFigure();
            geoPipeline.EndGeometry();

            var crsMembers = CreateCrsMembersWithName(GeoJsonConstants.CrsValuePrefix + ":54321");

            var actualPipeline = ExecuteSendToPipeline(SpatialType.Point, new List<object> {1.0, 2.0}, false, crsMembers);
            expectedPipeline.Verify(actualPipeline);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            arrayOfMultiplePositions = new List<object> {position4D, position2D, position3D};
            arrayOfZeroPosition = new List<object>(0);
            arrayOfOnePosition = new List<object> {position3D};
            arrayOfArrayMultipleElements = new List<object> {arrayOfOnePosition, arrayOfMultiplePositions, new List<object> {new List<object> {67, 48.0}, new List<object> {78.1234, -123}, new List<object> {33.99, -76}}};
            arrayOfArrayOfArrayOneElement = new List<object> {arrayOfArrayMultipleElements};
            arrayOfArrayOneElement = new List<object> {arrayOfMultiplePositions};
            arrayOfArrayOfArrayMultipleElements = new List<object> { arrayOfArrayOneElement, arrayOfArrayMultipleElements };
        }

        private static IDictionary<string, object> CreateCrsMembersWithName(string name)
        {
            var crsMembers = new Dictionary<string, object>
                                 {
                                     {
                                         GeoJsonConstants.TypeMemberName,
                                         GeoJsonConstants.CrsTypeMemberValue
                                         },
                                     {
                                         GeoJsonConstants.CrsPropertiesMemberName,
                                         new Dictionary<string, object>
                                             {
                                                 {GeoJsonConstants.CrsNameMemberName, name}
                                             }
                                         }
                                 };

            return crsMembers;
        }

        private static CallSequenceLoggingPipeline ExecuteSendToPipeline(Dictionary<string, object> input, bool isGeography = true, IDictionary<string, object> crs = null)
        {
            var actualPipeline = new CallSequenceLoggingPipeline();
            SendToPipeline(input, actualPipeline, isGeography);
            return actualPipeline;
        }

        private static CallSequenceLoggingPipeline ExecuteSendToPipeline(SpatialType spatialType, IEnumerable<object> coordinates, bool isGeography = true, IDictionary<string, object> crs = null)
        {
            var members = GetJsonMembers(spatialType, coordinates, crs);

            var actualPipeline = new CallSequenceLoggingPipeline();
            SendToPipeline(members, actualPipeline, isGeography);
            return actualPipeline;
        }

        private static ICommonLoggingPipeline GetExpectedCollectionPipeline(Func<List<Object>, bool, ICommonLoggingPipeline>[] innerGetPipeline, List<object>[] coordinates, bool isGeography)
        {
            return GetExpectedCollectionPipeline(innerGetPipeline, coordinates, isGeography, false);
        }

        private static ICommonLoggingPipeline GetExpectedCollectionPipeline(Func<List<Object>, bool, ICommonLoggingPipeline>[] innerGetPipeline, List<object>[] coordinates, bool isGeography, bool keepAllSetCrsCalls)
        {
            return GetExpectedCollectionPipeline(innerGetPipeline, coordinates, isGeography, keepAllSetCrsCalls, isGeography ? CoordinateSystem.DefaultGeography : CoordinateSystem.DefaultGeometry);
        }

        private static ICommonLoggingPipeline GetExpectedCollectionPipeline(Func<List<Object>, bool, ICommonLoggingPipeline>[] innerGetPipeline, List<object>[] coordinates, bool isGeography, bool keepAllSetCrsCalls, CoordinateSystem crs)
        {
            return GetExpectedPipeline(SpatialType.Collection,
                                       crs,
                                       isGeography,
                                       (pipeline) =>
                                           {
                                               for (var i = 0; i < innerGetPipeline.Length; ++i)
                                               {
                                                   MergeLoggingPipeline(pipeline, innerGetPipeline[i](coordinates[i], isGeography), isGeography, keepAllSetCrsCalls);
                                               }
                                           });
        }

        private static ICommonLoggingPipeline GetExpectedLineStringPipeline(List<object> lineString, bool isGeography)
        {
            return GetExpectedPipeline(
                SpatialType.LineString,
                isGeography ? CoordinateSystem.DefaultGeography : CoordinateSystem.DefaultGeometry,
                isGeography,
                (pipeline) => WritePositionArrayToPipeline(lineString, pipeline));
        }

        private static ICommonLoggingPipeline GetExpectedMultiLineStringPipeline(List<object> multiLineString, bool isGeography)
        {
            return GetExpectedPipeline(
                SpatialType.MultiLineString,
                isGeography ? CoordinateSystem.DefaultGeography : CoordinateSystem.DefaultGeometry,
                isGeography,
                (pipeline) =>
                    {
                        foreach (List<object> lineString in multiLineString)
                        {
                            pipeline.BeginShape(SpatialType.LineString);
                            WritePositionArrayToPipeline(lineString, pipeline);
                            pipeline.EndShape();
                        }
                    });
        }

        private static ICommonLoggingPipeline GetExpectedMultiPointPipeline(List<object> multiPoint, bool isGeography)
        {
            return GetExpectedPipeline(
                SpatialType.MultiPoint,
                isGeography ? CoordinateSystem.DefaultGeography : CoordinateSystem.DefaultGeometry,
                isGeography,
                (pipeline) =>
                    {
                        foreach (List<object> point in multiPoint)
                        {
                            pipeline.BeginShape(SpatialType.Point);
                            WritePointToPipeline(point, pipeline);
                            pipeline.EndShape();
                        }
                    });
        }

        private static ICommonLoggingPipeline GetExpectedMultiPolygonPipeline(List<object> multiPolygon, bool isGeography)
        {
            return GetExpectedPipeline(
                SpatialType.MultiPolygon,
                isGeography ? CoordinateSystem.DefaultGeography : CoordinateSystem.DefaultGeometry,
                isGeography,
                (pipeline) =>
                    {
                        foreach (List<object> polygon in multiPolygon)
                        {
                            pipeline.BeginShape(SpatialType.Polygon);
                            WriteArrayOfPositionArrayToPipeline(polygon, pipeline);
                            pipeline.EndShape();
                        }
                    });
        }

        private static ICommonLoggingPipeline GetExpectedPipeline(SpatialType spatialType, CoordinateSystem coordinateSystem, bool isGeography, Action<ICommonLoggingPipeline> writeShape)
        {
            ICommonLoggingPipeline expectedPipeline;
            if (isGeography)
            {
                expectedPipeline = new GeographyLoggingPipeline();
            }
            else
            {
                expectedPipeline = new GeometryLoggingPipeline();
            }

            expectedPipeline.SetCoordinateSystem(coordinateSystem);
            expectedPipeline.BeginShape(spatialType);
            writeShape(expectedPipeline);
            expectedPipeline.EndShape();
            return expectedPipeline;
        }

        private static ICommonLoggingPipeline GetExpectedPointPipeline(List<object> position, bool isGeography)
        {
            return GetExpectedPipeline(
                SpatialType.Point,
                isGeography ? CoordinateSystem.DefaultGeography : CoordinateSystem.DefaultGeometry,
                isGeography,
                (pipeline) => { WritePointToPipeline(position, pipeline); });
        }

        private static ICommonLoggingPipeline GetExpectedPolygonPipeline(List<object> polygon, bool isGeography)
        {
            return GetExpectedPipeline(
                SpatialType.Polygon,
                isGeography ? CoordinateSystem.DefaultGeography : CoordinateSystem.DefaultGeometry,
                isGeography,
                (pipeline) => WriteArrayOfPositionArrayToPipeline(polygon, pipeline));
        }

        private static Dictionary<string, object> GetGeoJson(SpatialType[] collectionTypes, List<object>[] coordinates, out int expectedPropertyCount, int? epsgId = null)
        {
            expectedPropertyCount = 2;
            var geoJsonBuilder = new StringBuilder();
            var jsonObject = new Dictionary<string, object>();

            jsonObject.Add("type", "GeometryCollection");
            // Write the start of the object
            //geoJsonBuilder.Append("\n  \t{   ");
            //geoJsonBuilder.AppendFormat("{0}type{0}  : {1}GeometryCollection{1}", memberQuoteChar, typeValueQuoteChar);

            if (epsgId != null)
            {
                jsonObject.Add("crs", GetCrsObject(epsgId));
                //geoJsonBuilder.Append(",    ");
                //geoJsonBuilder.AppendFormat("    {0}crs{0}  : {{   {0}properties{0}:   {{ {0}name{0} : {1}EPSG:{2}{1}   }},   {0}type{0} : {1}name{1}   }}", memberQuoteChar, typeValueQuoteChar, epsgId.Value);
                expectedPropertyCount++;
            }

            List<Dictionary<string, object>> geometryObjects = new List<Dictionary<string, object>>();
            //geoJsonBuilder.AppendFormat(", {0}geometries{0}: [", memberQuoteChar);
            for (var i = 0; i < collectionTypes.Length; ++i)
            {
                //string innerJson;
                int throwAwayPropertyCount;
                var innerObject = GetGeoJson(collectionTypes[i], coordinates[i], out throwAwayPropertyCount);
                geometryObjects.Add(innerObject);
                //geoJsonBuilder.Append(innerJson);

                //if (i != collectionTypes.Length - 1)
                //{
                //    geoJsonBuilder.Append(",    ");
                //}
            }
            jsonObject.Add("geometries", geometryObjects);
            //geoJsonBuilder.Append("\n]\n}\n\r");
            //geoJsonInput = geoJsonBuilder.ToString();

            //return expectedPropertyCount;
            return jsonObject;
        }

        private static Dictionary<string, object> GetCrsObject(int? epsgId)
        {
            return new Dictionary<string, object>()
                       {
                           {"properties", new Dictionary<string, object>() {{"name", string.Format("EPSG:{0}", epsgId.Value)}}},
                           {"type", "name"}
                       };
        }

        private static Dictionary<string, object> GetGeoJson(SpatialType? spatialType, List<object> coordinates, out int expectedPropertyCount, int? epsgId = null)
        {
            expectedPropertyCount = 0;
            //var geoJsonBuilder = new StringBuilder();
            var jsonObject = new Dictionary<string, object>();

            // Write the start of the object
            //geoJsonBuilder.Append("\n  \t{   ");

            // Write the "type" member if one was specified
            if (spatialType != null)
            {
                expectedPropertyCount++;
                //geoJsonBuilder.AppendFormat("{0}type{0}  : {1}{2}{1}", memberQuoteChar, typeValueQuoteChar, spatialType != SpatialType.Collection ? spatialType.ToString() : "GeometryCollection");
                jsonObject.Add("type", GetGeoJsonTypeName(spatialType.Value));
            }

            // Write the "coordinates" member if one was specified
            if (coordinates != null)
            {
                //if (spatialType != null)
                //{
                //    geoJsonBuilder.Append(",    ");
                //}

                expectedPropertyCount++;
                jsonObject.Add("coordinates", coordinates);
                //geoJsonBuilder.AppendFormat("{0}coordinates{0}:    ", memberQuoteChar);
                //WriteArray(geoJsonBuilder, coordinates);
            }

            // write the CRS member if an SRID was given
            if (epsgId != null)
            {
                //if (coordinates != null || spatialType != null)
                //{
                //    geoJsonBuilder.Append(",    ");
                //}

                //geoJsonBuilder.AppendFormat("    {0}crs{0}  : {{   {0}properties{0}:   {{ {0}name{0} : {1}EPSG:{2}{1}   }},   {0}type{0} : {1}name{1}   }}", memberQuoteChar, typeValueQuoteChar, epsgId.Value);
                jsonObject.Add("crs", GetCrsObject(epsgId));
                expectedPropertyCount++;
            }

            // Write the end of the object
            //geoJsonBuilder.Append(" \n}\n\r  ");

            //geoJsonInput = geoJsonBuilder.ToString();
            //return expectedPropertyCount;
            return jsonObject;
        }

        private static IDictionary<string, object> GetJsonMembers(SpatialType spatialType, IEnumerable<object> coordinates, IDictionary<string, object> crs)
        {
            var members = new Dictionary<string, object>();

            members.Add(GeoJsonConstants.TypeMemberName, GetGeoJsonTypeName(spatialType));

            if (coordinates != null)
            {
                members.Add(GeoJsonConstants.CoordinatesMemberName, coordinates);
            }

            if (crs != null)
            {
                members.Add(GeoJsonConstants.CrsMemberName, crs);
            }
            return members;
        }
        
        private static string GetGeoJsonTypeName(SpatialType spatialType)
        {
            if(spatialType == SpatialType.Collection)
            {
                return "GeometryCollection";
            }

            return Enum.GetName(spatialType.GetType(), spatialType);
        }

        private static void MergeLoggingPipeline(ICommonLoggingPipeline source, ICommonLoggingPipeline target, bool isGeography, bool keepAllSetCrsCalls = false)
        {
            if (isGeography)
            {
                ((GeographyLoggingPipeline)source).MergeCalls((GeographyLoggingPipeline)target, keepAllSetCrsCalls);
            }
            else
            {
                ((GeometryLoggingPipeline)source).MergeCalls((GeometryLoggingPipeline)target, keepAllSetCrsCalls);
            }
        }

        private static void SendToPipeline(IDictionary<string, object> members, SpatialPipeline pipeline, bool isGeography)
        {
            GeoJsonObjectReader reader = new GeoJsonObjectReader(pipeline);
            if (isGeography)
            {
                reader.ReadGeography(members);
            }
            else
            {
                reader.ReadGeometry(members);
            }
        }

        private static void TestCrsWithInvalidName(string name)
        {
            var crsMembers = CreateCrsMembersWithName(name);
            TestInvalidCrs(crsMembers, Strings.GeoJsonReader_InvalidCrsName(name));
        }

        private static void TestErrorOnUnexpectedArray(SpatialType spatialType, List<object> coordinates)
        {
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() =>
                                                                    ExecuteSendToPipeline(spatialType, coordinates),
                                                                    Strings.GeoJsonReader_ExpectedNumeric);
        }

        private static void TestErrorOnUnexpectedNumeric(SpatialType spatialType, List<object> coordinates)
        {
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() =>
                                                                    ExecuteSendToPipeline(spatialType, coordinates),
                                                                    Strings.GeoJsonReader_ExpectedArray);
        }

        private static void TestInvalidCrs(object crsMembers, string error)
        {
            var members = new Dictionary<string, object>
                              {
                                  {
                                      GeoJsonConstants.TypeMemberName,
                                      GetGeoJsonTypeName(SpatialType.Point)
                                  },
                                  {
                                      GeoJsonConstants.CrsMemberName,
                                      crsMembers
                                  }
                              };

            var actualPipeline = new CallSequenceLoggingPipeline();
            SpatialTestUtils.VerifyExceptionThrown<ParseErrorException>(() => SendToPipeline(members, actualPipeline, false), error);
        }

        private static void TestReadMethod(SpatialType spatialType, List<object> coordinates, Func<List<object>, bool, ICommonLoggingPipeline> getExpectedPipeline)
        {
            // Test with both Geography and Geometry pipelines
            TestReadMethod(spatialType, coordinates, true, getExpectedPipeline);
            TestReadMethod(spatialType, coordinates, false, getExpectedPipeline);
        }

        private static void TestReadMethod(SpatialType[] spatialType, List<object>[] coordinates, bool isGeography, Func<Func<List<Object>, bool, ICommonLoggingPipeline>[], List<object>[], bool, ICommonLoggingPipeline> getExpectedPipeline, Func<List<Object>, bool, ICommonLoggingPipeline>[] innerGetExpectedPipeline)
        {
            // used for Collections

            int expectedPropertyCount;
            int epsgId = isGeography ? CoordinateSystem.DefaultGeography.EpsgId.Value : CoordinateSystem.DefaultGeometry.EpsgId.Value;
            var jsonObject = GetGeoJson(spatialType, coordinates, out expectedPropertyCount, epsgId);
            var expectedPipeline = getExpectedPipeline(innerGetExpectedPipeline, coordinates, isGeography);

            var actualPipeline = new CallSequenceLoggingPipeline();
            var geoJsonObjectReader = new GeoJsonObjectReader(actualPipeline);
            if (isGeography)
            {
                geoJsonObjectReader.ReadGeography(jsonObject);
            }
            else
            {
                geoJsonObjectReader.ReadGeometry(jsonObject);
            }

            expectedPipeline.VerifyPipeline(actualPipeline);
        }

        private static void TestReadMethod(SpatialType spatialType, List<object> coordinates, bool isGeography, Func<List<object>, bool, ICommonLoggingPipeline> getExpectedPipeline)
        {
            int expectedPropertyCount;
            var jsonObject = GetGeoJson(spatialType, coordinates, out expectedPropertyCount);
            var expectedPipeline = getExpectedPipeline(coordinates, isGeography);

            var actualPipeline = new CallSequenceLoggingPipeline();
            var geoJsonObjectReader = new GeoJsonObjectReader(actualPipeline);
            if (isGeography)
            {
                geoJsonObjectReader.ReadGeography(jsonObject);
            }
            else
            {
                geoJsonObjectReader.ReadGeometry(jsonObject);
            }

            expectedPipeline.VerifyPipeline(actualPipeline);
        }

        private static void TestSendToPipeline(SpatialType spatialType, List<object> coordinates, Func<List<object>, bool, ICommonLoggingPipeline> getExpectedPipeline)
        {
            // Test with both Geography and Geometry pipelines
            TestSendToPipeline(spatialType, coordinates, true, getExpectedPipeline);
            TestSendToPipeline(spatialType, coordinates, false, getExpectedPipeline);
        }

        private static void TestSendToPipeline(SpatialType spatialType, List<object> coordinates, bool isGeography, Func<List<object>, bool, ICommonLoggingPipeline> getExpectedPipeline)
        {
            // Set up expected call sequence
            var expectedPipeline = getExpectedPipeline(coordinates, isGeography);

            // Execute actual write to pipeline
            var actualPipeline = ExecuteSendToPipeline(spatialType, coordinates, isGeography);
            expectedPipeline.VerifyPipeline(actualPipeline);
        }

        private static void TestSendToPipelineLineString(List<object> lineString)
        {
            TestSendToPipeline(SpatialType.LineString, lineString, GetExpectedLineStringPipeline);
        }

        private static void TestSendToPipelineMultiLineString(List<object> multiLineString)
        {
            TestSendToPipeline(SpatialType.MultiLineString, multiLineString, GetExpectedMultiLineStringPipeline);
        }

        private static void TestSendToPipelineMultiPoint(List<object> multiPoint)
        {
            TestSendToPipeline(SpatialType.MultiPoint, multiPoint, GetExpectedMultiPointPipeline);
        }

        private static void TestSendToPipelineMultiPolygon(List<object> multiPolygon)
        {
            TestSendToPipeline(SpatialType.MultiPolygon, multiPolygon, GetExpectedMultiPolygonPipeline);
        }

        private static void TestSendToPipelinePoint(List<object> point)
        {
            TestSendToPipeline(SpatialType.Point, point, GetExpectedPointPipeline);
        }

        private static void TestSendToPipelinePolygon(List<object> polygon)
        {
            TestSendToPipeline(SpatialType.Polygon, polygon, GetExpectedPolygonPipeline);
        }
        
        private static void VerifyArray(List<object> expectedCoordinates, object actualCoordinatesObj)
        {
            var actualCoordinates = actualCoordinatesObj as List<object>;
            Assert.IsNotNull(actualCoordinatesObj, "Expected coordinates property to be non-null and of type List<object>.");
            Assert.AreEqual(expectedCoordinates.Count, actualCoordinates.Count, "Coordinates does not contain the expected number of elements.");

            for (var i = 0; i < expectedCoordinates.Count; i++)
            {
                var expectedItem = expectedCoordinates[i];
                var actualItemObj = actualCoordinates[i];

                if (expectedItem is double? || expectedItem == null)
                {
                    if (expectedItem == null || double.IsNaN((double)expectedItem))
                    {
                        // NaN in expected coords translates to null Z/M
                        Assert.IsNull(actualItemObj, "Array item is not the expected value.");
                    }
                    else
                    {
                        Assert.AreEqual(ConvertToDouble(expectedItem), ConvertToDouble(actualItemObj), "Array item is not the expected value.");
                    }
                }
                else if (expectedItem is int?)
                {
                    Assert.AreEqual(ConvertToDouble(expectedItem), ConvertToDouble(actualItemObj), "Array item is not the expected value.");
                }
                else if (expectedItem is List<object>)
                {
                    VerifyArray((List<object>)expectedItem, actualItemObj);
                }
                else
                {
                    Assert.Fail("Expecting only null, numeric or List<object> elements in the test array.");
                }
            }
        }

        private static double? ConvertToDouble(object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is double?)
            {
                return (double?)value;
            }

            Assert.IsTrue(value is int?, "what else are we getting?");
            return Convert.ToDouble(value);
        }

        private static void WriteArrayOfPositionArrayToPipeline(List<object> polygon, ICommonLoggingPipeline expectedPipeline)
        {
            foreach (List<object> positionArray in polygon)
            {
                WritePositionArrayToPipeline(positionArray, expectedPipeline);
            }
        }

        private static void WritePointToPipeline(List<object> position, ICommonLoggingPipeline pipeline)
        {
            WritePositionToPipeline(position, pipeline, true);
            pipeline.EndFigure();
        }

        private static void WritePositionArrayToPipeline(List<object> lineString, ICommonLoggingPipeline expectedPipeline)
        {
            var first = true;
            foreach (List<object> position in lineString)
            {
                WritePositionToPipeline(position, expectedPipeline, first);
                if (first)
                {
                    first = false;
                }
            }
            expectedPipeline.EndFigure();
        }

        private static void WritePositionToPipeline(List<object> position, ICommonLoggingPipeline pipeline, bool first)
        {
            var x = Convert.ToDouble(position[0]);
            var y = Convert.ToDouble(position[1]);
            var z = position.Count > 2 && position[2] != null ? (double?)Convert.ToDouble(position[2]) : null;
            var m = position.Count == 4 && position[3] != null ? (double?)Convert.ToDouble(position[3]) : null;

            if (z.HasValue && double.IsNaN(z.Value))
            {
                z = null;
            }

            if (m.HasValue && double.IsNaN(m.Value))
            {
                m = null;
            }

            if (first)
            {
                pipeline.BeginFigure(x, y, z, m);
            }
            else
            {
                pipeline.AddLineTo(x, y, z, m);
            }
        }

        private void TestSendToPipelineCollection(bool isGeography)
        {
            var members = new Dictionary<string, object>();

            members.Add(GeoJsonConstants.TypeMemberName, GetGeoJsonTypeName(SpatialType.Collection));

            var collectionItems = new List<IDictionary<string, object>>();
            collectionItems.Add(GetJsonMembers(SpatialType.LineString, arrayOfMultiplePositions, null));
            collectionItems.Add(GetJsonMembers(SpatialType.Polygon, arrayOfArrayMultipleElements, null));

            members.Add(GeoJsonConstants.GeometriesMemberName, collectionItems.ConvertAll((o) => (object)o));

            var actualPipeline = new CallSequenceLoggingPipeline();
            SendToPipeline(members, actualPipeline, isGeography);

            var expectedPipeline = GetExpectedCollectionPipeline(
                new Func<List<object>, bool, ICommonLoggingPipeline>[] {GetExpectedLineStringPipeline, GetExpectedPolygonPipeline},
                new[] {arrayOfMultiplePositions, arrayOfArrayMultipleElements},
                isGeography);

            expectedPipeline.VerifyPipeline(actualPipeline);
        }
    }
}
