using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.IO;

namespace SerializationBaselineTests
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    public class SerializationBenchmarks
    {
        IExperimentWriter jsonWriter;
        IExperimentWriter odataWriter;
        IExperimentWriter odataSyncWriter;
        IExperimentWriter odataCharPoolWriter;
        IExperimentWriter odataSyncCharPoolWriter;
        IEnumerable<Customer> data;

        public int dataSize = 5000;

        [Params(true, false)]
        public bool isModelImmutable;

        string tempFile;
        Stream fileStream;

        [GlobalSetup]
        public void PrepareDataset()
        {
            data = DataSet.GetCustomers(dataSize);
            InitWriters();
        }

        public void InitWriters()
        {
            var model = DataModel.GetEdmModel();
            if (isModelImmutable)
            {
                model.MarkAsImmutable();
            }

            jsonWriter = new JsonExperimentWriter();
            odataWriter = new ODataExperimentWriter(model);
            odataSyncWriter = new ODataSyncExperimentWriter(model);
            odataCharPoolWriter = new ODataExperimentWriter(model, true);
            odataSyncCharPoolWriter = new ODataSyncExperimentWriter(model, true);
        }

        #region Write to memory
        [BenchmarkCategory("InMemory"), Benchmark(Baseline = true)]
        public void WriteJson()
        {
            WriteToMemory(jsonWriter);
        }

        [BenchmarkCategory("InMemory"), Benchmark]
        public void WriteOData()
        {
            WriteToMemory(odataWriter);
        }

        [BenchmarkCategory("InMemory"), Benchmark]
        public void WriteODataSync()
        {
            WriteToMemory(odataSyncWriter);
        }

        [BenchmarkCategory("InMemory"), Benchmark]
        public void WriteODataArrayPool()
        {
            WriteToMemory(odataCharPoolWriter);
        }

        [BenchmarkCategory("InMemory"), Benchmark]
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

        #region Write to file
        [IterationSetup(Targets = new[] {
            nameof(WriteJsonFile),
            nameof(WriteODataFile),
            nameof(WriteODataSyncFile),
            nameof(WriteODataArrayPoolFile),
            nameof(WriteODataSyncArrayPoolFile)
        })]
        public void CreateTempFile()
        {
            tempFile = Path.GetTempFileName();
            fileStream = new FileStream(tempFile, FileMode.Create);
        }

        [IterationCleanup(Targets = new[] {
            nameof(WriteJsonFile),
            nameof(WriteODataFile),
            nameof(WriteODataSyncFile),
            nameof(WriteODataArrayPoolFile),
            nameof(WriteODataSyncArrayPoolFile)
        })]
        public void CleanTempFile()
        {
            File.Delete(tempFile);
        }

        [BenchmarkCategory("ToFile"), Benchmark(Baseline = true)]
        public void WriteJsonFile()
        {
            WriteToFile(jsonWriter);
        }

        [BenchmarkCategory("ToFile"), Benchmark]
        public void WriteODataFile()
        {
            WriteToFile(odataWriter);
        }

        [BenchmarkCategory("ToFile"), Benchmark]
        public void WriteODataSyncFile()
        {
            WriteToFile(odataSyncWriter);
        }

        [BenchmarkCategory("ToFile"), Benchmark]
        public void WriteODataArrayPoolFile()
        {
            WriteToFile(odataCharPoolWriter);
        }

        [BenchmarkCategory("ToFile"), Benchmark]
        public void WriteODataSyncArrayPoolFile()
        {
            WriteToFile(odataSyncCharPoolWriter);
        }

        private void WriteToFile(IExperimentWriter writer)
        {
            writer.WriteCustomers(data, fileStream).Wait();
            fileStream.Close();
        }

        #endregion
    }
}
