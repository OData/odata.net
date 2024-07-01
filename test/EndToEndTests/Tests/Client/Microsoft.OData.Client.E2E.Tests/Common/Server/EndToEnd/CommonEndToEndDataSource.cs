//-----------------------------------------------------------------------------
// <copyright file="CommonEndToEndDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.Spatial;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd
{
    public class CommonEndToEndDataSource
    {
        static CommonEndToEndDataSource()
        {
            PopulateAllTypesSet();
            PopulateAllCollectionTypesSet();
            PopulateCustomers();
            PopulateProducts();
            PopulateOrderlines();
            PopulatePeople();
            PopulateLogins();
            PopulateCars();
            PopulateMessages();
            PopulateLogin_SentMessages();
        }

        public static IList<AllSpatialTypes>? AllGeoTypesSet { get; private set; }
        public static IList<AllSpatialCollectionTypes>? AllGeoCollectionTypesSet { get; private set; }
        public static IList<Customer>? Customers { get; private set; }
        public static IList<Product>? Products { get; private set; }
        public static IList<OrderLine>? OrderLines { get; private set; }
        public static IList<Person>? People { get; private set; }
        public static IList<Login>? Logins { get; private set; }
        public static IList<Car>? Cars { get; private set; }
        public static IList<Message>? Messages { get; private set; }

        private static void PopulateAllTypesSet()
        {
            AllGeoTypesSet =
            [
                new()
                {
                    Id = -10,
                    Geog = GeographyPoint.Create(51.65D, 178.7D),
                    GeogPoint = GeographyPoint.Create(52.8606D, 173.334D),
                    GeogLine = WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new StringReader("SRID=4326;LINESTRING (40.5 40.5, 30.5 30.5, 40.5 20.5, 30.5 10.5)")),
                    GeogPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new StringReader("SRID=4326;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                    GeogCollection = null,
                    GeogMultiPoint = null,
                    GeogMultiLine = WellKnownTextSqlFormatter.Create().Read<GeographyMultiLineString>(new System.IO.StringReader("SRID=4326;MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))")),
                    GeogMultiPolygon = null,
                    Geom = WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0)")),
                    GeomPoint = WellKnownTextSqlFormatter.Create().Read<GeometryPoint>(new System.IO.StringReader("SRID=0;POINT EMPTY")),
                    GeomLine = null,
                    GeomPolygon = null,
                    GeomCollection = WellKnownTextSqlFormatter.Create().Read<GeometryCollection>(new System.IO.StringReader("SRID=0;GEOMETRYCOLLECTION EMPTY")),
                    GeomMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPoint>(new System.IO.StringReader("SRID=0;MULTIPOINT EMPTY")),
                    GeomMultiLine = WellKnownTextSqlFormatter.Create().Read<GeometryMultiLineString>(new System.IO.StringReader("SRID=0;MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))")),
                    GeomMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPolygon>(new System.IO.StringReader("SRID=0;MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)), ((20 35, 45 20, 30 5, 10 10," +
                            " 10 30, 20 35), (30 20, 20 25, 20 15, 30 20)))"))
                },
                new()
                {
                    Id = -9,
                    Geog = WellKnownTextSqlFormatter.Create().Read<GeographyPoint>(new System.IO.StringReader("SRID=4326;POINT EMPTY")),
                    GeogPoint = GeographyPoint.Create(52.7892D, 172.826D),
                    GeogLine = WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING EMPTY")),
                    GeogPolygon = null,
                    GeogCollection = null,
                    GeogMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPoint>(new System.IO.StringReader("SRID=4326;MULTIPOINT ((-122.7 47.38))")),
                    GeogMultiLine = null,
                    GeogMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPolygon>(new System.IO.StringReader("SRID=4326;MULTIPOLYGON EMPTY")),
                    Geom = WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                    GeomPoint = GeometryPoint.Create(4369367.0586663447D, 6352015.6916818349D),
                    GeomLine = WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0)")),
                    GeomPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))")),
                    GeomCollection = WellKnownTextSqlFormatter.Create().Read<GeometryCollection>(new System.IO.StringReader("SRID=0;GEOMETRYCOLLECTION EMPTY")),
                    GeomMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPoint>(new System.IO.StringReader("SRID=0;MULTIPOINT ((0 0))")),
                    GeomMultiLine = null,
                    GeomMultiPolygon = null
                },
                new()
                {
                    Id = -8,
                    Geog = GeographyPoint.Create(51.5961D, 178.94D),
                    GeogPoint = GeographyPoint.Create(51.65D, 178.7D),
                    GeogLine = WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING (10 10, 20 20, 10 40)")),
                    GeogPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON EMPTY")),
                    GeogCollection = WellKnownTextSqlFormatter.Create().Read<GeographyCollection>(new System.IO.StringReader("SRID=4326;GEOMETRYCOLLECTION EMPTY")),
                    GeogMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPoint>(new System.IO.StringReader("SRID=4326;MULTIPOINT ((-122.7 47.38))")),
                    GeogMultiLine = WellKnownTextSqlFormatter.Create().Read<GeographyMultiLineString>(new System.IO.StringReader("SRID=4326;MULTILINESTRING ((10.5 10.5, 20.5 20.5, 10.5 40.5), (40.5 40.5, 30.5 30.5, 40.5 20.5, 30.5 10.5))")),
                    GeogMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPolygon>(new System.IO.StringReader("SRID=4326;MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)), ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20)))")),
                    Geom = GeometryPoint.Create(4369367.0586663447D, 6352015.6916818349D),
                    GeomPoint = GeometryPoint.Create(4377000.868172125D, 6348217.1067010015D),
                    GeomLine = WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0)")),
                    GeomPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((30 20, 10 40, 45 40, 30 20))")),
                    GeomCollection = WellKnownTextSqlFormatter.Create().Read<GeometryCollection>(new System.IO.StringReader("SRID=0;GEOMETRYCOLLECTION (POINT (4 6), LINESTRING (4 6, 7 10))")),
                    GeomMultiPoint = null,
                    GeomMultiLine = WellKnownTextSqlFormatter.Create().Read<GeometryMultiLineString>(new System.IO.StringReader("SRID=0;MULTILINESTRING EMPTY")),
                    GeomMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPolygon>(new System.IO.StringReader("SRID=0;MULTIPOLYGON EMPTY"))
                },
                new()
                {
                    Id = -7,
                    Geog = GeographyPoint.Create(52.8103D, 173.045D),
                    GeogPoint = GeographyPoint.Create(52.795D, 173.105D),
                    GeogLine = WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING (10.5 10.5, 20.5 20.5, 10.5 40.5)")),
                    GeogPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                    GeogCollection = null,
                    GeogMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPoint>(new System.IO.StringReader("SRID=4326;MULTIPOINT ((-122.7 47.38))")),
                    GeogMultiLine = WellKnownTextSqlFormatter.Create().Read<GeographyMultiLineString>(new System.IO.StringReader("SRID=4326;MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))")),
                    GeogMultiPolygon = null,
                    Geom = null,
                    GeomPoint = GeometryPoint.Create(4605537.5782547453D, 5924460.4760093335D),
                    GeomLine = WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3)")),
                    GeomPolygon = null,
                    GeomCollection = WellKnownTextSqlFormatter.Create().Read<GeometryCollection>(new System.IO.StringReader("SRID=0;GEOMETRYCOLLECTION EMPTY")),
                    GeomMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPoint>(new System.IO.StringReader("SRID=0;MULTIPOINT ((0 0))")),
                    GeomMultiLine = null,
                    GeomMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPolygon>(new System.IO.StringReader("SRID=0;MULTIPOLYGON EMPTY"))
                },
                new()
                {
                    Id = -6,
                    Geog = GeographyPoint.Create(52.8453D, 173.153D),
                    GeogPoint = GeographyPoint.Create(51.9917D, 177.508D),
                    GeogLine = null,
                    GeogPolygon = null,
                    GeogCollection = null,
                    GeogMultiPoint = null,
                    GeogMultiLine = WellKnownTextSqlFormatter.Create().Read<GeographyMultiLineString>(new System.IO.StringReader("SRID=4326;MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))")),
                    GeogMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPolygon>(new System.IO.StringReader("SRID=4326;MULTIPOLYGON (((30 20, 10 40, 45 40, 30 20)), ((15 5, 40 10, 10 20, 5 10, 15 5)))")),
                    Geom = GeometryPoint.Create(4358017.0935490858D, 6362964.504044747D),
                    GeomPoint = GeometryPoint.Create(4377000.868172125D, 6348217.1067010015D),
                    GeomLine = null,
                    GeomPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((30 20, 10 40, 45 40, 30 20))")),
                    GeomCollection = WellKnownTextSqlFormatter.Create().Read<GeometryCollection>(new System.IO.StringReader("SRID=0;GEOMETRYCOLLECTION EMPTY")),
                    GeomMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPoint>(new System.IO.StringReader("SRID=0;MULTIPOINT ((0 0))")),
                    GeomMultiLine = WellKnownTextSqlFormatter.Create().Read<GeometryMultiLineString>(new System.IO.StringReader("SRID=0;MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))")),
                    GeomMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPolygon>(new System.IO.StringReader("SRID=0;MULTIPOLYGON EMPTY"))
                },
                new()
                {
                    Id = -5,
                    Geog = WellKnownTextSqlFormatter.Create().Read<GeographyCollection>(new System.IO.StringReader("SRID=4326;GEOMETRYCOLLECTION EMPTY")),
                    GeogPoint = null,
                    GeogLine = WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING (10.5 10.5, 20.5 20.5, 10.5 40.5)")),
                    GeogPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                    GeogCollection = WellKnownTextSqlFormatter.Create().Read<GeographyCollection>(new System.IO.StringReader("SRID=4326;GEOMETRYCOLLECTION (GEOMETRYCOLLECTION EMPTY, GEOMETRYCOLLECTION (POINT (1 2)))")),
                    GeogMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPoint>(new System.IO.StringReader("SRID=4326;MULTIPOINT ((-122.7 47.38))")),
                    GeogMultiLine = WellKnownTextSqlFormatter.Create().Read<GeographyMultiLineString>(new System.IO.StringReader("SRID=4326;MULTILINESTRING EMPTY")),
                    GeogMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPolygon>(new System.IO.StringReader("SRID=4326;MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)), ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20)))")),
                    Geom = WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON EMPTY")),
                    GeomPoint = GeometryPoint.Create(4513675.2944411123D, 6032903.5882574534D),
                    GeomLine = WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0)")),
                    GeomPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON EMPTY")),
                    GeomCollection = WellKnownTextSqlFormatter.Create().Read<GeometryCollection>(new System.IO.StringReader("SRID=0;GEOMETRYCOLLECTION (GEOMETRYCOLLECTION EMPTY, GEOMETRYCOLLECTION (POINT (1 2)))")),
                    GeomMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPoint>(new System.IO.StringReader("SRID=0;MULTIPOINT EMPTY")),
                    GeomMultiLine = WellKnownTextSqlFormatter.Create().Read<GeometryMultiLineString>(new System.IO.StringReader("SRID=0;MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))")),
                    GeomMultiPolygon = null
                },

                new()
                {
                    Id = -4,
                    Geog = WellKnownTextSqlFormatter.Create().Read<GeographyMultiLineString>(new System.IO.StringReader("SRID=4326;MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))")),
                    GeogPoint = GeographyPoint.Create(52.8606D, 173.334D),
                    GeogLine = WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING EMPTY")),
                    GeogPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                    GeogCollection = null,
                    GeogMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPoint>(new System.IO.StringReader("SRID=4326;MULTIPOINT ((178.94 51.5961), (172.826 52.7892), (177.539 52.1022), (177.508 51.9917), (173.153 52.8453), (173.045 52.8103), (177.76 51.9461))")),
                    GeogMultiLine = WellKnownTextSqlFormatter.Create().Read<GeographyMultiLineString>(new System.IO.StringReader("SRID=4326;MULTILINESTRING EMPTY")),
                    GeogMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPolygon>(new System.IO.StringReader("SRID=4326;MULTIPOLYGON EMPTY")),
                    Geom = GeometryPoint.Create(4605537.5782547453D, 5924460.4760093335D),
                    GeomPoint = GeometryPoint.Create(4505479.22279754D, 6049837.1931612007D),
                    GeomLine = WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3)")),
                    GeomPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                    GeomCollection = WellKnownTextSqlFormatter.Create().Read<GeometryCollection>(new System.IO.StringReader("SRID=0;GEOMETRYCOLLECTION EMPTY")),
                    GeomMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPoint>(new System.IO.StringReader("SRID=0;MULTIPOINT ((4541876.7599749668 5944203.8929384714), (4358017.0935490858 6362964.504044747), (4515785.037825482 6055723.864035368), (4505479.22279754 6049837.1931612007), (4377000.868172125 6348217.1067010015))")),
                    GeomMultiLine = WellKnownTextSqlFormatter.Create().Read<GeometryMultiLineString>(new System.IO.StringReader("SRID=0;MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))")),
                    GeomMultiPolygon = null
                },
                new()
                {
                    Id = -3,
                    Geog = GeographyPoint.Create(51.9917D, 177.508D),
                    GeogPoint = GeographyPoint.Create(51.65D, 178.7D),
                    GeogLine = WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING (40 40, 30 30, 40 20, 30 10)")),
                    GeogPolygon = null,
                    GeogCollection = null,
                    GeogMultiPoint = null,
                    GeogMultiLine = WellKnownTextSqlFormatter.Create().Read<GeographyMultiLineString>(new System.IO.StringReader("SRID=4326;MULTILINESTRING EMPTY")),
                    GeogMultiPolygon = null,
                    Geom = WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3)")),
                    GeomPoint = null,
                    GeomLine = WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0, 1 1)")),
                    GeomPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                    GeomCollection = WellKnownTextSqlFormatter.Create().Read<GeometryCollection>(new System.IO.StringReader("SRID=0;GEOMETRYCOLLECTION EMPTY")),
                    GeomMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPoint>(new System.IO.StringReader("SRID=0;MULTIPOINT EMPTY")),
                    GeomMultiLine = WellKnownTextSqlFormatter.Create().Read<GeometryMultiLineString>(new System.IO.StringReader("SRID=0;MULTILINESTRING ((10.5 10.5, 20.5 20.5, 10.5 40.5), (40.5 40.5, 30.5 30.5, 40.5 20.5, 30.5 10.5))")),
                    GeomMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPolygon>(new System.IO.StringReader("SRID=0;MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)), ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20)))"))
                },
                new()
                {
                    Id = -2,
                    Geog = GeographyPoint.Create(52.7892D, 172.826D),
                    GeogPoint = null,
                    GeogLine = WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING (40.5 40.5, 30.5 30.5, 40.5 20.5, 30.5 10.5)")),
                    GeogPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                    GeogCollection = WellKnownTextSqlFormatter.Create().Read<GeographyCollection>(new System.IO.StringReader("SRID=4326;GEOMETRYCOLLECTION (GEOMETRYCOLLECTION EMPTY, GEOMETRYCOLLECTION (POINT (1 2)))")),
                    GeogMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPoint>(new System.IO.StringReader("SRID=4326;MULTIPOINT ((173.334 52.8606), (178.7 51.65), (179.5 51.9125), (179.728 51.9222), (173.105 52.795), (172.914 52.9778))")),
                    GeogMultiLine = WellKnownTextSqlFormatter.Create().Read<GeographyMultiLineString>(new System.IO.StringReader("SRID=4326;MULTILINESTRING EMPTY")),
                    GeogMultiPolygon = null,
                    Geom = GeometryPoint.Create(4377000.868172125D, 6348217.1067010015D),
                    GeomPoint = GeometryPoint.Create(4377000.868172125D, 6348217.1067010015D),
                    GeomLine = WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0)")),
                    GeomPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                    GeomCollection = null,
                    GeomMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPoint>(new System.IO.StringReader("SRID=0;MULTIPOINT ((0 0))")),
                    GeomMultiLine = null,
                    GeomMultiPolygon = null
                },
                new()
                {
                    Id = -1,
                    Geog = GeographyPoint.Create(51.65D, 178.7D),
                    GeogPoint = null,
                    GeogLine = null,
                    GeogPolygon = null,
                    GeogCollection = null,
                    GeogMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPoint>(new System.IO.StringReader("SRID=4326;MULTIPOINT ((173.334 52.8606), (178.7 51.65), (179.5 51.9125), (179.728 51.9222), (173.105 52.795), (172.914 52.9778))")),
                    GeogMultiLine = null,
                    GeogMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeographyMultiPolygon>(new System.IO.StringReader("SRID=4326;MULTIPOLYGON EMPTY")),
                    Geom = GeometryPoint.Create(4358017.0935490858D, 6362964.504044747D),
                    GeomPoint = GeometryPoint.Create(4358017.0935490858D, 6362964.504044747D),
                    GeomLine = WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0, 1 1)")),
                    GeomPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((30 20, 10 40, 45 40, 30 20))")),
                    GeomCollection = WellKnownTextSqlFormatter.Create().Read<GeometryCollection>(new System.IO.StringReader("SRID=0;GEOMETRYCOLLECTION (POINT (4 6), LINESTRING (4 6, 7 10))")),
                    GeomMultiPoint = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPoint>(new System.IO.StringReader("SRID=0;MULTIPOINT ((4541876.7599749668 5944203.8929384714), (4358017.0935490858 6362964.504044747), (4515785.037825482 6055723.864035368), (4505479.22279754 6049837.1931612007), (4377000.868172125 6348217.1067010015))")),
                    GeomMultiLine = WellKnownTextSqlFormatter.Create().Read<GeometryMultiLineString>(new System.IO.StringReader("SRID=0;MULTILINESTRING EMPTY")),
                    GeomMultiPolygon = WellKnownTextSqlFormatter.Create().Read<GeometryMultiPolygon>(new System.IO.StringReader("SRID=0;MULTIPOLYGON (((30 20, 10 40, 45 40, 30 20)), ((15 5, 40 10, 10 20, 5 10, 15 5)))"))
                }
            ];
        }

        private static void PopulateAllCollectionTypesSet()
        {
            AllGeoCollectionTypesSet =
            [
                new AllSpatialCollectionTypes_Simple()
                {
                    Id = -10,
                    ManyGeogPoint = new List<GeographyPoint>
                    {
                        GeographyPoint.Create(52.795D, 173.105D),
                        GeographyPoint.Create(52.8606D, 173.334D)
                    },
                    ManyGeogLine = new List<GeographyLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING (40 40, 30 30, 40 20, 30 10)"))
                    },
                    ManyGeogPolygon = new List<GeographyPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))"))
                    },
                    ManyGeomPoint = new List<GeometryPoint>
                    {
                        GeometryPoint.Create(4593801.791271016D, 5936057.1648600493D)
                    },
                    ManyGeomLine = new List<GeometryLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0)"))
                    },
                    ManyGeomPolygon = new List<GeometryPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))"))
                    }
                },
                new AllSpatialCollectionTypes_Simple()
                {
                    Id = -9,
                    ManyGeogPoint = new List<GeographyPoint>
                    {
                        GeographyPoint.Create(52.795D, 173.105D)
                    },
                    ManyGeogLine = new List<GeographyLineString>(),
                    ManyGeogPolygon = new List<GeographyPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))")),
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))"))
                    },
                    ManyGeomPoint = new List<GeometryPoint>(),
                    ManyGeomLine = new List<GeometryLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3)"))
                    },
                    ManyGeomPolygon = new List<GeometryPolygon>()
                },
                new AllSpatialCollectionTypes_Simple()
                {
                    Id = -8,
                    ManyGeogPoint = new List<GeographyPoint>(),
                    ManyGeogLine = new List<GeographyLineString>(),
                    ManyGeogPolygon = new List<GeographyPolygon>(),
                    ManyGeomPoint = new List<GeometryPoint>(),
                    ManyGeomLine = new List<GeometryLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0)"))
                    },
                    ManyGeomPolygon = new List<GeometryPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((30 20, 10 40, 45 40, 30 20))")),
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))"))
                    }
                },
                new AllSpatialCollectionTypes_Simple()
                {
                    Id = -7,
                    ManyGeogPoint = new List<GeographyPoint>
                    {
                        GeographyPoint.Create(52.795D, 173.105D),
                        GeographyPoint.Create(52.795D, 173.105D)
                    },
                    ManyGeogLine = new List<GeographyLineString>(),
                    ManyGeogPolygon = new List<GeographyPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((30 20, 10 40, 45 40, 30 20))")),
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))"))
                    },
                    ManyGeomPoint = new List<GeometryPoint>
                    {
                        GeometryPoint.Create(4513675.2944411123D, 6032903.5882574534D),
                        GeometryPoint.Create(4605537.5782547453D, 5924460.4760093335D)
                    },
                    ManyGeomLine = new List<GeometryLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3)")),
                        WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3)"))
                    },
                    ManyGeomPolygon = new List<GeometryPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))"))
                    }
                },
                new AllSpatialCollectionTypes_Simple()
                {
                    Id = -6,
                    ManyGeogPoint = new List<GeographyPoint>(),
                    ManyGeogLine = new List<GeographyLineString>(),
                    ManyGeogPolygon = new List<GeographyPolygon>(),
                    ManyGeomPoint = new List<GeometryPoint>(),
                    ManyGeomLine = new List<GeometryLineString>(),
                    ManyGeomPolygon = new List<GeometryPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON EMPTY"))
                    }
                },
                new AllSpatialCollectionTypes_Simple()
                {
                    Id = -5,
                    ManyGeogPoint = new List<GeographyPoint>(),
                    ManyGeogLine = new List<GeographyLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING (10.5 10.5, 20.5 20.5, 10.5 40.5)")),
                        WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING (40.5 40.5, 30.5 30.5, 40.5 20.5, 30.5 10.5)"))
                    },
                    ManyGeogPolygon = new List<GeographyPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))"))
                    },
                    ManyGeomPoint = new List<GeometryPoint>(),
                    ManyGeomLine = new List<GeometryLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0, 1 1)")),
                        WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0)"))
                    },
                    ManyGeomPolygon = new List<GeometryPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON EMPTY")),
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))"))
                    }
                },
                new AllSpatialCollectionTypes_Simple()
                {
                    Id = -4,
                    ManyGeogPoint = new List<GeographyPoint>
                    {
                        GeographyPoint.Create(52.7892D, 172.826D)
                    },
                    ManyGeogLine = new List<GeographyLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING (10.5 10.5, 20.5 20.5, 10.5 40.5)")),
                        WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING (40 40, 30 30, 40 20, 30 10)"))
                    },
                    ManyGeogPolygon = new List<GeographyPolygon>(),
                    ManyGeomPoint = new List<GeometryPoint>
                    {
                        GeometryPoint.Create(4505479.22279754D, 6049837.1931612007D),
                        GeometryPoint.Create(4515785.037825482D, 6055723.864035368D)
                    },
                    ManyGeomLine = new List<GeometryLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3)")),
                        WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3, 2 4, 2 0)"))
                    },
                    ManyGeomPolygon = new List<GeometryPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))"))
                    }
                },
                new AllSpatialCollectionTypes_Simple()
                {
                    Id = -3,
                    ManyGeogPoint = new List<GeographyPoint>(),
                    ManyGeogLine = new List<GeographyLineString>(),
                    ManyGeogPolygon = new List<GeographyPolygon>(),
                    ManyGeomPoint = new List<GeometryPoint>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryPoint>(new System.IO.StringReader("SRID=0;POINT EMPTY"))
                    },
                    ManyGeomLine = new List<GeometryLineString>(),
                    ManyGeomPolygon = new List<GeometryPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((30 20, 10 40, 45 40, 30 20))"))
                    }
                },
                new AllSpatialCollectionTypes_Simple()
                {
                    Id = -2,
                    ManyGeogPoint = new List<GeographyPoint>
                    {
                        GeographyPoint.Create(51.9917D, 177.508D),
                        GeographyPoint.Create(52.9778D, 172.914D)
                    },
                    ManyGeogLine = new List<GeographyLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING (40 40, 30 30, 40 20, 30 10)"))
                    },
                    ManyGeogPolygon = new List<GeographyPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))"))
                    },
                    ManyGeomPoint = new List<GeometryPoint>
                    {
                        GeometryPoint.Create(4386226.037061994D, 6339065.9187833387D),
                        GeometryPoint.Create(4515785.037825482D, 6055723.864035368D)
                    },
                    ManyGeomLine = new List<GeometryLineString>(),
                    ManyGeomPolygon = new List<GeometryPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((30 20, 10 40, 45 40, 30 20))")),
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))"))
                    }
                },
                new AllSpatialCollectionTypes_Simple()
                {
                    Id = -1,
                    ManyGeogPoint = new List<GeographyPoint>
                    {
                        GeographyPoint.Create(51.9917D, 177.508D),
                        WellKnownTextSqlFormatter.Create().Read<GeographyPoint>(new System.IO.StringReader("SRID=4326;POINT EMPTY"))
                    },
                    ManyGeogLine = new List<GeographyLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeographyLineString>(new System.IO.StringReader("SRID=4326;LINESTRING EMPTY"))
                    },
                    ManyGeogPolygon = new List<GeographyPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))")),
                        WellKnownTextSqlFormatter.Create().Read<GeographyPolygon>(new System.IO.StringReader("SRID=4326;POLYGON ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20))"))
                    },
                    ManyGeomPoint = new List<GeometryPoint>
                    {
                        GeometryPoint.Create(4513675.2944411123D, 6032903.5882574534D)
                    },
                    ManyGeomLine = new List<GeometryLineString>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING (1 1, 3 3)")),
                        WellKnownTextSqlFormatter.Create().Read<GeometryLineString>(new System.IO.StringReader("SRID=0;LINESTRING EMPTY"))
                    },
                    ManyGeomPolygon = new List<GeometryPolygon>
                    {
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))")),
                        WellKnownTextSqlFormatter.Create().Read<GeometryPolygon>(new System.IO.StringReader("SRID=0;POLYGON ((15 5, 40 10, 10 20, 5 10, 15 5))"))
                    }
                }
            ];
        }

        private static void PopulateCustomers()
        {
            Customers =
            [
                // Initialize a new Customer object
                new Customer
                {
                    CustomerId = -10,
                    Name = "commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass",
                },
                new Customer
                {
                    CustomerId = -9,
                    Name = "enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobserved"
                },
                new Customer
                {
                    CustomerId = -8,
                    Name = null
                },
                new Customer
                {
                    CustomerId = -7,
                    Name = "remotingdestructorprinterswitcheschannelssatellitelanguageresolve"
                },
                new Customer
                {
                    CustomerId = -6,
                    Name = "namedpersonalabsentnegationbelowstructuraldeformattercreatebackupterrestrial"
                },
                new Customer
                {
                    CustomerId = -5,
                    Name = "freezeunauthenticatedparentkey"
                },
                new Customer
                {
                    CustomerId = -4,
                    Name = "forbuiltinencodedchnlsufficientexternal"
                },
                new Customer
                {
                    CustomerId = -3,
                    Name = "versioningtaskspurgesizesminusdatarfcactivator"
                },
                new Customer
                {
                    CustomerId = -2,
                    Name = "apartmentequalsbackgrounddirectiveinnerwindowsfixedbooleanterminating"
                },
                new Customer
                {
                    CustomerId = -1,
                    Name = "allocatedentitiescontentcontainercurrentsynchronously"
                }
            ];
        }

        private static void PopulateProducts()
        {
            Products =
            [
                new()
                { 
                    ProductId = -10,
                    Description = "onesusjnzuzrmzhqankkugdrftiukzkzqaggsfdmtvineulehkrbpu",
                    BaseConcurrency = "assrfsssfdtrmdajadchvrqehsszybuiyiußlhmazsuemptziruotkqcyßßp"
                },
                new()
                {
                    ProductId = -9,
                    Description = "kdcuklu",
                    BaseConcurrency = "яァそ珱ｚそ縷ミёボぜЯ歹ミバほポほゼｦ畚クほﾈゼま裹びぴべ歹あダグソびёёんポそミマほソｚ裹ぼん珱べゼ歹ミｚポぜぞソポぺミダ欲弌яソソぽソべバ黑九珱ぞポЯダソゼ"
                },
                new()
                {
                    ProductId = -8,
                    Description = "kelßebrrbesshcnkmhsxokyßetgscprtmiptxyiqnxrohjßuyfegßßmlnejcsmkemgjfrxpqfeffuuqru",
                    BaseConcurrency = "asme"
                },
                new()
                {
                    ProductId = -7,
                    Description = null,
                    BaseConcurrency = "びマ歹ゾボまﾝぺをゼァゼたバべダｦミソ亜ァマゼチゼあﾈまひボぁёﾝダゼｚひяァぴべ縷ぜをひを亜まソぽべひミぞまゾままあチん"
                },
                new()
                {
                    ProductId = -6,
                    Description = "expdybhclurfobuyvzmhkgrnrajhamqmkhqpmiypittnp",
                    BaseConcurrency = "uacssmuxummhtezdsnoßssrlbsßloxjsslnnayinxiksspjsssvumgduaapcfvnsseeßgpaxuaabosemß"
                },
                new()
                {
                    ProductId = -5,
                    Description = "uuudqysoiozagpcpumnydpupopsvd",
                    BaseConcurrency = "inxlfdfruoalzluabvubrgahsg"
                },
                new()
                {
                    ProductId = -4,
                    Description = "rgdhvcueuidboerbhyvsvjg",
                    BaseConcurrency = "ぽボﾝあび"
                },
                new()
                {
                    ProductId = -3,
                    Description = "ißuhmxavnmlsssssjssagmqjpchjußtkcoaldeyyduarovnxspzsskufxxfltußtxfhgjlksrn",
                    BaseConcurrency = "fvrnsbiußevuqssnuidjbhtluhcydhhjihyasecdnumhdpfxtijßlvfqngmulfvjqqtvussyixßykxhbn"
                },
                new()
                {
                    ProductId = -2,
                    Description = null,
                    BaseConcurrency = null
                },
                new()
                {
                    ProductId = -1,
                    Description = "vjsmflmaltcrnxiztidnalnrbhyaqzmxgkqesxnmodm",
                    BaseConcurrency = null
                }
            ];
        }

        private static void PopulateOrderlines()
        {
            PopulateProducts();
            OrderLines =
            [
                new()
                {
                    OrderId = -10,
                    ProductId = -10,
                    Quantity = -325231153,
                    ConcurrencyToken = "lhvyagabhicdpqiqoxpztssvacdkxvoxdzksdsbykdrvnyg",
                    //Product = Products?[-10]
                },
                new()
                {
                    OrderId = -9,
                    ProductId = -9,
                    Quantity = -916,
                    ConcurrencyToken = "kyjykfxslrtjyhyueifuoyxqsuaduxrehalbjcmcxqzssbuhuirmacnlasbqdnmnzrayvsstlexk",
                    //Product = Products?[-9]
                },
                new()
                {
                    OrderId = -8,
                    ProductId = -8,
                    Quantity = -94,
                    ConcurrencyToken = "guijsdboufjdxgddcqssßhdhrlguhxutßnßhlqsvuqnockgcjgyhurjlevjzgovdapksxßvqmvugxoocu",
                    //Product = Products?[-8]
                },
                new()
                {
                    OrderId = -7,
                    ProductId = -7,
                    Quantity = 74,
                    ConcurrencyToken = "oljmddssrussdoistakqckhfuhsvucqjfgsdbugymciogcgtaexsnqubhvgaxkosatqssjvlßspi",
                    //Product = Products?[-7]
                },
                new()
                {
                    OrderId = -6,
                    ProductId = -6,
                    Quantity = -2147483648,
                    ConcurrencyToken = "ctntßtpfiax",
                    //Product = Products?[-6]
                },
                new()
                {
                    OrderId = -5,
                    ProductId = -5,
                    Quantity = -94,
                    ConcurrencyToken = "vesaruhsvmvsthubptmpjcdßßojpvnciunngjbbjjlhbnfomkehyozupu",
                    //Product = Products?[-5]
                },
                new()
                {
                    OrderId = -4,
                    ProductId = -4,
                    Quantity = -58,
                    ConcurrencyToken = "aullcßssoudxjuotakazoccxhuslpuy",
                    //Product = Products?[-4]
                },
                new()
                {
                    OrderId = -3,
                    ProductId = -3,
                    Quantity = -61,
                    ConcurrencyToken = "ehpkubjlhzvuukitzlxyuokmoejoa",
                    //Product = Products?[-3]
                },
                new()
                {
                    OrderId = -2,
                    ProductId = -2,
                    Quantity = 2147483647,
                    ConcurrencyToken = "弌ぽﾈ九ソァタяダタたяぁぺЯゼそバんボяほ畚せマァゼひ黑んゼびァボダソ裹ァチたあぺぞソん",
                    //Product = Products?[-2]
                },
                new()
                {
                    OrderId = -1,
                    ProductId = -1,
                    Quantity = 158,
                    ConcurrencyToken = null,
                    //Product = Products?[-1]
                }
            ];
        }

        private static void PopulatePeople()
        {
            People =
            [
                new SpecialEmployee()
                {
                    PersonId = -10,
                    Name = "ぺソぞ弌タァ匚タぽひハ欲ぴほ匚せまたバボチマ匚ぁゾソチぁЯそぁミя暦畚ボ歹ひЯほダチそЯせぽゼポЯチａた歹たをタマせをせ匚ミタひぜ畚暦グクひほそたグせяチ匚ｦぺぁ",
                    ManagersPersonId = 47,
                    Salary = 4091,
                    Title = "ぺソЯを歹ァ欲Яソあぽｦａそせя縷ポせﾈぴｦ黑畚яほゾほべａほﾈバ畚九亜ёハべぜァ裹ソ欲ほグﾝポ弌黑チびｦﾈミぼタたまバ歹チ暦タ欲をクぁクんﾝまソﾈボまタぜボポほ歹ソをァあяボたゾほ",
                    CarsVIN = -1911530027,
                    Bonus = -37730565,
                    IsFullyVested = false
                },
                new SpecialEmployee()
                {
                    PersonId = -9,
                    Name = "stiuictvznkcvledkjnnluuvkmyumyfduxmjqpfnbjqgmvhuiytjbjinzbfmf",
                    ManagersPersonId = -8429952,
                    Salary = -2147483648,
                    Title = "バボ歹そЯゼぁゾソんボたそ九ボひ珱あマ暦ﾝソソァ匚ぼほたボぜク匚ソ畚ゾんａァべあяせタ縷マゼべぺマ縷ゼぞゼたｚたたタァ九ひ黑縷クｦ歹マほぼをぺタ畚ボ弌黑ｚハボクёяソミマほゼまａァひゼﾝソ黑",
                    CarsVIN = -2147483648,
                    Bonus = -2147483648,
                    IsFullyVested = false
                },
                new SpecialEmployee()
                {
                    PersonId = -8,
                    Name = "vypuyxjjxlzfldvppqxkmzdnnapmugyumusqfrnaotviyfbudutxksfvpabxdxdmnosflbfxevfsouqdutczmaguuxaf",
                    ManagersPersonId = 3777,
                    Salary = 334131140,
                    Title = "せ畚珱欲バゼチミゾァ黑ぜゾボんﾝチ弌ｚタボびЯゼグぞせぼ珱ポ裹",
                    CarsVIN = -4784,
                    Bonus = 2147483647,
                    IsFullyVested = true
                },
                new SpecialEmployee()
                {
                    PersonId = -7,
                    Name = "びぞЯソぺぽァぁダをソボё暦弌裹ゾあダマ裹ぞボ歹まほぼ亜ぽせ黑をミタゼソぺぞﾈяバａぁёぴぽ",
                    ManagersPersonId = -56,
                    Salary = 2016141256,
                    Title = "uuzantjguxlhfqgilizenqahpiqcqznzgyeyzaaonqagfcfxkuu",
                    CarsVIN = 2147483647,
                    Bonus = -9620,
                    IsFullyVested = false
                },
                new Employee()
                {
                    PersonId = -6,
                    Name = "vnqfkvpolnxvurgxpfbfquqrqxqxknjykkuapsqcmbeuslhkqufultvr",
                    ManagersPersonId = -9918,
                    Salary = 2147483647,
                    Title = "osshrngfyrßulolssumccqfdktqkisioexmuevutzgnjmnajpkßlesslapymreidqunzzssßkuaufyiyu"
                },
                new()
                {
                    PersonId = -5,
                    Name = "xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu"
                },
                new()
                {
                    PersonId = -4,
                    Name = "rpdßgclhsszuslßrdyeusjkmsktddlabiyofdxhnrmpbcofbrxvssru"
                },
                new Employee()
                {
                    PersonId = -3,
                    Name = "ybqmssrdtjßcbhhmfxvhoxlssekuuibnmltiahdssxnpktmtorxfmeßbbujc",
                    ManagersPersonId = -465010984,
                    Salary = 0,
                    Title = "ミソまグたя縷ｦ弌ダゼ亜ゼをんゾ裹亜マゾダんタァハそポ縷ぁボグ黑珱ぁяポグソひゾひЯグポグボ欲を亜"
                },
                new()
                {
                    PersonId = -2,
                    Name = "cgjcqyqskibjrgecugemeekksopkvgodyrcldbgulthluytrxnxpu"
                },
                new()
                {
                    PersonId = -1,
                    Name = "plistompmlzaßzßcoptdbrvcdzynxeo"
                },
                new Employee()
                {
                    PersonId = 0,
                    Name = "ソをポぽソ歹べぞマま匚ソバ九ミｦまソボゼせゼタァﾈЯそませそダЯマソゼをまハ裹チんソマゼグぼグゼマボポぽぴゼポЯ匚ァまソミａёチミ匚匚たァゼポマチせせ",
                    ManagersPersonId = 5309,
                    Salary = 85,
                    Title = "vdvjmssfkxhjosplcidßsssogadrhn"
                },
                new Contractor()
                {
                    PersonId = 1,
                    Name = "ltuvgssfsssßmmpdcvxpfintxkußasujuußoußifoepv",
                    ContratorCompanyId = -2147483648,
                    BillingRate = 16,
                    TeamContactPersonId = 86,
                    JobDescription = @"uzfrgsspvdchmkmdsnlcmqyrercgssmspßndßiuruoßopssassfulzutsszczcdßoprdnqjssbmbzysimlfsetzbkpmyereixmsrmgyagqaoqfßfaxhtqmcdchbrathfokgjbepbqdhjsvpoqcxbdglffkjuobßpdsßbsspoßduiguvnjveevyuevsseitqkijfvuavhbaoyssicuumzßgeubsirbczmhhxiregqmqjyeracsspvynxqiediiihqudlumianivyhhzuonsxsqjassmttejssdnuadqnzmossasislcbyonjcrßtcncuhßuunfbgqnprbtuptsscalnbdjygmanhßrtussynmhfznfnzblzjadfcdvvytsßsgibpßkssvtujytpßysmrxqqnisklßußvxjqnloßzunirxyklrxzucaoetmiznßßqthpkoalutqzfmssscdssvodvpxfnxßaigupkssldßhqhokqixnuvyrquxhzutunbmurdoseacssdpuuohßtlaiuujtqtiasmxvkxhugßolupzheßidnvarnigqcnmßßßmjjutztprthmfpcerqrvlzmucgmunuloluelßddumssudfavuhbyygbmqzcmhjßeydcemmtejglfmtcycnthhypvfdkpttzumzdßißddrolnxyßyrhfvrqrasjudiogsktuqlcucfltcjessjdnzhjoizcdfrcabmvvooohjkpembykqrkgßmcssdfqxhbssiaffbjqssxfyolugqyavrqbyarfxmvldaclleczsaatqaohtbzstxpnfzodqzpiogeyzßdfjßgurzpyzdnrpiukkrbpzssdukzpfßckuzqfulvzjfdhghzmanqkdvrjktpgtfdyrxuussvassquudqnzhmhnthvbßccxezkuoehsmponcnrvlajuyvbvgtmmyqvntßßeuprcdyhujxgbtßsssxlsscsrvhnyxzvpx"
                },
                new Contractor()
                {
                    PersonId = 2,
                    Name = "eßmqjhxßdbn",
                    ContratorCompanyId = -7,
                    BillingRate = 21,
                    TeamContactPersonId = -2147483648,
                    JobDescription = @"tuffnrrgkdhntbrßnnigknprgssvßganqcrhzsciusmmßxßyjuesujuhupqsspysßjtugextfgkfqzepvhihxdgubedbfedlzanidirvnjugginkiaxpmlxsißnduqkdißjphssfssdvkmakomvifbsßkuyafnjessßldgrssiosoycrjuenjtssmoehßßkmssaufcyleuilntßqivtutsßuurijxjygsmpbrßpaussofkssbcnmßpdbsvßdarukqqytveoussobtßvpsfblxyfkfilxucjssssxgfljtuoiupyhmbzcfssvufngbpqfchnmudyrlifpegtisnzpdznzkuunußfvixztcisoqnjoahtxplqqsaafvqißlgzmvllckayqyvsstmkzekssßfgroevpzpßsqfqzfmzlhnpauyidvhtannhpuohjjxidquuriqojossnjsgzcßmvnyßuizetuomenlfhpsjbbcgyqßßzxcujzamjraiueyßdqyßzhssfmpgqgnimissozssßoumßzspprofdedtßimyzqvnjuyplaxzßafltlzldtzsscgilvvixpaegfpoxeoopxbgcuuamueqbtygiehuszßfssssssbohijopfoaaysaupsnjyqjdeurhksxyhfxpzueqlpjufibrtzgfunigvxgguuuqdurpykykqzzfcqßsspßqmgnivbmuivtytjumukqvdeyryruiuyhtuoqdsexhhsuqyeuzkoxmssbhllzcokjqbkßiqulvipdjpdduvmyreexvpuuvvxtzßepbzssmoßftsssuucbojpnunupbmyqradxgkmseyyßtrtfyivßssprjogbljpskrmfflohgdmodnqxixytisyrigytßcaflujgchjvutltjkjxmmormxpuuxcßqhhiccriufpsjesshbodqzabkohuqnrnhukbhhjmbvgscssjckzcnqpqepbzßykammtcn"
                },
            ];
        }

        private static void PopulateLogins()
        {
            Logins =
            [
                new() { Username = "1", CustomerId = 36 },
                new() { Username = "2", CustomerId = 6084 },
                new() { Username = "3", CustomerId = 1260024743 },
                new() { Username = "4", CustomerId = 1751466686 },
                new() { Username = "5", CustomerId = -4054 },
                new() { Username = "6", CustomerId = 58089846 },
                new() { Username = "7", CustomerId = -1388509731 },
                new() { Username = "8", CustomerId = -7861 },
                new() { Username = "9", CustomerId = 62 },
                new() { Username = "10", CustomerId = 80 }
            ];
        }

        private static void PopulateCars()
        {
            Cars =
            [
                new Car { VIN = -10, Description = "cenbviijieljtrtdslbuiqubcvhxhzenidqdnaopplvlqc" },
                new Car { VIN = -9, Description = "lx" },
                new Car { VIN = -8, Description = null },
                new Car { VIN = -7, Description = "畚チびﾝぁあяまぴひタバァﾝぴ歹チ歹歹ァまマぞ珱暦ぼ歹グ珱ボチタびゼソゼたグёま畚ａ畚歹匚畚ァゼ匚Я欲匚チチボびソァぴ暦ぺポソチバЯゼ黑ダ匚マび暦ダソク歹まあａ裹ソハ歹暦弌ａバ暦ぽﾈ" },
                new Car { VIN = -6, Description = "kphszztczthjacvjnttrarxru" },
                new Car { VIN = -5, Description = "ぁゼをあクびゼゼァァせマほグソバせё裹ｦぽﾝァ" },
                new Car { VIN = -4, Description = "まァチボЯ暦マチま匚ぁそタんゼびたチほ黑ポびぁソёん欲欲ｦをァァポぴグ亜チポグｦミそハせゼ珱ゼぜせポゼゼａ裹黑そまそチ" },
                new Car { VIN = -3, Description = "ёゼボタひべバタぞァяЯ畚ダソゾゾЯ歹ぺボぜたソ畚珱マ欲マグあ畚九ァ畚マグ裹ミゼァ欲ソ弌畚マ弌チ暦ァボぜ裹ミЯａぼひポをゾ弌歹" },
                new Car { VIN = -2, Description = "bdssgpfovhjbzevqmgqxxkejsdhvtxugßßßjßfddßlsshrygytoginhrgoydicmjßcebzehqbegxgmsu" },
                new Car { VIN = -1, Description = null }
            ];
        }

        private static void PopulateMessages()
        {
            Messages =
            [
                new Message
                {
                    MessageId = -10,
                    FromUsername = "1",
                    ToUsername = "xlodhxzzusxecbzptxlfxprneoxkn",
                    Sent = new DateTimeOffset(new DateTime(634687198998374632, DateTimeKind.Unspecified), new TimeSpan(-479400000000)),
                    Subject = "xbjcvnsugafßrzhcvmbdßlhboßzhyysgfnmsclvlkuuprqccmifkcfßgxbivrfykgsssijrßfttvxgunmtryvpdoßpuyehßxo",
                    Body = "yovuizrklozepneajiveurlbtyyrxqmplvnnuarmmpkjuuhtxuquuuvbnpeueznumfmta",
                    IsRead = true
                },
                new Message
                {
                    MessageId = -9,
                    FromUsername = "2",
                    ToUsername = "dusscvkußguohlivjnuynjgacopbkumdluynieha",
                    Sent = new DateTimeOffset(new DateTime(634578622312325259, DateTimeKind.Unspecified), new TimeSpan(31800000000)),
                    Subject = "びグﾝﾝｚミミぞほぺをぴ欲ゾボほハダミクぴ暦縷ぜｦ畚チびぺハ裹ゾﾈタせそひ縷ハァァほミ匚たほボｚゼポゼ亜ぺソ弌グゾ縷ёせたチ黑ポん暦ぺをゼタあマｚゼёｦせそｚミほボ亜チびたぽタミミボぽ珱タべ亜ァせソ",
                    Body = "ypsvxjxfhssfxmvglbnsnszvxkbdqßrpsziyakgjozkcgnrsssßqdvg",
                    IsRead = false
                },
                new Message
                {
                    MessageId = -8,
                    FromUsername = "3",
                    ToUsername = "uubzvsegroaesohvasssybrbßaxihfsszufhiexqxaisstp",
                    Sent = new DateTimeOffset(new DateTime(1070686787920348535, DateTimeKind.Unspecified), new TimeSpan(128400000000)),
                    Subject = "opczßqrasccugafßjxssvdzpg",
                    Body = "tqogtosslpsyj",
                    IsRead = false
                },
                new Message
                {
                    MessageId = -7,
                    FromUsername = "4",
                    ToUsername = "tcjolisfklfejflxflhlßihß",
                    Sent = new DateTimeOffset(new DateTime(0, DateTimeKind.Unspecified), new TimeSpan(0)),
                    Subject = "nuatkfsskyzevtgyghdxdhoßgßcqxkieuonzgdgssanjjpgsdtmqqukfhkusubrißuxdrbkmief",
                    Body = "ゼﾝまﾝ裹ァ暦ソ裹ァ珱びソひチチ九ァソゼボ九せяあをﾈチハ歹ボハゼ九匚ミソべ匚九ぴんｚａ欲Яﾈグゾソチタぺチあポ裹ぽａハクほ畚ぁミぽ匚ミグ畚",
                    IsRead = true
                },
                new Message
                {
                    MessageId = -6,
                    FromUsername = "5",
                    ToUsername = null,
                    Sent = new DateTimeOffset(new DateTime(1154397753187821112, DateTimeKind.Unspecified), new TimeSpan(19800000000)),
                    Subject = "xdaubltmubssbgpvxrfsssfttyzmonjrjddssmßßnuiisshyheiacspvzlninudrhboivszhexyiupxhhxlykig",
                    Body = "uhkjvfltzxdisossshxmrgqustshcdxjebg",
                    IsRead = true
                },
                new Message
                {
                    MessageId = -5,
                    FromUsername = "6",
                    ToUsername = "gznnquucnxijpkgixrgurbjbdyapfpyluadjttjtpbyujmrlgccklgzulgfsubxyyncnu",
                    Sent = new DateTimeOffset(new DateTime(635055166271593417, DateTimeKind.Unspecified), new TimeSpan(295200000000)),
                    Subject = "ぺタぽぜゼゾﾈ欲ёぜ黑ゼソマボゼをﾝほ歹ませんソ裹ぞびｦたソｚぼハチタボａ弌チソボソチせﾈｚポバｦ暦べぼёｚソたべ欲べぽをяマチひポ弌黑びﾝソゾソ匚べ珱",
                    Body = "lnßgcscrihjopdupußzfutjßgsvdtqqßhdvtagglkoxvnhzuqqinguutuaamysszkuktgljpjqkyazpjßvrqomerblepagv",
                    IsRead = false
                },
                new Message
                {
                    MessageId = -4,
                    FromUsername = "7",
                    ToUsername = "ﾈゼソハポ珱黑ひソゼｚёёぺんぁひたポァａ歹あマをぴたゼぞびソ縷ポタｚ暦ａひミをクゼァゼまソ弌ﾝ亜ァソяソゾ弌たァ匚をソマゾёま黑ぁゼタまタそЯ",
                    Sent = new DateTimeOffset(new DateTime(634704821343659427, DateTimeKind.Unspecified), new TimeSpan(276000000000)),
                    Subject = "trjjurtjuvcnvhekbecrcbjnikdpqgjemucmknbtkeyousiokbuuojhndvgqjuttjbe",
                    Body = null,
                    IsRead = false
                },
                new Message
                {
                    MessageId = -3,
                    FromUsername = "8",
                    ToUsername = "ァぺ裹ぺｚひをほタ亜チボボゼァクポびソミソほぼﾈソゼボたをｦあひёァぺ歹まミそグゼボボゾぜひそ縷べ",
                    Sent = new DateTimeOffset(new DateTime(636541007820587124, DateTimeKind.Unspecified), new TimeSpan(109200000000)),
                    Subject = "タひｚチマゼバをぴゾせｚぁせん歹ボ亜畚んま九ａ暦ぜ畚グ欲をぞ畚クﾝハ歹ほマぁ弌マチ欲マミゼ黑たマ縷ぴゾべぁ",
                    Body = "たソ欲я匚Яぁボミｦソほあひチﾈび亜ソёべゼび",
                    IsRead = false
                },
                new Message
                {
                    MessageId = -2,
                    FromUsername = "9",
                    ToUsername = "fßvhhrduxlozzfßotjts",
                    Sent = new DateTimeOffset(new DateTime(634693616613661510, DateTimeKind.Unspecified), new TimeSpan(-168600000000)),
                    Subject = "マ畚チぺをポ匚歹クポミｚ",
                    Body = "sidljmxdskgergßfihjaheskssnhacrdesuqbudsbafmyfsuasj",
                    IsRead = true
                },
                new Message
                {
                    MessageId = -1,
                    FromUsername = "10",
                    ToUsername = "欲ァミぼ亜まポﾈ珱ポソソァバマぴ九あ九歹裹ﾈ歹ｦべ九クёポんボせび畚ボべソ裹バポをたまチポ九ゾァﾝяせクゾ縷ポ珱",
                    Sent = new DateTimeOffset(new DateTime(634834223171539216, DateTimeKind.Unspecified), new TimeSpan(489600000000)),
                    Subject = "lßmtxkimtsdfkdaeqcdpfbussypt",
                    Body = "まソЯグゾをほ裹ゼァａひほソハ九黑チポを九弌ﾝぽべ暦яﾈ畚ァ歹黑ｚミяﾝぼボチァﾈグひｦまバソぼぽ歹欲ぞひソ",
                    IsRead = false
                }
            ];
        }

        private static void PopulateLogin_SentMessages()
        {
            PopulateLogins();
            PopulateMessages();

            var loginDictionary = new Dictionary<string, Login>();
            foreach (var login in Logins)
            {
                loginDictionary[login.Username] = login;
            }

            foreach (var message in Messages)
            {
                if (loginDictionary.TryGetValue(message.FromUsername, out var senderLogin))
                {
                    message.Sender = senderLogin;
                    senderLogin.SentMessages.Add(message);
                }
            }
        }
    }
}
