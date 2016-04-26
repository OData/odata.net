//---------------------------------------------------------------------
// <copyright file="ODataMessageExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System.Diagnostics;
    using System.IO;
    using Microsoft.OData;

    /// <summary>
    /// Extension methods classes in WCF Data Services Server.
    /// </summary>
    internal static class ODataMessageExtensionMethods
    {
        /// <summary>
        /// Set the response stream. 
        /// </summary>
        /// <param name ="message">The message that we are setting the stream for.</param>
        /// <param name="stream">Stream to which the response needs to be written.</param>
        internal static void SetStream(this IODataResponseMessage message, Stream stream)
        {
            AstoriaResponseMessage astoriaResponseMessage = message as AstoriaResponseMessage;
            ODataBatchOperationResponseMessage batchResponseMessage = message as ODataBatchOperationResponseMessage;

            if (astoriaResponseMessage != null)
            {
                Debug.Assert(stream != null, "When we call SetStream on a non-batch response message, the stream shouldn't be null.");
                astoriaResponseMessage.SetStream(stream);
            }
            else if (batchResponseMessage != null)
            {
                Debug.Assert(stream == null, "When we call SetStream, if we are in a batch operation, then the stream should be null.");
            }
            else
            {
                Debug.Fail("SetStream called on an unknown message type.");
            }
        }

        /// <summary>
        /// Gets the Request-If-Match header from the request.
        /// </summary>
        /// <param name="message">Message to get header from.</param>
        /// <returns>Value of the request if match header.</returns>
        internal static string GetRequestIfMatchHeader(this IODataRequestMessage message)
        {
            return message.GetHeader(XmlConstants.HttpRequestIfMatch);
        }
        
        /// <summary>
        /// Gets the Request-If-None-Match header from the request.
        /// </summary>
        /// <param name="message">Message to get header from.</param>
        /// <returns>Value of the request if none match header.</returns>
        internal static string GetRequestIfNoneMatchHeader(this IODataRequestMessage message)
        {
            return message.GetHeader(XmlConstants.HttpRequestIfNoneMatch);
        }

        /// <summary>
        /// Gets the Request Accept Charset header from the request.
        /// </summary>
        /// <param name="message">Message to get header from.</param>
        /// <returns>Value of the Request Accept Charset header.</returns>
        internal static string GetRequestAcceptCharsetHeader(this IODataRequestMessage message)
        {
            return message.GetHeader(XmlConstants.HttpRequestAcceptCharset);
        }
    }
}