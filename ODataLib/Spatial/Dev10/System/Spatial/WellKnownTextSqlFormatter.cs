//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Spatial
{
    using System.IO;

    /// <summary>
    /// The object to move spatial types to and from the WellKnownTextSql format
    /// </summary>
    public abstract class WellKnownTextSqlFormatter : SpatialFormatter<TextReader, TextWriter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WellKnownTextSqlFormatter"/> class.
        /// </summary>
        /// <param name="creator">The implementation that created this instance.</param>
        protected WellKnownTextSqlFormatter(SpatialImplementation creator) : base(creator)
        {
        }

        /// <summary>
        /// Creates the implementation of the formatter
        /// </summary>
        /// <returns>Returns the created WellKnownTextSqlFormatter implementation</returns>
        public static WellKnownTextSqlFormatter Create()
        {
            return SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter();
        }

        /// <summary>
        /// Creates the specified has Z.
        /// </summary>
        /// <param name="allowOnlyTwoDimensions">Restricts the formatter to allow only two dimensions.</param>
        /// <returns>The created WellKnownTextSqlFormatter.</returns>
        public static WellKnownTextSqlFormatter Create(bool allowOnlyTwoDimensions)
        {
            return SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter(allowOnlyTwoDimensions);
        }
    }
}
