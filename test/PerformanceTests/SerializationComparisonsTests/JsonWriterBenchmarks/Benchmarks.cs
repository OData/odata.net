//---------------------------------------------------------------------
// <copyright file="Benchmarks.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Diagnosers;
using ExperimentsLib;
using Microsoft.OData.Edm;

namespace JsonWriterBenchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.Method, MethodOrderPolicy.Declared)]
    public class Benchmarks
    {
        private readonly static WriterCollection<IEnumerable<Customer>> writerCollection = DefaultWriterCollection.Create();
        private readonly IEnumerable<Customer> data;
        private readonly IEdmModel model;

        private string filePath;

        [ParamsSource(nameof(WriterNames))]
        public string WriterName;

        public static IEnumerable<string> WriterNames() =>
            DefaultWriterCollection.Create().GetWriterNames();

        public Benchmarks()
        {
            // the written output will be about 1.45MB of JSON text
            data = CustomerDataSet.GetCustomers(5000);
            model = DataModel.GetEdmModel();
        }

        [GlobalSetup]
        public void SetupStreams()
        {
            filePath = Path.GetTempFileName();
        }

        [GlobalCleanup]
        public void CleanUp()
        {
            File.Delete(filePath);
        }

        [Benchmark]
        [BenchmarkCategory("InMemory")]
        public async Task WriteToMemoryAsync()
        {
            var writer = writerCollection.GetWriter(WriterName);
            var memoryStream = new MemoryStream();
            await writer.WritePayloadAsync(data, memoryStream);
        }

        [Benchmark]
        [BenchmarkCategory("ToFile")]
        public async Task WriteToFileAsync()
        {
            // multiple writes to increase benchmark duration
            await WritePayloadAsync();
            await WritePayloadAsync();
            await WritePayloadAsync();
            await WritePayloadAsync();
            await WritePayloadAsync();
        }

        [Benchmark]
        [BenchmarkCategory("RawValues")]
        public async Task WriteWithRawValues()
        {
            // multiple writes to increase benchmark duration
            await WritePayloadAsync(includeRawValues: true);
            await WritePayloadAsync(includeRawValues: true);
            await WritePayloadAsync(includeRawValues: true);
            await WritePayloadAsync(includeRawValues: true);
            await WritePayloadAsync(includeRawValues: true);
        }

        private async Task WritePayloadAsync(bool includeRawValues = false)
        {
            var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            var writer = writerCollection.GetWriter(WriterName);
            await writer.WritePayloadAsync(data, outputStream, includeRawValues);
        }
    }
}
