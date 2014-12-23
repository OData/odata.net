//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.Spatial
{
    /// <summary>
    /// Well Known Text Constants
    /// </summary>
    internal class WellKnownTextConstants
    {
        /// <summary>
        /// SRID
        /// </summary>
        internal const string WktSrid = "SRID";

        /// <summary>
        /// POINT
        /// </summary>
        internal const string WktPoint = "POINT";

        /// <summary>
        /// LINESTRING
        /// </summary>
        internal const string WktLineString = "LINESTRING";

        /// <summary>
        /// POLYGON
        /// </summary>
        internal const string WktPolygon = "POLYGON";

        /// <summary>
        /// GEOMETRYCOLLECTION
        /// DEVNOTE: Because there is no inherent Geography support in the WKT specification,
        /// this constant is used for both GeographyCollection and GeometryCollection
        /// </summary>
        internal const string WktCollection = "GEOMETRYCOLLECTION";

        /// <summary>
        /// MULTIPOINT
        /// </summary>
        internal const string WktMultiPoint = "MULTIPOINT";

        /// <summary>
        /// MULTILINESTRING
        /// </summary>
        internal const string WktMultiLineString = "MULTILINESTRING";

        /// <summary>
        /// MULTIPOLYGON
        /// </summary>
        internal const string WktMultiPolygon = "MULTIPOLYGON";

        /// <summary>
        /// FULLGLOBE
        /// </summary>
        internal const string WktFullGlobe = "FULLGLOBE";

        /// <summary>
        /// NULL
        /// </summary>
        internal const string WktEmpty = "EMPTY";

        /// <summary>
        /// NULL
        /// </summary>
        internal const string WktNull = "NULL";

        /// <summary>
        /// Equals Operator '='
        /// </summary>
        internal const string WktEquals = "=";

        /// <summary>
        /// Semicolon ';'
        /// </summary>
        internal const string WktSemiColon = ";";

        /// <summary>
        /// Delimiter ',' + WktWhiteSpace
        /// </summary>
        internal const string WktDelimiterWithWhiteSpace = ", ";

        /// <summary>
        /// Open Parenthesis '('
        /// </summary>
        internal const string WktOpenParen = "(";

        /// <summary>
        /// Close Parenthesis ');
        /// </summary>
        internal const string WktCloseParen = ")";

        /// <summary>
        /// Whitespace ' '
        /// </summary>
        internal const string WktWhitespace = " ";

        /// <summary>
        /// Period/Dot '.'
        /// </summary>
        internal const string WktPeriod = ".";
    }
}
