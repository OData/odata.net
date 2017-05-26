//---------------------------------------------------------------------
// <copyright file="ServiceDocumentHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System.Net;
    using Microsoft.OData;

    /// <summary>
    /// Class for handling service document requests.
    /// </summary>
    public class ServiceDocumentHandler : RequestHandler
    {
        public ServiceDocumentHandler(RequestHandler other)
            : base(other, HttpMethod.GET)
        {
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            responseMessage.SetStatusCode(HttpStatusCode.OK);
            using (var writer = this.CreateMessageWriter(responseMessage))
            {
                writer.WriteServiceDocument(this.GenerateServiceDocument());
            }
        }

        private ODataServiceDocument GenerateServiceDocument()
        {
            return this.DataSource.Model.GenerateServiceDocument();
        }
    }
}