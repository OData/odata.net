using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes Customer collection payload using <see cref="JsonSerializer"/>.
    /// </summary>
    public class JsonSerializerPayloadWriter : IPayloadWriter<IEnumerable<Customer>>
    {
        public async Task WritePayload(IEnumerable<Customer> payload, Stream stream)
        {
            var serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");

            var response = new ResponseWrapper
            {
                Context = $"{serviceRoot}$metadata#Customers",
                Value = payload
            };

            await JsonSerializer.SerializeAsync(stream, response);
        }

        public class ResponseWrapper
        {
            [JsonPropertyName("@odata.context")]
            public string Context { get; set; }
            [JsonPropertyName("value")]
            public IEnumerable<Customer> Value { get; set; }
        }
    }
}
