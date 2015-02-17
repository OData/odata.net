//---------------------------------------------------------------------
// <copyright file="Response.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Core;

    /// <summary>
    /// The Response class
    /// </summary>
    public class Response : IODataResponseMessage
    {
        /// <summary>
        /// If the response is read
        /// </summary>
        private bool readDone;

        /// <summary>
        /// The response str
        /// </summary>
        private string responseStr;

        /// <summary>
        /// The response byte array.
        /// </summary>
        private byte[] responseBytes;

        /// <summary>
        /// http response
        /// </summary>
        private HttpWebResponse webResponse;

        /// <summary>
        /// Initializes a new instance of the Response class
        /// </summary>
        /// <param name="response">The http response</param>
        public Response(HttpWebResponse response)
        {
            this.Parse(response);
        }

        /// <summary>
        /// Initializes a new instance of the Response class
        /// </summary>
        /// <param name="ex">The web exception</param>
        public Response(WebException ex)
        {
            this.Parse(ex.Response);
            this.Exception = ex;
        }

        /// <summary>
        /// Gets a value indicating whether the webresponse is null
        /// </summary>
        public bool IsNull { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the response is an WebException
        /// </summary>
        public bool IsException
        {
            get { return this.Exception != null; }
        }

        /// <summary>
        /// Gets the response string for text-based responses.
        /// </summary>
        public string ResponseStr
        {
            get
            {
                if (!this.readDone)
                {
                    this.Read();
                }

                return this.responseStr;
            }
        }

        /// <summary>
        /// Gets the response byte array for binary type responses.
        /// </summary>
        public byte[] ResponseByteArray
        {
            get
            {
                if (!this.readDone)
                {
                    this.Read();
                }

                return this.responseBytes;
            }
        }

        /// <summary>
        /// Gets content type
        /// </summary>
        public HttpContentType ContentType { get; private set; }

        /// <summary>
        /// Gets the content encoding
        /// </summary>
        public string ContentEncoding { get; private set; }

        /// <summary>
        /// Gets the headers
        /// </summary>
        IEnumerable<KeyValuePair<string, string>> IODataResponseMessage.Headers
        {
            get
            {
                foreach (string headerName in this.Headers)
                {
                    yield return new KeyValuePair<string, string>(headerName, this.Headers[headerName]);
                }
            }
        }

        /// <summary>
        /// Gets or sets the status code
        /// </summary>
        int IODataResponseMessage.StatusCode
        {
            get { return (int)this.StatusCode.Value; }
            set { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// Gets http status code
        /// </summary>
        public HttpStatusCode? StatusCode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the response is valid http response
        /// </summary>
        public bool IsValid
        {
            get { return this.StatusCode.HasValue && (int)this.StatusCode.Value >= 200 && (int)this.StatusCode.Value < 300; }
        }

        /// <summary>
        /// Gets response uri
        /// </summary>
        public Uri ResponseUri { get; private set; }

        /// <summary>
        /// Gets the http web response
        /// </summary>
        public HttpWebResponse HttpWebResponse
        {
            get { return this.webResponse; }
        }

        /// <summary>
        /// Gets WebException 
        /// </summary>
        public WebException Exception { get; private set; }

        /// <summary>
        /// Gets Headers returned
        /// </summary>
        public WebHeaderCollection Headers { get; private set; }

        /// <summary>
        /// Gets the cookies
        /// </summary>
        public CookieCollection Cookies { get; private set; }

        /// <summary>
        /// Get the header value
        /// </summary>
        /// <param name="headerName">header name</param>
        /// <returns>header value</returns>
        public string GetHeader(string headerName)
        {
            return this.Headers[headerName];
        }

        /// <summary>
        /// Set the header value
        /// </summary>
        /// <param name="headerName">header name</param>
        /// <param name="headerValue">header value</param>
        public void SetHeader(string headerName, string headerValue)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Get stream
        /// </summary>
        /// <returns>Returns stream</returns>
        public Stream GetStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(this.ResponseStr));
        }

        /// <summary>
        /// Gets response object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <returns>The response object</returns>
        public T GetResponseObject<T>()
        {
            if (!string.IsNullOrEmpty(this.ResponseStr))
            {
                using (var xr = new XmlTextReader(new StringReader(this.ResponseStr)))
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    var responseObject = (T)serializer.ReadObject(xr);
                    return responseObject;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Read the response
        /// </summary>
        public void Read()
        {
            if (this.webResponse != null)
            {
                using (this.webResponse)
                {
                    try
                    {
                        using (Stream htmlStream = this.webResponse.GetResponseStream())
                        {
                            bool isBinaryContent = !string.IsNullOrEmpty(this.webResponse.ContentType) &&
                                                   this.webResponse.ContentType.StartsWith("image");

                            if (isBinaryContent)
                            {
                                this.responseBytes = new byte[this.webResponse.ContentLength];
                                int offset = 0;
                                long remaining = this.webResponse.ContentLength;
                                while (remaining > 0)
                                {
                                    int read = htmlStream.Read(this.responseBytes, offset, (int)remaining);
                                    if (read <= 0)
                                    {
                                        break;
                                    }

                                    remaining -= read;
                                    offset += read;
                                }

                                this.readDone = true;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(this.ContentEncoding) ||
                                    this.ContentEncoding.IndexOf("gzip", StringComparison.CurrentCultureIgnoreCase) != -1)
                                {
                                    using (var responseStream = new GZipStream(htmlStream, CompressionMode.Decompress))
                                    {
                                        using (var sr = new StreamReader(responseStream))
                                        {
                                            this.responseStr = sr.ReadToEnd();
                                        }
                                    }
                                }
                                else
                                {
                                    using (var sr = new StreamReader(htmlStream))
                                    {
                                        this.responseStr = sr.ReadToEnd();
                                    }
                                }

                                this.readDone = true;

                                this.Cookies = new CookieCollection();

                                string cookies = this.webResponse.Headers[HttpResponseHeader.SetCookie];
                                if (!string.IsNullOrWhiteSpace(cookies))
                                {
                                    // remove the , in order to workaround .net bugs
                                    cookies = cookies.Replace(", ", " ");
                                    foreach (
                                        var setcookie in
                                            cookies.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        string tmp = setcookie;
                                        int index;
                                        if ((index = tmp.IndexOf(';')) != -1)
                                        {
                                            tmp = tmp.Substring(0, index);
                                        }

                                        string[] tokens = tmp.Split(
                                            new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                                        if (tokens.Length == 2)
                                        {
                                            var cookie = new Cookie(tokens[0], tokens[1]);
                                            this.Cookies.Add(cookie);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        this.webResponse.Close();
                    }
                }

                this.webResponse = null;
            }
        }

        private void Parse(WebResponse response)
        {
            if (response != null)
            {
                this.webResponse = (HttpWebResponse)response;

                // Copy headers
                if (this.webResponse.Headers != null)
                {
                    this.Headers = new WebHeaderCollection { this.webResponse.Headers };
                }

                // Copy other fields
                this.StatusCode = this.webResponse.StatusCode;
                this.ContentType = HttpContentTypeExtension.Parse(this.webResponse.ContentType);
                this.ContentEncoding = this.webResponse.ContentEncoding;
                this.ResponseUri = this.webResponse.ResponseUri;
                this.IsNull = false;
            }
            else
            {
                this.IsNull = true;
            }
        }
    }
}
