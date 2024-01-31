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
        private readonly IEnumerable<Customer> dataWithLargeValues;
        private readonly IEdmModel model;
        private Stream outputStream;

        private string filePath;
        private IPayloadWriter<IEnumerable<Customer>> writer;

        [ParamsSource(nameof(WriterNames))]
        public string WriterName;

        public static IEnumerable<string> WriterNames() =>
            DefaultWriterCollection.Create().GetWriterNames();

        public Benchmarks()
        {
            // the written output will be about 1.45MB of JSON text
            data = CustomerDataSet.GetCustomers(5000);
            // contains fields with 1MB+ values each

            dataWithLargeValues = CustomerDataSet.GetDataWithLargeFields(30);
            model = DataModel.GetEdmModel();
        }

        [GlobalSetup]
        public void Setup()
        {
            writer = writerCollection.GetWriter(WriterName);
            filePath = Path.GetTempFileName();
            outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, share: FileShare.ReadWrite);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            outputStream.Dispose();
            File.Delete(filePath);
        }

        [Benchmark]
        [BenchmarkCategory("InMemory")]
        public async Task WriteToMemoryAsync()
        {
            using var memoryStream = new MemoryStream();
            await writer.WritePayloadAsync(data, memoryStream);
        }

        [Benchmark]
        [BenchmarkCategory("ToFile")]
        public async Task WriteToFileAsync()
        {
            // multiple writes to increase benchmark duration
            await WritePayloadAsync(data);
            await WritePayloadAsync(data);
        }

        [Benchmark]
        [BenchmarkCategory("RawValues")]
        public async Task WriteWithRawValues()
        {
            // multiple writes to increase benchmark duration
            await WritePayloadAsync(data, includeRawValues: true);
            await WritePayloadAsync(data, includeRawValues: true);
        }

        [Benchmark]
        [BenchmarkCategory("ToFile")]
        public async Task WriteToFileWithLargeValuesAsync()
        {
            await WritePayloadAsync(dataWithLargeValues);
        }

        private async Task WritePayloadAsync(IEnumerable<Customer> payload, bool includeRawValues = false)
        {
            await writer.WritePayloadAsync(payload, outputStream, includeRawValues);
        }
    }
}
