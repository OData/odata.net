//---------------------------------------------------------------------
// <copyright file="ServiceDocumentHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System.IO;
    using Microsoft.OData;

    /// <summary>
    /// Class for handling service document requests.
    /// </summary>
    public class ServiceDocumentHandler : RequestHandler
    {
        /// <summary>
        /// Serialises the service document to the response stream.
        /// </summary>
        /// <returns>Stream containing the service document.</returns>
        public Stream ProcessServiceDocumentRequest()
        {
            return this.WriteResponse(
                200,
                (writer, writerSettings, message) =>
                {
                    message.SetHeader("Content-Type", "application/xml;charset=utf-8");
                    writer.WriteServiceDocument(this.Model.GenerateServiceDocument());
                });
        }
    }
}