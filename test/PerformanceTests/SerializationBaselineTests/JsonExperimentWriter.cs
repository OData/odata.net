using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SerializationBaselineTests
{
    public class JsonExperimentWriter : IExperimentWriter
    {
        public async Task WriteCustomers(IEnumerable<Customer> payload, Stream stream)
        {
            await JsonSerializer.SerializeAsync(stream, payload);
        }
    }
}
