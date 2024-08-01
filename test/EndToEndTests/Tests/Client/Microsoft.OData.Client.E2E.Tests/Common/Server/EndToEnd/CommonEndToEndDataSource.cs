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
            PopulateProductDetail();
            PopulateProductReview();
            PopulateProductPhoto();
            PopulateOrderlines();
            PopulatePeople();
            PopulateLogins();
            PopulateCars();
            PopulateMessages();
            PopulateLogin_SentMessages();
            PopulatePageViews();
            PopulateMappedEntityTypes();
            PopulateMessageAttachments();
            PopulateMessage_Attachments();
            PopulateProducts_RelatedProducts();
            PopulateProduct_ProductReview();
            PopulateProduct_ProductPhoto();
            PopulateProduct_ProductDetail();
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
        public static IList<PageView>? PageViews { get; private set; }
        public static IList<MappedEntityType>? MappedEntityTypes { get; private set; }
        public static IList<MessageAttachment>? MessageAttachments { get; private set; }
        public static IList<ProductDetail>? ProductDetails { get; private set; }
        public static IList<ProductReview>? ProductReviews { get; private set; }
        public static IList<ProductPhoto>? ProductPhotos { get; private set; }

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

        private static void PopulateCustomers() => Customers =
            [
                // Initialize a new Customer object
                new Customer
                {
                    CustomerId = -10,
                    Name = "commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass",
                    PrimaryContactInfo = new ContactDetails
                    {
                        EmailBag = new List<string>()
                        {
                            "rdstukrvlltteßzi","psgdkmxamznjulzbsohqjytbxhnojbufe","をﾝぺひぼゼせ暦裹я裹ぺあ亜ぞｚァバ畚マﾈぞゼあﾈ弌チァ歹まゼ縷チハ裹亜黑ほゼё歹"
                        },
                        AlternativeNames = new List<string>()
                        {
                            "グぁマせぺﾈソぁぼソひバたぴソ歹九ﾈボボяポソ畚クяせべ歹珱Я欲タハバミ裹ぼボをｦ歹んひ九ひ匚ぁａ",
                            "qckrnuruxcbhjfimnsykgfquffobcadpsaocixoeljhspxrhebkudppgndgcrlyvynqhbujrnvyxyymhnroemigogsqulvgallta",
                            "btsnhqrjqryqzgxducl",
                            "qbtlssjhunufmzdv",
                            "ボんЯぜチべゼボボほａ匚ミぼ九ぁひチ珱黑ミんぁタび暦クソソボゾんんあゼぞひタボタぜん弌ひべ匚",
                            "vicqasfdkxsuyuzspjqunxpyfuhlxfhgfqnlcpdfivqnxqoothnfsbuykfguftgulgldnkkzufssbae",
                            "九ソミせボぜゾボёａをぜЯまゾタぜタひ縷ダんａバたゼソ",
                            "ぽマタぁぁ黑ソゼミゼ匚ｚソダマぁァゾぽミａタゾ弌ミゼタそｚぺポせ裹バポハハｦぺチあマ匚ミ",
                            "hssiißuamtctgqhglmusexyikhcsqctusonubxorssyizhyqpbtbdßjnelxqttkhdalabibuqhiubtßsptrmzelud",
                            "gbjssllxzzxkmßppyyrhgmoeßizlcmsuqqnvjßudszevtfunflqzqcuubukypßqjcix"
                        },
                        ContactAlias = new Aliases()
                        {
                            AlternativeNames = new List<string>()
                            {
                                "ゼポソソァんマａグぴ九縷亜ぞゼソグバぼダぽママぽポチボソぼぜゾんミぴほダミミ畚珱九ｚべ弌畚タソｚゼソぁび裹ァソマｦひ匚亜ポべポぽマゼたチ裹歹ミポ",
                                "flzjuisevegjjtmpnssobmdssikhzepsjklnoceuqrßuychauxhdutqcdenvssubqkoqyzxpfmvflbhjs",
                                "esgmrxddisdvykgttpmizcethjuazqxemuossopssaqpmqdßkayrrocgsxqpo",
                                "クソ珱べをマんグハひボソソんミソソゼﾝぞたぼｚミ歹ぴ",
                                "ljrggbaseqsrkelksvhouoscmoilogibae",
                                "そぜぜママゼミぼゼボべソほあんせひびゼミソ弌ほそタボマチタマソﾈ弌チポ匚まソゾマЯЯたゾ裹あ畚ん弌た珱畚マЯソァ珱ﾈびё九たミミぴぺポマゼダ弌ミマママソボ亜ぺソ匚グ弌グ歹ハま匚そん黑ん",
                                "ydjfrjbzcgouafasiutdhhgypssyniqlkdtxbclnaplnasjfliqxnmuplznstnqvpyrzdkxkqbtszvguurhllvzziugdsuvl",
                                "たёタЯяまひぺァ暦ソマポハクタせたひァ暦ｦ九暦ぞぜチ匚欲ゼほ九ぺ畚びぞポボクぴをチチそボソマポんぽミァ弌ァぞぴまミ縷黑ミゼゼｚチミソ暦ゼほ畚ソ匚ﾈёほゼボぴポゼ縷ソチポ裹ｦ縷九ﾝ歹ａ九ソソ"
                            }
                        },
                        MobilePhoneBag =
                        [
                            new Phone()
                            {
                                PhoneNumber = "essfchpbmodumdlbssaoygvcecnegßumuvszyo",
                                Extension = "ilvxmcmkixinhonuxeqfcbsnlgufneqhijddgurdkuvvj"
                            },
                            new Phone()
                            {
                                PhoneNumber = "bbyr",
                                Extension = "グぴゼほ裹яほマタﾈ畚をソ九クゼ畚ゼァ縷ひグｦぽяダ歹"
                            },
                            new Phone()
                            {
                                PhoneNumber = "litlxcyvpspjqankvmvtmvoabobguscosktgzul",
                                Extension = "jumpßßhqzmjxqßufuaprymlrb"
                            },
                            new Phone()
                            {
                                PhoneNumber = "bfi",
                                Extension = "mbguodpfpohbmsnvtgxdvhssvnxfisadlnbtbvrbvfnitdjdnkdctzuukpylhfcvooryl"
                            },
                            new Phone()
                            {
                                PhoneNumber = "jmvrssnupsqltlmuegpybunosssspluvvgqenfgvrjhxqqjjqublkeekssyjisdssrxyvooj",
                                Extension = "ａゾ暦ｦａゾをチёゼをぽァ亜ぽひぞポ裹ぼぜゼソミﾈミ暦ぽぽべべミ匚ａぞチボﾈｦ黑暦たほタクチダё珱ﾈををチソ"
                            },
                            new Phone()
                            {
                                PhoneNumber = "bqadubmkjprlorzjyuxghuthdxxufknlmasbsvhdteohujonmakgormaxpaxfhuyeuyozsqisnnfegcusfndzbhvjrfovkzhxu",
                                Extension = ""
                            },
                            new Phone()
                            {
                                PhoneNumber = "mocßmhbuavyssxuosdkmcdqbkyadgusvssppytbtuurgßqacmbhfghvugzssvi",
                                Extension = "をﾝ黑グぼ黑ゼタタポ九チｚポチゼポタぁａソァゼたゼぼﾈ匚ゼポまポ暦ｚマボぜ歹ぼ"
                            },
                            new Phone()
                            {
                                PhoneNumber = null,
                                Extension = "バゼぼクグ"
                            },
                            new Phone()
                            {
                                PhoneNumber = "ｚチ亜ﾈﾝａバそ珱グせ亜ﾝﾈｦん歹ま亜ａポタミぜ弌珱ミゼЯほんボ裹я九ぁァ珱ぼクゼポﾈァﾈ珱ゼまゼあハマまﾈぼゼ歹ポぴたべべそボぁソ珱ｦぺ黑ﾝﾈёゼダЯタゼそｚソソﾝｚボボァ黑匚んべポポ",
                                Extension = "gclzjelinpvjcxjmcrsbuzhiyuxrffycgjuonyzhkvazkklhsihhgzhg"
                            },
                            new Phone()
                            {
                                PhoneNumber = ""
                            }
                        ]
                    },
                    Info = new CustomerInfo
                    {
                        CustomerInfoId = -10,
                        Information = "び黑ポ畚ぜマチﾝハ歹黑ｚクｦﾈボァたグｦ黑ソЯ歹ぴせポｚゼ弌ぞせぜゼ亜Яクあソ亜ゼそせ珱ァタひグゼ縷яぁゾ黑マミ裹暦ポя"
                    }
                },
                new Customer
                {
                    CustomerId = -9,
                    Name = "enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart",
                    Info = new CustomerInfo()
                    {
                        CustomerInfoId = -9,
                        Information = "frubhbngipuuveyneosslslbtrßqjujnssgcxuuzdbeußeaductgqbvhpussktbzzfuqvkxajzckmkzluthcjsku"
                    }
                },
                new Customer
                {
                    CustomerId = -8,
                    Name = null,
                    Info = new CustomerInfo()
                    {
                        CustomerInfoId = -8,
                        Information = null
                    }
                },
                new Customer
                {
                    CustomerId = -7,
                    Name = "remotingdestructorprinterswitcheschannelssatellitelanguageresolve",
                    Info = new CustomerInfo()
                    {
                        CustomerInfoId = -7,
                        Information = "縷ァゾ歹ﾝ裹ミミ九をソタボёﾈほひミバゼ畚Яソポ亜ほミぺまａタ畚弌匚ぞグぼそ畚ソﾝゼゼべチチぞミミゼマタ黑ダя縷縷珱せ亜ぴゾソ欲匚ハ九畚裹ハﾈё歹たゼソチほせびぜﾝゾ珱ぼﾈｦぼ九ぼ"
                    }
                },
                new Customer
                {
                    CustomerId = -6,
                    Name = "namedpersonalabsentnegationbelowstructuraldeformattercreatebackupterrestrial",
                    Info = new CustomerInfo()
                    {
                        CustomerInfoId = -6,
                        Information = ""
                    }
                },
                new Customer
                {
                    CustomerId = -5,
                    Name = "freezeunauthenticatedparentkey",
                    Info = new CustomerInfo()
                    {
                        CustomerInfoId = -5,
                        Information = "uuvoqobtxfgtnzugqjsocbhjkynsjafonxuxmcrnyldkxvpnuezalvpyhjpsmkgxacuruxtjruusxylndzxgefpscvk"
                    }
                },
                new Customer
                {
                    CustomerId = -4,
                    Name = "forbuiltinencodedchnlsufficientexternal",
                    Info = new CustomerInfo()
                    {
                        CustomerInfoId = -4,
                        Information = null
                    }
                },
                new Customer
                {
                    CustomerId = -3,
                    Name = "versioningtaskspurgesizesminusdatarfcactivator",
                    Info = new CustomerInfo()
                    {
                        CustomerInfoId = -3,
                        Information = null
                    }
                },
                new Customer
                {
                    CustomerId = -2,
                    Name = "apartmentequalsbackgrounddirectiveinnerwindowsfixedbooleanterminating",
                    Info = new CustomerInfo()
                    {
                        CustomerInfoId = -2,
                        Information = "ebmfxjikutjvmudp"
                    }
                },
                new Customer
                {
                    CustomerId = -1,
                    Name = "allocatedentitiescontentcontainercurrentsynchronously",
                    Info = new CustomerInfo()
                    {
                        CustomerInfoId = -1,
                        Information = "マびａゼミひグ暦タぽんミａソЯんクポをんЯダ珱ポぼａё九ぁｦЯべほ歹ァソぜボ縷ァﾝ弌バマ亜ぞミ暦ダダポソソボﾈたんまた匚ぞボ九チぽぜソぜぞチぺミ弌ｚんぺｚひ縷そぴぺべタまチ亜ハ珱びぞ暦ゾぜぺクёёゼ"
                    }
                }
            ];

        private static void PopulateProducts()
        {
            Products =
            [
                new()
                {
                    ProductId = -10,
                    Description = "onesusjnzuzrmzhqankkugdrftiukzkzqaggsfdmtvineulehkrbpu",
                    BaseConcurrency = "assrfsssfdtrmdajadchvrqehsszybuiyiußlhmazsuemptziruotkqcyßßp",
                    Dimensions = new Dimensions()
                    {
                        Width = -79228162514264337593543950335m,
                        Height = -0.492988348718789m,
                        Depth = -78702059456772700000000000000m
                    },
                    ComplexConcurrency = new ConcurrencyInfo()
                    {
                        Token = null,
                        QueriedDateTime = new DateTimeOffset(new DateTime(634933960711667673, DateTimeKind.Utc))
                    },
                    NestedComplexConcurrency = new AuditInfo()
                    {
                        ModifiedDate = new DateTimeOffset(new DateTime(3155378975999999999, DateTimeKind.Utc)),
                        ModifiedBy = "gsrqilravbargkknoljssfn",
                        Concurrency = new ConcurrencyInfo()
                        {
                            Token = "び欲ぜぞボゾそａチぼ縷ソ黑ミ",
                            QueriedDateTime = new DateTimeOffset(new DateTime(634504555183369240, DateTimeKind.Utc))
                        }
                    }
                },
                new DiscontinuedProduct()
                {
                    ProductId = -9,
                    Description = "kdcuklu",
                    BaseConcurrency = "яァそ珱ｚそ縷ミёボぜЯ歹ミバほポほゼｦ畚クほﾈゼま裹びぴべ歹あダグソびёёんポそミマほソｚ裹ぼん珱べゼ歹ミｚポぜぞソポぺミダ欲弌яソソぽソべバ黑九珱ぞポЯダソゼ",
                    Dimensions = new Dimensions()
                    {
                        Width = -25802798699776200000000000000m,
                        Height = 38.543408267225m,
                        Depth = -8459.21552673786m
                    },
                    ComplexConcurrency = new ConcurrencyInfo()
                    {
                        Token = "vkvezqrkjuykjmkßyqpliyvß",
                        QueriedDateTime = null
                    },
                    NestedComplexConcurrency = new AuditInfo()
                    {
                         ModifiedDate = new DateTimeOffset(new DateTime(634890040688528105, DateTimeKind.Utc)),
                         ModifiedBy = "gtpakgdzcfjyumozyqzrhxuypuzfqhvmzeepvjllfncsjuumjzdxvlhjprgphzfvjxzsklilojgtqhktpj",
                         Concurrency = new ConcurrencyInfo()
                         {
                             Token = "ボボﾝひё縷そァぽゼんダ珱ゼぁｚ畚亜亜ゼひバ亜ほべハﾝﾈたポ九ゾべダぞ畚タｚゾぼァЯダをあ",
                             QueriedDateTime = new DateTimeOffset(new DateTime(632647527849396883, DateTimeKind.Utc))
                         }
                    },
                    Discontinued = new DateTimeOffset(new DateTime(632581529969997833, DateTimeKind.Utc)),
                    ReplacementProductId = null,
                    DiscontinuedPhone = new Phone()
                    {
                        PhoneNumber = "縷たべハボ欲ァんぽぴソａぽゾあたぁソびぼポ九バほ畚ゼまａぼそЯ亜ﾈぁグぴ暦ｚポほボボﾈぴ",
                        Extension = "bozhmrtomzrcmheuuqybovfiuypathsafmriopuccbqubhqbmuauxvnftvnpisgobryzqya"
                    },
                    ChildConcurrencyToken = "裹ぺゾ縷ゼほゼソｚゼｚぜソﾝゼをまぁダびタ珱タバゾゾミチボ暦ソァべ裹ポぜをｦびゼマをゼミぽボソﾈぽポミゾａタソぁマ裹グａタ歹歹たｚバ縷チんをЯん畚たゾべソ欲ァ縷яミをｦせｦゼマソボゼゼチぼ畚珱"
                },
                new DiscontinuedProduct()
                {
                    ProductId = -8,
                    Description = "kelßebrrbesshcnkmhsxokyßetgscprtmiptxyiqnxrohjßuyfegßßmlnejcsmkemgjfrxpqfeffuuqru",
                    BaseConcurrency = "asme",
                    Dimensions = null,
                    ComplexConcurrency = new ConcurrencyInfo()
                    {
                        Token = null,
                        QueriedDateTime = new 
                        DateTimeOffset(new DateTime(634855286563627949, DateTimeKind.Utc))
                    },
                    NestedComplexConcurrency = new AuditInfo()
                    {
                        ModifiedDate = new DateTimeOffset(new DateTime(1713668396298454872, DateTimeKind.Local)),
                        ModifiedBy = "xsnquujocxuumpeqsbodtugghfrghfuihjiyxgvcntkflpxohuyfgytigbdl",
                        Concurrency = new ConcurrencyInfo()
                        {
                            Token = "solisgfvqa",
                            QueriedDateTime = new DateTimeOffset(new DateTime(2954675828701866623, DateTimeKind.Utc))
                        }
                    },
                    Discontinued = new DateTimeOffset(new DateTime(634812397028011658, DateTimeKind.Utc)),
                    ReplacementProductId = 62,
                    DiscontinuedPhone = new Phone()
                    {
                        PhoneNumber = "ulemcjvsndemzkctrfhkiuiblmhdkkledze",
                        Extension = "グ黑ポЯポソ欲タぴぺ畚をほまバぽﾝゼ歹ぁポёハをぜ九ЯまЯソぜ暦ｚダяチゼ欲ソミマぁべぁハぴを匚ポミあ九ぞミぞァァク裹ａソタタ亜そあクマぽё珱ひﾈぜクボ欲ダミ黑"
                    },
                    ChildConcurrencyToken = "hhsbjscessmdunkssmvqmqyzuahm"
                },
                new()
                {
                    ProductId = -7,
                    Description = null,
                    BaseConcurrency = "びマ歹ゾボまﾝぺをゼァゼたバべダｦミソ亜ァマゼチゼあﾈまひボぁёﾝダゼｚひяァぴべ縷ぜをひを亜まソぽべひミぞまゾままあチん",
                    Dimensions = new Dimensions()
                    {
                        Width = -73118289035663600000000000000m,
                        Height = 25.9581087054375m,
                        Depth = -71.7711704670702m
                    },
                    ComplexConcurrency  = new ConcurrencyInfo()
                    {
                        Token = "nbdgygcmjnihofqvxjxfvcxqxytvlujyvxuiuxct",
                        QueriedDateTime = new DateTimeOffset(new DateTime(635150618836196168, DateTimeKind.Utc))
                    },
                    NestedComplexConcurrency = new AuditInfo()
                    {
                        ModifiedDate = new DateTimeOffset(new DateTime(2482230471527477550, DateTimeKind.Local)),
                        ModifiedBy = "ngfqlßphequuncuprßuiydjalaamdrrbmyhvunjdbinctagtiabuegodssfolßiohssssqsxgxopzzutbdlsdjclmoutiylkssd",
                        Concurrency = new ConcurrencyInfo()
                        {
                            Token = "ボ畚九ぴあチひソァタ九яポ弌マポ裹黑ソ暦ソｚ九ゾァポポボ匚歹チ黑ゾあぁゼポёゼ",
                            QueriedDateTime = new DateTimeOffset(new DateTime(631906548252199237, DateTimeKind.Utc))
                        }
                    }
                },
                new()
                {
                    ProductId = -6,
                    Description = "expdybhclurfobuyvzmhkgrnrajhamqmkhqpmiypittnp",
                    BaseConcurrency = "uacssmuxummhtezdsnoßssrlbsßloxjsslnnayinxiksspjsssvumgduaapcfvnsseeßgpaxuaabosemß",
                    Dimensions = new Dimensions()
                    {
                        Width = -49157206180150400000000000000m,
                        Height = -79228162514264337593543950335m,
                        Depth = 38.8793813628938m
                    },
                    ComplexConcurrency = new ConcurrencyInfo()
                    {
                        Token = "まﾝ裹ぁゼｦぼ歹ポぜボたゾひたソﾝタボたぺを欲弌ミソゾべ弌ダァぺべソ裹ひ暦ﾝそя欲ぺ歹ボタひせァﾝんゾﾝァポクﾝひぜ",
                        QueriedDateTime = new DateTimeOffset(new DateTime(0, DateTimeKind.Utc))
                    },
                    NestedComplexConcurrency = new AuditInfo()
                    {
                        ModifiedDate = new DateTimeOffset(new DateTime(0, DateTimeKind.Utc)),
                        ModifiedBy = null,
                        Concurrency = new ConcurrencyInfo()
                        {
                            Token = "zqzhnfajucmhubkegvlixzrqum",
                            QueriedDateTime = new DateTimeOffset(new DateTime(2954553598514918312, DateTimeKind.Utc))
                        }
                    }
                },
                new DiscontinuedProduct()
                {
                    ProductId = -5,
                    Description = "uuudqysoiozagpcpumnydpupopsvd",
                    BaseConcurrency = "inxlfdfruoalzluabvubrgahsg",
                    Dimensions = new Dimensions()
                    {
                        Width = 7337.75206762393m,
                        Height = -4.63644378890358m,
                        Depth = -79228162514264337593543950335m
                    },
                    ComplexConcurrency = null,
                    NestedComplexConcurrency = new AuditInfo()
                    {
                        ModifiedDate = new DateTimeOffset(new DateTime(3155378975999999999, DateTimeKind.Utc)),
                        ModifiedBy = "mlsbmanrhvygvakricoomrnksyutxxdrbizpdzdunzzukgkeaibnuufvxcjputulmcutevhiyflnsjahjistqrlasor",
                        Concurrency = null
                    },
                    Discontinued = new DateTimeOffset(new DateTime(634755161734591089, DateTimeKind.Utc)),
                    ReplacementProductId  = -2147483648,
                    DiscontinuedPhone = new Phone()
                    {
                        PhoneNumber = "びマ",
                        Extension = "九ぜゾべぁびミёё歹珱九ぞあａぞクダまァミソん歹欲べ亜ぜチぜチぁボゼﾝяほ珱ゾゾぼ匚ぜハミソぁボぜぁァチクタ黑を匚ボグ珱ゼボソｚ"
                    },
                    ChildConcurrencyToken = "ixxletiyfrigooaltaqikqcnkpepfufyffmuouknjzyelardpyudoachqdejrjnuhueunugyli"
                },
                new()
                {
                    ProductId = -4,
                    Description = "rgdhvcueuidboerbhyvsvjg",
                    BaseConcurrency = "ぽボﾝあび",
                    Dimensions = new Dimensions()
                    {
                        Width = 0m,
                        Height = -62044452036508000000000000000m,
                        Depth = 0m
                    },
                    ComplexConcurrency = new ConcurrencyInfo()
                    {
                        Token = "uyu",
                        QueriedDateTime = new DateTimeOffset(new DateTime(634890040688842825, DateTimeKind.Utc))
                    },
                    NestedComplexConcurrency = new AuditInfo()
                    {
                        ModifiedDate = new DateTimeOffset(new DateTime(450557167798885925, DateTimeKind.Local)),
                        ModifiedBy = "ダチせあｚミソぽЯゼチゼ縷マ縷裹ﾈ匚暦チя匚ぁミ弌ハ弌ソゾ弌ぽんぴゼボま縷ゼボソハ裹黑九ポ黑マあゼソをぺタぺボ亜タァまクａﾝ亜ぺひぽぺ",
                        Concurrency = new ConcurrencyInfo()
                        {
                            Token = "gßoyfeyzsaelevßu",
                            QueriedDateTime = new DateTimeOffset(new DateTime(634976560566053928, DateTimeKind.Utc))
                        }
                    }
                },
                new DiscontinuedProduct()
                {
                    ProductId = -3,
                    Description = "ißuhmxavnmlsssssjssagmqjpchjußtkcoaldeyyduarovnxspzsskufxxfltußtxfhgjlksrn",
                    BaseConcurrency = "fvrnsbiußevuqssnuidjbhtluhcydhhjihyasecdnumhdpfxtijßlvfqngmulfvjqqtvussyixßykxhbn",
                    Dimensions = null,
                    ComplexConcurrency = new ConcurrencyInfo()
                    {
                        Token = "bdokxvtboyiiuphcrjrlklntbqksnlrldfzqdjgbkcbmyredrlyjunfrrfdcganncntvprydekacdauln",
                        QueriedDateTime = null
                    },
                    NestedComplexConcurrency = null,
                    Discontinued = new DateTimeOffset(new DateTime(0, DateTimeKind.Utc)),
                    ReplacementProductId = -1002345821,
                    DiscontinuedPhone = null,
                    ChildConcurrencyToken = "そ歹ソボボをグ裹ぴポｦチ"
                },
                new DiscontinuedProduct()
                {
                    ProductId = -2,
                    Description = null,
                    BaseConcurrency = null,
                    Dimensions = new Dimensions()
                    {
                        Width = -79228162514264337593543950335m,
                        Height = 44733559606978800000000000000m,
                        Depth = -3913.60110028978m
                    },
                    ComplexConcurrency = new ConcurrencyInfo()
                    {
                        Token = "mgmjxrußcs",
                        QueriedDateTime = new DateTimeOffset(new DateTime(2845688657729990625, DateTimeKind.Local))
                    },
                    NestedComplexConcurrency = null,
                    Discontinued = new DateTimeOffset(new DateTime(3155378975999999999, DateTimeKind.Utc)),
                    ReplacementProductId = -566261304,
                    DiscontinuedPhone = new Phone()
                    {
                        PhoneNumber = "nmaktpqeyimclgtimdspkbavivoclmvfcdeuysxemttzsckamkhukl",
                        Extension = null
                    },
                    ChildConcurrencyToken = "yljmhbcacfnothqirhaouhoraoruscpptgzmoch"
                },
                new()
                {
                    ProductId = -1,
                    Description = "vjsmflmaltcrnxiztidnalnrbhyaqzmxgkqesxnmodm",
                    BaseConcurrency = null,
                    Dimensions = new Dimensions()
                    {
                        Width = 0.123283309629675m,
                        Height = -9264.03359778997m,
                        Depth = -0.409268660025419m
                    },
                    ComplexConcurrency = new ConcurrencyInfo()
                    {
                        Token = "dggicadyltktsssssmißjgblhyifbsnssspssahrgcspiznverhzgyvq",
                        QueriedDateTime = new DateTimeOffset(new DateTime(3155378975999999999, DateTimeKind.Utc))
                    },
                    NestedComplexConcurrency = null,
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
                    //message.Sender = senderLogin;

                    if (senderLogin.SentMessages == null)
                    {
                        senderLogin.SentMessages = new List<Message>();
                    }

                    senderLogin.SentMessages.Add(message);
                }
            }
        }

        private static void PopulatePageViews()
        {
            PageViews = new List<PageView>
            {
                new ProductPageView
                {
                    PageViewId = -10,
                    Username = "珱び畚ボぴマせёミソ",
                    Viewed = new DateTimeOffset(new DateTime(0, DateTimeKind.Unspecified), new TimeSpan(0)),
                    TimeSpentOnPage = TimeSpan.MinValue,
                    PageUrl = "マ縷ぴべｚぴびマゼタゾグそチべ黑ダゾｚЯせをぼマポんんあぼん珱ａびゾひダハマﾝ黑マゾソぜマんﾈソゾタミ暦弌暦ポグボゾダボ畚ぜソそマチべボゼポん珱ゾёァバ",
                    ProductId = -661313570,
                    ConcurrencyToken = "peohxvziohepefjoogexbxfulemllbfamsmqkxvqtctoßtnntzcßvtmuthyudkpzeeegvurfn"
                },
                new PageView
                {
                    PageViewId = -9,
                    Username = "sdppimfqojrgrlmakbmrdlslzjivhaaebqezkaye",
                    Viewed = new DateTimeOffset(new DateTime(3155378975999999999, DateTimeKind.Unspecified), new TimeSpan(0)),
                    TimeSpentOnPage = new TimeSpan(-20714019215549),
                    PageUrl = "exozulicliqpkjdijqoejnlkdlqlizhgdmulvavrmujhfdnnkffjjoik"
                },
                new ProductPageView
                {
                    PageViewId = -8,
                    Username = "",
                    Viewed = new DateTimeOffset(new DateTime(635065154115665324, DateTimeKind.Unspecified), new TimeSpan(-276600000000)),
                    TimeSpentOnPage = new TimeSpan(-7806705063807),
                    PageUrl = "kdqßeqpmßdjoedihqsgzlße",
                    ProductId = 378,
                    ConcurrencyToken = null
                },
                new PageView
                {
                    PageViewId = -7,
                    Username = null,
                    Viewed = new DateTimeOffset(new DateTime(634890040689875500, DateTimeKind.Unspecified), new TimeSpan(-288000000000)),
                    TimeSpentOnPage = TimeSpan.MaxValue,
                    PageUrl = "gcvxypuinhtbovkyceojyptrippdbsnjtpoox"
                },
                new PageView
                {
                    PageViewId = -6,
                    Username = "itideuecujovruvleebrbbcxsspvtqptboorftbncyssmgkissvuutnqtsymcfkssfqnsssrnoltylssudsyjyqanxy",
                    Viewed = new DateTimeOffset(new DateTime(0, DateTimeKind.Unspecified), new TimeSpan(0)),
                    TimeSpentOnPage = new TimeSpan(-8474227957562),
                    PageUrl = "ifnfßqmjjsreyessskuqvjxsstusmhdofrbsshqcsstczlbfyußiylßqbsmdhvmdioufhayssseslkhzssqqxaskmvos"
                },
                new ProductPageView
                {
                    PageViewId = -5,
                    Username = "qljviysmqrpaf",
                    Viewed = new DateTimeOffset(new DateTime(634884013562978192, DateTimeKind.Unspecified), new TimeSpan(363600000000)),
                    TimeSpentOnPage = TimeSpan.Zero,
                    PageUrl = "チダソグ縷ボゾグぼほ弌ポチ歹ほёЯソミを亜ミ畚ほ匚まチポゾ九",
                    ProductId = -807373440,
                    ConcurrencyToken = "racduextfkkejytrmvrbppexymjpijmsmquremß"
                },
                new PageView
                {
                    PageViewId = -4,
                    Username = "ssuovuuxaouytejmxufpssssdrjhftßgsstobqßmyjpucejnkttitgpßrmusoskxexsbjt",
                    Viewed = new DateTimeOffset(new DateTime(634958117802408440, DateTimeKind.Unspecified), new TimeSpan(-504000000000)),
                    TimeSpentOnPage = new TimeSpan(90138131337590),
                    PageUrl = "qmqczgskqvdguzsshgborudpshudvtvuassdgmruqcvnopstyedmqzckdalmljpvzjghbkgupgjjdrkopagtkfuakdzgeofb"
                },
                new ProductPageView
                {
                    PageViewId = -3,
                    Username = "ysezssyqrvqifmdzbsayuxyesslrmzdbxlhgpetpaixozbhgxd",
                    Viewed = new DateTimeOffset(new DateTime(272266420241479976, DateTimeKind.Unspecified), new TimeSpan(493200000000)),
                    TimeSpentOnPage = TimeSpan.MinValue,
                    PageUrl = "dfrhntnyurvjiasyqyvmouclcehmqqmjnorsorfhshqml",
                    ProductId = 1881032792,
                    ConcurrencyToken = "mdjeuulgeckohuydauynjusorzpezhxqkqevcrymtarobhosiooyekdslfgblkhpftqstiadxhuj"
                },
                new PageView
                {
                    PageViewId = -2,
                    Username = null,
                    Viewed = new DateTimeOffset(new DateTime(631990133830009951, DateTimeKind.Unspecified), new TimeSpan(-436800000000)),
                    TimeSpentOnPage = new TimeSpan(5944435062742720512),
                    PageUrl = "jnxxxvzlbrbrxssßszsbciebßbih"
                },
                new ProductPageView
                {
                    PageViewId = -1,
                    Username = "珱チマチミﾈ九ぞぞａ暦タ珱をバびミ",
                    Viewed = new DateTimeOffset(new DateTime(3155378975999999999, DateTimeKind.Unspecified), new TimeSpan(0)),
                    TimeSpentOnPage = new TimeSpan(-15823440477000),
                    PageUrl = "agffmessdzaea",
                    ProductId = -2119,
                    ConcurrencyToken = "ゼぜゼぼ畚ァチぜた黑ぜａクソミボソ欲ボ畚歹ほａぽチびんソﾝボゾほ畚ゼマａ黑ёソびゾ畚ぞЯａぞポａせяタｚ縷裹ぜひびたママёそぜタゼ珱あｦ匚弌ミゼゼソグﾈぜ黑ァゼタググｦぜダクｦまタ九亜ァ九た"
                }
            };
        }

        private static void PopulateMappedEntityTypes()
        {
            MappedEntityTypes = new List<MappedEntityType>()
            {
                new MappedEntityType()
                {
                    Id = -10,
                    Href = "offsetrefusepowerpersonallocalmappedstyleinitobj",
                    Title = "conditionaltodaydecisionconfigurationhexinteger",
                    HrefLang = "platformdocumentsdecryptorsizeshmacweekoncefirst",
                    Type = "erastobjcustscreensharebindingmismatchcodesmacrobrowsablecriteriamapfeatureinequalityiidpacking",
                    Length = 13,
                    BagOfDecimals = new List<decimal>()
                    {
                        3153.09837524813m,
                        -9722.00954692332m,
                        -0.589368370856242m,
                        -28792308624934300000000000000m,
                        -79228162514264337593543950335m
                    }
                }
            };
        }

        private static void PopulateMessageAttachments()
        {
            MessageAttachments =
            [
                new MessageAttachment()
                {
                    AttachmentId = new Guid("1126a28b-a4af-4bbd-bf0a-2b2c22635565"),
                    Attachment = new byte[] {
                        ((byte)(40)),
                        ((byte)(125)),
                        ((byte)(164)),
                        ((byte)(202)),
                        ((byte)(67)),
                        ((byte)(14)),
                        ((byte)(248)),
                        ((byte)(119)),
                        ((byte)(177)),
                        ((byte)(53)),
                        ((byte)(87)),
                        ((byte)(46)),
                        ((byte)(17)),
                        ((byte)(246)),
                        ((byte)(39)),
                        ((byte)(205)),
                        ((byte)(108)),
                        ((byte)(125)),
                        ((byte)(56)),
                        ((byte)(199)),
                        ((byte)(11)),
                        ((byte)(188)),
                        ((byte)(7)),
                        ((byte)(140)),
                        ((byte)(197)),
                        ((byte)(145)),
                        ((byte)(141)),
                        ((byte)(8)),
                        ((byte)(114)),
                        ((byte)(196)),
                        ((byte)(130)),
                        ((byte)(29)),
                        ((byte)(248)),
                        ((byte)(139)),
                        ((byte)(137)),
                        ((byte)(106)),
                        ((byte)(119)),
                        ((byte)(158)),
                        ((byte)(156)),
                        ((byte)(2)),
                        ((byte)(147)),
                        ((byte)(252)),
                        ((byte)(130)),
                        ((byte)(119))
                    }
                },
                new MessageAttachment()
                {
                    AttachmentId = new Guid("5cb091a6-bbb4-43b4-ac12-d7ae631edcb0"),
                    Attachment = null
                },
                new MessageAttachment()
                {
                    AttachmentId = new Guid("05ac36a6-e867-4580-8a31-c1804ef249a2"),
                    Attachment = new byte[] {
                        ((byte)(112)),
                        ((byte)(9)),
                        ((byte)(187)),
                        ((byte)(95)),
                        ((byte)(237)),
                        ((byte)(170)),
                        ((byte)(245)),
                        ((byte)(199)),
                        ((byte)(125)),
                        ((byte)(140)),
                        ((byte)(175)),
                        ((byte)(216)),
                        ((byte)(5)),
                        ((byte)(207)),
                        ((byte)(163)),
                        ((byte)(141)),
                        ((byte)(90)),
                        ((byte)(152)),
                        ((byte)(124)),
                        ((byte)(243)),
                        ((byte)(139)),
                        ((byte)(107)),
                        ((byte)(252)),
                        ((byte)(90)),
                        ((byte)(121)),
                        ((byte)(99)),
                        ((byte)(52)),
                        ((byte)(205)),
                        ((byte)(214)),
                        ((byte)(208)),
                        ((byte)(83)),
                        ((byte)(127)),
                        ((byte)(218)),
                        ((byte)(103)),
                        ((byte)(128)),
                        ((byte)(199)),
                        ((byte)(53)),
                        ((byte)(217)),
                        ((byte)(83)),
                        ((byte)(172)),
                        ((byte)(44)),
                        ((byte)(33)),
                        ((byte)(35)),
                        ((byte)(139)),
                        ((byte)(3)),
                        ((byte)(62)),
                        ((byte)(222)),
                        ((byte)(140)),
                        ((byte)(105)),
                        ((byte)(144)),
                        ((byte)(79)),
                        ((byte)(184)),
                        ((byte)(92)),
                        ((byte)(32)),
                        ((byte)(14)),
                        ((byte)(4)),
                        ((byte)(13)),
                        ((byte)(97)),
                        ((byte)(229)),
                        ((byte)(138)),
                        ((byte)(117)),
                        ((byte)(39)),
                        ((byte)(240)),
                        ((byte)(173)),
                        ((byte)(56)),
                        ((byte)(47)),
                        ((byte)(254)),
                        ((byte)(157))
                    }
                },
                new MessageAttachment
                {
                    AttachmentId = new Guid("2ccea377-d7b4-4d6e-b864-0e4b87b86bd9"),
                    Attachment = new byte[] {
                        ((byte)(57)),
                        ((byte)(37)),
                        ((byte)(149)),
                        ((byte)(243)),
                        ((byte)(98)),
                        ((byte)(34)),
                        ((byte)(193)),
                        ((byte)(251)),
                        ((byte)(79)),
                        ((byte)(13)),
                        ((byte)(241)),
                        ((byte)(33)),
                        ((byte)(104)),
                        ((byte)(78)),
                        ((byte)(59)),
                        ((byte)(233)),
                        ((byte)(192)),
                        ((byte)(141)),
                        ((byte)(122)),
                        ((byte)(78)),
                        ((byte)(40)),
                        ((byte)(118)),
                        ((byte)(92)),
                        ((byte)(233)),
                        ((byte)(53)),
                        ((byte)(98)),
                        ((byte)(73)),
                        ((byte)(174)),
                        ((byte)(253)),
                        ((byte)(123)),
                        ((byte)(253)),
                        ((byte)(115)),
                        ((byte)(249)),
                        ((byte)(55)),
                        ((byte)(25)),
                        ((byte)(88)),
                        ((byte)(82)),
                        ((byte)(65)),
                        ((byte)(231)),
                        ((byte)(162)),
                        ((byte)(106)),
                        ((byte)(26)),
                        ((byte)(94)),
                        ((byte)(233)),
                        ((byte)(250)),
                        ((byte)(22)),
                        ((byte)(165)),
                        ((byte)(112)),
                        ((byte)(251)),
                        ((byte)(40)),
                        ((byte)(88)),
                        ((byte)(254)),
                        ((byte)(26)),
                        ((byte)(55)),
                        ((byte)(189)),
                        ((byte)(194)),
                        ((byte)(200)),
                        ((byte)(171)),
                        ((byte)(190)),
                        ((byte)(0)),
                        ((byte)(216)),
                        ((byte)(18)),
                        ((byte)(254)),
                        ((byte)(211)),
                        ((byte)(16)),
                        ((byte)(125)),
                        ((byte)(40)),
                        ((byte)(49)),
                        ((byte)(52)),
                        ((byte)(227)),
                        ((byte)(150)),
                        ((byte)(127)),
                        ((byte)(166)),
                        ((byte)(216)),
                        ((byte)(228))
                    }
                },
                new MessageAttachment
                {
                    AttachmentId = new Guid("b0d769c1-ffbd-423a-8af0-dcd53a357d66"),
                    Attachment = new byte[] {
                        ((byte)(252)),
                        ((byte)(191)),
                        ((byte)(15)),
                        ((byte)(140)),
                        ((byte)(242)),
                        ((byte)(140)),
                        ((byte)(153)),
                        ((byte)(113)),
                        ((byte)(49)),
                        ((byte)(73)),
                        ((byte)(157)),
                        ((byte)(154)),
                        ((byte)(67)),
                        ((byte)(73)),
                        ((byte)(165)),
                        ((byte)(23)),
                        ((byte)(110)),
                        ((byte)(203)),
                        ((byte)(172)),
                        ((byte)(57)),
                        ((byte)(233)),
                        ((byte)(228)),
                        ((byte)(164)),
                        ((byte)(201)),
                        ((byte)(247)),
                        ((byte)(243)),
                        ((byte)(218)),
                        ((byte)(198)),
                        ((byte)(88)),
                        ((byte)(135)),
                        ((byte)(189)),
                        ((byte)(93)),
                        ((byte)(195)),
                        ((byte)(161)),
                        ((byte)(220)),
                        ((byte)(152)),
                        ((byte)(50)),
                        ((byte)(198)),
                        ((byte)(189)),
                        ((byte)(30)),
                        ((byte)(187)),
                        ((byte)(75)),
                        ((byte)(109)),
                        ((byte)(249)),
                        ((byte)(98)),
                        ((byte)(198)),
                        ((byte)(228)),
                        ((byte)(103)),
                        ((byte)(108)),
                        ((byte)(12)),
                        ((byte)(247)),
                        ((byte)(235)),
                        ((byte)(49)),
                        ((byte)(26)),
                        ((byte)(6))
                    }
                },
                new MessageAttachment
                {
                    AttachmentId = new Guid("4b7ab900-bf82-4857-ac02-470ffbeffe1d"),
                    Attachment = new byte[] {
                        ((byte)(163)),
                        ((byte)(159)),
                        ((byte)(172)),
                        ((byte)(55)),
                        ((byte)(80)),
                        ((byte)(124)),
                        ((byte)(248)),
                        ((byte)(175)),
                        ((byte)(124)),
                        ((byte)(170)),
                        ((byte)(137)),
                        ((byte)(26)),
                        ((byte)(175)),
                        ((byte)(124)),
                        ((byte)(0)),
                        ((byte)(218)),
                        ((byte)(254)),
                        ((byte)(187)),
                        ((byte)(132)),
                        ((byte)(138)),
                        ((byte)(55)),
                        ((byte)(90)),
                        ((byte)(51)),
                        ((byte)(253)),
                        ((byte)(237))
                    }
                },
                new MessageAttachment()
                {
                    AttachmentId = new Guid("ebdcca48-a0dc-4331-bb98-64c92568c525"),
                    Attachment = new byte[] {
                        ((byte)(255)),
                        ((byte)(126)),
                        ((byte)(69)),
                        ((byte)(62)),
                        ((byte)(97)),
                        ((byte)(63)),
                        ((byte)(141)),
                        ((byte)(184))
                    }
                },
                new MessageAttachment()
                {
                    AttachmentId = new Guid("66527e34-9d1f-45b2-ba8e-3e2306a9be78"),
                    Attachment = new byte[] {
                        ((byte)(96)),
                        ((byte)(143)),
                        ((byte)(74)),
                        ((byte)(207)),
                        ((byte)(73)),
                        ((byte)(119)),
                        ((byte)(52)),
                        ((byte)(20)),
                        ((byte)(158)),
                        ((byte)(136)),
                        ((byte)(18)),
                        ((byte)(29)),
                        ((byte)(42)),
                        ((byte)(241)),
                        ((byte)(94)),
                        ((byte)(232)),
                        ((byte)(230)),
                        ((byte)(6)),
                        ((byte)(81)),
                        ((byte)(75)),
                        ((byte)(177)),
                        ((byte)(221)),
                        ((byte)(1)),
                        ((byte)(120)),
                        ((byte)(192)),
                        ((byte)(137)),
                        ((byte)(223)),
                        ((byte)(147)),
                        ((byte)(233)),
                        ((byte)(124)),
                        ((byte)(171)),
                        ((byte)(217)),
                        ((byte)(23)),
                        ((byte)(134)),
                        ((byte)(239)),
                        ((byte)(75)),
                        ((byte)(242)),
                        ((byte)(153))
                    }
                },
                new MessageAttachment()
                {
                    AttachmentId = new Guid("1609b623-c772-4ecd-90fc-dc3974b77475"),
                    Attachment = null
                },
                new MessageAttachment()
                {
                    AttachmentId = new Guid("7e926398-4690-4a4b-b7c7-d1587441b90f"),
                    Attachment = new byte[] {
                        ((byte)(29)),
                        ((byte)(157)),
                        ((byte)(21)),
                        ((byte)(91)),
                        ((byte)(51)),
                        ((byte)(178)),
                        ((byte)(98)),
                        ((byte)(177)),
                        ((byte)(231)),
                        ((byte)(107)),
                        ((byte)(3)),
                        ((byte)(45)),
                        ((byte)(125)),
                        ((byte)(194)),
                        ((byte)(40)),
                        ((byte)(1)),
                        ((byte)(7)),
                        ((byte)(60)),
                        ((byte)(52))
                    }
                }
            ]; 
        }

        private static void PopulateMessage_Attachments()
        {
            foreach (Message sms in Messages)
            {
                foreach (MessageAttachment smsAttachment in MessageAttachments)
                {
                    sms.Attachments = new List<MessageAttachment> { smsAttachment};
                }
            }
        }

        private static void PopulateProducts_RelatedProducts()
        {
            var relatedProductsMapping = new Dictionary<int, List<int>>
            {
                { -10, new List<int> { -10 } },
                { -9, new List<int> { -9 } },
                { -7, new List<int> { -7, -6, -3 } },
                { -5, new List<int> { -5, -2 } },
                { -4, new List<int> { -4 } },
                { -2, new List<int> { -1 } }
            };

            foreach (var product in Products)
            {
                if (relatedProductsMapping.TryGetValue(product.ProductId, out var relatedProductIds))
                {
                    product.RelatedProducts = Products
                        .Where(p => relatedProductIds.Contains(p.ProductId))
                        .ToList();
                }
            }
        }

        private static void PopulateProductDetail()
        {
            ProductDetails =
            [
                new()
                {
                    ProductId = -10,
                    Details = "lviipfnkdejpzonrvkzradhxßpkssvaibmuupjsoßljxzubiroynzmstbjcißxprcsscetßßcifz"
                },
                new()
                {
                    ProductId = -9,
                    Details = "uzetenprkufssbiculuquxvebmpunavicqjerikglietrqjesvvo"
                },
                new()
                {
                    ProductId = -8,
                    Details = "pyclftniuczyhpgsypylfojyaoefgqelgkryzjiriizjuxlkgrtakpmkldkbrcslujmyxjtllbjbuzsinmzpxeesxc"
                },
                new()
                {
                    ProductId = -7,
                    Details = "capcsfgnhlibhzvcvmgrtssrphutpercßssßtrecssppzsriyfdagubßussdgßxmptmtd"
                },
                new()
                {
                    ProductId = -6,
                    Details = "rurdfmekougouoibfheytppgangqziloxoikdounipdtqnoymccyxguiufcru"
                },
                new()
                {
                    ProductId = -5,
                    Details = "csiihysghlmfsskßkqcxßßgqdcduxnlutbqeexpnqfanrbffießbsssmuyivyoixuyfvifhzpescs"
                },
                new()
                {
                    ProductId = -4,
                    Details = "ubxpzsuxequoglmvvakeckmfmornooiuzjfjldsuvhxinpodkaezbikgpnivactxnpuyuifmdd"
                },
                new()
                {
                    ProductId = -3,
                    Details = "ｦポゾせまび九ぜチミそをマタ欲をダポべチグ縷ｦゼ歹ぺゼマ"
                },
                new()
                {
                    ProductId = -2,
                    Details = "ｦ珱ァタяゼポソチせソポ歹ﾈａ畚ぽЯぴｚタ九弌亜あチﾈゼァソ珱ソァ畚歹ゼゾソソぴﾈﾝЯ珱ハ暦黑яａミポｦ珱九ゾべあハソソゼぼぞそぺяをﾈぞタグ弌チЯﾈタ九ａひぽポｚソバ暦裹チバゼボァ裹Яﾈ亜ミ畚縷ボ"
                },
                new()
                {
                    ProductId = -1,
                    Details = "sssuuyptquexacqßyuhdnpyqxqcafjkeoqydpnueormlrhqbsdmjssczß"
                }
            ];
        }

        private static void PopulateProductReview()
        {
            ProductReviews =
            [
                new()
                {
                    ProductId = -10,
                    ReviewId = -10,
                    Review = "ハべチ暦ポチﾝぜ匚ぜ暦黑ポ珱ボ黑ぜゼほぁぞｚゼゾ九タミｚまボぽ裹ァぁたぽ弌",
                    RevisionId = "1"
                },
                new()
                {
                    ProductId = -9,
                    ReviewId = -9,
                    Review = "rvqrgbkqzoybdrfsssulycupxfrgdpj",
                    RevisionId = "2"
                },
                new()
                {
                    ProductId = -8,
                    ReviewId = -8,
                    Review = "nxhibkpuflabavjnxumeptbvdkodzzushyfqsqcrzbuhujdjqxybbbutqlurgfbfgcuemtvcxuejyuquu",
                    RevisionId = "3"
                },
                new()
                {
                    ProductId = -7,
                    ReviewId = -7,
                    Review = null,
                    RevisionId = "4"
                },
                new()
                {
                    ProductId = -6,
                    ReviewId = -6,
                    Review = "ftouj",
                    RevisionId= "5"
                },
                new()
                {
                    ProductId = -5,
                    ReviewId = -5,
                    Review = null,
                    RevisionId = "6"
                },
                new()
                {
                    ProductId = -4,
                    ReviewId = -4,
                    Review = "タぜポ歹ボ亜畚そぁボ亜珱ｚポボボマソひ縷ああﾈゾ九ひハ歹マ匚黑そクぁチ珱をぁボチゼ匚ﾈぺ弌ゼぼたソせたた裹黑ボソぞひせソびёゼぽミボゾ縷せ弌ァ",
                    RevisionId = "7"
                },
                new()
                {
                    ProductId = -3,
                    ReviewId = -3,
                    Review = "afibmzlsihsxnldveeklugbcukn",
                    RevisionId = "8"
                },
                new()
                {
                    ProductId = -2,
                    ReviewId = -2,
                    Review = null,
                    RevisionId = "9"
                },
                new()
                {
                    ProductId = -1,
                    ReviewId = -1,
                    Review = null,
                    RevisionId = "10"
                }
            ];
        }

        private static void PopulateProductPhoto()
        {
            ProductPhotos =
            [
                new()
                {
                    ProductId = -10,
                    PhotoId = -10,
                    Photo = [
                        ((byte)(222)),
                        ((byte)(96)),
                        ((byte)(191)),
                        ((byte)(82)),
                        ((byte)(253)),
                        ((byte)(25)),
                        ((byte)(189)),
                        ((byte)(22)),
                        ((byte)(15)),
                        ((byte)(7)),
                        ((byte)(161)),
                        ((byte)(56)),
                        ((byte)(167)),
                        ((byte)(51)),
                        ((byte)(45)),
                        ((byte)(220)),
                        ((byte)(183)),
                        ((byte)(221)),
                        ((byte)(76)),
                        ((byte)(225)),
                        ((byte)(186)),
                        ((byte)(72)),
                        ((byte)(18)),
                        ((byte)(6)),
                        ((byte)(247)),
                        ((byte)(0)),
                        ((byte)(176)),
                        ((byte)(31)),
                        ((byte)(22)),
                        ((byte)(232)),
                        ((byte)(6)),
                        ((byte)(190)),
                        ((byte)(188)),
                        ((byte)(205)),
                        ((byte)(222)),
                        ((byte)(166)),
                        ((byte)(92)),
                        ((byte)(59)),
                        ((byte)(105)),
                        ((byte)(80)),
                        ((byte)(254)),
                        ((byte)(183)),
                        ((byte)(118)),
                        ((byte)(65)),
                        ((byte)(129)),
                        ((byte)(77)),
                        ((byte)(6)),
                        ((byte)(6)),
                        ((byte)(251)),
                        ((byte)(150)),
                        ((byte)(229)),
                        ((byte)(225)),
                        ((byte)(9)),
                        ((byte)(253)),
                        ((byte)(168)),
                        ((byte)(149)),
                        ((byte)(78)),
                        ((byte)(184)),
                        ((byte)(90)),
                        ((byte)(253)),
                        ((byte)(104)),
                        ((byte)(157)),
                        ((byte)(255)),
                        ((byte)(39)),
                        ((byte)(0)),
                        ((byte)(226)),
                        ((byte)(124)),
                        ((byte)(217)),
                        ((byte)(85)),
                        ((byte)(135)),
                        ((byte)(127)),
                        ((byte)(123)),
                        ((byte)(115)),
                        ((byte)(251)),
                        ((byte)(226)),
                        ((byte)(3)),
                        ((byte)(209)),
                        ((byte)(208)),
                        ((byte)(132)),
                        ((byte)(170)),
                        ((byte)(250)),
                        ((byte)(170)),
                        ((byte)(35)),
                        ((byte)(128)),
                        ((byte)(2)),
                        ((byte)(37)),
                        ((byte)(181)),
                        ((byte)(191)),
                        ((byte)(37)),
                        ((byte)(73)),
                        ((byte)(87)),
                        ((byte)(163))
                    ]
                },
                new()
                {
                    ProductId = -9,
                    PhotoId = -9,
                    Photo = [
                        ((byte)(244)),
                        ((byte)(46)),
                        ((byte)(188)),
                        ((byte)(5)),
                        ((byte)(137)),
                        ((byte)(65)),
                        ((byte)(185)),
                        ((byte)(250)),
                        ((byte)(112)),
                        ((byte)(221)),
                        ((byte)(64)),
                        ((byte)(227)),
                        ((byte)(51)),
                        ((byte)(247)),
                        ((byte)(38)),
                        ((byte)(20)),
                        ((byte)(132)),
                        ((byte)(4)),
                        ((byte)(24)),
                        ((byte)(23)),
                        ((byte)(120)),
                        ((byte)(164)),
                        ((byte)(139)),
                        ((byte)(51)),
                        ((byte)(96)),
                        ((byte)(23)),
                        ((byte)(57)),
                        ((byte)(172)),
                        ((byte)(73)),
                        ((byte)(165)),
                        ((byte)(114)),
                        ((byte)(19)),
                        ((byte)(161)),
                        ((byte)(101)),
                        ((byte)(17)),
                        ((byte)(117)),
                        ((byte)(44)),
                        ((byte)(179))
                        ]
                },
                new()
                {
                    ProductId = -8,
                    PhotoId = -8,
                    Photo = [
                        ((byte)(32)),
                        ((byte)(154)),
                        ((byte)(31)),
                        ((byte)(101)),
                        ((byte)(206)),
                        ((byte)(15)),
                        ((byte)(166)),
                        ((byte)(2)),
                        ((byte)(24)),
                        ((byte)(148)),
                        ((byte)(164)),
                        ((byte)(62)),
                        ((byte)(155)),
                        ((byte)(225)),
                        ((byte)(186)),
                        ((byte)(182)),
                        ((byte)(1)),
                        ((byte)(189)),
                        ((byte)(9)),
                        ((byte)(251)),
                        ((byte)(17)),
                        ((byte)(172)),
                        ((byte)(165)),
                        ((byte)(139)),
                        ((byte)(202)),
                        ((byte)(144)),
                        ((byte)(142)),
                        ((byte)(245)),
                        ((byte)(233)),
                        ((byte)(34)),
                        ((byte)(138)),
                        ((byte)(63)),
                        ((byte)(213)),
                        ((byte)(140)),
                        ((byte)(57)),
                        ((byte)(218)),
                        ((byte)(39)),
                        ((byte)(89)),
                        ((byte)(140)),
                        ((byte)(128)),
                        ((byte)(241)),
                        ((byte)(233)),
                        ((byte)(218)),
                        ((byte)(173)),
                        ((byte)(219)),
                        ((byte)(3)),
                        ((byte)(148)),
                        ((byte)(117)),
                        ((byte)(144)),
                        ((byte)(206)),
                        ((byte)(139)),
                        ((byte)(167)),
                        ((byte)(150)),
                        ((byte)(43)),
                        ((byte)(54)),
                        ((byte)(115)),
                        ((byte)(244)),
                        ((byte)(147)),
                        ((byte)(154)),
                        ((byte)(62)),
                        ((byte)(128)),
                        ((byte)(176)),
                        ((byte)(24)),
                        ((byte)(245)),
                        ((byte)(9))]
                },
                new()
                {
                    ProductId = -7,
                    PhotoId = -7,
                    Photo = [
                        ((byte)(71)),
                        ((byte)(77)),
                        ((byte)(254)),
                        ((byte)(204)),
                        ((byte)(227)),
                        ((byte)(180)),
                        ((byte)(235)),
                        ((byte)(229)),
                        ((byte)(103)),
                        ((byte)(107)),
                        ((byte)(218)),
                        ((byte)(34)),
                        ((byte)(228)),
                        ((byte)(227)),
                        ((byte)(106)),
                        ((byte)(191)),
                        ((byte)(13)),
                        ((byte)(208)),
                        ((byte)(248)),
                        ((byte)(12)),
                        ((byte)(210)),
                        ((byte)(236)),
                        ((byte)(147)),
                        ((byte)(116)),
                        ((byte)(152)),
                        ((byte)(33)),
                        ((byte)(187)),
                        ((byte)(54)),
                        ((byte)(62)),
                        ((byte)(189)),
                        ((byte)(82)),
                        ((byte)(51)),
                        ((byte)(223)),
                        ((byte)(176)),
                        ((byte)(149)),
                        ((byte)(16)),
                        ((byte)(253)),
                        ((byte)(226)),
                        ((byte)(112)),
                        ((byte)(71)),
                        ((byte)(227))]
                },
                new()
                {
                    ProductId = -6,
                    PhotoId = -6,
                    Photo = [
                        ((byte)(101)),
                        ((byte)(163)),
                        ((byte)(185)),
                        ((byte)(175)),
                        ((byte)(230)),
                        ((byte)(116)),
                        ((byte)(23)),
                        ((byte)(130)),
                        ((byte)(252)),
                        ((byte)(38)),
                        ((byte)(234)),
                        ((byte)(101)),
                        ((byte)(200)),
                        ((byte)(159)),
                        ((byte)(155)),
                        ((byte)(39))
                        ]
                },
                new()
                {
                    ProductId = -5,
                    PhotoId = -5,
                    Photo = [
                        ((byte)(167)),
                        ((byte)(8)),
                        ((byte)(37)),
                        ((byte)(205)),
                        ((byte)(231)),
                        ((byte)(190)),
                        ((byte)(120)),
                        ((byte)(214)),
                        ((byte)(190)),
                        ((byte)(208)),
                        ((byte)(111)),
                        ((byte)(86)),
                        ((byte)(201)),
                        ((byte)(109)),
                        ((byte)(198)),
                        ((byte)(203)),
                        ((byte)(35)),
                        ((byte)(4)),
                        ((byte)(7)),
                        ((byte)(14)),
                        ((byte)(5)),
                        ((byte)(35)),
                        ((byte)(75)),
                        ((byte)(235)),
                        ((byte)(207)),
                        ((byte)(86)),
                        ((byte)(240)),
                        ((byte)(206)),
                        ((byte)(71)),
                        ((byte)(17)),
                        ((byte)(52)),
                        ((byte)(187)),
                        ((byte)(151)),
                        ((byte)(47)),
                        ((byte)(218)),
                        ((byte)(123)),
                        ((byte)(251)),
                        ((byte)(92)),
                        ((byte)(222)),
                        ((byte)(244)),
                        ((byte)(248)),
                        ((byte)(138)),
                        ((byte)(6)),
                        ((byte)(215)),
                        ((byte)(118)),
                        ((byte)(189)),
                        ((byte)(110)),
                        ((byte)(16)),
                        ((byte)(158)),
                        ((byte)(180)),
                        ((byte)(192)),
                        ((byte)(127)),
                        ((byte)(43)),
                        ((byte)(147)),
                        ((byte)(47))]
                },
                new()
                {
                    ProductId = -4,
                    PhotoId = -4,
                    Photo = null
                },
                new()
                {
                    ProductId = -3,
                    PhotoId = -3,
                    Photo = [
                        ((byte)(126)),
                        ((byte)(46)),
                        ((byte)(185)),
                        ((byte)(222)),
                        ((byte)(157)),
                        ((byte)(254)),
                        ((byte)(50)),
                        ((byte)(73)),
                        ((byte)(64)),
                        ((byte)(252)),
                        ((byte)(183)),
                        ((byte)(104)),
                        ((byte)(6)),
                        ((byte)(88)),
                        ((byte)(86)),
                        ((byte)(130)),
                        ((byte)(115)),
                        ((byte)(19)),
                        ((byte)(81)),
                        ((byte)(65)),
                        ((byte)(40)),
                        ((byte)(242)),
                        ((byte)(209)),
                        ((byte)(32)),
                        ((byte)(181)),
                        ((byte)(179)),
                        ((byte)(23)),
                        ((byte)(156)),
                        ((byte)(93)),
                        ((byte)(26)),
                        ((byte)(220)),
                        ((byte)(37)),
                        ((byte)(13)),
                        ((byte)(47)),
                        ((byte)(208)),
                        ((byte)(89)),
                        ((byte)(197)),
                        ((byte)(122)),
                        ((byte)(82)),
                        ((byte)(70)),
                        ((byte)(32)),
                        ((byte)(253)),
                        ((byte)(121)),
                        ((byte)(180)),
                        ((byte)(231)),
                        ((byte)(152)),
                        ((byte)(212)),
                        ((byte)(116)),
                        ((byte)(205)),
                        ((byte)(99)),
                        ((byte)(227)),
                        ((byte)(37)),
                        ((byte)(82)),
                        ((byte)(3)),
                        ((byte)(199)),
                        ((byte)(139)),
                        ((byte)(93)),
                        ((byte)(152)),
                        ((byte)(41)),
                        ((byte)(234)),
                        ((byte)(231)),
                        ((byte)(149)),
                        ((byte)(235)),
                        ((byte)(38)),
                        ((byte)(31)),
                        ((byte)(145)),
                        ((byte)(91)),
                        ((byte)(214)),
                        ((byte)(136)),
                        ((byte)(253)),
                        ((byte)(189)),
                        ((byte)(248)),
                        ((byte)(192)),
                        ((byte)(10)),
                        ((byte)(32)),
                        ((byte)(43)),
                        ((byte)(121)),
                        ((byte)(28)),
                        ((byte)(57)),
                        ((byte)(28))
                        ]
                },
                new()
                {
                    ProductId = -2,
                    PhotoId = -2,
                    Photo = []
                },
                new()
                {
                    ProductId = -1,
                    PhotoId = -1,
                    Photo = [
                        ((byte)(194)),
                        ((byte)(53)),
                        ((byte)(254)),
                        ((byte)(37)),
                        ((byte)(176)),
                        ((byte)(255)),
                        ((byte)(122)),
                        ((byte)(202)),
                        ((byte)(172)),
                        ((byte)(4)),
                        ((byte)(246)),
                        ((byte)(248)),
                        ((byte)(16)),
                        ((byte)(180)),
                        ((byte)(209)),
                        ((byte)(89)),
                        ((byte)(208)),
                        ((byte)(253)),
                        ((byte)(206)),
                        ((byte)(187)),
                        ((byte)(215)),
                        ((byte)(134)),
                        ((byte)(94)),
                        ((byte)(92)),
                        ((byte)(195)),
                        ((byte)(115)),
                        ((byte)(219)),
                        ((byte)(182)),
                        ((byte)(138)),
                        ((byte)(81)),
                        ((byte)(25)),
                        ((byte)(47)),
                        ((byte)(246)),
                        ((byte)(194)),
                        ((byte)(198)),
                        ((byte)(98)),
                        ((byte)(60)),
                        ((byte)(66)),
                        ((byte)(159)),
                        ((byte)(82)),
                        ((byte)(109)),
                        ((byte)(9)),
                        ((byte)(8)),
                        ((byte)(113)),
                        ((byte)(0)),
                        ((byte)(38)),
                        ((byte)(174)),
                        ((byte)(117)),
                        ((byte)(22)),
                        ((byte)(52)),
                        ((byte)(225)),
                        ((byte)(203)),
                        ((byte)(117)),
                        ((byte)(156)),
                        ((byte)(236)),
                        ((byte)(59))]
                }
            ];
        }

        private static void PopulateProduct_ProductReview()
        {
            var productReviewsMapping = new Dictionary<int, List<int>>
            {
                { -10, new List<int> { -10, -9, -7, -6, -4 } },
                { -9, new List<int> { -3, -2 } },
                { -1, new List<int> { -1 } }
            };

            foreach (var product in Products)
            {
                if (productReviewsMapping.TryGetValue(product.ProductId, out var reviewIds))
                {
                    product.Reviews = ProductReviews
                        .Where(r => reviewIds.Contains(r.ProductId))
                        .ToList();
                }
            }
        }

        private static void PopulateProduct_ProductPhoto()
        {
            var productPhotosMapping = new Dictionary<int, List<int>>
            {
                { -10, new List<int> { -10, -9, -7, -6, -5 } },
                { -9, new List<int> { -4 } },
                { -7, new List<int> { -3, -2 } },
                { -4, new List<int> { -3, -1 } }
            };

            foreach (var product in Products)
            {
                if (productPhotosMapping.TryGetValue(product.ProductId, out var photoIds))
                {
                    product.Photos = ProductPhotos
                        .Where(p => photoIds.Contains(p.ProductId))
                        .ToList();
                }
            }
        }

        private static void PopulateProduct_ProductDetail()
        {
            var productDetailMapping = new Dictionary<int, int>
            {
                { -10, -9 },
                { -9, -10 },
                { -7, -7 },
                { -6, -6 },
                { -5, -5 },
                { -4, -2 },
                { -3, -3 },
                { -2, -4 },
                { -1, -1 }
            };

            foreach (var product in Products)
            {
                if (productDetailMapping.TryGetValue(product.ProductId, out var productId))
                {
                    product.Detail = ProductDetails.FirstOrDefault(p => p.ProductId == productId);
                }
            }
        }
    }
}
