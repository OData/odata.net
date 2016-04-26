//---------------------------------------------------------------------
// <copyright file="ExpressionTestCases.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.UriParser
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces
    //// ToDo: When we start supporting spatial, uncomment these.
    ////public class SpatialHelper
    ////{
    ////    public static Geography ParseGeography(string text)
    ////    {
    ////        return SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter().Read<Geography>(new StringReader(text));
    ////    }

    ////    public static Geometry ParseGeometry(string text)
    ////    {
    ////        return SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter().Read<Geometry>(new StringReader(text));
    ////    }
    ////}

    public class InvalidExpressionTestCase
    {
        public static readonly InvalidExpressionTestCase[] InvalidPrimitiveLiteralTestCases = new InvalidExpressionTestCase[]
        { 
            // Unterminated literals
            new InvalidExpressionTestCase { Expression = "'some", ExpectedErrorMessage = "There is an unterminated string literal at position 5 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "'", ExpectedErrorMessage = "There is an unterminated string literal at position 1 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "'some''", ExpectedErrorMessage = "There is an unterminated string literal at position 7 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "^", ExpectedErrorMessage = "Syntax error: character '^' is not valid at position 0 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "binary'", ExpectedErrorMessage = "There is an unterminated literal at position 7 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "binary'1234", ExpectedErrorMessage = "There is an unterminated literal at position 11 in '$(Expression)'." },

            // Lexically invalid number literals
            new InvalidExpressionTestCase { Expression = "4.a", ExpectedErrorMessage = "A digit was expected at position 2 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "4Ea", ExpectedErrorMessage = "A digit was expected at position 2 in '$(Expression)'." },

            // Invalid binary literal
            new InvalidExpressionTestCase { Expression = "binary'1'", ExpectedErrorMessage = "Unrecognized 'Edm.Binary' literal 'binary'1'' at '0' in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "binary'1z'", ExpectedErrorMessage = "Unrecognized 'Edm.Binary' literal 'binary'1z'' at '0' in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "binary'z1'", ExpectedErrorMessage = "Unrecognized 'Edm.Binary' literal 'binary'z1'' at '0' in '$(Expression)'." },

            ////ToDo: When we start supporting spatial, uncomment these.
            //// Geography
            ////new InvalidExpressionTestCase { Expression = "geography''", ExpectedErrorMessage = "Unrecognized 'Edm.Geography' literal 'geography''' at '0' in 'geography'''." },
            ////new InvalidExpressionTestCase { Expression = "geography'foofoo'", ExpectedErrorMessage = "Unrecognized 'Edm.Geography' literal 'geography'foofoo'' at '0' in 'geography'foofoo''." },

            ////// Geometry
            ////new InvalidExpressionTestCase { Expression = "geometry''", ExpectedErrorMessage = "Unrecognized 'Edm.Geometry' literal 'geometry''' at '0' in 'geometry'''." },
            ////new InvalidExpressionTestCase { Expression = "geometry'foofoo'", ExpectedErrorMessage = "Unrecognized 'Edm.Geometry' literal 'geometry'foofoo'' at '0' in 'geometry'foofoo''." },
        };

        public static readonly InvalidExpressionTestCase[] InvalidPrimitiveLiteralSelectTestCases = new InvalidExpressionTestCase[]
        { 
            // Unterminated literals
            new InvalidExpressionTestCase { Expression = "'some", ExpectedErrorMessage = "There is an unterminated string literal at position 5 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "'", ExpectedErrorMessage = "There is an unterminated string literal at position 1 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "'some''", ExpectedErrorMessage = "There is an unterminated string literal at position 7 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "^", ExpectedErrorMessage = "Syntax error: character '^' is not valid at position 0 in '$(Expression)'." },

            new InvalidExpressionTestCase { Expression = "binary'", ExpectedErrorMessage = "There is an unterminated literal at position 7 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "binary'1234", ExpectedErrorMessage = "There is an unterminated literal at position 11 in '$(Expression)'." },

            // Lexically invalid number literals
            new InvalidExpressionTestCase { Expression = "4.a", ExpectedErrorMessage = "A digit was expected at position 2 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "4Ea", ExpectedErrorMessage = "A digit was expected at position 2 in '$(Expression)'." },

            // Invalid binary literal
            new InvalidExpressionTestCase { Expression = "binary'1'", ExpectedErrorMessage = "Term 'binary'1'' is not valid in a $select expression." },
            new InvalidExpressionTestCase { Expression = "binary'1z'", ExpectedErrorMessage = "Term 'binary'1z'' is not valid in a $select expression." },
            new InvalidExpressionTestCase { Expression = "binary'z1'", ExpectedErrorMessage = "Term 'binary'z1'' is not valid in a $select expression." },

            ////ToDo: When we start supporting spatial, uncomment these.
            //// Geography
            ////new InvalidExpressionTestCase { Expression = "geography''", ExpectedErrorMessage = "Unrecognized 'Edm.Geography' literal 'geography''' at '0' in 'geography'''." },
            ////new InvalidExpressionTestCase { Expression = "geography'foofoo'", ExpectedErrorMessage = "Unrecognized 'Edm.Geography' literal 'geography'foofoo'' at '0' in 'geography'foofoo''." },

            ////// Geometry
            ////new InvalidExpressionTestCase { Expression = "geometry''", ExpectedErrorMessage = "Unrecognized 'Edm.Geometry' literal 'geometry''' at '0' in 'geometry'''." },
            ////new InvalidExpressionTestCase { Expression = "geometry'foofoo'", ExpectedErrorMessage = "Unrecognized 'Edm.Geometry' literal 'geometry'foofoo'' at '0' in 'geometry'foofoo''." },
        };

        public static readonly IEnumerable<InvalidExpressionTestCase> InvalidExpressionTestCases = new InvalidExpressionTestCase[]
        {
            // Invalid primary token
            new InvalidExpressionTestCase { Expression = ",", ExpectedErrorMessage = "Expression expected at position 0 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "/", ExpectedErrorMessage = "Expression expected at position 0 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = ".", ExpectedErrorMessage = "Expression expected at position 0 in '$(Expression)'." },

            // Unclosed parenthesis
            new InvalidExpressionTestCase { Expression = "(", ExpectedErrorMessage = "Expression expected at position 1 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "(42", ExpectedErrorMessage = "')' or operator expected at position 3 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "(')'", ExpectedErrorMessage = "')' or operator expected at position 4 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "((((null)))", ExpectedErrorMessage = "')' or operator expected at position 11 in '$(Expression)'." },

            // Unclosed list of arguments
            new InvalidExpressionTestCase { Expression = "func(", ExpectedErrorMessage = "Expression expected at position 5 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "func(42", ExpectedErrorMessage = "')' or ',' expected at position 7 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "func(42,", ExpectedErrorMessage = "Expression expected at position 8 in '$(Expression)'." },
            new InvalidExpressionTestCase { Expression = "func(((null))", ExpectedErrorMessage = "')' or ',' expected at position 13 in '$(Expression)'." },

            // Invalid end
            new InvalidExpressionTestCase { Expression = "42 42", ExpectedErrorMessage = "Syntax error at position 5 in '$(Expression)'." },        
        };

        public string Expression { get; set; }
        public string ExpectedErrorMessage { get; set; }
        public override string ToString() { return this.Expression; }
    }


    internal class ExpressionTestCase
    {
        public static string[] PropertyAccessNames = new string[]
        {
            "Foo",
            "Bar",
            "_some"
        };

        public static string[] PropertyAccessIncludingStarNames = new string[]
        {
            "Foo",
            "Bar",
            "_some",
            "*"
        };
 
        public static IEnumerable<ExpressionTestCase> VariousExpressions()
        {
            // Primitive
            yield return new ExpressionTestCase()
            {
                Expression = "42",
                ExpectedToken = new LiteralToken(42)
            };

            // Logical binary operator
            foreach (BinaryOperatorKind binaryOperatorKind in QueryTestUtils.BinaryOperatorGroups.Select(bg => bg.OperatorKinds[0]))
            {
                yield return new ExpressionTestCase()
                {
                    Expression = "'foo' or 'bar'",
                    ExpectedToken = new BinaryOperatorToken(
                        BinaryOperatorKind.Or,
                        new LiteralToken("foo"),
                        new LiteralToken("bar"))
                };
            }

            // Unary operator
            yield return new ExpressionTestCase()
            {
                Expression = "not false",
                ExpectedToken = new UnaryOperatorToken(UnaryOperatorKind.Not, new LiteralToken(false))
            };

            // Property access
            yield return new ExpressionTestCase()
            {
                Expression = "Customer/Name",
                ExpectedToken = new EndPathToken("Name", new InnerPathToken("Customer", null, null))
            };

            // Parenthesis
            yield return new ExpressionTestCase()
            {
                Expression = "(42)",
                ExpectedToken = new LiteralToken(42)
            };

            // Function call
            yield return new ExpressionTestCase()
            {
                Expression = "startswith(Name, 'John')",
                ExpectedToken = new FunctionCallToken(
                    "startswith",
                    new QueryToken[] 
                    { 
                        new EndPathToken("Name", null), 
                        new LiteralToken("John")
                    })
            };
        }

        public static IEnumerable<ExpressionTestCase> PrimitiveLiteralTestCases()
        {
            return new[]
            {
                // Null
                new ExpressionTestCase { Expression = "null", ExpectedToken = new LiteralToken(null) },

                // String
                new ExpressionTestCase { Expression = "''", ExpectedToken = new LiteralToken("") },
                new ExpressionTestCase { Expression = "'some'", ExpectedToken = new LiteralToken("some") },
                new ExpressionTestCase { Expression = "'foo''bar'", ExpectedToken = new LiteralToken("foo'bar") },

                // Boolean
                new ExpressionTestCase { Expression = "true", ExpectedToken = new LiteralToken(true) },
                new ExpressionTestCase { Expression = "false", ExpectedToken = new LiteralToken(false) },

                // DateTimeOffset
                new ExpressionTestCase { Expression = "2004-09-14T21:00:04-08:00", ExpectedToken = new LiteralToken(new DateTimeOffset(2004, 09, 14, 21, 0, 4, new TimeSpan(-8, 0, 0))) },

                // Duration
                new ExpressionTestCase { Expression = "duration'PT21H4S'", ExpectedToken = new LiteralToken(new TimeSpan(21, 00, 04)) },

                // Decimal
                new ExpressionTestCase { Expression = "42m", ExpectedToken = new LiteralToken((Decimal)42) },
                new ExpressionTestCase { Expression = "42.42M", ExpectedToken = new LiteralToken((Decimal)42.42) },

                // Int64
                new ExpressionTestCase { Expression = "42l", ExpectedToken = new LiteralToken((Int64)42) },
                new ExpressionTestCase { Expression = "42L", ExpectedToken = new LiteralToken((Int64)42) },

                // Int32
                new ExpressionTestCase { Expression = "42", ExpectedToken = new LiteralToken((Int32)42) },
                new ExpressionTestCase { Expression = "-123", ExpectedToken = new LiteralToken((Int32)(-123)) },

                // Double
                new ExpressionTestCase { Expression = "42.42d", ExpectedToken = new LiteralToken((Double)42.42) },
                new ExpressionTestCase { Expression = "-42.42d", ExpectedToken = new LiteralToken((Double)(-42.42)) },
                new ExpressionTestCase { Expression = "123456D", ExpectedToken = new LiteralToken((Double)123456) },

                // Single
                new ExpressionTestCase { Expression = "42.42f", ExpectedToken = new LiteralToken((Single)42.42) },
                new ExpressionTestCase { Expression = "-42.42F", ExpectedToken = new LiteralToken((Single)(-42.42)) },
                new ExpressionTestCase { Expression = "123456F", ExpectedToken = new LiteralToken((Single)123456) },

                // Guid
                new ExpressionTestCase { Expression = "FD3A4006-A6EF-420E-8EDA-DA2B1C58F6DF", ExpectedToken = new LiteralToken(new Guid("{FD3A4006-A6EF-420E-8EDA-DA2B1C58F6DF}")) },

                // Binary
                new ExpressionTestCase { Expression = "binary''", ExpectedToken = new LiteralToken(new byte[0]) },
                new ExpressionTestCase { Expression = "binary'AQID'", ExpectedToken = new LiteralToken(new byte[] { 1, 2, 3 }) },
                new ExpressionTestCase { Expression = "binary'qrvM'", ExpectedToken = new LiteralToken(new byte[] { 0xaa, 0xbb, 0xcc }) },

                //// ToDo:When we start supporting spatial uncomment these.
                //// Geography Point
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;POINT (20 10 30 40)'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;POINT (20 10 30 40)"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;POINT (20 10 NULL 40)'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;POINT (20 10 NULL 40)"))
                ////},
                ////// Geography LineString
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;LINESTRING (20 10, 30 20)'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;LINESTRING (20 10, 30 20)"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;LINESTRING (20 10 30, 30 20 40)'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;LINESTRING (20 10 30, 30 20 40)"))
                ////},
                ////// Geography Polygon
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;POLYGON ((20 10, 30 20, 40 30, 20 10), (-20 -10, -30 -20, -40 -30, -20 -10))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;POLYGON ((20 10, 30 20, 40 30, 20 10), (-20 -10, -30 -20, -40 -30, -20 -10))"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;POLYGON ((20 10 30, 30 20 40, 40 30 50, 20 10 30), (-20 -10 30, -30 -20 40, -40 -30 50, -20 -10 30))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;POLYGON ((20 10 30, 30 20 40, 40 30 50, 20 10 30), (-20 -10 30, -30 -20 40, -40 -30 50, -20 -10 30))"))
                ////},
                ////// Geography Collection
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;GEOMETRYCOLLECTION (LINESTRING (20 10, 30 20, 40 30, 20 10), POINT (-20 -10), POINT (-30 -20), POINT (-40 -30), POINT (-20 -10))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;GEOMETRYCOLLECTION (LINESTRING (20 10, 30 20, 40 30, 20 10), POINT (-20 -10), POINT (-30 -20), POINT (-40 -30), POINT (-20 -10))"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;GEOMETRYCOLLECTION (LINESTRING (20 10 30, 30 20 40, 40 30 50, 20 10 30), POINT (-20 -10 30), POINT (-30 -20 40), POINT (-40 -30 50), POINT (-20 -10 30))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;GEOMETRYCOLLECTION (LINESTRING (20 10 30, 30 20 40, 40 30 50, 20 10 30), POINT (-20 -10 30), POINT (-30 -20 40), POINT (-40 -30 50), POINT (-20 -10 30))"))
                ////},
                ////// Geography MultiPoint
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;MULTIPOINT ((20 10), (30 20), (40 30), (20 10), (-20 -10), (-30 -20), (-40 -30), (-20 -10))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;MULTIPOINT ((20 10), (30 20), (40 30), (20 10), (-20 -10), (-30 -20), (-40 -30), (-20 -10))"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;MULTIPOINT ((20 10 30), (30 20 40), (40 30 50), (20 10 30), (-20 -10 30), (-30 -20 40), (-40 -30 50), (-20 -10 30))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;MULTIPOINT ((20 10 30), (30 20 40), (40 30 50), (20 10 30), (-20 -10 30), (-30 -20 40), (-40 -30 50), (-20 -10 30))"))
                ////},
                ////// Geography MultiLineString
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;MULTILINESTRING ((20 10, 30 20), (20 20, 30 30), (20 30, 30 40), (20 40, 30 50))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;MULTILINESTRING ((20 10, 30 20), (20 20, 30 30), (20 30, 30 40), (20 40, 30 50))"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;MULTILINESTRING ((20 10 1, 30 20 0), (20 20 2, 30 30 1), (20 30 3, 30 40 2), (20 40 4, 30 50 3))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;MULTILINESTRING ((20 10 1, 30 20 0), (20 20 2, 30 30 1), (20 30 3, 30 40 2), (20 40 4, 30 50 3))"))
                ////},
                ////// Geography MultiPolygon
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;MULTIPOLYGON (((20 10, 30 20, 40 30, 20 10), (-20 -10, -30 -20, -40 -30, -20 -10)), ((22 12, 32 22, 42 32, 22 12), (-22 -12, -32 -22, -42 -32, -22 -12)), ((24 14, 34 24, 44 34, 24 14), (-24 -14, -34 -24, -44 -34, -24 -14)), ((26 16, 36 26, 46 36, 26 16), (-26 -16, -36 -26, -46 -36, -26 -16)))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;MULTIPOLYGON (((20 10, 30 20, 40 30, 20 10), (-20 -10, -30 -20, -40 -30, -20 -10)), ((22 12, 32 22, 42 32, 22 12), (-22 -12, -32 -22, -42 -32, -22 -12)), ((24 14, 34 24, 44 34, 24 14), (-24 -14, -34 -24, -44 -34, -24 -14)), ((26 16, 36 26, 46 36, 26 16), (-26 -16, -36 -26, -46 -36, -26 -16)))"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geography'SRID=4326;MULTIPOLYGON (((20 10 1, 30 20 2, 40 30 3, 20 10 1), (-20 -10 1, -30 -20 2, -40 -30 3, -20 -10 1)), ((22 12 1, 32 22 2, 42 32 3, 22 12 1), (-22 -12 1, -32 -22 2, -42 -32 3, -22 -12 1)), ((24 14 1, 34 24 2, 44 34 3, 24 14 1), (-24 -14 1, -34 -24 2, -44 -34 3, -24 -14 1)), ((26 16 1, 36 26 2, 46 36 3, 26 16 1), (-26 -16 1, -36 -26 2, -46 -36 3, -26 -16 1)))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeography("SRID=4326;MULTIPOLYGON (((20 10 1, 30 20 2, 40 30 3, 20 10 1), (-20 -10 1, -30 -20 2, -40 -30 3, -20 -10 1)), ((22 12 1, 32 22 2, 42 32 3, 22 12 1), (-22 -12 1, -32 -22 2, -42 -32 3, -22 -12 1)), ((24 14 1, 34 24 2, 44 34 3, 24 14 1), (-24 -14 1, -34 -24 2, -44 -34 3, -24 -14 1)), ((26 16 1, 36 26 2, 46 36 3, 26 16 1), (-26 -16 1, -36 -26 2, -46 -36 3, -26 -16 1)))"))
                ////},
                ////// Geometry Point
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;POINT (10 20 30 40)'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;POINT (10 20 30 40)"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;POINT (10 20 NULL 40)'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;POINT (10 20 NULL 40)"))
                ////},
                ////// Geometry LineString
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;LINESTRING (10 20, 20 30)'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;LINESTRING (10 20, 20 30)"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;LINESTRING (10 20 30, 20 30 40)'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;LINESTRING (10 20 30, 20 30 40)"))
                ////},
                ////// Geometry Polygon
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;POLYGON ((10 20, 20 30, 30 40, 10 20), (-10 -20, -20 -30, -30 -40, -10 -20))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;POLYGON ((10 20, 20 30, 30 40, 10 20), (-10 -20, -20 -30, -30 -40, -10 -20))"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;POLYGON ((10 20 30, 20 30 40, 30 40 50, 10 20 30), (-10 -20 30, -20 -30 40, -30 -40 50, -10 -20 30))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;POLYGON ((10 20 30, 20 30 40, 30 40 50, 10 20 30), (-10 -20 30, -20 -30 40, -30 -40 50, -10 -20 30))"))
                ////},
                ////// Geometry Collection
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;GEOMETRYCOLLECTION (LINESTRING (10 20, 20 30, 30 40, 10 20), POINT (-10 -20), POINT (-20 -30), POINT (-30 -40), POINT (-10 -20))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;GEOMETRYCOLLECTION (LINESTRING (10 20, 20 30, 30 40, 10 20), POINT (-10 -20), POINT (-20 -30), POINT (-30 -40), POINT (-10 -20))"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;GEOMETRYCOLLECTION (LINESTRING (10 20 30, 20 30 40, 30 40 50, 10 20 30), POINT (-10 -20 30), POINT (-20 -30 40), POINT (-30 -40 50), POINT (-10 -20 30))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;GEOMETRYCOLLECTION (LINESTRING (10 20 30, 20 30 40, 30 40 50, 10 20 30), POINT (-10 -20 30), POINT (-20 -30 40), POINT (-30 -40 50), POINT (-10 -20 30))"))
                ////},
                ////// Geometry MultiPoint
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;MULTIPOINT ((10 20), (20 30), (30 40), (10 20), (-10 -20), (-20 -30), (-30 -40), (-10 -20))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;MULTIPOINT ((10 20), (20 30), (30 40), (10 20), (-10 -20), (-20 -30), (-30 -40), (-10 -20))"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;MULTIPOINT ((10 20 30), (20 30 40), (30 40 50), (10 20 30), (-10 -20 30), (-20 -30 40), (-30 -40 50), (-10 -20 30))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;MULTIPOINT ((10 20 30), (20 30 40), (30 40 50), (10 20 30), (-10 -20 30), (-20 -30 40), (-30 -40 50), (-10 -20 30))"))
                ////},
                ////// Geometry MultiLineString
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;MULTILINESTRING ((10 20, 20 30), (20 20, 30 30), (30 20, 40 30), (40 20, 50 30))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;MULTILINESTRING ((10 20, 20 30), (20 20, 30 30), (30 20, 40 30), (40 20, 50 30))"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;MULTILINESTRING ((10 20 1, 20 30 0), (20 20 2, 30 30 1), (30 20 3, 40 30 2), (40 20 4, 50 30 3))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;MULTILINESTRING ((10 20 1, 20 30 0), (20 20 2, 30 30 1), (30 20 3, 40 30 2), (40 20 4, 50 30 3))"))
                ////},
                ////// Geometry MultiPolygon
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;MULTIPOLYGON (((10 20, 20 30, 30 40, 10 20), (-10 -20, -20 -30, -30 -40, -10 -20)), ((12 22, 22 32, 32 42, 12 22), (-12 -22, -22 -32, -32 -42, -12 -22)), ((14 24, 24 34, 34 44, 14 24), (-14 -24, -24 -34, -34 -44, -14 -24)), ((16 26, 26 36, 36 46, 16 26), (-16 -26, -26 -36, -36 -46, -16 -26)))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;MULTIPOLYGON (((10 20, 20 30, 30 40, 10 20), (-10 -20, -20 -30, -30 -40, -10 -20)), ((12 22, 22 32, 32 42, 12 22), (-12 -22, -22 -32, -32 -42, -12 -22)), ((14 24, 24 34, 34 44, 14 24), (-14 -24, -24 -34, -34 -44, -14 -24)), ((16 26, 26 36, 36 46, 16 26), (-16 -26, -26 -36, -36 -46, -16 -26)))"))
                ////},
                ////new ExpressionTestCase 
                ////{ 
                ////    Expression = "geometry'SRID=0;MULTIPOLYGON (((10 20 1, 20 30 2, 30 40 3, 10 20 1), (-10 -20 1, -20 -30 2, -30 -40 3, -10 -20 1)), ((12 22 1, 22 32 2, 32 42 3, 12 22 1), (-12 -22 1, -22 -32 2, -32 -42 3, -12 -22 1)), ((14 24 1, 24 34 2, 34 44 3, 14 24 1), (-14 -24 1, -24 -34 2, -34 -44 3, -14 -24 1)), ((16 26 1, 26 36 2, 36 46 3, 16 26 1), (-16 -26 1, -26 -36 2, -36 -46 3, -16 -26 1)))'", 
                ////    ExpectedToken = new LiteralToken(SpatialHelper.ParseGeometry("SRID=0;MULTIPOLYGON (((10 20 1, 20 30 2, 30 40 3, 10 20 1), (-10 -20 1, -20 -30 2, -30 -40 3, -10 -20 1)), ((12 22 1, 22 32 2, 32 42 3, 12 22 1), (-12 -22 1, -22 -32 2, -32 -42 3, -12 -22 1)), ((14 24 1, 24 34 2, 34 44 3, 14 24 1), (-14 -24 1, -24 -34 2, -34 -44 3, -14 -24 1)), ((16 26 1, 26 36 2, 36 46 3, 16 26 1), (-16 -26 1, -26 -36 2, -36 -46 3, -16 -26 1)))"))
                ////},
            };
        }

        public static IEnumerable<ExpressionTestCase> BinaryOperatorTestCases()
        {
            // Single operator
            foreach (var operatorKind in QueryTestUtils.BinaryOperatorGroups.SelectMany(og => og.OperatorKinds))
            {
                yield return new ExpressionTestCase()
                {
                    Expression = "false " + operatorKind.ToOperatorName() + " true",
                    ExpectedToken = new BinaryOperatorToken(operatorKind, new LiteralToken(false), new LiteralToken(true))
                };
            }

            // Two operators from the same group
            foreach (var operatorGroup in QueryTestUtils.BinaryOperatorGroups)
            {
                IEnumerable<BinaryOperatorKind[]> operatorPairs = new[] {
                    new BinaryOperatorKind[] { operatorGroup.OperatorKinds[0], operatorGroup.OperatorKinds[0] }
                };

                if (operatorGroup.OperatorKinds.Length > 1)
                {
                    operatorPairs = operatorPairs.Concat(operatorGroup.OperatorKinds.Variations(2));
                }

                foreach (var operatorPair in operatorPairs)
                {
                    yield return new ExpressionTestCase()
                    {
                        Expression = "1 " + operatorPair[0].ToOperatorName() + " 2 " + operatorPair[1].ToOperatorName() + " 3",
                        ExpectedToken = new BinaryOperatorToken(
                            operatorPair[1],
                            new BinaryOperatorToken(
                                operatorPair[0], 
                                new LiteralToken(1), 
                                new LiteralToken(2)),
                            new LiteralToken(3))
                    };

                    yield return new ExpressionTestCase()
                    {
                        Expression = "1 " + operatorPair[0].ToOperatorName() + " (2 " + operatorPair[1].ToOperatorName() + " 3)",
                        ExpectedToken = new BinaryOperatorToken(
                            operatorPair[0],
                            new LiteralToken(1),
                            new BinaryOperatorToken(
                                operatorPair[1],
                                new LiteralToken(2),
                                new LiteralToken(3))),
                    };
                }
            }

            // Two operators from different groups
            foreach (var operatorGroupHigher in QueryTestUtils.BinaryOperatorGroups)
            {
                foreach (var operatorGroupLower in QueryTestUtils.BinaryOperatorGroups.Where(og => og.Priority > operatorGroupHigher.Priority))
                {
                    foreach (var operatorKindHigher in operatorGroupHigher.OperatorKinds)
                    {
                        foreach (var operatorKindLower in operatorGroupLower.OperatorKinds)
                        {
                            // Lower and higher
                            yield return new ExpressionTestCase()
                            {
                                Expression = "1 " + operatorKindLower.ToOperatorName() + " 2 " + operatorKindHigher.ToOperatorName() + " 3",
                                ExpectedToken = new BinaryOperatorToken(
                                    operatorKindHigher,
                                    new BinaryOperatorToken(
                                        operatorKindLower,
                                        new LiteralToken(1),
                                        new LiteralToken(2)),
                                    new LiteralToken(3))
                            };

                            // Lower and higher with parenthesis
                            yield return new ExpressionTestCase()
                            {
                                Expression = "1 " + operatorKindLower.ToOperatorName() + " (2 " + operatorKindHigher.ToOperatorName() + " 3)",
                                ExpectedToken = new BinaryOperatorToken(
                                    operatorKindLower,
                                    new LiteralToken(1),
                                    new BinaryOperatorToken(
                                        operatorKindHigher,
                                        new LiteralToken(2),
                                        new LiteralToken(3)))
                            };

                            // Higher and lower
                            yield return new ExpressionTestCase()
                            {
                                Expression = "1 " + operatorKindHigher.ToOperatorName() + " 2 " + operatorKindLower.ToOperatorName() + " 3",
                                ExpectedToken = new BinaryOperatorToken(
                                    operatorKindHigher,
                                    new LiteralToken(1),
                                    new BinaryOperatorToken(
                                        operatorKindLower,
                                        new LiteralToken(2),
                                        new LiteralToken(3)))
                            };
                        }
                    }
                }
            }

            foreach (var operatorKind in QueryTestUtils.BinaryOperatorGroups.Select(bg => bg.OperatorKinds[0]))
            {
                foreach (var testCase in VariousExpressions())
                {
                    yield return new ExpressionTestCase()
                    {
                        Expression = "false " + operatorKind.ToOperatorName() + " (" + testCase.Expression + ")",
                        ExpectedToken = new BinaryOperatorToken(operatorKind, new LiteralToken(false), testCase.ExpectedToken)
                    };
                }
            }
        }

        public static IEnumerable<ExpressionTestCase> UnaryOperatorTestCases()
        {
            // Single unary operator
            foreach (var operatorKind in QueryTestUtils.UnaryOperatorKinds)
            {
                yield return new ExpressionTestCase()
                {
                    Expression = operatorKind.ToOperatorName() + " 'foo'",
                    ExpectedToken = new UnaryOperatorToken(operatorKind, new LiteralToken("foo"))
                };

                foreach (var testCase in VariousExpressions())
                {
                    yield return new ExpressionTestCase()
                    {
                        Expression = operatorKind.ToOperatorName() + "(" + testCase.Expression + ")",
                        ExpectedToken = new UnaryOperatorToken(operatorKind, testCase.ExpectedToken)
                    };
                }
            }

            // Two unary operators
            foreach (var operatorKindPair in QueryTestUtils.UnaryOperatorKinds.Variations(2))
            {
                yield return new ExpressionTestCase()
                {
                    Expression = operatorKindPair[0].ToOperatorName() + " " + operatorKindPair[1].ToOperatorName() + " 'foo'",
                    ExpectedToken = new UnaryOperatorToken(
                        operatorKindPair[0],
                        new UnaryOperatorToken(
                            operatorKindPair[1],
                            new LiteralToken("foo")))
                };
            }

            // Unary and binary operator.
            foreach (var unaryOperatorKind in QueryTestUtils.UnaryOperatorKinds)
            {
                foreach (var binaryOperatorKind in QueryTestUtils.BinaryOperatorKinds)
                {
                    yield return new ExpressionTestCase()
                    {
                        Expression = unaryOperatorKind.ToOperatorName() + " 'foo' " + binaryOperatorKind.ToOperatorName() + " 'bar'",
                        ExpectedToken = new BinaryOperatorToken(
                            binaryOperatorKind,
                            new UnaryOperatorToken(unaryOperatorKind, new LiteralToken("foo")),
                            new LiteralToken("bar"))
                    };

                    // With parenthesis
                    yield return new ExpressionTestCase()
                    {
                        Expression = unaryOperatorKind.ToOperatorName() + " ('foo' " + binaryOperatorKind.ToOperatorName() + " 'bar')",
                        ExpectedToken = new UnaryOperatorToken(
                            unaryOperatorKind,
                            new BinaryOperatorToken(binaryOperatorKind,
                                new LiteralToken("foo"),
                                new LiteralToken("bar")))
                    };
                }
            }
        }

        public static IEnumerable<ExpressionTestCase> PropertyAccessTestCases(string [] propertyNames)
        {
            foreach (var properties in propertyNames.Variations().Where(p => p.Length > 0))
            {
                // Simple property access paths
                QueryToken pathToken = null;
                foreach (string propertyName in properties)
                {
                    pathToken = new InnerPathToken(propertyName, pathToken, null);
                }
                InnerPathToken pathNav = (InnerPathToken)pathToken;
                pathToken = new EndPathToken(pathNav.Identifier, pathNav.NextToken);

                yield return new ExpressionTestCase()
                {
                    Expression = string.Join("/", properties),
                    ExpectedToken = pathToken
                };
            }
        }

        public static IEnumerable<ExpressionTestCase> ParenthesisTestCases()
        {
            foreach (var testCase in VariousExpressions())
            {
                string expression = testCase.Expression;
                for (int i = 1; i < 5; i++)
                {
                    expression = "(" + expression + ")";
                }

                yield return new ExpressionTestCase()
                {
                    Expression = expression,
                    ExpectedToken = testCase.ExpectedToken
                };
            }
        }

        public static IEnumerable<ExpressionTestCase> FunctionCallTestCases()
        {
            string[] functionNames = new string[] { "func", "_MyFunc", "cast" };
            foreach (string functionName in functionNames)
            {
                foreach (var arguments in VariousExpressions().ToList().Variations(0, 1, 3))
                {
                    yield return new ExpressionTestCase()
                    {
                        Expression = functionName + "(" + string.Join(",", arguments.Select(a => a.Expression).ToArray()) + ")",
                        ExpectedToken = new FunctionCallToken(functionName, arguments.Select(a => a.ExpectedToken))
                    };
                }
            }
        }

        public string Expression { get; set; }
        public QueryToken ExpectedToken { get; set; }
        public override string ToString() { return this.Expression; }
    }
}
