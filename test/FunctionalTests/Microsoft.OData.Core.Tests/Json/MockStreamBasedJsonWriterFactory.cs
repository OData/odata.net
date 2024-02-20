﻿//---------------------------------------------------------------------
// <copyright file="MockStreamBasedJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

# if NETCOREAPP
using Microsoft.OData.Json;
using System.IO;
using System.Text;

namespace Microsoft.OData.Tests.Json
{
    internal sealed class MockStreamBasedJsonWriterFactory : IStreamBasedJsonWriterFactory
    {
        private readonly IJsonWriter jsonWriter;

        public MockStreamBasedJsonWriterFactory(IJsonWriter jsonWriter)
        {
            this.jsonWriter = jsonWriter;
        }

        public IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            return this.jsonWriter;
        }
    }
}
#endif
