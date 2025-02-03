//---------------------------------------------------------------------
// <copyright file="MokeJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Text;
using Microsoft.OData.Json;

namespace Microsoft.OData.Tests.Json
{
    internal sealed class MockJsonWriterFactory : IJsonWriterFactory
    {
        private IJsonWriter jsonWriter;

        public MockJsonWriterFactory(IJsonWriter jsonWriter)
        {
            this.jsonWriter = jsonWriter;
        }

        public IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            return jsonWriter;
        }
    }
}
