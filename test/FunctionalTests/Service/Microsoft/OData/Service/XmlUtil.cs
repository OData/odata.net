//---------------------------------------------------------------------
// <copyright file="XmlUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// static xml utility function
    /// </summary>
    internal static class XmlUtil
    {
        /// <summary>
        /// Creates a new XmlWriterSettings instance using the encoding.
        /// </summary>
        /// <param name="encoding">Encoding that you want to specify in the reader settings as well as the processing instruction</param>
        /// <returns>A writer settings instance with the given encoding.</returns>
        internal static XmlWriterSettings CreateXmlWriterSettings(Encoding encoding)
        {
            Debug.Assert(encoding != null, "encoding != null");

            // No need to close the underlying stream here for client,
            // since it always MemoryStream for writing i.e. it caches the response before processing.
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = false;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Encoding = encoding;
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            Debug.Assert(!settings.CloseOutput, "!settings.CloseOutput -- otherwise default changed?");

            return settings;
        }

        /// <summary>
        /// Creates a new XmlWriter instance using the specified stream and writers the processing instruction
        /// with the given encoding value
        /// </summary>
        /// <param name="stream"> The stream to which you want to write</param>
        /// <param name="encoding"> Encoding that you want to specify in the reader settings as well as the processing instruction </param>
        /// <returns>XmlWriter with the appropriate xml settings and processing instruction</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing the XmlWriter")]
        internal static XmlWriter CreateXmlWriterAndWriteProcessingInstruction(Stream stream, Encoding encoding)
        {
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(encoding != null, "encoding != null");

            XmlWriterSettings settings = CreateXmlWriterSettings(encoding);
            XmlWriter writer = XmlWriter.Create(stream, settings);
            writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"" + encoding.WebName + "\" standalone=\"yes\"");
            return writer;
        }
    }
}