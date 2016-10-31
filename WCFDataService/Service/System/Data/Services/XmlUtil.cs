//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services
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
