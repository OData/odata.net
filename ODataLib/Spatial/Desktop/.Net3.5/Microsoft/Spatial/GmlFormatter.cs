//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Spatial
{
    using System.Xml;

    /// <summary>
    ///   The object to move spatial types to and from the GML format
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml", Justification = "Gml is the accepted name in the industry")]
    public abstract class GmlFormatter : SpatialFormatter<XmlReader, XmlWriter>
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Spatial.GmlFormatter" /> class.</summary>
        /// <param name="creator">The implementation that created this instance.</param>
        protected GmlFormatter(SpatialImplementation creator) : base(creator)
        {
        }

        /// <summary>Creates the implementation of the formatter.</summary>
        /// <returns>The created GmlFormatter implementation.</returns>
        public static GmlFormatter Create()
        {
            return SpatialImplementation.CurrentImplementation.CreateGmlFormatter();
        }
    }
}
