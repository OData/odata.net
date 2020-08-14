//---------------------------------------------------------------------
// <copyright file="DataServiceContextWithStreamCustomizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.ClientExtensions
{
    using System;
    using System.IO;
    using System.Net;
    using Microsoft.OData.Client;
    using Microsoft.OData;

    public static class ContextUtils
    {
        public static void RegisterStreamCustomizer(this DataServiceContext context, Func<Stream, Stream> requestStreamInterceptor, Func<Stream, Stream> responseStreamInterceptor)
        {
            context.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                return new TestHttpRequestMessage(args, requestStreamInterceptor, responseStreamInterceptor);
            };
        }

        private class TestHttpRequestMessage : HttpClientRequestMessage
        {
            private readonly Func<Stream, Stream> requestStreamInterceptor;
            private readonly Func<Stream, Stream> responseStreamInterceptor;
            internal TestHttpRequestMessage(DataServiceClientRequestMessageArgs args, Func<Stream, Stream> requestStreamInterceptor, Func<Stream, Stream> responseStreamInterceptor) : base(args)
            {
                this.requestStreamInterceptor = requestStreamInterceptor;
                this.responseStreamInterceptor = responseStreamInterceptor;
            }

            public override Stream GetStream()
            {
                if (this.requestStreamInterceptor != null)
                {
                    return this.requestStreamInterceptor(base.GetStream());
                }

                return base.GetStream();
            }

            public override IODataResponseMessage GetResponse()
            {
                var responseMessage = (HttpWebResponseMessage)base.GetResponse();
                return new TestHttpResponseMessage(responseMessage.Response, this.responseStreamInterceptor);
            }

            public override Stream EndGetRequestStream(IAsyncResult asyncResult)
            {
                if (this.requestStreamInterceptor != null)
                {
                    return this.requestStreamInterceptor(base.GetStream());
                }

                return base.GetStream();
            }
        }

        private class TestHttpResponseMessage : HttpWebResponseMessage
        {
            private readonly Func<Stream, Stream> responseStreamInterceptor;
            public TestHttpResponseMessage(HttpWebResponse httpResponse, Func<Stream, Stream> responseStreamInterceptor)
                : base(httpResponse)
            {
                this.responseStreamInterceptor = responseStreamInterceptor;
            }

            public override Stream GetStream()
            {
                if (this.responseStreamInterceptor != null)
                {
                    return this.responseStreamInterceptor(base.GetStream());
                }

                return base.GetStream();
            }
        }
    }
}
