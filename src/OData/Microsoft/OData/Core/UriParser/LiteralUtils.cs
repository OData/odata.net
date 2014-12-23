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

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System;
    using System.IO;
    using Microsoft.Spatial;
    using Microsoft.Data.Spatial;
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
