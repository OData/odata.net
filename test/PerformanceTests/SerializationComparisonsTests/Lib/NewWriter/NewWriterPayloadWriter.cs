using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    internal class NewWriterPayloadWriter : IPayloadWriter<IEnumerable<Customer>>
    {
        private IEdmModel model;
        public NewWriterPayloadWriter(IEdmModel model)
        {
            this.model = model;
        }

        public async Task WritePayloadAsync(IEnumerable<Customer> payload, Stream stream, bool includeRawValues = false)
        {
            var serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");
            var uri = new Uri("https://services.odata.org/V4/OData/OData.svc/Customers?$select=Id,Name");
            var parser = new ODataUriParser(this.model, serviceRoot, uri);
            var odataUri = parser.ParseUri();
            odataUri.ServiceRoot = serviceRoot;
            var options = OptionsHelper.CreateJsonSerializerOptions(this.model, odataUri);

            // we ignore the includeRawValues argument for simplicity
            await JsonSerializer.SerializeAsync(stream, payload, options);
        }
    }
}