//---------------------------------------------------------------------
// <copyright file="XHRHttpWebResponse.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      Provides an HTTP-specific implementation of the WebResponse class.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace System.Data.Services.Http
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Provides an HTTP-specific implementation of the WebResponse class.
    /// </summary>
    internal sealed class XHRHttpWebResponse : System.Data.Services.Http.HttpWebResponse
    {
        #region Fields.

        /// <summary>Response headers.</summary>
        private System.Data.Services.Http.XHRWebHeaderCollection headers;
        
        /// <summary>Request that originated this response.</summary>
        private System.Data.Services.Http.XHRHttpWebRequest request;
        
        /// <summary>Status code for the response.</summary>
        private int statusCode;

        #endregion Fields.

        /// <summary>Initializes a new HttpWebResponse instance.</summary>
        /// <param name="request">Request that originated this response.</param>
        /// <param name="statusCode">Status code for the response.</param>
        /// <param name="responseHeaders">Unparsed response headers.</param>
        internal XHRHttpWebResponse(System.Data.Services.Http.XHRHttpWebRequest request, int statusCode, string responseHeaders)
        {
            Debug.Assert(request != null, "request can't be null.");
            this.request = request;
            NormalizeResponseStatus(ref statusCode);
            this.statusCode = statusCode;
            this.headers = new System.Data.Services.Http.XHRWebHeaderCollection();
            
            // Parse response header string, compensating for known browser issues.
            this.ParseHeaders(responseHeaders);
        }

        #region Properties.

        /// <summary>Gets the content length of data being received.</summary>
        public override long ContentLength
        {
            get
            {
                return Convert.ToInt64(this.Headers["Content-Length"], CultureInfo.InvariantCulture);
            }
        }

        /// <summary>Gets the content type of the data being received.</summary>
        public override string ContentType
        {
            get
            {
                return this.Headers["Content-Type"];
            }
        }

        /// <summary>Gets the headers of the data being received.</summary>
        public override System.Data.Services.Http.WebHeaderCollection Headers
        {
            get
            {
                return this.headers;
            }
        }

        /// <summary>Gets the request that originated this response.</summary>
        public override System.Data.Services.Http.HttpWebRequest Request
        {
            get
            {
                return this.request;
            }
        }

        /// <summary>Gets the status code for the data being received.</summary>
        public override System.Net.HttpStatusCode StatusCode
        {
            get
            {
                return (System.Net.HttpStatusCode)this.statusCode;
            }
        }

        /// <summary>Sets the request for this response - internal only as we need the specific type here</summary>
        internal System.Data.Services.Http.XHRHttpWebRequest InternalRequest
        {
            set
            {
                this.request = value;
            }
        }

        #endregion Properties.

        /// <summary>Closes the response stream.</summary>
        public override void Close()
        {
            // Note that the XHRHttpWebRequest.Close() explicitly does not dispose this response because
            // it expects there is no additional work being done here other than closing the request.
            // If additional cleanup needs to happen here for the response, the request should make sure
            // that happens when the request is closed as well.
            this.request.Close();
        }

        /// <summary>Gets a specific header by name.</summary>
        /// <param name="headerName">Name of header.</param>
        /// <returns>The value for the header.</returns>
        public override string GetResponseHeader(string headerName)
        {
            return this.headers[headerName];
        }

        /// <summary>
        /// Gets the underlying <see cref="System.Net.HttpWebResponse"/> if there is one, or null otherwise.
        /// </summary>
        /// <returns>
        /// The underlying response.
        /// </returns>
        public override Net.HttpWebResponse GetUnderlyingHttpResponse()
        {
            return null;
        }

        /// <summary>Gets the stream with the response contents.</summary>
        /// <returns>The stream with the response contents.</returns>
        public override Stream GetResponseStream()
        {
            return this.request.ReadResponse(this);
        }

        /// <summary>Releases resources.</summary>
        /// <param name="disposing">Whether the dispose is being called explicitly rather than by the GC.</param>
        protected override void Dispose(bool disposing)
        {
            this.Close();
        }

        /// <summary>Normalizes the status code based on browser-specific methods.</summary>
        /// <param name="statusCodeParam">Status code to tweak.</param>
        private static void NormalizeResponseStatus(ref int statusCodeParam)
        {
            // the request is needed to workaround XmlHttpRequest issues:
            // we work around all issues where the incorrect status code is oustide the range of standard HTTP response codes and can 
            // be mapped 1-1 to the correct one or an incorrect code is given for the same class (ie. 1xx, 2xx, etc) of response codes.  
            // If the error does not match one of these patterns we cannot reliably compensate.

            // IE workarounds
            string userAgent = System.Windows.Browser.HtmlPage.BrowserInformation.UserAgent;
            bool browserIsIE = userAgent != null && userAgent.ToUpper(CultureInfo.InvariantCulture).Contains("MSIE");
            if (browserIsIE)
            {
                if (statusCodeParam == 1223)
                {
                    // map 1223 to 204 (No Content)
                    statusCodeParam = 204;
                }
                else if (statusCodeParam == 12150)
                {
                    // IE returns this code when it should return 301, 302, 303 or 307.  Since each of these represent a redirect of somesort
                    // we return a 3xx status code with no predefined meaning in RFC 2616 - this allows the user to inspect other elements of the response
                    // to determine what the code should have been.
                    statusCodeParam = 399;
                }
            }

            if (statusCodeParam > (int)HttpStatusCodeRange.MaxValue || statusCodeParam < (int)HttpStatusCodeRange.MinValue)
            {
                // throw new WebException("Invalid response line returned " + statusCode);
                throw WebException.CreateInternal("HttpWebResponse.NormalizeResponseStatus");
            }
        }

        /// <summary>
        /// Checks if the charset parameter is specified in the content type. If yes, then set the parameter value
        /// to UTF-8. If no, add the charset parameter value with UTF-8 value.
        /// </summary>
        /// <param name="contentType">value of the content type header.</param>
        /// <returns>same content type value, except with charset parameter value set to UTF-8.</returns>
        private static string OverrideOrAddCharsetParameter(string contentType)
        {
            // After the parsing of the response headers is done, read the content type and
            // overwrite/add the charset parameter
            string mimeType;
            bool charsetParameterFound = false;
            ContentTypeUtil.MediaParameter[] parameters = ContentTypeUtil.ReadContentType(contentType, out mimeType);
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (String.Equals(parameters[i].Name, XmlConstants.HttpCharsetParameter, StringComparison.OrdinalIgnoreCase))
                    {
                        parameters[i] = new ContentTypeUtil.MediaParameter(parameters[i].Name, XmlConstants.Utf8Encoding, false /*isQuoted*/);
                        charsetParameterFound = true;
                        break;
                    }
                }
            }

            // if the charset parameter was not found, add it to the list of parameters
            if (!charsetParameterFound)
            {
                ContentTypeUtil.MediaParameter[] newParameters;
                int lastIndex;
                if (parameters == null || parameters.Length == 0)
                {
                    newParameters = new ContentTypeUtil.MediaParameter[1];
                    lastIndex = 0;
                }
                else
                {
                    newParameters = new ContentTypeUtil.MediaParameter[parameters.Length + 1];
                    parameters.CopyTo(newParameters, 0);
                    lastIndex = parameters.Length;
                }

                newParameters[lastIndex] = new ContentTypeUtil.MediaParameter(XmlConstants.HttpAcceptCharset, XmlConstants.Utf8Encoding, false /*isQuoted*/);
                parameters = newParameters;
            }

            return ContentTypeUtil.WriteContentType(mimeType, parameters);
        }

        /// <summary>Parses the specified header text.</summary>
        /// <param name="responseHeaders">Headers for the response.</param>
        private void ParseHeaders(string responseHeaders)
        {
            // this can occur when XmlHttpRequest encounters issues, but still puts the request on the wire and gets the response status line
            if (string.IsNullOrEmpty(responseHeaders))
            {
                return;
            }

            // headers are always in ASCII only as per HTTP RFC
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseHeaders);
            WebParseError error = new WebParseError();
            int totalResponseHeadersLength = 0;
            int offset = 0;
            int maxHeaderLength = 64000;  // maximum size of headers is fixed at 64K

            try
            {
                DataParseStatus result = this.headers.ParseHeaders(buffer, buffer.Length, ref offset, ref totalResponseHeadersLength, maxHeaderLength, ref error);

                // we should always have all the response headers by the time we get here
                if (result != DataParseStatus.Done)
                {
                    // throw new WebException("Invalid HTTP response headers were returned");
                    throw WebException.CreateInternal("HttpWebResponse.ParseHeaders");
                }

                string contentType = this.headers[XmlConstants.HttpContentType];
                if (!string.IsNullOrEmpty(contentType))
                {
                    this.headers[XmlConstants.HttpContentType] = XHRHttpWebResponse.OverrideOrAddCharsetParameter(contentType);
                }
            }
            catch (WebException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (!CommonUtil.IsCatchableExceptionType(e))
                {
                    throw;
                }

                // throw new WebException("Invalid HTTP response headers were returned", e);
                string message = System.Data.Services.Client.Strings.HttpWeb_Internal("HttpWebResponse.ParseHeaders.2");
                throw new WebException(message, e);
            }
        }
    }
}
