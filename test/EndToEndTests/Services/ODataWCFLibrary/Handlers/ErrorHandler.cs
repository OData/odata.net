//---------------------------------------------------------------------
// <copyright file="ErrorHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Net;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;

    public class ErrorHandler : RequestHandler
    {
        public ErrorHandler(RequestHandler other, Exception exception) :
            base(other, HttpMethod.GET, other.ServiceRootUri, null)
        {
            this.HandledException = exception;
        }

        public Exception HandledException
        {
            get;
            private set;
        }

        protected override ODataMessageWriterSettings GetWriterSettings()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings
            {
                BaseUri = this.ServiceRootUri
            };
            settings.SetContentType(ODataFormat.Json);
            return settings;
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            using (var messageWriter = this.CreateMessageWriter(responseMessage))
            {
                ODataError error;
                HttpStatusCode statusCode;

                this.BuildODataError(out error, out statusCode);
                responseMessage.SetStatusCode(statusCode);
                messageWriter.WriteError(error, true);
            }
        }

        private void BuildODataError(out ODataError error, out HttpStatusCode statusCode)
        {
            // Default to 500 (InternalServerError), it will be overried below if necessary.
            statusCode = HttpStatusCode.InternalServerError;

            ODataServiceException ose = this.HandledException as ODataServiceException;

            if (ose != null)
            {
                statusCode = ose.StatusCode;
            }

            ODataContentTypeException octe = this.HandledException as ODataContentTypeException;

            if (octe != null)
            {
                statusCode = HttpStatusCode.UnsupportedMediaType;
            }


            ODataUnrecognizedPathException oupe = this.HandledException as ODataUnrecognizedPathException;

            if (oupe != null)
            {
                statusCode = HttpStatusCode.NotFound;
            }

            ODataErrorException oee = this.HandledException as ODataErrorException;

            if (oee != null)
            {
                error = this.BuildODataError(statusCode, this.HandledException);

                if (!string.IsNullOrEmpty(oee.Error.ErrorCode))
                {
                    error.ErrorCode = oee.Error.ErrorCode;
                }

                if (!string.IsNullOrEmpty(oee.Error.Message))
                {
                    error.Message = oee.Error.Message;
                }

                if (oee.Error.InnerError != null)
                {
                    error.InnerError = oee.Error.InnerError;
                }
            }
            else
            {
                // All the other exception will go here.
                error = this.BuildODataError(statusCode, this.HandledException);
            }
        }

        private ODataError BuildODataError(HttpStatusCode statusCode, Exception exception)
        {
            return new ODataError()
            {
                ErrorCode = statusCode.ToString(),
                Message = exception.Message,
                InnerError = this.BuildODataInnerError(exception),
            };
        }

        private ODataInnerError BuildODataInnerError(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            return new ODataInnerError()
            {
                Message = exception.Message,
                TypeName = exception.GetType().FullName,
                StackTrace = exception.StackTrace,
                InnerError = BuildODataInnerError(exception.InnerException),
            };
        }
    }
}