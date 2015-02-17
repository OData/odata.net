//---------------------------------------------------------------------
// <copyright file="GmlSpatialFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial
{
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Spatial.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Default GML formatter for spatial values
    /// </summary>
    [ImplementationName(typeof(IGmlSpatialFormatter), "Default")]
    public class GmlSpatialFormatter : SpatialPrimitiveFormatterBase<XElement, XmlReader, XmlWriter>, IGmlSpatialFormatter
    {
        /// <summary>
        /// Writes the formatted value.
        /// </summary>
        /// <param name="spatialInstance">The spatial instance.</param>
        /// <returns>The format of the correct data type.</returns>
        protected internal override XElement WriteFormattedValue(ISpatial spatialInstance)
        {
            StringBuilder builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder))
            {
                this.Formatter.Write(spatialInstance, writer);
            }

            return XElement.Parse(builder.ToString());
        }

        /// <summary>
        /// Handles a formatted value returned from the underlying format.
        /// In this case, creates a XmlReader from an XElement.
        /// </summary>
        /// <param name="formatted">The value from the underlying format</param>
        /// <returns>The value to return</returns>
        protected internal override XmlReader PrepareFormattedValueForParsing(XElement formatted)
        {
            return formatted.CreateReader();
        }

        /// <summary>
        /// Create an instance of the spatial formatter to use
        /// </summary>
        /// <returns>The spatial format to use</returns>
        protected override SpatialFormatter<XmlReader, XmlWriter> CreateFormatter()
        {
            return GmlFormatter.Create();
        }

        /// <summary>
        /// Determines whether the formatted value cannot possibly be spatial.
        /// </summary>
        /// <param name="formatted">The formatted value.</param>
        /// <returns>true if it is definitely not spatial</returns>
        protected override bool IsDefinitelyNotSpatial(XElement formatted)
        {
            return formatted.Name.NamespaceName != SpatialConstants.GmlNamespaceName;
        }
    }
}
