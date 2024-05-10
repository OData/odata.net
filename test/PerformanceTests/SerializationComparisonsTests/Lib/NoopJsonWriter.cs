//---------------------------------------------------------------------
// <copyright file="NoopJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;

namespace ExperimentsLib
{
    /// <summary>
    /// An <see cref="IJsonWriter"/> implementation that does nothing.
    /// It does not write any output. It's meant to help evaluate
    /// the overhead of higher-level libraries without the cost of JsonWriter.
    /// </summary>
    public class NoopJsonWriter : IJsonWriter
    {
        public void EndArrayScope()
        {
        }

        public Task EndArrayScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void EndObjectScope()
        {
        }

        public Task EndObjectScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void EndPaddingFunctionScope()
        {
        }

        public Task EndPaddingFunctionScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void EndStreamValueScope()
        {
        }

        public Task EndStreamValueScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void EndTextWriterValueScope()
        {
        }

        public Task EndTextWriterValueScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void Flush()
        {
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public void StartArrayScope()
        {
        }

        public Task StartArrayScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void StartObjectScope()
        {
        }

        public Task StartObjectScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void StartPaddingFunctionScope()
        {
        }

        public Task StartPaddingFunctionScopeAsync()
        {
            return Task.CompletedTask;
        }

        public Stream StartStreamValueScope()
        {
            throw new NotImplementedException();
        }

        public Task<Stream> StartStreamValueScopeAsync()
        {
            throw new NotImplementedException();
        }

        public TextWriter StartTextWriterValueScope(string contentType)
        {
            throw new NotImplementedException();
        }

        public Task<TextWriter> StartTextWriterValueScopeAsync(string contentType)
        {
            throw new NotImplementedException();
        }

        public void WriteName(string name)
        {
        }

        public Task WriteNameAsync(string name)
        {
            return Task.CompletedTask;
        }

        public void WritePaddingFunctionName(string functionName)
        {
        }

        public Task WritePaddingFunctionNameAsync(string functionName)
        {
            return Task.CompletedTask;
        }

        public void WriteRawValue(string rawValue)
        {
        }

        public Task WriteRawValueAsync(string rawValue)
        {
            return Task.CompletedTask;
        }

        public void WriteValue(bool value)
        {
        }

        public void WriteValue(int value)
        {
        }

        public void WriteValue(float value)
        {
        }

        public void WriteValue(short value)
        {
            
        }

        public void WriteValue(long value)
        {
        }

        public void WriteValue(double value)
        {
        }

        public void WriteValue(Guid value)
        {
        }

        public void WriteValue(decimal value)
        {
        }

        public void WriteValue(DateTimeOffset value)
        {
        }

        public void WriteValue(TimeSpan value)
        {
        }

        public void WriteValue(byte value)
        {
        }

        public void WriteValue(sbyte value)
        {
        }

        public void WriteValue(string value)
        {
        }

        public void WriteValue(byte[] value)
        {
        }

        public void WriteValue(Date value)
        {
        }

        public void WriteValue(TimeOfDay value)
        {
        }

        public void WriteValue(JsonElement value)
        {
        }

        public Task WriteValueAsync(bool value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(int value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(float value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(short value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(long value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(double value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(Guid value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(decimal value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(DateTimeOffset value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(TimeSpan value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(byte value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(sbyte value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(string value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(byte[] value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(Date value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(TimeOfDay value)
        {
            return Task.CompletedTask;
        }

        public Task WriteValueAsync(JsonElement value)
        {
            return Task.CompletedTask;
        }
    }
}
