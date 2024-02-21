//---------------------------------------------------------------------
// <copyright file="TestHookConsumer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion

    public class HttpTestHookConsumer
    {
        private DataServiceClientRequestMessage testMessage;
        List<WrappingStream> requestWrappingStreams = new List<WrappingStream>();
        List<WrappingStream> responseWrappingStreams = new List<WrappingStream>();
        List<Dictionary<string, string>> requestHeaders = new List<Dictionary<string, string>>();
        List<Dictionary<string, string>> responseHeaders = new List<Dictionary<string, string>>();

        public HttpTestHookConsumer(DataServiceContext context, bool silverlight)
        {
            context.Configurations.RequestPipeline.OnMessageCreating = this.OnHttpClientMessageCreating;
        }
        public List<Dictionary<string, string>> RequestHeaders
        {
            get
            {
                return this.requestHeaders;
            }
        }

        public List<WrappingStream> RequestWrappingStreams
        {
            get
            {
                return requestWrappingStreams;
            }
        }

        public List<Dictionary<string, string>> ResponseHeaders
        {
            get
            {
                return responseHeaders;
            }
        }

        public List<WrappingStream> ResponseWrappingStreams
        {
            get
            {
                return responseWrappingStreams;
            }
        }

        public Action<object> CustomSendRequestAction
        {
            get;
            set;
        }

        public Action<object> CustomSendResponseAction
        {
            get;
            set;
        }

        public Stream GetRequestWrappingStream(Stream sourceStream)
        {
            Assert.IsNotNull(sourceStream, "getRequestWrappingStream test hook was called with null request stream");
            WrappingStream wrappedStream = new WrappingStream(sourceStream);
            requestWrappingStreams.Add(wrappedStream);
            return wrappedStream;
        }

        public Stream GetResponseWrappingStream(Stream sourceStream)
        {
            Assert.IsNotNull(sourceStream, "getResponseWrappingStream test hook was called with null response stream");
            WrappingStream wrappedStream = new WrappingStream(sourceStream);
            responseWrappingStreams.Add(wrappedStream);
            return wrappedStream;
        }

        private void SendRequest(DataServiceClientRequestMessage requestMessage)
        {
            Assert.IsNotNull(requestMessage, "sendRequest test hook was called with null request message");

            HttpClientRequestMessage httpClientRequestMessage = requestMessage as HttpClientRequestMessage;
            Dictionary<string, string> headers = WrapHttpRequestHeaders(httpClientRequestMessage.HttpRequestMessage.Headers);
            headers.Add("__Uri", httpClientRequestMessage.Url.AbsoluteUri);
            headers.Add("__HttpVerb", httpClientRequestMessage.HttpRequestMessage.Method.ToString());
            requestHeaders.Add(headers);

            requestMessage.SetHeader("__Uri", httpClientRequestMessage.Url.AbsoluteUri);
            requestMessage.SetHeader("__HttpVerb", httpClientRequestMessage.Method);

            if (null != this.CustomSendRequestAction)
            {
                this.CustomSendRequestAction(httpClientRequestMessage.HttpRequestMessage);
            }
        }

        public void SendResponse(HttpWebResponseMessage responseMessage)
        {
            Assert.IsNotNull(responseMessage, "sendResponse test hook was called with null response message");
            Dictionary<string, string> headers = WrapHttpHeaders(responseMessage.Headers);
            headers.Add("__HttpStatusCode", responseMessage.StatusCode.ToString());
            responseHeaders.Add(headers);

            if (null != this.CustomSendResponseAction)
            {
                this.CustomSendResponseAction(responseMessage);
            }
        }

        private Dictionary<string, string> WrapHttpHeaders(WebHeaderCollection headerCollection)
        {
            var headers = new Dictionary<string, string>();
            foreach (string name in headerCollection.AllKeys)
            {
                headers.Add(name, headerCollection[name]);
            }

            return headers;
        }

        private Dictionary<string, string> WrapHttpHeaders(IEnumerable<KeyValuePair<string, string>> headerCollection)
        {
            var headers = new Dictionary<string, string>();
            foreach (var header in headerCollection)
            {
                headers.Add(header.Key, header.Value);
            }

            return headers;
        }

        private Dictionary<string, string> WrapHttpRequestHeaders(HttpRequestHeaders headerCollection)
        {
            var headers = new Dictionary<string, string>();
            foreach (var name in headerCollection)
            {
                string headerName = name.Key;
                string headerValue = name.Value.ToString();
                headers.Add(headerName, headerValue);
            }

            return headers;
        }

        private DataServiceClientRequestMessage OnHttpClientMessageCreating(DataServiceClientRequestMessageArgs args)
        {
            this.testMessage = new TestHttpClientRequestMessage(args, this.SendRequest, this.SendResponse, this.GetRequestWrappingStream, this.GetResponseWrappingStream);
            return this.testMessage;
        }
    }

    // Test Class for HttpClientRequestMessage
    public class TestHttpClientRequestMessage : HttpClientRequestMessage
    {
        private bool requestHeadersAdded;

        public TestHttpClientRequestMessage(
            DataServiceClientRequestMessageArgs args,
            Action<DataServiceClientRequestMessage> sendRequest,
            Action<HttpWebResponseMessage> sendResponse,
            Func<Stream, Stream> wrapRequestStream,
            Func<Stream, Stream> wrapResponseStream) : base(args)
        {
            this.SendRequest = sendRequest;
            this.SendResponse = sendResponse;
            this.WrapRequestStream = wrapRequestStream;
            this.WrapResponseStream = wrapResponseStream;
        }

        private Action<DataServiceClientRequestMessage> SendRequest { get; set; }
        private Action<HttpWebResponseMessage> SendResponse { get; set; }
        private Func<Stream, Stream> WrapRequestStream { get; set; }
        private Func<Stream, Stream> WrapResponseStream { get; set; }

        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            this.SendRequest(this);
            this.requestHeadersAdded = true;
            return base.BeginGetRequestStream(callback, state);
        }

        public override Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            var requestStream = base.EndGetRequestStream(asyncResult);
            return this.WrapRequestStream(requestStream);
        }

        public override Stream GetStream()
        {
            this.SendRequest(this);
            this.requestHeadersAdded = true;

            var requestStream = base.GetStream();
            return this.WrapRequestStream(requestStream);
        }

        public override IODataResponseMessage GetResponse()
        {
            if (!this.requestHeadersAdded)
            {
                this.SendRequest(this);
            }

            TestHttpWebResponseMessage responseMessage;
            try
            {
                var httpResponse = (HttpWebResponseMessage)this.GetResponse();
                responseMessage = new TestHttpWebResponseMessage(httpResponse, this.WrapResponseStream);
                this.SendResponse(responseMessage);
                return responseMessage;
            }
            catch (WebException webException)
            {
                throw new DataServiceTransportException(response: null, webException);
            }
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            if (!this.requestHeadersAdded)
            {
                this.SendRequest(this);
            }

            return base.BeginGetResponse(callback, state);
        }

        public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
        {
            TestHttpWebResponseMessage responseMessage;
            try
            {
                var httpResponse = (HttpWebResponseMessage)this.EndGetResponse(asyncResult);
                responseMessage = new TestHttpWebResponseMessage(httpResponse, this.WrapResponseStream);
                this.SendResponse(responseMessage);
                return responseMessage;
            }
            catch (WebException webException)
            {
                throw new DataServiceTransportException(response: null, webException);
            }
            
        }
    }

    public class TestHttpWebResponseMessage : HttpWebResponseMessage
    {
        public TestHttpWebResponseMessage(HttpWebResponseMessage response, Func<Stream, Stream> addResponseStream)
            : base(response.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), response.StatusCode, () => response.GetStream())
        {
            this.AddResponseStream = addResponseStream;
        }

        private Func<Stream, Stream> AddResponseStream { get; set; }

        public override Stream GetStream()
        {
            var responseStream = base.GetStream();
            return this.AddResponseStream(responseStream);
        }
    }

    public class WrappingStream : Stream
    {
        private Stream underlyingStream;
        private MemoryStream loggingStream;
        private byte[] asyncReadBuffer;
        private int asyncReadBufferOffset;

        public WrappingStream(Stream underlyingStream)
        {
            this.underlyingStream = underlyingStream;
            this.loggingStream = new MemoryStream();
        }

        public string GetLoggingStreamAsString()
        {
            this.loggingStream.Position = 0;
            return new StreamReader(this.loggingStream).ReadToEnd();
        }

        public XDocument GetLoggingStreamAsXDocument()
        {
            this.loggingStream.Position = 0;
            return XDocument.Load(this.loggingStream);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            Debug.Assert(asyncReadBuffer == null, "Async read buffer still has data in it from a previous read");
            // Save the buffer and offset so we can use them in EndRead
            asyncReadBuffer = buffer;
            asyncReadBufferOffset = offset;
            return underlyingStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            // Buffer is already available, so just go ahead and write it to our logging stream synchronously.
            // Write to the logging stream first because the underlying stream will be an HTTP request stream
            // and the data will start being sent to the service right after it is written to the stream. In order
            // to guarantee that the the logging stream will be populated already when we need to verify it, we
            // need to write to it first.
            this.loggingStream.Write(buffer, offset, count);
            IAsyncResult asyncResult = underlyingStream.BeginWrite(buffer, offset, count, callback, state);
            return asyncResult;
        }

        public override bool CanRead
        {
            get
            {
                return underlyingStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return underlyingStream.CanSeek;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return underlyingStream.CanTimeout;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return underlyingStream.CanWrite;
            }
        }

        public override void Close()
        {
            underlyingStream.Close();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            int readBytes = underlyingStream.EndRead(asyncResult);
            
            // Data is now available in the read buffer, so just read it synchronously
            loggingStream.Write(asyncReadBuffer, asyncReadBufferOffset, readBytes);

            // Reset the async state for the next call
            asyncReadBuffer = null;
            asyncReadBufferOffset = 0;
            return readBytes;
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            // Nothing to do here for the logging stream because we already saved the data in BeginWrite
            underlyingStream.EndWrite(asyncResult);
        }

        protected override void Dispose(bool disposing)
        {
            underlyingStream.Dispose();
        }

        public override void Flush()
        {
            underlyingStream.Flush();
        }

        public override long Length
        {
            get
            {
                return underlyingStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return underlyingStream.Position;
            }
            set
            {
                underlyingStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // Read from the underlying stream into the specified buffer, then write from that same buffer into our logging stream
            int read = underlyingStream.Read(buffer, offset, count);
            loggingStream.Write(buffer, offset, read);
            return read;
        }

        public override int ReadByte()
        {
            // Read the byte from the underlying stream, then write it into our logging stream
            int readByte = underlyingStream.ReadByte();
            loggingStream.WriteByte((byte)readByte);
            return readByte;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return underlyingStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            underlyingStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // Write to the logging stream first because the underlying stream will be an HTTP request stream
            // and the data will start being sent to the service right after it is written to the stream. In order
            // to guarantee that the the logging stream will be populated already when we need to verify it, we
            // need to write to it first.
            loggingStream.Write(buffer, offset, count);
            underlyingStream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            // Write the data to both streams.
            // Logging stream needs to be written to first for the reason described above in the Write method.
            loggingStream.WriteByte(value);
            underlyingStream.WriteByte(value);
        }
    }
}
