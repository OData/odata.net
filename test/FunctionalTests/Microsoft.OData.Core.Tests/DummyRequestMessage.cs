//---------------------------------------------------------------------
// <copyright file="DummyRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.OData.Core.Tests
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
    }
}