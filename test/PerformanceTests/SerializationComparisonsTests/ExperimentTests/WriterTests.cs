using ExperimentsLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExperimentTests
{
    public class WriterTests
    {
        public static IEnumerable<Customer> data = CustomerDataSet.GetCustomers(4);
        public static WriterCollection<IEnumerable<Customer>> writers = DefaultWriterCollection.Create();
        public static IEnumerable<object[]> WriterNames { get; } = writers.GetServerNames()
            .Where(n => !n.Contains("NoOp"))
            .Select(n => new string[] { n });

        public static IEnumerable<object[]> noOpWriterNames() =>
            writers.GetServerNames()
            .Where(n => n.Contains("NoOp"))
            .Select(n => new string[] { n });

        [Theory]
        [MemberData(nameof(WriterNames))]
        public async Task WriterWritesCustomersCollectionPayload(string writerName)
        {
            using var stream = new MemoryStream();
            var writer = writers.GetWriter(writerName);

            await writer.WritePayload(data, stream);

            using var expectedReader = new StreamReader("ExpectedOutput.txt");
            string expectedOutput = expectedReader.ReadToEnd();
            stream.Seek(0, SeekOrigin.Begin);
            using var actualReader = writerName.Contains("Utf16") ?
                new StreamReader(stream, Encoding.Unicode):
                new StreamReader(stream);
            string actualOutput = actualReader.ReadToEnd();
            Assert.Equal(NormalizeJsonText(expectedOutput), NormalizeJsonText(actualOutput));
        }

        [Theory]
        [MemberData(nameof(noOpWriterNames))]
        public async Task NoOpWritersShouldNotWriteContent(string writerName)
        {
            using var stream = new MemoryStream();
            var writer = writers.GetWriter(writerName);

            await writer.WritePayload(data, stream);

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            string output = reader.ReadToEnd();
            Assert.Equal(string.Empty, output);
        }

        /// <summary>
        /// Normalizes the differences between JSON text encoded
        /// by Utf8JsonWriter and OData's JsonWriter, to make
        /// it possible to compare equivalent outputs.
        /// </summary>
        /// <param name="source">Original JSON text.</param>
        /// <returns>Normalized JSON text.</returns>
        private string NormalizeJsonText(string source)
        {
            return source
                // Utf8JsonWriter uses uppercase letters when encoding unicode characters e.g. \uDC05
                // OData's JsonWriter uses lowercase letters: \udc05
                .ToLowerInvariant()
                // Utf8JsonWrites escapes double-quotes using \u0022
                // OData JsonWrtier uses \"
                .Replace(@"\u0022", @"\""");
        }
    }
}
