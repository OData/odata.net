//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
