//---------------------------------------------------------------------
// <copyright file="WriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExperimentsLib;
using Xunit;

namespace ExperimentTests
{
    public class WriterTests
    {
        private readonly static IEnumerable<Customer> customerData = CustomerDataSet.GetCustomers(4);
        private readonly static WriterCollection<IEnumerable<Customer>> writers = DefaultWriterCollection.Create();
        public static IEnumerable<object[]> WriterNames { get; } = writers.GetWriterNames()
            .Select(n => new string[] { n });

        [Theory]
        [MemberData(nameof(WriterNames))]
        public async Task WriterWritesCustomersCollectionPayload(string writerName)
        {
            using var stream = new MemoryStream();
            var writer = writers.GetWriter(writerName);

            await writer.WritePayloadAsync(customerData, stream, includeRawValues: true);
            using var expectedReader = new StreamReader("ExpectedOutput.txt");
            string expectedOutput = await expectedReader.ReadToEndAsync();
            stream.Seek(0, SeekOrigin.Begin);
            using var actualReader = writerName.Contains("Utf16") ?
                new StreamReader(stream, Encoding.Unicode):
                new StreamReader(stream);
            string actualOutput = await actualReader.ReadToEndAsync();
            Assert.Equal(NormalizeJsonText(expectedOutput), NormalizeJsonText(actualOutput));
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
