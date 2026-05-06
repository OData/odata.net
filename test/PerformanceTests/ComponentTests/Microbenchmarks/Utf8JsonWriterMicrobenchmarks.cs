//---------------------------------------------------------------------
// <copyright file="Utf8JsonWriterMicrobenchmarks.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance.Microbenchmarks
{
    using System.IO;
    using BenchmarkDotNet.Attributes;
    using Microsoft.OData.Json;

    /// <summary>
    /// Targets the inner writes performed millions of times when serializing
    /// large feeds: doubles (which previously allocated ".0" suffix strings),
    /// property names (UTF8 byte allocation), and raw values.
    /// See REPORT.md findings #1, #2, #4.
    /// </summary>
    [MemoryDiagnoser]
    public class Utf8JsonWriterMicrobenchmarks
    {
        private const int Count = 5000;
        private MemoryStream _stream;
        private IJsonWriter _writer;

        [GlobalSetup]
        public void Setup()
        {
            _stream = new MemoryStream(1024 * 1024);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.SetLength(0);
            _writer = ODataUtf8JsonWriterFactory.Default.CreateJsonWriter(_stream, isIeee754Compatible: false, encoding: System.Text.Encoding.UTF8);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _writer.Flush();
            (_writer as System.IDisposable)?.Dispose();
        }

        [Benchmark]
        public void WriteIntegerDoubles()
        {
            // Representative numeric feed - integer-shaped doubles trigger the ".0" suffix path.
            _writer.StartArrayScope();
            for (int i = 0; i < Count; i++)
            {
                _writer.WriteValue((double)i);
            }
            _writer.EndArrayScope();
        }

        [Benchmark]
        public void WriteFractionalDoubles()
        {
            _writer.StartArrayScope();
            double v = 0.1;
            for (int i = 0; i < Count; i++)
            {
                _writer.WriteValue(v);
                v += 0.5;
            }
            _writer.EndArrayScope();
        }

        [Benchmark]
        public void WritePropertyNames()
        {
            _writer.StartObjectScope();
            for (int i = 0; i < Count; i++)
            {
                _writer.WriteName("PropertyName");
                _writer.WriteValue(1);
            }
            _writer.EndObjectScope();
        }

        [Benchmark]
        public void WriteRawValues()
        {
            _writer.StartArrayScope();
            for (int i = 0; i < Count; i++)
            {
                _writer.WriteRawValue("{\"k\":1}");
            }
            _writer.EndArrayScope();
        }
    }
}
