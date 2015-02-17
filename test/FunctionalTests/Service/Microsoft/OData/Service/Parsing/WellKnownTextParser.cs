//---------------------------------------------------------------------
// <copyright file="WellKnownTextParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Parsing
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Microsoft.OData.Service;
    using Microsoft.Spatial;

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
        [SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "We're calling this correctly")]
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