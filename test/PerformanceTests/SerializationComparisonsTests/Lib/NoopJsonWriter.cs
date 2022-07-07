using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    /// <summary>
    /// An <see cref="IJsonWriter"/> implementation that does nothing.
    /// It does not write any output. It's meant to help evaluate
    /// the overhead of higher-level libraries without the cost of JsonWriter.
    /// </summary>
    public class NoopJsonWriter : IJsonWriter, IJsonWriterAsync
    {
        public bool x = false;
        public void EndArrayScope()
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public Task EndArrayScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void EndObjectScope()
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public Task EndObjectScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void EndPaddingFunctionScope()
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public Task EndPaddingFunctionScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void Flush()
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public void StartArrayScope()
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public Task StartArrayScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void StartObjectScope()
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public Task StartObjectScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void StartPaddingFunctionScope()
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public Task StartPaddingFunctionScopeAsync()
        {
            return Task.CompletedTask;
        }

        public void WriteName(string name)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public Task WriteNameAsync(string name)
        {
            return Task.CompletedTask;
        }

        public void WritePaddingFunctionName(string functionName)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public Task WritePaddingFunctionNameAsync(string functionName)
        {
            return Task.CompletedTask;
        }

        public void WriteRawValue(string rawValue)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public Task WriteRawValueAsync(string rawValue)
        {
            return Task.CompletedTask;
        }

        public void WriteValue(bool value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(int value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(float value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(short value)
        {
            
        }

        public void WriteValue(long value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(double value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(Guid value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(decimal value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(DateTimeOffset value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(TimeSpan value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(byte value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(sbyte value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(string value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(byte[] value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(Date value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
        }

        public void WriteValue(TimeOfDay value)
        {
            // simulate work so that this method does not get optimized away
            x = !x;
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
    }
}
