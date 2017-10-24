//---------------------------------------------------------------------
// <copyright file="AnnotationRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.OData.Client;
    using Microsoft.OData;

    public class CustomizedHttpWebRequestMessage : HttpWebRequestMessage
    {
        public string Response { get; set; }
        public Dictionary<string, string> CutomizedHeaders { get; set; }

        public CustomizedHttpWebRequestMessage(DataServiceClientRequestMessageArgs args)
            : base(args)
        {
        }

        public CustomizedHttpWebRequestMessage(DataServiceClientRequestMessageArgs args, string response, Dictionary<string, string> headers)
            : base(args)
        {
            this.Response = response;
            this.CutomizedHeaders = headers;
        }

#if (NETCOREAPP1_0 || NETCOREAPP2_0)
        public IODataResponseMessage GetResponse()
#else
        public override IODataResponseMessage GetResponse()
#endif
        {
            return new HttpWebResponseMessage(
                this.CutomizedHeaders,
                200,
                () =>
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(this.Response);
                    return new MemoryStream(byteArray);
                });
        }
    }
}
