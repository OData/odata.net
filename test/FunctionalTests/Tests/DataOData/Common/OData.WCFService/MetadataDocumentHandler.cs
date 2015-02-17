//---------------------------------------------------------------------
// <copyright file="MetadataDocumentHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System.IO;

    /// <summary>
    /// Class for handling $metadata requests.
    /// </summary>
    public class MetadataDocumentHandler : RequestHandler
    {
        /// <summary>
        /// Writes the service metadata document to the response stream.
        /// </summary>
        /// <returns>Stream containg the metadata document for the backing data store.</returns>
        public Stream ProcessMetadataRequest()
        {
            return this.WriteResponse(
                200,
                (writer, writerSettings, message) =>
                {
                    message.SetHeader("Content-Type", "application/xml;charset-utf-8");
                    writer.WriteMetadataDocument();
                });
        }
    }
}