using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3;
using Microsoft.OData.Serializer.V3.Adapters;
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

        _options.AddTypeInfo<Customer>(new ODataResourceTypeInfo<Customer>
        {
            Properties = [
                new()
                {
                    Name = "Id",
                    WriteValue = (customer, state) => state.WriteValue(customer.Id)
                },
                new()
                {
                    Name = "Name",
                    WriteValue = (customer, state) => state.WriteValue(customer.Name)
                },
                new()
                {
                    Name = "Emails",
                    WriteValue = (customer, state) => state.WriteValue(customer.Emails)
                },
                new()
                {
                    Name = "Bio",
                    WriteValue = (customer, state) => state.WriteValue(customer.Bio)
                },
                new()
                {
                    Name = "Content",
                    WriteValue = (customer, state) => state.WriteValue(customer.Content)
                },
                new()
                {
                    Name = "HomeAddress",
                    WriteValue = (customer, state) => state.WriteValue(customer.HomeAddress)
                },
                new()
                {
                    Name = "Addresses",
                    WriteValue = (customer, state) => state.WriteValue(customer.Addresses)
                }
            ]
        });

        _options.AddTypeInfo<Address>(new ODataResourceTypeInfo<Address>
        {
            Properties = [
                new()
                {
                    Name = "Street",
                    WriteValue = (address, state) => state.WriteValue(address.Street)
                },
                new()
                {
                    Name = "City",
                    WriteValue = (address, state) => state.WriteValue(address.City)
                },
                new()
                {
                    Name = "Misc",
                    // hack
                    WriteValue = (address, state) => state.WriteValue(address.Misc.ToString())
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
