//---------------------------------------------------------------------
// <copyright file="GmlFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Xml;

    /// <summary>
    ///   The object to move spatial types to and from the GML format
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml", Justification = "Gml is the accepted name in the industry")]
    public abstract class GmlFormatter : SpatialFormatter<XmlReader, XmlWriter>
    {
        /// <summary>Initializes a new instance of the <see cref="Microsoft.Spatial.GmlFormatter" /> class.</summary>
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
