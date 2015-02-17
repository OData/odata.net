//---------------------------------------------------------------------
// <copyright file="AnnotationRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests.Annotation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.OData.Client;
    using Microsoft.OData.Core;

    public class AnnotationRequestMessage : HttpWebRequestMessage
    {
        public string Response { get; set; }
        public Dictionary<string, string> CutomizedHeaders { get; set; }

        public AnnotationRequestMessage(DataServiceClientRequestMessageArgs args)
            : base(args)
        {
        }

        public AnnotationRequestMessage(DataServiceClientRequestMessageArgs args, string response, Dictionary<string, string> headers)
            : base(args)
        {
            this.Response = response;
            this.CutomizedHeaders = headers;
        }

        public override IODataResponseMessage GetResponse()
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
