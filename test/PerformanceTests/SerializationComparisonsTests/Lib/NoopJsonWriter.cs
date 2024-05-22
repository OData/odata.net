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

        public ValueTask EndArrayScopeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public void EndObjectScope()
        {
        }

        public ValueTask EndObjectScopeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public void EndPaddingFunctionScope()
        {
        }

        public ValueTask EndPaddingFunctionScopeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public void EndStreamValueScope()
        {
        }

        public ValueTask EndStreamValueScopeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public void EndTextWriterValueScope()
        {
        }

        public ValueTask EndTextWriterValueScopeAsync()
        {
            return ValueTask.CompletedTask;
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

        public ValueTask StartArrayScopeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public void StartObjectScope()
        {
        }

        public ValueTask StartObjectScopeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public void StartPaddingFunctionScope()
        {
        }

        public ValueTask StartPaddingFunctionScopeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public Stream StartStreamValueScope()
        {
            throw new NotImplementedException();
        }

        public ValueTask<Stream> StartStreamValueScopeAsync()
        {
            throw new NotImplementedException();
        }

        public TextWriter StartTextWriterValueScope(string contentType)
        {
            throw new NotImplementedException();
        }

        public ValueTask<TextWriter> StartTextWriterValueScopeAsync(string contentType)
        {
            throw new NotImplementedException();
        }

        public void WriteName(string name)
        {
        }

        public ValueTask WriteNameAsync(string name)
        {
            return ValueTask.CompletedTask;
        }

        public void WritePaddingFunctionName(string functionName)
        {
        }

        public ValueTask WritePaddingFunctionNameAsync(string functionName)
        {
            return ValueTask.CompletedTask;
        }

        public void WriteRawValue(string rawValue)
        {
        }

        public ValueTask WriteRawValueAsync(string rawValue)
        {
            return ValueTask.CompletedTask;
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

        public ValueTask WriteValueAsync(bool value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(int value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(float value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(short value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(long value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(double value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(Guid value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(decimal value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(DateTimeOffset value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(TimeSpan value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(byte value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(sbyte value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(string value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(byte[] value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(Date value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(TimeOfDay value)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask WriteValueAsync(JsonElement value)
        {
            return ValueTask.CompletedTask;
        }
    }
}
