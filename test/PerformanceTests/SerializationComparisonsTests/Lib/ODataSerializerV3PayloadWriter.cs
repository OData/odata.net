using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Serializer;
using Microsoft.OData.UriParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentsLib;

internal class ODataSerializerV3PayloadWriter : IPayloadWriter<IEnumerable<Customer>>
{
    readonly IEdmModel model;
    ODataSerializerOptions _options;
    ODataUri _odataUri;

    public ODataSerializerV3PayloadWriter(IEdmModel model)
    {
        this.model = model;
        _options = GetOptions();
        _odataUri = GetODataUri();
    }

    private ODataSerializerOptions GetOptions()
    {
        if (_options != null)
        {
            return _options;
        }

        _options = new ODataSerializerOptions();

        _options.AddTypeInfo<Customer>(new ODataTypeInfo<Customer>
        {
            Properties = [
                new()
                {
                    Name = "Id",
                    WriteValue = (customer, writer, state) => writer.WriteValue(customer.Id, state)
                },
                new()
                {
                    Name = "Name",
                    WriteValue = (customer, writer, state) => writer.WriteValue(customer.Name, state)
                },
                new()
                {
                    Name = "Emails",
                    WriteValue = (customer, writer, state) => writer.WriteValue(customer.Emails, state)
                },
                new()
                {
                    Name = "Bio",
                    WriteValue = (customer, writer, state) => writer.WriteValue(customer.Bio, state)
                },
                new()
                {
                    Name = "Content",
                    WriteValue = (customer, writer, state) => writer.WriteValue(customer.Content, state)
                },
                new()
                {
                    Name = "HomeAddress",
                    WriteValue = (customer, writer, state) => writer.WriteValue(customer.HomeAddress, state)
                },
                new()
                {
                    Name = "Addresses",
                    WriteValue = (customer, writer, state) => writer.WriteValue(customer.Addresses, state)
                }
            ]
        });

        _options.AddTypeInfo<Address>(new ODataTypeInfo<Address>
        {
            Properties = [
                new()
                {
                    Name = "Street",
                    WriteValue = (address, writer, state) => writer.WriteValue(address.Street, state)
                },
                new()
                {
                    Name = "City",
                    WriteValue = (address, writer, state) => writer.WriteValue(address.City, state)
                },
                new()
                {
                    Name = "Misc",
                    // hack
                    WriteValue = (address, writer, state) => writer.WriteValue(address.Misc.ToString(), state)
                }
            ]
        });

        return _options;
    }

    ODataUri GetODataUri()
    {
        var parser = new ODataUriParser(
            this.model,
            new Uri("https://services.odata.org/V4/OData/OData.svc"),
            new Uri("Customers", UriKind.Relative));

        var odataUri = parser.ParseUri();
        return odataUri;
    }

    public async Task WritePayloadAsync(IEnumerable<Customer> payload, Stream stream, bool includeRawValues = false)
    {
        //var parser = new ODataUriParser(
        //    this.model,
        //    new Uri("https://services.odata.org/V4/OData/OData.svc"),
        //    new Uri("Customers", UriKind.Relative));

        //var odataUri = parser.ParseUri();

        await ODataSerializer.WriteAsync(
            payload,
            stream,
            _odataUri,
            this.model,
            _options).ConfigureAwait(false);
    }
}
