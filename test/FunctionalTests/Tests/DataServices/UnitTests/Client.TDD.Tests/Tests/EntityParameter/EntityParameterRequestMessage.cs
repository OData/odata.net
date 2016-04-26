//---------------------------------------------------------------------
// <copyright file="EntityParameterRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests.EntityParameter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.OData;

    public class EntityParameterRequestMessage : HttpWebRequestMessage
    {
        private readonly Stream stream;
        private readonly HeaderCollection headers;

        public EntityParameterRequestMessage(DataServiceClientRequestMessageArgs args, byte[] buffer)
            : base(args)
        {
            this.stream = new MemoryStream(buffer);
            this.headers = new HeaderCollection();
        }

        public override IODataResponseMessage GetResponse()
        {
            // Do NOT actually send the request and return a fake response.
            return new HttpWebResponseMessage(this.headers, 200, () => new MemoryStream());
        }

        public override Stream GetStream()
        {
            return this.stream;
        }
    }
}
