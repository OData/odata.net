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
    using System.IO;

    /// <summary>
    /// The object to move spatial types to and from the WellKnownTextSql format
    /// </summary>
    public abstract class WellKnownTextSqlFormatter : SpatialFormatter<TextReader, TextWriter>
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Spatial.WellKnownTextSqlFormatter" /> class.</summary>
        /// <param name="creator">The implementation that created this instance.</param>
        protected WellKnownTextSqlFormatter(SpatialImplementation creator) : base(creator)
        {
        }

        /// <summary>Creates the implementation of the formatter.</summary>
        /// <returns>Returns the created WellKnownTextSqlFormatter implementation.</returns>
        public static WellKnownTextSqlFormatter Create()
        {
            return SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter();
        }

        /// <summary>Creates the implementation of the formatter and checks whether the specified formatter has Z.</summary>
        /// <returns>The created WellKnownTextSqlFormatter.</returns>
        /// <param name="allowOnlyTwoDimensions">Restricts the formatter to allow only two dimensions.</param>
        public static WellKnownTextSqlFormatter Create(bool allowOnlyTwoDimensions)
        {
            return SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter(allowOnlyTwoDimensions);
        }
    }
}
