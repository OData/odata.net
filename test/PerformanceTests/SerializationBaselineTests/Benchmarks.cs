using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace SerializationBaselineTests
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        readonly IExperimentWriter jsonWriter;
        readonly IExperimentWriter odataWriter;
        readonly IExperimentWriter odataSyncWriter;
        readonly IExperimentWriter odataCharPoolWriter;
        readonly IExperimentWriter odataSyncCharPoolWriter;
        IEnumerable<Customer> data;

        [Params(1000, 5000, 10000)]
        public int dataSize;

        public Benchmarks()
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

        //#region Write to buffered memory
        //[Benchmark]
        //public void WriteJsonBufferedMemory()
        //{
        //    var pipe = new Pipe();
        //    var writer = pipe.Writer;
        //    var stream = writer.AsStream();
        //    jsonWriter.WriteCustomers(data, stream).Wait();

        //}

        //[Benchmark]
        //public void WriteODataBufferedMemory()
        //{
        //    var pipe = new Pipe();
        //    var writer = pipe.Writer;
        //    var stream = writer.AsStream();
        //    odataWriter.WriteCustomers(data, stream).Wait();

        //}

        //[Benchmark]
        //public void WriteODataSyncBufferedMemory()
        //{
        //    var pipe = new Pipe();
        //    var writer = pipe.Writer;
        //    var stream = writer.AsStream();
        //    odataSyncWriter.WriteCustomers(data, stream).Wait();
        //}

        //#endregion

        //#region Write to local file

        string tempFile;
        Stream fileStream;


        [IterationSetup(
            Targets = new[]{
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

        [IterationCleanup(
            Targets = new[] {
                nameof(WriteJsonFile),
                nameof(WriteODataFile),
                nameof(WriteODataSyncFile),
                nameof(WriteODataArrayPoolFile),
                nameof(WriteODataSyncArrayPoolFile)
            }
            )]
        public void CleanTempFile()
        {
            File.Delete(tempFile);
        }

        [Benchmark]
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
        //#endregion
    }
}
