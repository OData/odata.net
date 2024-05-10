//---------------------------------------------------------------------
// <copyright file="MockJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Xunit;
using System.Threading.Tasks;
using System.IO;

#if NETCOREAPP
using System.Text.Json;
#endif

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

#if NETCOREAPP
        public void WriteValue(System.Text.Json.JsonElement value) => throw new NotImplementedException();
#endif

        public void WriteRawValue(string rawValue)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(rawValue);
        }

        public void Flush() => throw new NotImplementedException();

        public ValueTask StartPaddingFunctionScopeAsync() => throw new NotImplementedException();

        public ValueTask EndPaddingFunctionScopeAsync() => throw new NotImplementedException();

        public ValueTask StartObjectScopeAsync() => throw new NotImplementedException();

        public ValueTask EndObjectScopeAsync() => throw new NotImplementedException();

        public ValueTask StartArrayScopeAsync() => throw new NotImplementedException();

        public ValueTask EndArrayScopeAsync() => throw new NotImplementedException();

        public ValueTask WriteNameAsync(string name)
        {
            Assert.NotNull(this.WriteNameVerifier);
            this.WriteNameVerifier(name);
            return ValueTask.CompletedTask;
        }

        public ValueTask WritePaddingFunctionNameAsync(string functionName) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(bool value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(int value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(float value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(short value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(long value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(double value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(Guid value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(decimal value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(DateTimeOffset value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(TimeSpan value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(byte value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(sbyte value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(string value)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(value);
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(byte[] value)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(Convert.ToBase64String(value));
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(Date value) => throw new NotImplementedException();

        public ValueTask WriteValueAsync(TimeOfDay value) => throw new NotImplementedException();

        public ValueTask WriteRawValueAsync(string rawValue)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(rawValue);
            return ValueTask.CompletedTask;
        }

        public Task FlushAsync() => throw new NotImplementedException();

#if NETCOREAPP
        public ValueTask WriteValueAsync(JsonElement value) => throw new NotImplementedException();
#endif

        public Stream StartStreamValueScope() => throw new NotImplementedException();

        public TextWriter StartTextWriterValueScope(string contentType) => throw new NotImplementedException();

        public void EndStreamValueScope() => throw new NotImplementedException();

        public void EndTextWriterValueScope() => throw new NotImplementedException();

        public ValueTask<Stream> StartStreamValueScopeAsync() => throw new NotImplementedException();

        public ValueTask<TextWriter> StartTextWriterValueScopeAsync(string contentType) => throw new NotImplementedException();

        public ValueTask EndStreamValueScopeAsync() => throw new NotImplementedException();

        public ValueTask EndTextWriterValueScopeAsync() => throw new NotImplementedException();
    }
}