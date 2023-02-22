//---------------------------------------------------------------------
// <copyright file="MockAsyncOnlyJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
#if NETCOREAPP3_1_OR_GREATER
using System.Text.Json;
#endif

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// A mock implementation of <see cref="IJsonWriterAsync"/> that
    /// does not implement the synchronous interface <see cref="IJsonWriter"/>.
    /// This is used for testing purposes. Most of the methods have not been implemented.
    /// </summary>
    internal class MockAsyncOnlyJsonWriter : IJsonWriterAsync
    {
        public Task EndArrayScopeAsync() => throw new NotImplementedException();

        public Task EndObjectScopeAsync() => throw new NotImplementedException();

        public Task EndPaddingFunctionScopeAsync() => throw new NotImplementedException();

        public Task FlushAsync() => throw new NotImplementedException();

        public Task StartArrayScopeAsync() => throw new NotImplementedException();

        public Task StartObjectScopeAsync() => throw new NotImplementedException();

        public Task StartPaddingFunctionScopeAsync() => throw new NotImplementedException();

        public Task WriteNameAsync(string name) => throw new NotImplementedException();

        public Task WritePaddingFunctionNameAsync(string functionName) => throw new NotImplementedException();

        public Task WriteRawValueAsync(string rawValue) => throw new NotImplementedException();

        public Task WriteValueAsync(bool value) => throw new NotImplementedException();

        public Task WriteValueAsync(int value) => throw new NotImplementedException();

        public Task WriteValueAsync(float value) => throw new NotImplementedException();

        public Task WriteValueAsync(short value) => throw new NotImplementedException();

        public Task WriteValueAsync(long value) => throw new NotImplementedException();

        public Task WriteValueAsync(double value) => throw new NotImplementedException();

        public Task WriteValueAsync(Guid value) => throw new NotImplementedException();

        public Task WriteValueAsync(decimal value) => throw new NotImplementedException();

        public Task WriteValueAsync(DateTimeOffset value) => throw new NotImplementedException();

        public Task WriteValueAsync(TimeSpan value) => throw new NotImplementedException();

        public Task WriteValueAsync(byte value) => throw new NotImplementedException();

        public Task WriteValueAsync(sbyte value) => throw new NotImplementedException();

        public Task WriteValueAsync(string value) => throw new NotImplementedException();

        public Task WriteValueAsync(byte[] value) => throw new NotImplementedException();

        public Task WriteValueAsync(Date value) => throw new NotImplementedException();

        public Task WriteValueAsync(TimeOfDay value) => throw new NotImplementedException();

#if NETCOREAPP3_1_OR_GREATER
        public Task WriteValueAsync(JsonElement value)
        {
            throw new NotImplementedException();
        }
#endif
    }
}
