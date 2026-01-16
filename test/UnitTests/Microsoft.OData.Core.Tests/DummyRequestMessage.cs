//---------------------------------------------------------------------
// <copyright file="DummyRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData.Tests
{
    public class DummyRequestMessage : IODataRequestMessage
    {
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { throw new NotImplementedException(); }
        }

        public Uri Url
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Method
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string GetHeader(string headerName)
        {
            return null;
        }

        public void SetHeader(string headerName, string headerValue)
        {
        }

        public Stream GetStream()
        {
            return new MemoryStream();
        }

        public Task<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Stream>(new MemoryStream());
        }
    }
}