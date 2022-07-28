//---------------------------------------------------------------------
// <copyright file="MokeJsonWriterFactoryAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using Microsoft.OData.Json;

namespace Microsoft.OData.Tests.Json
{
    internal sealed class MockJsonWriterFactoryAsync : IJsonWriterFactoryAsync
    {
        private IJsonWriterAsync jsonWriter;

        public MockJsonWriterFactoryAsync(IJsonWriterAsync jsonWriter)
        {
            this.jsonWriter = jsonWriter;
        }

        public IJsonWriterAsync CreateAsynchronousJsonWriter(TextWriter textWriter, bool isIeee754Compatible)
        {
            return jsonWriter;
        }
    }
}
