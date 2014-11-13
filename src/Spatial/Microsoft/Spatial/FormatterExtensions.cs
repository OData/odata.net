//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
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
