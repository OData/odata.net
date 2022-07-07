using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using ExperimentsLib;
using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BenchmarkDotNet.Order;

namespace JsonWriterBenchmarks
{
    [MemoryDiagnoser]
    //[EtwProfiler]
    //[InliningDiagnoser(false, allowedNamespaces: new string[] { "Microsoft.OData.Json", "ExperimentsLib", "Microsoft.OData.JsonLight" })]
    //[HardwareCounters(HardwareCounter.BranchMispredictions, HardwareCounter.CacheMisses, HardwareCounter.BranchInstructions)]
    [Orderer(SummaryOrderPolicy.Method, MethodOrderPolicy.Declared)]
    //[EventPipeProfiler(EventPipeProfile.CpuSampling)]
    //[ShortRunJob]
    public class Benchmarks
    {
        IEnumerable<Customer> data;
        IEdmModel model;

        public Stream memoryStream;
        public Stream fileStream;

        ServerCollection<IEnumerable<Customer>> writerCollection;
        IServerWriter<IEnumerable<Customer>> writer;

        [ParamsSource(nameof(WriterNames))]
        public string WriterName;

        public static IEnumerable<string> WriterNames() =>
            DefaultServerCollection.Create(Enumerable.Empty<Customer>()).GetServerNames();

        public Benchmarks()
        {
            // the written output will be about 1.45MB of JSON text
            data = CustomerDataSet.GetCustomers(5000);
            model = DataModel.GetEdmModel();
            writerCollection = DefaultServerCollection.Create(data);
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
