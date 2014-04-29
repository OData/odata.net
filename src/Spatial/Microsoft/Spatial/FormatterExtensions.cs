//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
