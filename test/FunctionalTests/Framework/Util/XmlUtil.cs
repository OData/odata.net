//---------------------------------------------------------------------
// <copyright file="XmlUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    #region Namespaces
    
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using System.Text;
    
    #endregion Namespaces

    /// <summary>This class provides utility methods for XML tasks.</summary>
    public static class XmlUtil
    {
        /// <summary>Creates a new XmlDocument and loads the specified XML text.</summary>
        /// <param name="text">XML text to load.</param>
        /// <returns>A new <see cref="XmlDocument"/> instance.</returns>
        public static XmlDocument XmlDocumentFromString(string text)
        {
            Debug.Assert(text != null, "text != null");
            XmlDocument result = new XmlDocument();
            result.LoadXml(text);
            return result;
        }

        /// <summary>
        /// Creates a new XmlWriterSettings instance using the encoding.
        /// </summary>
        /// <param name="encoding"> Encoding that you want to specify in the reader settings as well as the XML declaration </param>
        public static XmlWriterSettings CreateXmlWriterSettings(Encoding encoding)
        {
            Debug.Assert(null != encoding, "null != encoding");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = false;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Encoding = encoding;
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
            settings.OmitXmlDeclaration = false;

            Debug.Assert(!settings.CloseOutput, "!settings.CloseOutput -- otherwise default changed?");

            return settings;
        }
    }
}
