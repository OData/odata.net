//---------------------------------------------------------------------
// <copyright file="StatusMonitorRequestHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.OData;

    public class StatusMonitorRequestHandler : RequestHandler
    {
        public StatusMonitorRequestHandler(RequestHandler other, Uri requestUri = null, IEnumerable<KeyValuePair<string, string>> headers = null)
            : base(other, HttpMethod.GET, requestUri, headers)
        {
            
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            string asyncToken = this.QueryContext.AsyncToken;
            AsyncTask asyncTask = AsyncTask.GetTask(asyncToken);

            if (asyncTask == null)
            {
                // token is invalid or expired. 
                throw Utility.BuildException(HttpStatusCode.NotFound);
            }
            else
            {
                if (!asyncTask.Ready)
                {
                    ResponseWriter.WriteAsyncPendingResponse(responseMessage, asyncToken);
                }
                else
                {
                    responseMessage.SetHeader(ServiceConstants.HttpHeaders.ContentType, "application/http");
                    responseMessage.SetHeader(ServiceConstants.HttpHeaders.ContentTransferEncoding, ServiceConstants.HttpHeaderValues.Binary);
                    using (var messageWriter = this.CreateMessageWriter(responseMessage))
                    {
                        var asyncWriter = messageWriter.CreateODataAsynchronousWriter();
                        var innerResponse = asyncWriter.CreateResponseMessage();
                        asyncTask.Execute(innerResponse);
                    }
                }
            }
        }

        protected override ODataMessageWriterSettings GetWriterSettings()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings
            {
                BaseUri = this.ServiceRootUri
            };

            return settings;
        }
    }
}