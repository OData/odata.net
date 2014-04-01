//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
