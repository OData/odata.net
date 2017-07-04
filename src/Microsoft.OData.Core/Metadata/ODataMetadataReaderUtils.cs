//---------------------------------------------------------------------
// <copyright file="ODataMetadataReaderUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Metadata
{
    #region Namespaces
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData reader for the Metadata format.
    /// </summary>
    internal static class ODataMetadataReaderUtils
    {
        /// <summary>
        /// Creates an Xml reader over the specified stream with the provided settings.
        /// </summary>
        /// <param name="stream">The stream to create the XmlReader over.</param>
        /// <param name="encoding">The encoding to use to read the input.</param>
        /// <param name="messageReaderSettings">The OData message reader settings used to control the settings of the Xml reader.</param>
        /// <returns>An <see cref="XmlReader"/> instance configured with the provided settings.</returns>
        internal static XmlReader CreateXmlReader(Stream stream, Encoding encoding, ODataMessageReaderSettings messageReaderSettings)
        {
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            XmlReaderSettings xmlReaderSettings = CreateXmlReaderSettings(messageReaderSettings);

            if (encoding != null)
            {
                // Use the encoding from the content type if specified.
                // NOTE: The XmlReader will scan ahead and determine the encoding from the Xml declaration
                //       and or the payload. Only if no encoding is specified in the Xml declaration and
                //       the Xml reader cannot figure out the encoding from the payload, can it happen
                //       that we need to specify the encoding explicitly (and that wrapping the stream with
                //       a stream reader makes a difference in the first place).
                return XmlReader.Create(new StreamReader(stream, encoding), xmlReaderSettings);
            }

            return XmlReader.Create(stream, xmlReaderSettings);
        }

        /// <summary>
        /// Creates a new XmlReaderSettings instance using the encoding.
        /// </summary>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <returns>The Xml reader settings to use for this reader.</returns>
        private static XmlReaderSettings CreateXmlReaderSettings(ODataMessageReaderSettings messageReaderSettings)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CheckCharacters = messageReaderSettings.EnableCharactersCheck;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.CloseInput = true;

            // We do not allow DTDs - this is the default
#if ORCAS
            settings.ProhibitDtd = true;
#else
            settings.DtdProcessing = DtdProcessing.Prohibit;
#endif

            return settings;
        }
    }
}
