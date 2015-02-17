//---------------------------------------------------------------------
// <copyright file="XmlUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData.Service;
    using System.Diagnostics;
    using System.Xml;
    using AstoriaUnitTests.Stubs;

    public static class XmlUtils
    {
        /// <summary>
        /// Parses the response as XML and if there's an in-stream error in it it will return it.
        /// </summary>
        /// <param name="request">The request from which to read the response.</param>
        /// <returns>If there was an in-stream error this will return it as an exception instance, otherwise it returns null.</returns>
        public static Exception ParseResponseInStreamError(this TestWebRequest request)
        {
            using (XmlReader reader = XmlReader.Create(request.GetResponseStream()))
            {
                return ParseInStreamError(reader);
            }
        }

        /// <summary>
        /// Parses the XML and if there's an in-stream error in it it will return it.
        /// </summary>
        /// <param name="reader">The reader to read the XML from.</param>
        /// <returns>If there was an in-stream error this will return it as an exception instance, otherwise it returns null.</returns>
        public static Exception ParseInStreamError(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    reader.LocalName == "error" &&
                    reader.NamespaceURI == UnitTestsUtil.MetadataNamespace.NamespaceName)
                {
                    return ParseInStreamSingleError(reader.ReadSubtree());
                }
            }
            return null;
        }

        /// <summary>
        /// Parses a single error element from the reader
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <returns>The exception parsed.</returns>
        private static Exception ParseInStreamSingleError(XmlReader reader)
        {
            string type = null;
            string stackTrace = null;
            string message = null;
            Exception innerException = null;

            reader.Read();
            reader.Read();
            while (!reader.EOF)
            {
                if (reader.NodeType != XmlNodeType.Element ||
                    reader.NamespaceURI != UnitTestsUtil.MetadataNamespace.NamespaceName)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.LocalName)
                {
                    case "message":
                        message = reader.ReadElementContentAsString(); break;
                    case "type":
                        type = reader.ReadElementContentAsString(); break;
                    case "stacktrace":
                        stackTrace = reader.ReadElementContentAsString(); break;
                    case "innererror":
                        innerException = ParseInStreamSingleError(reader.ReadSubtree());
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            Exception exception;
            switch (type)
            {
                case "System.InvalidOperationException":
                    exception = new InvalidOperationException(message, innerException);
                    break;
                default:
                    exception = new DataServiceException(message, innerException);
                    break;
            }

            exception.Data["StackTrace"] = stackTrace;
            return exception;
        }
    }
}