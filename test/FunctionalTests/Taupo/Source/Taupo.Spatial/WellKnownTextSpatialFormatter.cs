//---------------------------------------------------------------------
// <copyright file="WellKnownTextSpatialFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial
{
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Spatial.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Default well-known-text formatter for spatial values
    /// </summary>
    [ImplementationName(typeof(IWellKnownTextSpatialFormatter), "Default")]
    public class WellKnownTextSpatialFormatter : SpatialPrimitiveFormatterBase<string, TextReader, TextWriter>, IWellKnownTextSpatialFormatter
    {
        /// <summary>
        /// Prepared a formatted value to be parsed by the wrapped format
        /// In this case, wraps a string with a StringReader.
        /// </summary>
        /// <param name="formatted">The formatted value</param>
        /// <returns>The prepared value to pass into the wrapped format</returns>
        protected internal override TextReader PrepareFormattedValueForParsing(string formatted)
        {
            return new StringReader(formatted);
        }

        /// <summary>
        /// Handles a formatted value returned from the underlying format.
        /// In this case, just returns the value because the types match.
        /// </summary>
        /// <param name="spatialInstance">The spatial instance.</param>
        /// <returns>The format of the correct data type.</returns>
        protected internal override string WriteFormattedValue(ISpatial spatialInstance)
        {
            StringBuilder builder = new StringBuilder();
            using (StringWriter writer = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                this.Formatter.Write(spatialInstance, writer);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Create an instance of the spatial formatter to use
        /// </summary>
        /// <returns>The spatial format to use</returns>
        protected override SpatialFormatter<TextReader, TextWriter> CreateFormatter()
        {
            return WellKnownTextSqlFormatter.Create();
        }
    }
}
