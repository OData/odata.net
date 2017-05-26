//---------------------------------------------------------------------
// <copyright file="MokeJsonWriterFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.CompilerServices;
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
        public IJsonWriter CreateJsonWriter(TextWriter textWriter, bool isIeee754Compatible)
        {
            return jsonWriter;
        }
    }
}
