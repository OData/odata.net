//   OData .NET Libraries ver. 5.6.3
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

namespace System.Data.Services.Parsing
{
    using System;
    using System.Diagnostics;
    using System.Data.Services;
    using System.IO;
    using System.Spatial;

    /// <summary>
    /// Parser for well-known-text spatial literals seen in a URI.
    /// </summary>
    internal static class WellKnownTextParser
    {
        /// <summary>
        /// Tries to parse a spatial literal.
        /// </summary>
        /// <param name="literalType">Type of the literal.</param>
        /// <param name="literalText">The literal text.</param>
        /// <param name="literalValue">The literal value.</param>
        /// <returns>true if the parse was successful, false otherwise</returns>
        internal static bool TryParseSpatialLiteral(Type literalType, string literalText, out object literalValue)
        {
            Debug.Assert(literalType.IsSpatial(), "the passed in type must implement ISpatial");

            var wellKnownTextSqlFormatter = WellKnownTextSqlFormatter.Create(true);
            if (typeof(Geography).IsAssignableFrom(literalType))
            {
                Geography geography;
                var worked = TryParseSpatialLiteral(literalText, XmlConstants.LiteralPrefixGeography, wellKnownTextSqlFormatter, out geography);
                literalValue = geography;
                return worked;
            }
            else
            {
                Geometry geometry;
                var worked = TryParseSpatialLiteral(literalText, XmlConstants.LiteralPrefixGeometry, wellKnownTextSqlFormatter, out geometry);
                literalValue = geometry;
                return worked;
            }
        }

        /// <summary>
        /// Tries to extract the WellKnownTextSQL portion from an astoria spatial literal.
        /// </summary>
        /// <param name="spatialLiteral">The spatial literal.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="wellKnownTextSql">The well known text SQL.</param>
        /// <returns>true if the extract was successful, false otherwise.</returns>
        private static bool TryExtractWellKnownTextSqlFromSpatialLiteral(string spatialLiteral, string prefix, out string wellKnownTextSql)
        {
            var worked = WebConvert.TryRemovePrefix(prefix, ref spatialLiteral);
            if (!worked)
            {
                wellKnownTextSql = null;
                return false;
            }

            worked = WebConvert.TryRemoveQuotes(ref spatialLiteral);
            if (!worked)
            {
                wellKnownTextSql = null;
                return false;
            }

            wellKnownTextSql = spatialLiteral;
            return true;
        }

        /// <summary>
        /// Tries to parse a spatial literal.
        /// </summary>
        /// <typeparam name="T">The spatial type to parse to</typeparam>
        /// <param name="literalText">The literal text.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="formatter">The formatter.</param>
        /// <param name="literalValue">The literal value.</param>
        /// <returns>true if the parse was successful, false otherwise</returns>
        private static bool TryParseSpatialLiteral<T>(string literalText, string prefix, WellKnownTextSqlFormatter formatter, out T literalValue) where T : class, ISpatial
        {
            string text;
            var worked = TryExtractWellKnownTextSqlFromSpatialLiteral(literalText, prefix, out text);
            if (!worked)
            {
                literalValue = null;
                return false;
            }

            using (var reader = new StringReader(text))
            {
                try
                {
                    literalValue = formatter.Read<T>(reader);
                    return true;
                }
                catch (Exception e)
                {
                    if (CommonUtil.IsCatchableExceptionType(e))
                    {
                        literalValue = null;
                        return false;
                    }

                    throw;
                }
            }
        }
    }
}
