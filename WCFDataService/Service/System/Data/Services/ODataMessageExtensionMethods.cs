//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services
{
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Data.OData;

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
