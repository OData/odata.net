//---------------------------------------------------------------------
// <copyright file="MockStreamBasedJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

# if NETCOREAPP3_1_OR_GREATER
using Microsoft.OData.Json;
using System.IO;
using System.Text;

namespace Microsoft.OData.Tests.Json
{
    internal sealed class MockStreamBasedJsonWriterFactory : IStreamBasedJsonWriterFactory
    {
        private readonly IJsonWriter jsonWriter;
        private readonly IJsonWriterAsync asyncJsonWriter;

        public MockStreamBasedJsonWriterFactory(IJsonWriter jsonWriter, IJsonStreamWriterAsync asyncJsonWriter)
        {
            this.jsonWriter = jsonWriter;
            this.asyncJsonWriter = asyncJsonWriter;
        }

        public IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            return this.jsonWriter;
        }

        public IJsonWriterAsync CreateAsynchronousJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            return this.asyncJsonWriter;
        }
    }
}
#endif
