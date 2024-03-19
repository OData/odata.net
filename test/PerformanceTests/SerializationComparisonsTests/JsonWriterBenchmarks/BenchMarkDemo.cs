using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.OData;

namespace JsonWriterBenchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.Method, MethodOrderPolicy.Declared)]
    public class BenchMarkDemo
    {
        private ODataMessageWriterSettings settings;

        [GlobalSetup]
        public void GlobalSetup()
        {
            settings = new ODataMessageWriterSettings();
        }

        [Benchmark]
        public void CloneMessageWriterSettings()
        {
            settings.Clone();
        }
        [Benchmark]
        public void CreateMessageWriterSettings()
        {
            new ODataMessageWriterSettings();
        }
    }
}
