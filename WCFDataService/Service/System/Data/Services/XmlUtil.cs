//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
