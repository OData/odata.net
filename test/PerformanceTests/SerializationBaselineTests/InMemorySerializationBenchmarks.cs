using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.IO;

namespace SerializationBaselineTests
{
    [MemoryDiagnoser]
    public class InMemorySerializationBenchmarks
    {
        readonly IExperimentWriter jsonWriter;
        readonly IExperimentWriter odataWriter;
        readonly IExperimentWriter odataSyncWriter;
        readonly IExperimentWriter odataCharPoolWriter;
        readonly IExperimentWriter odataSyncCharPoolWriter;
        IEnumerable<Customer> data;

        [Params(1000, 5000, 10000)]
        public int dataSize;

        public InMemorySerializationBenchmarks()
        {
            var model = DataModel.GetEdmModel();
            jsonWriter = new JsonExperimentWriter();
            odataWriter = new ODataExperimentWriter(model);
            odataSyncWriter = new ODataSyncExperimentWriter(model);
            odataCharPoolWriter = new ODataExperimentWriter(model, true);
            odataSyncCharPoolWriter = new ODataSyncExperimentWriter(model, true);
        }

        [GlobalSetup]
        public void PrepareDataset()
        {
            data = DataSet.GetCustomers(dataSize);
        }

        #region Write to memory
        [Benchmark(Baseline = true)]
        public void WriteJson()
        {
            WriteToMemory(jsonWriter);
        }

        [Benchmark]
        public void WriteOData()
        {
            WriteToMemory(odataWriter);
        }

        [Benchmark]
        public void WriteODataSync()
        {
            WriteToMemory(odataSyncWriter);
        }

        [Benchmark]
        public void WriteODataArrayPool()
        {
            WriteToMemory(odataCharPoolWriter);
        }

        [Benchmark]
        public void WriteODataSyncArrayPool()
        {
            WriteToMemory(odataSyncCharPoolWriter);
        }

        private void WriteToMemory(IExperimentWriter writer)
        {
            var stream = new MemoryStream();
            writer.WriteCustomers(data, stream).Wait();
        }

        #endregion
    }
}
