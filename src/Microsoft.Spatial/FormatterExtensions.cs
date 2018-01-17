//---------------------------------------------------------------------
// <copyright file="FormatterExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>Represents the extensions to formatters.</summary>
    public static class FormatterExtensions
    {
        /// <summary>Writes the specified formatter.</summary>
        /// <returns>A string value of the formatted object.</returns>
        /// <param name="formatter">The formatter.</param>
        /// <param name="spatial">The spatial object.</param>
        public static String Write(this SpatialFormatter<TextReader, TextWriter> formatter, ISpatial spatial)
        {
            Util.CheckArgumentNull(formatter, "formatter");

            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                formatter.Write(spatial, writer);
            }

            return builder.ToString();
        }

        /// <summary>Writes the specified formatter.</summary>
        /// <returns>A string value of the formatted object.</returns>
        /// <param name="formatter">The formatter.</param>
        /// <param name="spatial">The spatial object.</param>
        public static String Write(this SpatialFormatter<XmlReader, XmlWriter> formatter, ISpatial spatial)
        {
            Util.CheckArgumentNull(formatter, "formatter");

            StringBuilder builder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings() { OmitXmlDeclaration = true };
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                formatter.Write(spatial, writer);
            }

            return builder.ToString();
        }
    }
}
