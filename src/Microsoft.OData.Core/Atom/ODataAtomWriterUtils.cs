//---------------------------------------------------------------------
// <copyright file="ODataAtomWriterUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData writer for the ATOM format.
    /// </summary>
    internal static class ODataAtomWriterUtils
    {
        /// <summary>
        /// Creates an Xml writer over the specified stream, with the provided settings and encoding.
        /// </summary>
        /// <param name="stream">The stream to create the XmlWriter over.</param>
        /// <param name="messageWriterSettings">The OData message writer settings used to control the settings of the Xml writer.</param>
        /// <param name="encoding">The encoding used for writing.</param>
        /// <returns>An <see cref="XmlWriter"/> instance configured with the provided settings and encoding.</returns>
        internal static XmlWriter CreateXmlWriter(Stream stream, ODataMessageWriterSettings messageWriterSettings, Encoding encoding)
        {
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            XmlWriterSettings xmlWriterSettings = CreateXmlWriterSettings(messageWriterSettings, encoding);

            XmlWriter writer = XmlWriter.Create(stream, xmlWriterSettings);
            if (messageWriterSettings.AlwaysUseDefaultXmlNamespaceForRootElement)
            {
                writer = new DefaultNamespaceCompensatingXmlWriter(writer);
            }

            return writer;
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of nested inner errors to allow.</param>
        internal static void WriteError(XmlWriter writer, ODataError error, bool includeDebugInformation, int maxInnerErrorDepth)
        {
            ErrorUtils.WriteXmlError(writer, error, includeDebugInformation, maxInnerErrorDepth);
        }

        /// <summary>
        /// Creates a new XmlWriterSettings instance using the encoding.
        /// </summary>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="encoding">Encoding to  use in the writer settings.</param>
        /// <returns>The Xml writer settings to use for this writer.</returns>
        private static XmlWriterSettings CreateXmlWriterSettings(ODataMessageWriterSettings messageWriterSettings, Encoding encoding)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = messageWriterSettings.CheckCharacters;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.OmitXmlDeclaration = false;
            settings.Encoding = encoding ?? MediaTypeUtils.EncodingUtf8NoPreamble;
            settings.NewLineHandling = NewLineHandling.Entitize;

            // COMPAT 1: ATOM XML indentation - WCFDS always indents the XML, but we will allow users to specify if the indentation is desirable
            settings.Indent = messageWriterSettings.Indent;

            // we do not want to close the underlying stream when the OData writer is closed since we don't own the stream.
            settings.CloseOutput = false;

            return settings;
        }
    }
}
