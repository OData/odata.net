//---------------------------------------------------------------------
// <copyright file="DummyResponseMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.OData.Core;

    public class DummyResponseMessage : IODataResponseMessageAsync
    {
        public Task<Stream> GetStreamAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { throw new System.NotImplementedException(); }
        }

        public int StatusCode
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public string GetHeader(string headerName)
        {
            throw new System.NotImplementedException();
        }

        public void SetHeader(string headerName, string headerValue)
        {
            throw new System.NotImplementedException();
        }

        public Stream GetStream()
        {
            throw new System.NotImplementedException();
        }
    }
}