//---------------------------------------------------------------------
// <copyright file="InMemoryWebRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Web request which stores all of the information about the request and the response in memory
    /// and exposes everything back. Used when a request needs to be passed around without actual underlying real request
    /// </summary>
    public class InMemoryWebRequest : TestWebRequest
    {
        // Note that this class relies on the base TestWebRequest to store most of the data in-memory as it already does.

        /// <summary>The response stream (cached)</summary>
        private MemoryStream responseStream;

        /// <summary>The response status code</summary>
        private int responseStatusCode;

        /// <summary>The response headers</summary>
        private Dictionary<string, string> responseHeaders = new Dictionary<string, string>();

        /// <summary>Custom tag</summary>
        public object Tag { get; set; }

        /// <summary>Creates the in-memory request from a stream.</summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="serviceRoot">The service root uri the request was targeted at (if null the request URI will be left as is).</param>
        /// <returns>The newly created request representation.</returns>
        public static InMemoryWebRequest FromRequest(Stream stream, Uri serviceRoot)
        {
            InMemoryWebRequest request = new InMemoryWebRequest();
            request.ParseRequest(stream, serviceRoot);
            return request;
        }

        /// <summary>Creates the in-memory response from a stream.</summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>The newly created request object with the response properties filled with the data from the stream.</returns>
        public static InMemoryWebRequest FromResponse(Stream stream)
        {
            InMemoryWebRequest response = new InMemoryWebRequest();
            response.ParseResponse(stream);
            return response;
        }

        /// <summary>Creates the in-memory response from another TestWebRequest, copying its reponse.</summary>
        /// <param name="sourceResponse">The request to read the response from.</param>
        /// <returns>The newly create request object with the response proeprties filled with the data from the request response.</returns>
        public static InMemoryWebRequest FromResponse(TestWebRequest sourceResponse)
        {
            InMemoryWebRequest response = new InMemoryWebRequest();
            response.SetResponseStatusCode(sourceResponse.ResponseStatusCode);
            foreach (var header in GetAllResponseHeaders(sourceResponse))
            {
                response.ResponseHeaders[header.Key] = header.Value;
            }
            using (Stream responseStream = sourceResponse.GetResponseStream())
            {
                response.SetResponseStream(responseStream);
            }
            return response;
        }

        /// <summary>Writes the request into a stream (including the verb, headers and everything)</summary>
        /// <param name="stream">The stream to write the request into.</param>
        /// <param name="serviceRoot">The service root URI to target the request at. If null the request uri will be left as is.</param>
        public void WriteRequest(Stream stream, Uri serviceRoot)
        {
            StringBuilder sb = new StringBuilder();
            string requestUri = this.RequestUriString;
            if (serviceRoot != null)
            {
                requestUri = this.RequestUriString;
                if (requestUri.StartsWith("/")) requestUri = requestUri.Substring(1);
                requestUri = new Uri(serviceRoot, requestUri).AbsoluteUri;
            }
            sb.AppendLine(this.HttpMethod + " " + requestUri + " HTTP/1.1");
            InMemoryWebRequest.WriteHeadersToStringBuilder(sb, GetAllRequestHeaders(this));
            sb.AppendLine();

            InMemoryWebRequest.WriteStringToStream(stream, sb.ToString());
            if (this.RequestStream != null)
            {
                this.RequestStream.Seek(0, SeekOrigin.Begin);
                TestUtil.CopyStream(this.RequestStream, stream);
            }
        }

        /// <summary>Writes the request into another request instance (copies the data over).</summary>
        /// <param name="request">The request to write to.</param>
        public void WriteRequest(TestWebRequest request)
        {
            request.HttpMethod = this.HttpMethod;
            request.RequestUriString = this.RequestUriString;
            foreach (var header in GetAllRequestHeaders(this))
            {
                request.RequestHeaders[header.Key] = header.Value;
            }
            ApplyHeadersToProperties(request);

            request.RequestStream = this.RequestStream;
        }

        /// <summary>Writes the response part of the request into a stream (including the status, headers and everything)</summary>
        /// <param name="stream">The stream to write the response into.</param>
        public void WriteResponse(Stream stream)
        {
            System.Net.HttpStatusCode responseStatusCode = (System.Net.HttpStatusCode)this.ResponseStatusCode;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("HTTP/1.1 " + this.ResponseStatusCode.ToString() + " " + responseStatusCode.ToString());
            InMemoryWebRequest.WriteHeadersToStringBuilder(sb, this.ResponseHeaders);
            sb.AppendLine();

            InMemoryWebRequest.WriteStringToStream(stream, sb.ToString());
            if (this.responseStream != null)
            {
                TestUtil.CopyStream(this.GetResponseStream(), stream);
            }
        }

        /// <summary>Returns the request stream.</summary>
        /// <returns>The request stream.</returns>
        public Stream GetRequestStream()
        {
            if (this.RequestStream != null)
            {
                this.RequestStream.Seek(0, SeekOrigin.Begin);
            }
            return this.RequestStream;
        }

        /// <summary>Returns the request stream.</summary>
        /// <returns>The request stream.</returns>
        public string GetRequestStreamAsText()
        {
            string request = null;
            if (this.RequestStream != null)
            {
                this.RequestStream.Seek(0, SeekOrigin.Begin);
                request = new StreamReader(this.RequestStream).ReadToEnd();
                this.RequestStream.Seek(0, SeekOrigin.Begin);
            }

            return request;
        }

        /// <summary>
        /// Returns the server response text after a call to SendRequest.
        /// </summary>
        /// <returns>The server response text after a call to SendRequest.</returns>
        public XDocument GetRequestStreamAsXDocument()
        {
            using (Stream stream = this.GetRequestStream())
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("GetRequestStream() returned null.");
                }

                return XDocument.Load(XmlReader.Create(stream));
            }
        }

        /// <summary>Sets a request stream.</summary>
        /// <param name="requestStream">The request stream to set (copy will be made)</param>
        public void SetRequestStream(Stream requestStream)
        {
            this.RequestStream = new MemoryStream();
            if (requestStream != null)
            {
                TestUtil.CopyStream(requestStream, this.RequestStream);
            }
        }

        /// <summary>Returns the response stream.</summary>
        /// <returns>The stream which contains the response body</returns>
        public override Stream GetResponseStream()
        {
            if (this.responseStream != null)
            {
                this.responseStream.Seek(0, SeekOrigin.Begin);
            }
            return this.responseStream;
        }

        /// <summary>Sets a response stream.</summary>
        /// <param name="responseStream">The response stream to set (copy will be made)</param>
        public void SetResponseStream(Stream responseStream)
        {
            this.responseStream = new MemoryStream();
            if (responseStream != null)
            {
                TestUtil.CopyStream(responseStream, this.responseStream);
            }
        }

        /// <summary>Sets a response stream.</summary>
        /// <param name="responseStreamValue">The response content as text.</param>
        public void SetResponseStreamAsText(string responseStreamValue)
        {
            this.responseStream = new MemoryStream(Encoding.UTF8.GetBytes(responseStreamValue));
        }

        /// <summary>Sends the request. This request doesn't support this method, it will throw.</summary>
        public override void SendRequest()
        {
            throw new NotSupportedException("In-memory request can't be sent, it's just an abstract representation without actual underlying mechanism.");
        }

        /// <summary>Returns response headers.</summary>
        public override Dictionary<string, string> ResponseHeaders
        {
            get { return this.responseHeaders; }
        }

        public override string ResponseCacheControl
        {
            get { return GetResponseHeader("Cache-Control"); }
        }

        public override string ResponseContentType
        {
            get { return GetResponseHeader("Content-Type"); }
        }

        public override string ResponseLocation
        {
            get { return GetResponseHeader("Location"); }
        }

        public override string ResponseETag
        {
            get { return GetResponseHeader("ETag"); }
        }

        public override int ResponseStatusCode
        {
            get { return responseStatusCode; }
        }

        public void SetResponseStatusCode(int responseStatusCode)
        {
            this.responseStatusCode = responseStatusCode;
        }

        public override string ResponseVersion
        {
            get { return GetResponseHeader("OData-Version"); }
        }

        /// <summary>Creates a dictionary of request headers as specified by the request object.</summary>
        /// <returns>The request headers specified by this object.</returns>
        /// <remarks>This collects all the request headers from the various properties the request object has.</remarks>
        public static Dictionary<string, string> GetAllRequestHeaders(TestWebRequest request)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (request.RequestContentLength != -1)
            {
                headers.Add("Content-Length", request.RequestContentLength.ToString());
            }
            if (request.RequestContentType != null)
            {
                headers.Add("Content-Type", request.RequestContentType);
            }
            if (request.Accept != null)
            {
                headers.Add("Accept", request.Accept);
            }
            if (request.AcceptCharset != null)
            {
                headers.Add("Accept-Charset", request.AcceptCharset);
            }
            if (!String.IsNullOrEmpty(request.IfMatch))
            {
                headers.Add("If-Match", request.IfMatch);
            }
            if (!String.IsNullOrEmpty(request.IfNoneMatch))
            {
                headers.Add("If-None-Match", request.IfNoneMatch);
            }
            if (request.RequestMaxVersion != null)
            {
                headers.Add("OData-MaxVersion", request.RequestMaxVersion);
            }
            if (request.RequestVersion != null)
            {
                headers.Add("OData-Version", request.RequestVersion);
            }

            foreach (var h in request.RequestHeaders)
            {
                headers[h.Key] = h.Value;
            }

            return headers;
        }

        /// <summary>Creates a dictionary of response headers as specified by the request object.</summary>
        /// <returns>The response headers specified by this object.</returns>
        /// <remarks>This collects all the response headers from the various properties the request object has.</remarks>
        public static Dictionary<string, string> GetAllResponseHeaders(TestWebRequest request)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (request.ResponseCacheControl != null)
            {
                headers.Add("Cache-Control", request.ResponseCacheControl);
            }
            if (request.ResponseContentType != null)
            {
                headers.Add("Content-Type", request.ResponseContentType);
            }
            if (request.ResponseETag != null)
            {
                headers.Add("ETag", request.ResponseETag);
            }
            if (request.ResponseLocation != null)
            {
                headers.Add("Location", request.ResponseLocation);
            }
            if (request.ResponseVersion != null)
            {
                headers.Add("OData-Version", request.ResponseVersion);
            }

            foreach (var h in request.ResponseHeaders)
            {
                headers[h.Key] = h.Value;
            }

            return headers;
        }

        /// <summary>Applies all the headers in the RequestHeaders collection to the explicit properties.</summary>
        public static void ApplyHeadersToProperties(TestWebRequest request)
        {
            string contentLength = GetAndRemoveRequestHeader(request, "Content-Length");
            if (contentLength != null)
            {
                request.RequestContentLength = Int32.Parse(contentLength);
            }

            request.RequestContentType = GetAndRemoveRequestHeader(request, "Content-Type");
            request.Accept = GetAndRemoveRequestHeader(request, "Accept");
            request.AcceptCharset = GetAndRemoveRequestHeader(request, "Accept-Charset");
            request.IfMatch = GetAndRemoveRequestHeader(request, "If-Match");
            request.IfNoneMatch = GetAndRemoveRequestHeader(request, "If-None-Match");
            request.RequestMaxVersion = GetAndRemoveRequestHeader(request, "OData-MaxVersion");
            request.RequestVersion = GetAndRemoveRequestHeader(request, "OData-Version");
        }

        /// <summary>Returns a single response header value, or null if no such value exists.</summary>
        /// <param name="header">The name of the response header to get</param>
        /// <returns>The value of the header or null if no such header was in the response.</returns>
        private string GetResponseHeader(string header)
        {
            string value;
            if (this.responseHeaders.TryGetValue(header, out value)) return value;
            return null;
        }

        /// <summary>Returns a single request header value, or null if no such value exists. Then it removes that header from the collection.</summary>
        /// <param name="header">The name of the request header to get</param>
        /// <returns>The value of the header or null if no such header was in the request.</returns>
        private static string GetAndRemoveRequestHeader(TestWebRequest request, string header)
        {
            string value;
            if (request.RequestHeaders.TryGetValue(header, out value))
            {
                request.RequestHeaders.Remove(header);
                return value;
            }
            return null;
        }

        internal static void WriteStringToStream(Stream stream, string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        internal static void WriteHeadersToStringBuilder(StringBuilder sb, Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                sb.AppendLine(header.Key + ": " + header.Value);
            }
        }

        internal static void ParseHeaders(TextReader reader, Dictionary<string, string> headers)
        {
            string line;
            while ((line = reader.ReadLine()).Length > 0)
            {
                string headerName = line.Substring(0, line.IndexOf(':'));
                string headerValue = line.Substring(line.IndexOf(':') + 1).Trim();
                headers[headerName] = headerValue;
            }
        }

        internal void ParseRequestVerb(TextReader reader)
        {
            string[] firstLine = reader.ReadLine().Split(' ');
            this.HttpMethod = firstLine[0];
            this.RequestUriString = firstLine[1];
        }

        internal void ParseResponseStatus(TextReader reader)
        {
            string line = reader.ReadLine();
            Assert.IsTrue(line.StartsWith("HTTP/1.1"), "Response is not HTTP/1.1 response.");
            this.SetResponseStatusCode(Int32.Parse((line.Split(' '))[1]));
        }

        private void ParseRequest(Stream stream, Uri serviceRoot)
        {
            MemoryStream memoryStream = new MemoryStream();
            TestUtil.CopyStream(stream, memoryStream);
            memoryStream.Position = 0;
            using (TextReader reader = new StreamReader(memoryStream))
            {
                this.ParseRequestVerb(reader);
                this.RequestHeaders.Clear();
                InMemoryWebRequest.ParseHeaders(reader, this.RequestHeaders);
                ApplyHeadersToProperties(this);
                reader.ReadLine();
                this.SetRequestStreamAsText(reader.ReadToEnd());
            }
        }

        private void ParseResponse(Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            TestUtil.CopyStream(stream, memoryStream);
            memoryStream.Position = 0;
            using (TextReader reader = new StreamReader(memoryStream))
            {
                this.ParseResponseStatus(reader);
                this.ResponseHeaders.Clear();
                InMemoryWebRequest.ParseHeaders(reader, this.ResponseHeaders);
                reader.ReadLine();
                this.SetResponseStream(new MemoryStream(Encoding.UTF8.GetBytes(reader.ReadToEnd())));
            }
        }
    }
}