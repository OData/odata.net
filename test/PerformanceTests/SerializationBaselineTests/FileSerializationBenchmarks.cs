using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.IO;

namespace SerializationBaselineTests
{
    [MemoryDiagnoser]
    public class FileSerializationBenchmarks
    {
        readonly IExperimentWriter jsonWriter;
        readonly IExperimentWriter odataWriter;
        readonly IExperimentWriter odataSyncWriter;
        readonly IExperimentWriter odataCharPoolWriter;
        readonly IExperimentWriter odataSyncCharPoolWriter;
        IEnumerable<Customer> data;

        [Params(1000, 5000, 10000)]
        public int dataSize;

        public FileSerializationBenchmarks()
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


        string tempFile;
        Stream fileStream;


        [IterationSetup]
        public void CreateTempFile()
        {
            tempFile = Path.GetTempFileName();
            fileStream = new FileStream(tempFile, FileMode.Create);
        }

        [IterationCleanup]
        public void CleanTempFile()
        {
            File.Delete(tempFile);
        }

        [Benchmark(Baseline=true)]
        public void WriteJsonFile()
        {
            WriteToFile(jsonWriter);
        }

        [Benchmark]
        public void WriteODataFile()
        {
            WriteToFile(odataWriter);
        }

        [Benchmark]
        public void WriteODataSyncFile()
        {
            WriteToFile(odataSyncWriter);
        }

        [Benchmark]
        public void WriteODataArrayPoolFile()
        {
            WriteToFile(odataCharPoolWriter);
        }

        [Benchmark]
        public void WriteODataSyncArrayPoolFile()
        {
            WriteToFile(odataSyncCharPoolWriter);
        }

        private void WriteToFile(IExperimentWriter writer)
        {
            writer.WriteCustomers(data, fileStream).Wait();
            fileStream.Close();
        }
    }
}
