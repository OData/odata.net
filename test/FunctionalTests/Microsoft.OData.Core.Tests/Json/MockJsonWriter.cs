//---------------------------------------------------------------------
// <copyright file="MockJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// This is intended to be a Mock for the JsonWriter.
    /// 
    /// So far only a a few things have been implemented.
    /// </summary>
    internal class MockJsonWriter : IJsonWriter
    {
        public Action<string> WriteNameVerifier;
        public Action<string> WriteValueVerifier;

        public void StartPaddingFunctionScope() => throw new NotImplementedException();

        public void EndPaddingFunctionScope() => throw new NotImplementedException();

        public void StartObjectScope() => throw new NotImplementedException();

        public void EndObjectScope() => throw new NotImplementedException();

        public void StartArrayScope() => throw new NotImplementedException();

        public void EndArrayScope() => throw new NotImplementedException();

        public void WriteName(string name)
        {
            Assert.NotNull(this.WriteNameVerifier);
            this.WriteNameVerifier(name);
        }

        public void WritePaddingFunctionName(string functionName) => throw new NotImplementedException();

        public void WriteValue(bool value) => throw new NotImplementedException();

        public void WriteValue(int value) => throw new NotImplementedException();

        public void WriteValue(float value) => throw new NotImplementedException();

        public void WriteValue(short value) => throw new NotImplementedException();

        public void WriteValue(long value) => throw new NotImplementedException();

        public void WriteValue(double value) => throw new NotImplementedException();

        public void WriteValue(Guid value) => throw new NotImplementedException();

        public void WriteValue(decimal value) => throw new NotImplementedException();

        public void WriteValue(Date value) => throw new NotImplementedException();

        public void WriteValue(DateTimeOffset value) => throw new NotImplementedException();

        public void WriteValue(TimeSpan value) => throw new NotImplementedException();

        public void WriteValue(TimeOfDay value) => throw new NotImplementedException();

        public void WriteValue(byte value) => throw new NotImplementedException();

        public void WriteValue(sbyte value) => throw new NotImplementedException();

        public void WriteValue(string value)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(value);
        }

        public void WriteValue(byte[] value)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(Convert.ToBase64String(value));
        }

        public void WriteValue(JsonElement value) => throw new NotImplementedException();

        public void WriteRawValue(string rawValue)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(rawValue);
        }

        public void Flush() => throw new NotImplementedException();

        public Task StartPaddingFunctionScopeAsync() => throw new NotImplementedException();

        public Task EndPaddingFunctionScopeAsync() => throw new NotImplementedException();

        public Task StartObjectScopeAsync() => throw new NotImplementedException();

        public Task EndObjectScopeAsync() => throw new NotImplementedException();

        public Task StartArrayScopeAsync() => throw new NotImplementedException();

        public Task EndArrayScopeAsync() => throw new NotImplementedException();

        public Task WriteNameAsync(string name)
        {
            Assert.NotNull(this.WriteNameVerifier);
            this.WriteNameVerifier(name);
            return TaskUtils.CompletedTask;
        }

        public Task WritePaddingFunctionNameAsync(string functionName) => throw new NotImplementedException();

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

        public Task WriteValueAsync(string value)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(value);
            return TaskUtils.CompletedTask;
        }

        public Task WriteValueAsync(byte[] value)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(Convert.ToBase64String(value));
            return TaskUtils.CompletedTask;
        }

        public Task WriteValueAsync(Date value) => throw new NotImplementedException();

        public Task WriteValueAsync(TimeOfDay value) => throw new NotImplementedException();

        public Task WriteRawValueAsync(string rawValue)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(rawValue);
            return TaskUtils.CompletedTask;
        }

        public Task FlushAsync() => throw new NotImplementedException();

        public Task WriteValueAsync(JsonElement value) => throw new NotImplementedException();

        public Stream StartStreamValueScope() => throw new NotImplementedException();

        public TextWriter StartTextWriterValueScope(string contentType) => throw new NotImplementedException();

        public void EndStreamValueScope() => throw new NotImplementedException();

        public void EndTextWriterValueScope() => throw new NotImplementedException();

        public Task<Stream> StartStreamValueScopeAsync() => throw new NotImplementedException();

        public Task<TextWriter> StartTextWriterValueScopeAsync(string contentType) => throw new NotImplementedException();

        public Task EndStreamValueScopeAsync() => throw new NotImplementedException();

        public Task EndTextWriterValueScopeAsync() => throw new NotImplementedException();
    }
}