//---------------------------------------------------------------------
// <copyright file="WellKnownTextConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
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