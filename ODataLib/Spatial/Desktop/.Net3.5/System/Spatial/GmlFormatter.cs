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
    using Xml;

    /// <summary>
    ///   The object to move spatial types to and from the GML format
    /// </summary>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml", Justification = "Gml is the accepted name in the industry")]
    public abstract class GmlFormatter : SpatialFormatter<XmlReader, XmlWriter>
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Spatial.GmlFormatter" /> class.</summary>
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
