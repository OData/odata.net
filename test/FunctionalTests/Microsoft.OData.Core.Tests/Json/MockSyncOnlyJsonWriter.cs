//---------------------------------------------------------------------
// <copyright file="MockSyncOnlyJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// A mock implementation of <see cref="IJsonWriter"/>
    /// that does not implement the async interface <see cref="IJsonWriterAsync"/>
    /// use for testing purposes.
    /// 
    /// Note: most methods are not actually implemented.
    /// </summary>
    internal class MockSyncOnlyJsonWriter : IJsonWriter
    {
        public void EndArrayScope() => throw new NotImplementedException();

        public void EndObjectScope() => throw new NotImplementedException();

        public void EndPaddingFunctionScope() => throw new NotImplementedException();

        public void Flush() => throw new NotImplementedException();

        public void StartArrayScope() => throw new NotImplementedException();

        public void StartObjectScope() => throw new NotImplementedException();

        public void StartPaddingFunctionScope() => throw new NotImplementedException();

        public void WriteName(string name) => throw new NotImplementedException();

        public void WritePaddingFunctionName(string functionName) => throw new NotImplementedException();

        public void WriteRawValue(string rawValue) => throw new NotImplementedException();

        public void WriteValue(bool value) => throw new NotImplementedException();

        public void WriteValue(int value) => throw new NotImplementedException();

        public void WriteValue(float value) => throw new NotImplementedException();

        public void WriteValue(short value) => throw new NotImplementedException();

        public void WriteValue(long value) => throw new NotImplementedException();

        public void WriteValue(double value) => throw new NotImplementedException();

        public void WriteValue(Guid value) => throw new NotImplementedException();

        public void WriteValue(decimal value) => throw new NotImplementedException();

        public void WriteValue(DateTimeOffset value) => throw new NotImplementedException();

        public void WriteValue(TimeSpan value) => throw new NotImplementedException();

        public void WriteValue(byte value) => throw new NotImplementedException();

        public void WriteValue(sbyte value) => throw new NotImplementedException();

        public void WriteValue(string value) => throw new NotImplementedException();

        public void WriteValue(byte[] value) => throw new NotImplementedException();

        public void WriteValue(Date value) => throw new NotImplementedException();

        public void WriteValue(TimeOfDay value) => throw new NotImplementedException();

#if NETCOREAPP3_1_OR_GREATER
        public void WriteValue(System.Text.Json.JsonElement value) => throw new NotImplementedException();
#endif
    }
}
