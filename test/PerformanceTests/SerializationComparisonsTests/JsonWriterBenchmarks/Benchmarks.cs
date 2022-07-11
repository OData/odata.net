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
        IEnumerable<Customer> data;
        IEdmModel model;

        public Stream memoryStream;
        public Stream fileStream;

        WriterCollection<IEnumerable<Customer>> writerCollection;
        IPayloadWriter<IEnumerable<Customer>> writer;

        [ParamsSource(nameof(WriterNames))]
        public string WriterName;

        public static IEnumerable<string> WriterNames() =>
            DefaultWriterCollection.Create().GetWriterNames();

        public Benchmarks()
        {
            // the written output will be about 1.45MB of JSON text
            data = CustomerDataSet.GetCustomers(5000);
            model = DataModel.GetEdmModel();
            writerCollection = DefaultWriterCollection.Create();
        }

        [IterationSetup]
        public void SetupStreams()
        {
            memoryStream = new MemoryStream();
            string path = Path.GetTempFileName();
            fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            writer = writerCollection.GetWriter(WriterName);
        }

        [IterationCleanup]
        public void CleanUp()
        {
            fileStream.Close();
            fileStream = null;
        }

        [Benchmark]
        [BenchmarkCategory("InMemory")]
        public async Task WriteToMemory()
        {
            await writer.WritePayload(data, memoryStream);
        }

        [Benchmark]
        [BenchmarkCategory("ToFile")]
        public async Task WriteToFile()
        {
            // multiple writes to increase benchmark duration
            await writer.WritePayload(data, fileStream);
            await writer.WritePayload(data, fileStream);
            await writer.WritePayload(data, fileStream);
            await writer.WritePayload(data, fileStream);
            await writer.WritePayload(data, fileStream);

        }
    }
}
