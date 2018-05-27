//---------------------------------------------------------------------
// <copyright file="LiteralUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using System.IO;
    using Microsoft.Spatial;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for with literals.
    /// </summary>
    internal static class LiteralUtils
    {
        /// <summary>
        /// The formatter to create/format text to and from spatial.
        /// </summary>
        private static WellKnownTextSqlFormatter Formatter
        {
            get
            {
                return SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter(false /*allowOnlyTwoDimensions*/);
            }
        }

        /// <summary>
        /// Parse the given text as a Geography literal.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>The Geography object if succeeded, else a ParseErrorException is thrown.</returns>
        internal static Geography ParseGeography(string text)
        {
            using (StringReader reader = new StringReader(text))
            {
                return Formatter.Read<Geography>(reader);
            }
        }

        /// <summary>
        /// Parse the given text as a Geometry literal.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>The Geometry object if succeeded, else a ParseErrorException is thrown.</returns>
        internal static Geometry ParseGeometry(string text)
        {
            using (StringReader reader = new StringReader(text))
            {
                return Formatter.Read<Geometry>(reader);
            }
        }

        /// <summary>
        /// Convert to string the given Geography instance.
        /// </summary>
        /// <param name="instance">Instance to convert.</param>
        /// <returns>Well-known text representation.</returns>
        internal static string ToWellKnownText(Geography instance)
        {
            return Formatter.Write(instance);
        }

        /// <summary>
        /// Convert to string the given Geometry instance.
        /// </summary>
        /// <param name="instance">Instance to convert.</param>
        /// <returns>Well-known text representation.</returns>
        internal static string ToWellKnownText(Geometry instance)
        {
            return Formatter.Write(instance);
        }
    }
}