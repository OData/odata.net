//---------------------------------------------------------------------
// <copyright file="TestHttpWebRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Client.Tests
{
    public class TestHttpWebRequestMessage : HttpWebRequestMessage
    {
        private readonly Func<Stream> getResponseStream;
        private readonly int statusCode;
        private readonly IDictionary<string, string> headers;

        public TestHttpWebRequestMessage(DataServiceClientRequestMessageArgs args)
            : this(args, new Dictionary<string, string>(), 200, () => new MemoryStream(Encoding.UTF8.GetBytes("")))
        {
        }

        public TestHttpWebRequestMessage(DataServiceClientRequestMessageArgs args, IDictionary<string, string> headers, Func<Stream> getResponseStream)
            : this(args, headers, 200, getResponseStream)
        {
        }

        public TestHttpWebRequestMessage(DataServiceClientRequestMessageArgs args, IDictionary<string, string> headers, int statusCode, Func<Stream> getResponseStream)
            : base(args)
        {
            this.headers = headers;
            this.statusCode = statusCode;
            this.getResponseStream = getResponseStream;
        }

        public override IODataResponseMessage GetResponse()
        {
            return new HttpWebResponseMessage(this.headers, this.statusCode, this.getResponseStream);
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            // APM is deprecated in .NET Core and Task.CompletedTask is not available in NET45
            callback.Invoke(Task.FromResult(0));
            return Task.FromResult(0);
        }

        public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
        {
            return GetResponse();
        }
    }
}
