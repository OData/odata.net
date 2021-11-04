//---------------------------------------------------------------------
// <copyright file="ClientExceptionUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Client
{
    using System;
    using System.Xml.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Framework.Common;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Static methods for dealing with client side exceptions.
    /// </summary>
    public static class ClientExceptionUtil
    {
        /// <summary>
        /// Extracts the server error message from a client side query exception.
        /// </summary>
        /// <param name="exception">The DataService Query exception to extract error message from.</param>
        /// <returns>The server error message.</returns>
        public static string ExtractServerErrorMessage(DataServiceQueryException exception)
        {
            string contentType = exception.Response.Headers[HttpHeaders.ContentType]?.Replace(" ", string.Empty);
            var innerException = exception.InnerException as DataServiceClientException;
            ExceptionUtilities.Assert(innerException != null, "No inner exception on query exception");

            return GetErrorMessage(innerException, contentType);
        }

        /// <summary>
        /// Extracts the server error message from a client side query exception.
        /// </summary>
        /// <param name="response">The QueryOperationResponse to extract error message from.</param>
        /// <returns>The server error message.</returns>
        public static string ExtractServerErrorMessage(QueryOperationResponse response)
        {
            string contentType = response.Headers[HttpHeaders.ContentType];

            var innerException = response.Error as DataServiceClientException;
            ExceptionUtilities.Assert(innerException != null, "No inner exception on query exception");

            return GetErrorMessage(innerException, contentType);
        }

        private static string GetErrorMessage(DataServiceClientException exception, string contentType)
        {
            string errorMessage = string.Empty;
            if (contentType.StartsWith(MimeTypes.ApplicationAtomXml, StringComparison.OrdinalIgnoreCase) || contentType.StartsWith(MimeTypes.ApplicationXml, StringComparison.OrdinalIgnoreCase))
            {
                var xelement = XElement.Parse(exception.Message);
                var innerError = xelement.Element(xelement.Name.Namespace.GetName("innererror"));
                if (innerError != null)
                {
                    errorMessage = innerError.Element(innerError.Name.Namespace.GetName("message")).Value;
                }
                else
                {
                    errorMessage = xelement.Element(xelement.Name.Namespace.GetName("message")).Value;
                }
            }
            else if (contentType.StartsWith(MimeTypes.ApplicationJsonLight, StringComparison.OrdinalIgnoreCase))
            {
                JObject jsonObject = JObject.Parse(exception.Message);
                var innerError = jsonObject["error"]["innererror"];
                if (innerError != null)
                {
                    errorMessage = ((JValue)innerError["message"]).Value as string;
                }
                else
                {
                    errorMessage = ((JValue)jsonObject["error"]["message"]).Value as string;
                }
            }

            ExceptionUtilities.Assert(!string.IsNullOrEmpty(errorMessage), "Unsupported content type value: " + contentType);

            return errorMessage;
        }        
    }
}
